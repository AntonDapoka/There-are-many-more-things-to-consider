using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCScript))]
public class ShootingScript : MonoBehaviour
{
    [Header("Gun Settings")]
    private NPCScript shooter;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float fireRate = 0.5f;

    private float fireCooldown;

   private void Start()
   {
      shooter = gameObject.GetComponent<NPCScript>();
   }

   private void Update()
    {
        if (shooter == null || bulletPrefab == null || firePoint == null) return;

        fireCooldown -= Time.deltaTime;

        if (shooter.CanShoot() && fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = fireRate;
        }
    }

    private void Shoot()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        BulletScript bullet = bulletObj.GetComponent<BulletScript>();
        bullet.Initialize(firePoint.right * bulletSpeed);
    }
}
