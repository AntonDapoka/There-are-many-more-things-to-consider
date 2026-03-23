using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierScript : NPCScript
{
    public enum Command
    {
        None,
        Follow,
        MoveNorth,
        MoveSouth,
        MoveEast,
        MoveWest,
        Halt,
        Fire
    }

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private EnemyManagerScript enemyManager;
    [SerializeField] private Transform gunTransform;

    [SerializeField] private float followDestinationNearPlayer = 1.25f;

    [Header("Combat (FIRE)")]
    [SerializeField] private float attackDistance = 8f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float shootCheckRadius = 0.1f;
    [SerializeField] private float rotationSpeed = 720f;

    private Transform currentFireTarget;
    public List<Transform> targets;

    [Header("Follow")]
    [SerializeField] private float followDistance = 2.5f;

    [Header("Movement")]
    [SerializeField] private float baseSpeed = 3.5f;
    [SerializeField] private float runMultiplier = 1.8f;
    [SerializeField] private float moveDistance = 10f;

    [Header("Idle shuffle")]
    [SerializeField] private float idleShuffleIntervalMin = 2.5f;
    [SerializeField] private float idleShuffleIntervalMax = 6f;
    [SerializeField] private float idleShuffleDistanceMin = 0.35f;
    [SerializeField] private float idleShuffleDistanceMax = 1.1f;
    [SerializeField] private float idleShuffleArrivedThreshold = 0.12f;

    protected Command currentCommand = Command.None;
    protected bool isRunning = false;

    private float idleShuffleTimer;
    private float idleShuffleNextAt;
    private int idleShuffleSideSign = 1;

    protected override void OnEnable()
    {
        base.OnEnable();
        ScheduleNextIdleShuffle();
    }

    protected void Update()
    {
        if (!agent3D.isOnNavMesh) return;

        if (currentCommand != Command.Fire)
            canShoot = false;

        HandleSpeed();
        HandleWobble();

        switch (currentCommand)
        {
            case Command.Follow:
                FollowPlayer();
                break;
            case Command.MoveNorth:
                Move(Vector2.up);
                break;
            case Command.MoveSouth:
                Move(Vector2.down);
                break;
            case Command.MoveEast:
                Move(Vector2.right);
                break;
            case Command.MoveWest:
                Move(Vector2.left);
                break;
            case Command.Halt:
                Stop();
                break;
            case Command.Fire:
                HandleFire();
                break;
        }

        if (ShouldIdleShuffle())
            UpdateIdleShuffle();

        transform.position = To2D(agent3D.nextPosition);
    }

    private bool ShouldIdleShuffle()
    {
        switch (currentCommand)
        {
            case Command.MoveNorth:
            case Command.MoveSouth:
            case Command.MoveEast:
            case Command.MoveWest:
            case Command.Fire:
                return false;
            case Command.Follow:
                return agent3D.isStopped;
            default:
                return true;
        }
    }

    private void ScheduleNextIdleShuffle()
    {
        idleShuffleNextAt = Random.Range(idleShuffleIntervalMin, idleShuffleIntervalMax);
        idleShuffleTimer = 0f;
    }

    private bool IdleShufflePathFinished()
    {
        if (agent3D.pathPending) return false;
        if (!agent3D.hasPath) return true;
        return agent3D.remainingDistance <= idleShuffleArrivedThreshold;
    }

    private void UpdateIdleShuffle()
    {
        if (!IdleShufflePathFinished())
            return;

        idleShuffleTimer += Time.deltaTime;
        if (idleShuffleTimer < idleShuffleNextAt)
            return;

        idleShuffleTimer = 0f;
        ScheduleNextIdleShuffle();

        Vector2 selfPos = To2D(agent3D.nextPosition);
        float step = Random.Range(idleShuffleDistanceMin, idleShuffleDistanceMax);
        Vector2 sideways = Vector2.right * (idleShuffleSideSign * step);
        idleShuffleSideSign *= -1;

        SetDestinationOnNavMesh(selfPos + sideways);
    }

    public void SetCommand(string command)
    {
        switch (command)
        {
            case "FOLLOWME":
                currentCommand = Command.Follow;
                break;
            case "MOVENORTH":
                currentCommand = Command.MoveNorth;
                break;
            case "MOVESOUTH":
                currentCommand = Command.MoveSouth;
                break;
            case "MOVEEAST":
                currentCommand = Command.MoveEast;
                break;
            case "MOVEWEST":
                currentCommand = Command.MoveWest;
                break;
            case "HALT":
                currentCommand = Command.Halt;
                Stop();
                break;
            case "FIRE":
                currentCommand = Command.Fire;
                Stop();
                break;
            case "RUN":
                SetRun(true);
                return;
        }
        Debug.Log(currentCommand.ToString());
    }

    private void SetRun(bool value)
    {
        isRunning = value;
    }

    private void HandleFire()
{
    Stop(); // Солдат не должен двигаться во время стрельбы

    currentFireTarget = FindNearestEnemy();

    if (currentFireTarget == null)
    {
        canShoot = false;
        return;
    }

    Vector2 selfPos = transform.position; // более надёжно, чем agent3D.nextPosition
    Vector2 targetPos = currentFireTarget.position;

    float distance = Vector2.Distance(selfPos, targetPos);
    bool hasLOS = HasLineOfSightStable(selfPos, targetPos);

    canShoot = hasLOS && distance <= attackDistance;

    if (gunTransform != null && currentFireTarget != null)
        SmoothLookAtGun(currentFireTarget);
}

    // Новый метод проверки линии огня с учётом стабильности
    private bool HasLineOfSightStable(Vector2 origin, Vector2 target)
    {
        Vector2 dir = (target - origin).normalized;
        float dist = Vector2.Distance(origin, target);
        if (dist < 0.001f) return true;

        // увеличиваем радиус для стабильности
        float checkRadius = Mathf.Max(shootCheckRadius, 0.2f);

        RaycastHit2D hit = Physics2D.CircleCast(origin, checkRadius, dir, dist, obstacleMask);
        return hit.collider == null || hit.transform == currentFireTarget;
    }

    private Transform FindNearestEnemy()
    {
        if (enemyManager == null) return null;


        IReadOnlyList<Transform> enemies = enemyManager.GetAliveEnemies();

        if (enemies == null || enemies.Count == 0) return null;
        Debug.Log(enemies.Count);

        float minDist = float.MaxValue;
        Vector2 selfPos = To2D(agent3D.nextPosition);
        Transform nearest = null;

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;
            float dist = Vector2.Distance(selfPos, enemy.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy;
            }
        }

        return nearest;
    }

    private bool HasLineOfSight(Vector2 origin2D, Vector2 target2D)
    {
        Vector2 dir = (target2D - origin2D).normalized;
        float dist = Vector2.Distance(origin2D, target2D);
        if (dist < 0.001f) return true;

        RaycastHit2D hit = Physics2D.CircleCast(origin2D, shootCheckRadius, dir, dist, obstacleMask);
        if (hit.collider != null && hit.transform != currentFireTarget)
            return false;

        return true;
    }

    private void SmoothLookAtGun(Transform target)
    {
        if (gunTransform == null || target == null) return;

        Vector2 direction = (target.position - gunTransform.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

        gunTransform.rotation = Quaternion.RotateTowards(
            gunTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    protected void FollowPlayer()
    {
        Transform targetPlayer = playerTransform;
        if (targetPlayer == null) return;

        Vector2 selfPos = To2D(agent3D.nextPosition);
        Vector2 playerPos = targetPlayer.position;

        float distance = Vector2.Distance(selfPos, playerPos);

        if (distance <= followDistance)
        {
            if (!agent3D.pathPending && agent3D.hasPath &&
                agent3D.remainingDistance > idleShuffleArrivedThreshold)
            {
                Vector2 dest2D = To2D(agent3D.destination);
                if (Vector2.Distance(dest2D, playerPos) <= followDestinationNearPlayer)
                    Stop();
            }
            else if (!agent3D.pathPending &&
                     (!agent3D.hasPath || agent3D.remainingDistance <= idleShuffleArrivedThreshold))
            {
                Stop();
            }

            return;
        }

        Vector2 target2D = playerPos + wobbleOffset;
        SetDestinationOnNavMesh(target2D);
    }

    protected void Move(Vector2 direction)
    {
        Vector2 selfPos = To2D(agent3D.nextPosition);
        Vector2 target2D = selfPos + direction.normalized * moveDistance;
        target2D += wobbleOffset;

        SetDestinationOnNavMesh(target2D);
    }
    protected void SetDestinationOnNavMesh(Vector2 target2D)
    {
        if (NavMesh.SamplePosition(ToNavMesh(target2D), out NavMeshHit navHit, 2f, NavMesh.AllAreas))
        {
            agent3D.isStopped = false;
            agent3D.SetDestination(navHit.position);
        }
        else
        {
            agent3D.isStopped = false;
            agent3D.SetDestination(ToNavMesh(target2D));
        }
    }

    protected void Stop()
    {
        isRunning = false;
        agent3D.isStopped = true;
    }

    protected new void HandleSpeed()
    {
        float speed = baseSpeed;

        if (isRunning && currentCommand != Command.Halt)
        {
            speed *= runMultiplier;
        }

        agent3D.speed = speed;
    }

    protected new void HandleWobble()
    {
        base.HandleWobble();

        if (isRunning)
            wobbleOffset *= 0.3f;
    }
}