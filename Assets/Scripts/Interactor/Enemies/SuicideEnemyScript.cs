using UnityEngine;

public class SuicideEnemyScript : EnemyScript
{
    [SerializeField] private Transform[] playerTransforms;

    private void Update()
    {
        if (!agent.isOnNavMesh) return;
        if (playerTransforms.Length == 0) return;

        Transform nearestPlayer = GetNearestPlayer();
        if (nearestPlayer == null) return;

        HandleSpeed();
        HandleWobble();

        Vector2 target2D = (Vector2)nearestPlayer.position;
        agent.SetDestination(ToNavMesh(target2D));

        Vector3 navPos = agent.nextPosition;
        transform.position = To2D(navPos);
    }

    private Transform GetNearestPlayer()
    {
        Transform nearest = null;
        float minDistSq = float.MaxValue;

        foreach (var player in playerTransforms)
        {
            if (player == null) continue;
            float distSq = ((Vector2)player.position - To2D(transform.position)).sqrMagnitude;
            if (distSq < minDistSq)
            {
                minDistSq = distSq;
                nearest = player;
            }
        }

        return nearest;
    }
}