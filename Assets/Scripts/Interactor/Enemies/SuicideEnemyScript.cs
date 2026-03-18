using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SuicideEnemyScript : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    [Header("Movement")]
    [SerializeField] private float speedMin = 1.5f;
    [SerializeField] private float speedMax = 3.5f;
    [SerializeField] private float speedChangeRate = 2f;

    [Header("Wobble")]
    [SerializeField] private float wobbleStrength = 1.1f;
    [SerializeField] private float wobbleChangeInterval = 2f;

    private NavMeshAgent agent;
    private float targetSpeed;
    private Vector2 wobbleOffset;
    private float wobbleTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        
        targetSpeed = Random.Range(speedMin, speedMax);
        wobbleOffset = Random.insideUnitCircle * wobbleStrength;
    }

    private void Start()
    {
        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(ToNavMesh(transform.position), out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }
        else
        {
            agent.Warp(ToNavMesh(transform.position));
        }
    }

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

    private Vector3 ToNavMesh(Vector2 pos)
    {
        return new Vector3(pos.x, 0f, pos.y);
    }

    private Vector2 To2D(Vector3 pos)
    {
        return new Vector2(pos.x, pos.z);
    }

    private void HandleSpeed()
    {
        if (Random.value < 0.01f)
        {
            targetSpeed = Random.Range(speedMin, speedMax);
        }

        agent.speed = Mathf.Lerp(agent.speed, targetSpeed, Time.deltaTime * speedChangeRate);
    }

    private void HandleWobble()
    {
        wobbleTimer += Time.deltaTime;

        if (wobbleTimer > wobbleChangeInterval)
        {
            wobbleTimer = 0f;
            wobbleOffset = Random.insideUnitCircle * wobbleStrength;
        }
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