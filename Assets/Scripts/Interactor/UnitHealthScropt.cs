using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHealthScropt : MonoBehaviour
{
    public int maxHP = 100;
    public GameObject character;
    private int currentHP;

    public bool isSoldier = false;

    void Awake()
    {
        currentHP = maxHP;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<BulletScript>() != null)
        {
            TakeDamage(25);
        }

        if (isSoldier && other.GetComponent<SuicideEnemyColliderScript>() != null)
        {
            TakeDamage(9999);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.GetComponent<BulletScript>() != null)
        {
            TakeDamage(25);
        }

        if (isSoldier && other.collider.GetComponent<SuicideEnemyColliderScript>() != null)
        {
            TakeDamage(9999);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isSoldier)
        {
            character.SetActive(false);
            currentHP = maxHP;
        }
        else
        {
            Destroy(character);
        }
    }
}