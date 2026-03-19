using System.Collections.Generic;
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

    protected bool canShoot;

    protected void Update()
    {
        if (!agent.isOnNavMesh) return;
        if (playerTransform == null) return;

        HandleSpeed();
        HandleWobble();

        Vector2 selfPos = To2D(agent.nextPosition);
        Vector2 playerPos = playerTransform.position;

        float distance = Vector2.Distance(selfPos, playerPos);
        bool hasLOS = HasLineOfSight();

        canShoot = hasLOS && distance <= attackDistance;

        if (distance < retreatDistance) Retreat(selfPos, playerPos);
        else if (!hasLOS) SeekLineOfSight(playerPos);
        else if (distance > attackDistance) Move(playerPos);
        else Stop();

        transform.position = To2D(agent.nextPosition);
    }

    protected void SeekLineOfSight(Vector2 playerPos)
    {
        Vector2 selfPos = To2D(agent.nextPosition);

        float desiredDistance = Mathf.Clamp(Vector2.Distance(selfPos, playerPos),retreatDistance,attackDistance);

        int samples = 8;
        float angleStep = 360f / samples;

        Vector2 bestPoint = playerPos;
        bool found = false;

        for (int i = 0; i < samples; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;

            Vector2 dir = new(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 candidate = playerPos + dir * desiredDistance;

            if (NavMesh.SamplePosition(ToNavMesh(candidate), out NavMeshHit navHit, 1.5f, NavMesh.AllAreas))
            {
                if (HasLineOfSightFrom(candidate, playerPos))
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
            Move(playerPos);
        }
    }

    protected bool HasLineOfSightFrom(Vector2 origin2D, Vector2 target2D)
    {
        Vector2 dir = (target2D - origin2D).normalized;
        float dist = Vector2.Distance(origin2D, target2D);

        RaycastHit2D hit = Physics2D.CircleCast(origin2D, shootCheckRadius, dir, dist, obstacleMask);

        if (hit.collider != null && hit.transform != playerTransform)
            return false;

        return true;
    }

    protected void Move(Vector2 playerPos)
    {
        Vector2 target2D = playerPos + wobbleOffset;
        agent.isStopped = false;
        agent.SetDestination(ToNavMesh(target2D));
    }

    protected void Stop()
    {
        agent.isStopped = true;
    }

    protected void Retreat(Vector2 selfPos, Vector2 playerPos)
    {
        Vector2 dir = (selfPos - playerPos).normalized;
        Vector2 target = selfPos + dir * (stoppingDistance + 1f);
        target += wobbleOffset;

        agent.isStopped = false;
        agent.SetDestination(ToNavMesh(target));
    }

    protected bool HasLineOfSight()
    {
        Vector2 origin2D = To2D(agent.nextPosition);
        Vector2 target2D = playerTransform.position;

        Vector2 dir = (target2D - origin2D).normalized;
        float dist = Vector2.Distance(origin2D, target2D);

        RaycastHit2D hit = Physics2D.CircleCast(origin2D, shootCheckRadius, dir, dist, obstacleMask);

        if (hit.collider != null && hit.transform != playerTransform)
            return false;

        Vector3 origin3D = new(origin2D.x, origin2D.y, 0f);
        Vector3 target3D = new(target2D.x, target2D.y, 0f);
        Debug.DrawLine(origin3D, target3D, Color.red);

        return true;
    }

    public bool CanShoot()
    {
        return canShoot;
    }
}