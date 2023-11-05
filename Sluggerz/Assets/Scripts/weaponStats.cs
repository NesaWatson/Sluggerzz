using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class weaponStats : ScriptableObject
{
    public float attackRate; 
    public int attackDamage;
    public int attackDistance;
    public int ammoCurr; 
    public int ammoMax;

    public GameObject model;
    public string weaponName;
    public ParticleSystem hitEffect;
    public AudioClip attackSound; 
}
