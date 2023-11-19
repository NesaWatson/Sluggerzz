using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu]

public class weaponStats : ScriptableObject
{
    public float shootSpeed; 
    public int gunDamage;
    public int fireDistance;

    public GameObject model;
    public string weaponName;
    public ParticleSystem hitEffect;
    public AudioClip weaponAudio;
   
}

