using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class weaponStats : ScriptableObject
{
    public float shootSpeed; 
    public int gunDamage;
    public int fireDistance;
    //public int ammoCurr; 
    //public int ammoMax;

    public GameObject model;
    public string weaponName;
    public ParticleSystem hitEffect;
    public AudioClip shootSound; 
}
