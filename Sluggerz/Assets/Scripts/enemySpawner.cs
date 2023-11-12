using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public float waveTimer = 10f;

    void Start()
    {
        StartCoroutine(spawnWaves());
    }
    
    IEnumerator spawnWaves()
    {
        while (true)
        {
            yield return new WaitForSeconds(waveTimer);

            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
