using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvalOfficeColliderScript : MonoBehaviour
{
    public bool isTurnOn = true;
    [Header("Ellipse Settings")]
    [SerializeField] private Transform center;
    [SerializeField] private float radiusX = 5f;
    [SerializeField] private float radiusY = 3f;
    [Header("Target")]
    [SerializeField] private Transform player;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = player.gameObject.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 pos = rb.position;

        float dx = pos.x - center.position.x;
        float dy = pos.y - center.position.y;

        float value = (dx * dx) / (radiusX * radiusX) +
                    (dy * dy) / (radiusY * radiusY);

        if (value > 1f)
        {
            float scale = Mathf.Sqrt(value);
            dx /= scale;
            dy /= scale;

            pos.x = center.position.x + dx;
            pos.y = center.position.y + dy;

            rb.position = pos;
        }
    }
}