using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] itemPrefab;
    [SerializeField] float spawnInterval;
    [SerializeField] int maxItems;

    [SerializeField] int currentItemCount;
    [SerializeField] float spawnRadius;

    private float spawnTimer;


    void Update()
    {
        if (currentItemCount < maxItems)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                SpawnItems();
                spawnTimer = spawnInterval;
            }
        }
    }

    void SpawnItems()
    {
        int randomIndex = Random.Range(0, itemPrefab.Length);
        GameObject selectedItemPrefab = itemPrefab[randomIndex];

        Vector3 randomSpawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
        randomSpawnPosition.y = 0;

        GameObject item = Instantiate(selectedItemPrefab, randomSpawnPosition, Quaternion.identity);
        itemPickup itemPickup = item.GetComponent<itemPickup>();

        if (itemPickup != null)
        {
            item.SetActive(true);
            currentItemCount++;
        }
        
    }
}
