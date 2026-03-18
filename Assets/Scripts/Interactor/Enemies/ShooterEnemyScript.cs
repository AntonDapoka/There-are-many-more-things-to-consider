using System.Collections.Generic;
using UnityEngine;

public class ShooterEnemyScript : EnemyScript
{
    [Header("Combat")]
    [SerializeField] private float attackDistance = 5f;
    [SerializeField] private float stoppingDistance = 4f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float shootCheckRadius = 0.1f;
    [SerializeField] private float retreatDistance = 2.5f;

    protected bool canShoot;

    private void Update()
    {
    if (!agent.isOnNavMesh) return;
    if (playerTransform == null) return;

    HandleSpeed();
    HandleWobble();

    Vector2 selfPos = To2D(agent.nextPosition);
    Vector2 playerPos = To2D(playerTransform.position);

    float distance = Vector2.Distance(selfPos, playerPos);

    if (distance < retreatDistance)
    {
        Retreat(selfPos, playerPos);
        canShoot = false;
    }
    else if (distance > stoppingDistance)
    {
        Move(playerPos);
        canShoot = false;
    }
    else
    {
        Stop();
        canShoot = HasLineOfSight();
    }
        Vector3 navPos = agent.nextPosition;
        transform.position = To2D(navPos);
    }

    private void Move(Vector2 playerPos)
    {
        Vector2 target2D = playerPos + wobbleOffset;
        agent.isStopped = false;
        agent.SetDestination(ToNavMesh(target2D));
    }

    private void Stop()
    {
        agent.isStopped = true;
    }

    private void Retreat(Vector2 selfPos, Vector2 playerPos)
    {
        // направление ОТ игрока
        Vector2 dir = (selfPos - playerPos).normalized;

        // точка отступления
        Vector2 target = selfPos + dir * (stoppingDistance + 1f);

        // немного "жизни"
        target += wobbleOffset;

        agent.isStopped = false;
        agent.SetDestination(ToNavMesh(target));
    }

    private void ApplyAgentPosition()
    {
        Vector3 navPos = agent.nextPosition;

        Vector2 pos2D = To2D(navPos);
        transform.position = new Vector3(pos2D.x, transform.position.y, pos2D.y);
    }

    private bool HasLineOfSight()
    {
        Vector3 origin = transform.position;
        Vector3 target = playerTransform.position;

        Vector3 dir = (target - origin).normalized;
        float dist = Vector3.Distance(origin, target);

        if (Physics.SphereCast(origin, shootCheckRadius, dir, out RaycastHit hit, dist, obstacleMask))
        {
            if (hit.transform != playerTransform)
                return false;
        }

        return true;
    }

    public bool CanShoot()
    {
        return canShoot;
    }
}