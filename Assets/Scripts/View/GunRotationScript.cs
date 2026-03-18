using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRotationScript : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform[] gunTransforms;

    private void Update()
    {
        if (playerTransform == null || gunTransforms == null) return;

        foreach (Transform gun in gunTransforms)
        {
            if (gun == null) continue;
            
            Vector2 direction = playerTransform.position - gun.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            gun.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}

