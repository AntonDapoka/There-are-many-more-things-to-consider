using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCScript : MonoBehaviour
{
    [SerializeField] protected Transform playerTransform;

    [Header("Movement")]
    [SerializeField] protected float speedMin = 1.5f;
    [SerializeField] protected float speedMax = 3.5f;
    [SerializeField] protected float speedChangeRate = 2f;

    [Header("Wobble")]
    [SerializeField] protected float wobbleStrength = 1.1f;
    [SerializeField] protected float wobbleChangeInterval = 2f;

    protected NavMeshAgent agent;
    protected float targetSpeed;
    protected Vector2 wobbleOffset;
    protected float wobbleTimer;

    protected bool canShoot = false;

    protected void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        
        targetSpeed = Random.Range(speedMin, speedMax);
        wobbleOffset = Random.insideUnitCircle * wobbleStrength;
    }

    protected void Start()
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

    protected void HandleSpeed()
    {
        if (Random.value < 0.01f)
        {
            targetSpeed = Random.Range(speedMin, speedMax);
        }

        agent.speed = Mathf.Lerp(agent.speed, targetSpeed, Time.deltaTime * speedChangeRate);
    }

    protected void HandleWobble()
    {
        wobbleTimer += Time.deltaTime;

        if (wobbleTimer > wobbleChangeInterval)
        {
            wobbleTimer = 0f;
            wobbleOffset = Random.insideUnitCircle * wobbleStrength;
        }
    }


    protected Vector3 ToNavMesh(Vector2 pos)
    {
        return new Vector3(pos.x, 0f, pos.y);
    }

    protected Vector2 To2D(Vector3 pos)
    {
        return new Vector2(pos.x, pos.z);
    }

    public bool CanShoot() => canShoot;

}
