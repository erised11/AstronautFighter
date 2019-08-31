using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    public static WeaponStats instance;

    public int startingDamage;
    private int _curDamage;
    public int curDamage {
        get { return _curDamage; }
    }

    public void Awake() {

        if (instance == null) {
            instance = this;
        }
        _curDamage = startingDamage;
    }

}
