using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int enemyCount = 20;

    [Header("Target")]
    [SerializeField] private Transform targetPoint;
    [SerializeField] private Transform player;

    [Header("Spawn Area")]
    [SerializeField] private Vector3 areaCenter = new Vector3(0f, 0.5f, 15f);
    [SerializeField] private Vector3 areaSize = new Vector3(20f, 1f, 10f);

    [Header("Spacing")]
    [SerializeField] private float minDistanceBetweenEnemies = 2f;
    [SerializeField] private int maxPlacementAttemptsPerEnemy = 30;

    [Header("Timing")]
    [SerializeField] private float spawnDelay = 1.5f;

    private List<Vector3> usedPositions = new List<Vector3>();

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        int spawned = 0;

        for (int i = 0; i < enemyCount; i++)
        {
            bool placed = false;

            for (int attempt = 0; attempt < maxPlacementAttemptsPerEnemy; attempt++)
            {
                Vector3 pos = GetRandomPositionInArea();

                if (IsFarEnough(pos))
                {
                    GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);

                    SetupEnemy(enemy);

                    usedPositions.Add(pos);
                    spawned++;
                    placed = true;
                    break;
                }
            }

            if (!placed)
            {
                Debug.LogWarning("Could not find valid position for enemy.");
            }

            yield return new WaitForSeconds(spawnDelay);
        }

        Debug.Log($"Spawned enemies: {spawned}/{enemyCount}");
    }

    private void SetupEnemy(GameObject enemy)
    {
        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();

        if (movement != null)
        {
            movement.defaultTarget = targetPoint;
            movement.player = player;          

        }
    }

    private Vector3 GetRandomPositionInArea()
    {
        float x = Random.Range(areaCenter.x - areaSize.x * 0.5f,
                               areaCenter.x + areaSize.x * 0.5f);

        float z = Random.Range(areaCenter.z - areaSize.z * 0.5f,
                               areaCenter.z + areaSize.z * 0.5f);

        return new Vector3(x, areaCenter.y, z);
    }

    private bool IsFarEnough(Vector3 candidate)
    {
        foreach (var pos in usedPositions)
        {
            if (Vector3.Distance(candidate, pos) < minDistanceBetweenEnemies)
                return false;
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(areaCenter, areaSize);
    }
}