using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponPickup : MonoBehaviour
{
    [SerializeField] weaponStats weapon;
    tutorialManager tutorialManager;

    void Start()
    {
        tutorialManager = FindObjectOfType<tutorialManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.weaponPickup(weapon);
            Destroy(gameObject);
            tutorialManager.UpdateTutorialSteps(2);
        }
    }

}
