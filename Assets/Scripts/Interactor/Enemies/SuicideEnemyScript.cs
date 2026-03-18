using UnityEngine;
using UnityEngine.AI;

public class SuicideEnemyScript : EnemyScript
{
    private void Update()
    {
        if (!agent.isOnNavMesh) return;
        if (playerTransform == null) return;

        HandleSpeed();
        HandleWobble();

        Vector2 target2D = (Vector2)playerTransform.position + wobbleOffset;
        agent.SetDestination(ToNavMesh(target2D));

        Vector3 navPos = agent.nextPosition;
        transform.position = To2D(navPos);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponentInParent<PlayerMarker>() != null)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<EnemyKillZoneMarker>() != null)
        {
            Destroy(gameObject);
        }
    }
}