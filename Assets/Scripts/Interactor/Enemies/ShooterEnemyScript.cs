using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShooterEnemyScript : EnemyScript
{
    [SerializeField] protected Transform[] targets;
    [Header("Combat")]
    [SerializeField] protected float attackDistance = 5f;
    [SerializeField] protected float stoppingDistance = 4f;
    [SerializeField] protected LayerMask obstacleMask;
    [SerializeField] protected float shootCheckRadius = 0.1f;
    [SerializeField] protected float retreatDistance = 2.5f;

    [SerializeField] private Transform gunTransform;
    protected Transform currentTarget;

    [SerializeField] private float rotationSpeed = 720f;

    protected void Update()
    {
        if (!agent.isOnNavMesh) return;

        currentTarget = FindClosestPlayer();

        if (currentTarget == null) return;

        HandleSpeed();
        HandleWobble();

        Vector2 selfPos = To2D(agent.nextPosition);
        Vector2 targetPos = currentTarget.position;

        float distance = Vector2.Distance(selfPos, targetPos);
        bool hasLOS = HasLineOfSight(selfPos, targetPos);

        canShoot = hasLOS && distance <= attackDistance;

        if (distance < retreatDistance) Retreat(selfPos, targetPos);
        else if (!hasLOS) SeekLineOfSight(targetPos);
        else if (distance > attackDistance) Move(targetPos);
        else Stop();

        transform.position = To2D(agent.nextPosition);

        if (currentTarget != null)
            SmoothLookAt(currentTarget);

        if (gunTransform != null)
            SmoothLookAtGun(currentTarget);
    }

    protected void SmoothLookAt(Transform target)
    {
        Vector2 direction = (target.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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
        float minDist = float.MaxValue;
        Transform closest = null;

        foreach (var player in targets)
        {
            float dist = Vector2.Distance(To2D(agent.nextPosition), player.position);
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
        Vector2 selfPos = To2D(agent.nextPosition);
        float desiredDistance = Mathf.Clamp(Vector2.Distance(selfPos, targetPos), retreatDistance, attackDistance);

        int samples = 8;
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
                if (HasLineOfSightFrom(candidate, targetPos))
                {
                    bestPoint = To2D(navHit.position);
                    found = true;
                    break;
                }
            }
        }

        if (found)
        {
            agent.isStopped = false;
            agent.SetDestination(ToNavMesh(bestPoint));
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
        agent.isStopped = false;
        agent.SetDestination(ToNavMesh(target2D));
    }

    protected void Stop()
    {
        agent.isStopped = true;
    }

    protected void Retreat(Vector2 selfPos, Vector2 targetPos)
    {
        Vector2 dir = (selfPos - targetPos).normalized;
        Vector2 target = selfPos + dir * (stoppingDistance + 1f);
        target += wobbleOffset;

        agent.isStopped = false;
        agent.SetDestination(ToNavMesh(target));
    }
}