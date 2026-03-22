using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent3D;  
    [SerializeField] private Transform[] playerTargets;

    [Header("Movement")]
    [SerializeField] private float speedMin = 1.5f;
    [SerializeField] private float speedMax = 3.5f;
    [SerializeField] private float speedChangeRate = 2f;

    [Header("Wobble")]
    [SerializeField] private float wobbleStrength = 1.1f;
    [SerializeField] private float wobbleChangeInterval = 2f;

    private float targetSpeed;
    private Vector2 wobbleOffset;
    private float wobbleTimer;

    private void Awake()
    {
        targetSpeed = Random.Range(speedMin, speedMax);
        wobbleOffset = Random.insideUnitCircle * wobbleStrength;

        agent3D.updateRotation = false;
        agent3D.updateUpAxis = false;
    }

    private void Start()
    {
        Vector3 startPos = ToNavMesh(transform.position);
        agent3D.Warp(startPos);
    }

    private void Update()
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

    private Transform GetNearestPlayer()
    {
        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (var p in playerTargets)
        {
            if (p == null) continue;

            float dist = ((Vector2)p.position - (Vector2)transform.position).sqrMagnitude;

            if (dist < minDist)
            {
                minDist = dist;
                nearest = p;
            }
        }

        return nearest;
    }

    private void HandleSpeed()
    {
        if (Random.value < 0.01f)
        {
            targetSpeed = Random.Range(speedMin, speedMax);
        }

        agent3D.speed = Mathf.Lerp(agent3D.speed, targetSpeed, Time.deltaTime * speedChangeRate);
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

    private Vector3 ToNavMesh(Vector2 pos)
    {
        return new Vector3(pos.x, 0f, pos.y);
    }

    private Vector2 To2D(Vector3 pos)
    {
        return new Vector2(pos.x, pos.z);
    }
}