using UnityEngine;

public class SuicideEnemyColliderScript : MonoBehaviour
{
    public EnemyPresenterScript enemyPresenter;
    [SerializeField] private GameObject enemy;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponentInParent<PlayerMarker>() != null || collision.collider.GetComponentInParent<SoldierMarker>() != null)
        {
            var suicideEnemyPresenter = enemyPresenter as SuicideEnemyPresenterScript;
            if (suicideEnemyPresenter != null)
            {
                suicideEnemyPresenter.PlaySuicideEffect(transform.position);
            }
            Destroy(enemy);
            //
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ( other.GetComponentInParent<PlayerMarker>() != null || other.GetComponentInParent<SoldierMarker>() != null)
        {
            var suicideEnemyPresenter = enemyPresenter as SuicideEnemyPresenterScript;
            if (suicideEnemyPresenter != null)
            {
                suicideEnemyPresenter.PlaySuicideEffect(transform.position);
                Debug.Log("dead1");
            }
            Destroy(enemy);
            //Debug.Log("dead1");
        }
    }
}
