using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideEnemyPresenterScript : EnemyPresenterScript
{
    [Header("Effect Settings")]
    [SerializeField] private ParticleSystem explosionEffect;
    [SerializeField] private AudioClip soundExplosion;

    public void PlaySuicideEffect(Vector3 effectPosition)
    {
        if (enemyView == null)
        {
            Debug.LogWarning("View is not assigned!");
            return;
        }

        enemyView.PlayEffect(explosionEffect, effectPosition);
        enemyView.PlaySound(soundExplosion);
    }
}
