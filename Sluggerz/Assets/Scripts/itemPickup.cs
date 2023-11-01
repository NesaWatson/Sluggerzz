using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemPickup : MonoBehaviour
{
    public enum PickupType
    {
        Health,
        Shield,
        Stamina
    }

    public PickupType type;
    public int amount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController player = other.GetComponent<playerController>();

            if(player != null)
            {
                switch (type)
                {
                    case PickupType.Health:
                        player.giveHP(amount); 
                        break;
                    case PickupType.Shield:
                        player.giveShield(amount);
                        break;
                    case PickupType.Stamina:
                        player.giveStamina(amount);
                        break;
                }
                gameObject.SetActive(false);
            }
        }
    }
}
