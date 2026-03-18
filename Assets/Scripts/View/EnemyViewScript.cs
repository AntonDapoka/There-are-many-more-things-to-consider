using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyViewScript : MonoBehaviour
{
    [SerializeField] protected AudioSource audioSource;

    public void PlayEffect(ParticleSystem effectPrefab, Vector3 targetPoint)
    {

        if (effectPrefab == null || targetPoint == null)
        {
            Debug.LogWarning("Effect or targetPoint is null!");
            return;
        }

        ParticleSystem effectInstance = Instantiate(effectPrefab,targetPoint,Quaternion.identity);

        effectInstance.Play();
        Debug.Log("EFFECT PLAYED");
        Destroy(effectInstance.gameObject, effectInstance.main.duration);
    }

    public void PlaySound(AudioClip sound)
    {
        if (audioSource != null && sound != null)
        {
            audioSource.PlayOneShot(sound);
        }
    }
}
