using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SoldierScript : NPCScript
{
    [SerializeField] private Transform playerTransform;

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

    [Header("Follow")]
    [SerializeField] private float followDistance = 2.5f;

    [Header("Movement")]
    [SerializeField] private float baseSpeed = 3.5f;
    [SerializeField] private float runMultiplier = 1.8f;

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
        if (playerTransform == null) return;

        Vector2 selfPos = To2D(agent3D.nextPosition);
        Vector2 playerPos = playerTransform.position;

        float distance = Vector2.Distance(selfPos, playerPos);

        if (distance <= followDistance)
        {
            Stop();
            return;
        }

        Vector2 target = playerPos + wobbleOffset;

        agent3D.isStopped = false;
        agent3D.SetDestination(ToNavMesh(target));
    }

    protected void Move(Vector2 direction)
    {
        Vector2 selfPos = To2D(agent3D.nextPosition);
        Vector2 target = selfPos + direction.normalized * 10f;

        target += wobbleOffset;

        agent3D.isStopped = false;
        agent3D.SetDestination(ToNavMesh(target));
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