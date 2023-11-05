using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAlertSystem : MonoBehaviour
{
    public static enemyAlertSystem instance;

    private void Awake()
    {
        instance = this;
    }
    public void AlertEnemies(Vector3 playerPos)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            iAlertable alertable = enemy.GetComponent<iAlertable>();    
            if (alertable != null)
            {
                alertable.Alert(playerPos);
            }
        }
    }
   
}
