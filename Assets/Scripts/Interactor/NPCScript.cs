using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected NavMeshAgent agent3D;  
    [SerializeField] protected Transform[] playerTargets;

    [Header("Movement")]
    [SerializeField] protected float speedMin = 1.5f;
    [SerializeField] protected float speedMax = 3.5f;
    [SerializeField] protected float speedChangeRate = 2f;

    [Header("Wobble")]
    [SerializeField] protected float wobbleStrength = 1.1f;
    [SerializeField] protected float wobbleChangeInterval = 2f;

    protected float targetSpeed;
    protected Vector2 wobbleOffset;
    protected float wobbleTimer;

    public bool canShoot = false;

    protected void Awake()
    {
        if (agent3D != null)
        {
            agent3D.updateRotation = false;
            agent3D.updateUpAxis = false;
        }
    }

    protected virtual void OnEnable()
    {
        targetSpeed = Random.Range(speedMin, speedMax);
        wobbleOffset = Random.insideUnitCircle * wobbleStrength;

        if (agent3D != null)
        {
            Vector3 navPos = ToNavMesh(To2D(transform.position));
            agent3D.Warp(navPos);
        }
    }

    protected void HandleSpeed()
    {
        if (Random.value < 0.01f)
        {
            targetSpeed = Random.Range(speedMin, speedMax);
        }

        agent3D.speed = Mathf.Lerp(agent3D.speed, targetSpeed, Time.deltaTime * speedChangeRate);
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

        
    protected Transform GetNearestPlayer()
    {
        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (var p in playerTargets)
        {
            if (p == null || !p.gameObject.activeSelf) continue;

            float dist = ((Vector2)p.position - (Vector2)transform.position).sqrMagnitude;

            if (dist < minDist)
            {
                minDist = dist;
                nearest = p;
            }
        }

        return nearest;
    }

    protected Vector3 ToNavMesh(Vector2 pos)
    {
        return new Vector3(pos.x, 0f, pos.y);
    }

    protected Vector2 To2D(Vector3 pos)
    {
        return new Vector2(pos.x, pos.z);
    }

    public void SetPlayerTargets(Transform[] targets)
    {
        playerTargets = targets;
    }
}
