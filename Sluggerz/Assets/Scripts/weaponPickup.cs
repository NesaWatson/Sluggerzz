using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

            if (SceneManager.GetActiveScene().name == "Tutorial Scene")
            { 
                tutorialManager.UpdateTutorialSteps(2); 
            }
        }
    }

}
