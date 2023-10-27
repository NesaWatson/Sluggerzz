using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class weaponStats : MonoBehaviour
{
    public float attackRate; 
    public int attackDamage;
    public int attackDistance;
    public int ammoCurr; 
    public int ammoMax;

    public GameObject model;
    public ParticleSystem hitEffect;
    public AudioClip attackSound; 
}
