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
        Halt
    }

    [Header("References")]
    [SerializeField] private Transform playerTransform;

    [Header("Follow")]
    [SerializeField] private float followDistance = 2.5f;

    [Header("Movement")]
    [SerializeField] private float baseSpeed = 3.5f;
    [SerializeField] private float runMultiplier = 1.8f;
    [SerializeField] private float moveDistance = 10f;

    protected Command currentCommand = Command.None;
    protected bool isRunning = false;

    protected void Update()
    {
        if (!agent3D.isOnNavMesh) return;

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
        }

        transform.position = To2D(agent3D.nextPosition);
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

    protected void FollowPlayer()
    {
        Transform targetPlayer = playerTransform;
        if (targetPlayer == null) return;

        Vector2 selfPos = To2D(agent3D.nextPosition);
        Vector2 playerPos = targetPlayer.position;

        float distance = Vector2.Distance(selfPos, playerPos);

        if (distance <= followDistance)
        {
            Stop();
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