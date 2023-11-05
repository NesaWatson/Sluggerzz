using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRuntimeData
{
    public weaponStats config;
    public int ammoCur;

    public void RefillAmmo()
    {
        ammoCur = config.ammoMax;
    }
}

