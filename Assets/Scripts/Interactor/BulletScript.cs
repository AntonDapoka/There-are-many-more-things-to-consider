using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BulletScript : MonoBehaviour
{
    private Vector2 velocity;
    [SerializeField] private float lifetime = 3f;

    public void Initialize(Vector2 velocity)
    {
        this.velocity = velocity;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += (Vector3)(velocity * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.GetComponentInParent<PlayerMarker>() || other.collider.GetComponentInParent<SoldierMarker>() || other.collider.GetComponentInParent<ObstacleMarker>())
        {
            Destroy(gameObject);
        }
    }
}