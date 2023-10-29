using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class battleRoyalManager : MonoBehaviour
{
    public GameObject playerPrefab; 
    public GameObject enemyPrefab;
    public int numOfPlayers;
    public int numOfEnemies; 
    public int maxPlayers;
    void Start()
    {
        spawnPlayer();

        for(int n = 0; n < numOfEnemies; n++)
        {
            spawnEnemy();
        }
    }
    void Update()
    {
        
    }
    private void spawnPlayer()
    {
        Instantiate(playerPrefab, getRandomSpawnPoint(), Quaternion.identity);
    }
    private void spawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, getRandomSpawnPoint(), Quaternion.identity);
    }
    private Vector3 getRandomSpawnPoint()
    {
        Vector3 randomSpawnerPoint = Vector3.zero;
        return randomSpawnerPoint;
    }
}
