using UnityEngine;
using UnityEngine.AI;

public class ShooterEnemyScript : EnemyScript
{
    [Header("Combat")]
    [SerializeField] protected float attackDistance = 5f;
    [SerializeField] protected float stoppingDistance = 4f;
    [SerializeField] protected LayerMask obstacleMask;
    [SerializeField] protected float shootCheckRadius = 0.1f;
    [SerializeField] protected float retreatDistance = 2.5f;

    [Header("References")]
    [SerializeField] private Transform gunTransform;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 720f;

    protected Transform currentTarget;

    protected void Update()
    {
        if (!agent3D.isOnNavMesh) return;

        currentTarget = FindClosestPlayer();

        if (currentTarget == null) return;

        HandleSpeed();
        HandleWobble();

        Vector2 selfPos = To2D(agent3D.nextPosition);
        Vector2 targetPos = currentTarget.position;

        float distance = Vector2.Distance(selfPos, targetPos);
        bool hasLOS = HasLineOfSight(selfPos, targetPos);

        canShoot = hasLOS && distance <= attackDistance;

        if (distance < retreatDistance) Retreat(selfPos, targetPos);
        else if (!hasLOS) SeekLineOfSight(targetPos);
        else if (distance > attackDistance) Move(targetPos);
        else Stop();

        transform.position = To2D(agent3D.nextPosition);

        // Only the gun rotates toward target; character body does not rotate
        if (gunTransform != null && currentTarget != null)
            SmoothLookAtGun(currentTarget);
    }

    protected void SmoothLookAtGun(Transform target)
    {
        Vector2 direction = (target.position - gunTransform.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

        gunTransform.rotation = Quaternion.RotateTowards(gunTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    protected Transform FindClosestPlayer()
    {
        Transform[] searchTargets = playerTargets;
        if (searchTargets == null) return null;

        float minDist = float.MaxValue;
        Transform closest = null;
        Vector2 selfPos = To2D(agent3D.nextPosition);

        foreach (var player in searchTargets)
        {
            if (player == null) continue;
            float dist = Vector2.Distance(selfPos, player.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = player;
            }
        }
        return closest;
    }

    protected void LookAtTarget(Transform target)
    {
        Vector2 direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    protected void SeekLineOfSight(Vector2 targetPos)
    {
        Vector2 selfPos = To2D(agent3D.nextPosition);
        float desiredDistance = Mathf.Clamp(Vector2.Distance(selfPos, targetPos), retreatDistance, attackDistance);

        int samples = 12;
        float angleStep = 360f / samples;

        Vector2 bestPoint = targetPos;
        bool found = false;

        for (int i = 0; i < samples; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector2 dir = new(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 candidate = targetPos + dir * desiredDistance;

            if (NavMesh.SamplePosition(ToNavMesh(candidate), out NavMeshHit navHit, 1.5f, NavMesh.AllAreas))
            {
                Vector2 sampledPoint = To2D(navHit.position);
                if (HasLineOfSightFrom(sampledPoint, targetPos))
                {
                    bestPoint = sampledPoint;
                    found = true;
                    break;
                }
            }
        }

        if (found)
        {
            agent3D.isStopped = false;
            agent3D.SetDestination(ToNavMesh(bestPoint + wobbleOffset));
        }
        else
        {
            Move(targetPos);
        }
    }

    protected bool HasLineOfSight(Vector2 origin2D, Vector2 target2D)
    {
        Vector2 dir = (target2D - origin2D).normalized;
        float dist = Vector2.Distance(origin2D, target2D);

        RaycastHit2D hit = Physics2D.CircleCast(origin2D, shootCheckRadius, dir, dist, obstacleMask);
        if (hit.collider != null && hit.transform != currentTarget)
            return false;

        Debug.DrawLine(new Vector3(origin2D.x, origin2D.y, 0), new Vector3(target2D.x, target2D.y, 0), Color.red);
        return true;
    }

    protected bool HasLineOfSightFrom(Vector2 origin2D, Vector2 target2D)
    {
        Vector2 dir = (target2D - origin2D).normalized;
        float dist = Vector2.Distance(origin2D, target2D);

        RaycastHit2D hit = Physics2D.CircleCast(origin2D, shootCheckRadius, dir, dist, obstacleMask);
        return hit.collider == null || hit.transform == currentTarget;
    }

    protected void Move(Vector2 targetPos)
    {
        Vector2 target2D = targetPos + wobbleOffset;
        agent3D.isStopped = false;
        agent3D.SetDestination(ToNavMesh(target2D));
    }

    protected void Stop()
    {
        agent3D.isStopped = true;
    }

    protected void Retreat(Vector2 selfPos, Vector2 targetPos)
    {
        Vector2 dir = (selfPos - targetPos).normalized;
        Vector2 retreatTarget = selfPos + dir * (stoppingDistance + 1f);
        retreatTarget += wobbleOffset;

        if (NavMesh.SamplePosition(ToNavMesh(retreatTarget), out NavMeshHit navHit, 2f, NavMesh.AllAreas))
        {
            agent3D.isStopped = false;
            agent3D.SetDestination(navHit.position);
        }
        else
        {
            agent3D.isStopped = false;
            agent3D.SetDestination(ToNavMesh(retreatTarget));
        }
    }
}