using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManagerScript : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject shooterEnemyPrefab;
    [SerializeField] private GameObject suicideEnemyPrefab;

    [Header("Targets")]
    [Tooltip("Player and soldiers - enemies will receive this array as targets")]
    [SerializeField] private Transform[] friendlies;

    [Header("Spawn Area")]
    [SerializeField] private float minSpawnDistance = 12f;
    [SerializeField] private float maxSpawnDistance = 25f;
    [SerializeField] private int spawnAttemptsPerTick = 10;

    [Header("Spawn Rate (increases over time)")]
    [SerializeField] private float initialSpawnInterval = 5f;
    [SerializeField] private float minSpawnInterval = 1f;
    [SerializeField] private float spawnIntervalDecayTime = 120f;

    [Header("Limit")]
    [SerializeField] private int maxEnemies = 30;

    private float spawnTimer;
    private readonly List<GameObject> spawnedEnemies = new();

    private void Start()
    {
        spawnTimer = 0f;
    }

    private void Update()
    {
        CleanupDeadEnemies();

        if (spawnedEnemies.Count >= maxEnemies) return;
        if (friendlies == null || friendlies.Length == 0) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer > 0f) return;

        TrySpawnEnemy();
        spawnTimer = GetCurrentSpawnInterval();
    }

    private float GetCurrentSpawnInterval()
    {
        float t = Mathf.Clamp01(Time.timeSinceLevelLoad / spawnIntervalDecayTime);
        return Mathf.Lerp(initialSpawnInterval, minSpawnInterval, t);
    }

    private void TrySpawnEnemy()
    {
        Vector2? spawnPos = FindValidSpawnPosition();
        if (!spawnPos.HasValue) return;

        GameObject prefab = Random.value < 0.5f ? shooterEnemyPrefab : suicideEnemyPrefab;
        if (prefab == null) return;

        Vector3 pos = new Vector3(spawnPos.Value.x, 0f, spawnPos.Value.y);
        GameObject enemy = Instantiate(prefab, pos, Quaternion.identity);

        if (enemy.TryGetComponent<NPCScript>(out var npc))
        {
            npc.SetPlayerTargets(friendlies);
        }

        spawnedEnemies.Add(enemy);
    }

    private Vector2? FindValidSpawnPosition()
    {
        Vector2 center = GetFriendliesCenter();
        Transform[] validFriendlies = GetValidFriendlies();

        for (int i = 0; i < spawnAttemptsPerTick; i++)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float dist = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector2 candidate = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;

            if (!IsOnNavMesh(candidate)) continue;
            if (!IsFarFromAll(candidate, validFriendlies)) continue;

            return candidate;
        }

        return null;
    }

    private Vector2 GetFriendliesCenter()
    {
        if (friendlies == null || friendlies.Length == 0)
            return (Vector2)transform.position;

        Vector2 sum = Vector2.zero;
        int count = 0;
        foreach (var t in friendlies)
        {
            if (t == null) continue;
            sum += (Vector2)t.position;
            count++;
        }
        return count > 0 ? sum / count : (Vector2)transform.position;
    }

    private Transform[] GetValidFriendlies()
    {
        if (friendlies == null) return System.Array.Empty<Transform>();

        var list = new List<Transform>();
        foreach (var t in friendlies)
        {
            if (t != null) list.Add(t);
        }
        return list.ToArray();
    }

    private bool IsOnNavMesh(Vector2 pos)
    {
        Vector3 navPos = new Vector3(pos.x, 0f, pos.y);
        return NavMesh.SamplePosition(navPos, out _, 1.5f, NavMesh.AllAreas);
    }

    private bool IsFarFromAll(Vector2 pos, Transform[] targets)
    {
        foreach (var t in targets)
        {
            if (t == null) continue;
            float dist = Vector2.Distance(pos, (Vector2)t.position);
            if (dist < minSpawnDistance) return false;
        }
        return true;
    }

    private void CleanupDeadEnemies()
    {
        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            if (spawnedEnemies[i] == null)
                spawnedEnemies.RemoveAt(i);
        }
    }

    /// <summary>
    /// Returns transforms of all alive enemies. Used by soldiers for FIRE command.
    /// </summary>
    public IReadOnlyList<Transform> GetAliveEnemies()
    {
        var list = new List<Transform>();
        foreach (var go in spawnedEnemies)
        {
            if (go != null)
                list.Add(go.transform);
        }
        return list;
    }
}
