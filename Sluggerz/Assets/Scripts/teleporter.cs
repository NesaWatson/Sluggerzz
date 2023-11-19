using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleporter : MonoBehaviour
{
    public Transform player, Des;
    public GameObject players;
    public guardEnemy guard;
    public AudioClip audioClip;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && guard == null)
        {
            if(guard == null || !guard.gameObject.activeSelf)
            {
                players.SetActive(false);
                player.position = Des.position;
                players.SetActive(true);
            }
            
        }
    }
    public void UpdateGuardReference(guardEnemy newGuard)
    {
        guard = newGuard;
    }
}
