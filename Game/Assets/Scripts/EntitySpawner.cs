using System.Collections;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    [SerializeField] private GameObject entityPrefab;
    // Time
    [SerializeField] private float spawnInterval = 1f;
    // Position
    [SerializeField] private Vector3 spawnPosition = new(0, 0, 0);

    private bool _isSpawning = true;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (_isSpawning)
        {
            SpawnEntity();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEntity()
    {
        Instantiate(entityPrefab, spawnPosition, Quaternion.identity);
    }
}