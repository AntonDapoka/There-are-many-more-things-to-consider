using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideEnemyColliderScript : MonoBehaviour
{
    [SerializeField] protected EnemyPresenterScript enemyPresenter;
    [SerializeField] private SuicideEnemyScript enemy;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponentInParent<PlayerMarker>() != null || collision.collider.GetComponentInParent<SoldierMarker>() != null)
        {
            var suicideEnemyPresenter = enemyPresenter as SuicideEnemyPresenterScript;
            if (suicideEnemyPresenter != null)
            {
                suicideEnemyPresenter.PlaySuicideEffect(transform.position);
            }
            Debug.Log("Here");
            Destroy(enemy.gameObject);
        }
    }
/*
    private void OnTriggerEnter2D(Collider2D other)
    {
        if ( other.GetComponentInParent<PlayerMarker>() != null || other.GetComponentInParent<SoldierMarker>() != null)
        {
            var suicideEnemyPresenter = enemyPresenter as SuicideEnemyPresenterScript;
            if (suicideEnemyPresenter != null)
            {
                suicideEnemyPresenter.PlaySuicideEffect(transform.position);
            }
            Debug.Log("NO Here");
            Destroy(enemy.gameObject);
        }
    }*/
}
