using UnityEngine;

public class SuicideEnemyScript : EnemyScript
{
    protected void Update()
    {
        if (!agent3D.isOnNavMesh) return;
        if (playerTargets.Length == 0) return;

        Transform target = GetNearestPlayer();
        if (target == null) return;

        HandleSpeed();
        HandleWobble();

        Vector2 target2D = (Vector2)target.position + wobbleOffset;

        agent3D.SetDestination(ToNavMesh(target2D));

        transform.position = To2D(agent3D.nextPosition);
    }

}