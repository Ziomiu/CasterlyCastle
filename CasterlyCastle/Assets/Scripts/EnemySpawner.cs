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

    [Header("Spawn Area")]
    [SerializeField] private Vector3 areaCenter = new Vector3(0f, 0.5f, 15f);
    [SerializeField] private Vector3 areaSize = new Vector3(20f, 1f, 10f);

    [Header("Spacing")]
    [SerializeField] private float minDistanceBetweenEnemies = 2f;
    [SerializeField] private int maxPlacementAttemptsPerEnemy = 30;

    [Header("Timing")]
    [SerializeField] private float spawnDelay = 1.5f;

    private readonly List<Vector3> usedPositions = new List<Vector3>();

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
        transform.position = areaCenter;
        transform.localScale = areaSize;
    }

    private IEnumerator SpawnEnemies()
    {
        int spawned = 0;

        for (int i = 0; i < enemyCount; i++)
        {
            bool placed = false;

            for (int attempt = 0; attempt < maxPlacementAttemptsPerEnemy; attempt++)
            {
                Vector3 randomPosition = GetRandomPositionInArea();

                if (IsFarEnough(randomPosition))
                {
                    GameObject enemy = Instantiate(enemyPrefab, randomPosition, Quaternion.identity);

                    EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
                    if (movement != null)
                    {
                        movement.target = targetPoint;
                    }

                    usedPositions.Add(randomPosition);

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

        Debug.Log("Spawned enemies: " + spawned + "/" + enemyCount);
    }

    private Vector3 GetRandomPositionInArea()
    {
        float randomX = Random.Range(
            areaCenter.x - areaSize.x * 0.5f,
            areaCenter.x + areaSize.x * 0.5f
        );

        float randomZ = Random.Range(
            areaCenter.z - areaSize.z * 0.5f,
            areaCenter.z + areaSize.z * 0.5f
        );

        float y = areaCenter.y;

        return new Vector3(randomX, y, randomZ);
    }

    private bool IsFarEnough(Vector3 candidatePosition)
    {
        foreach (Vector3 usedPosition in usedPositions)
        {
            float distance = Vector3.Distance(candidatePosition, usedPosition);

            if (distance < minDistanceBetweenEnemies)
            {
                return false;
            }
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(areaCenter, areaSize);
    }
}