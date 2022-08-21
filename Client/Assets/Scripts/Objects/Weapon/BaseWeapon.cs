using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SelectiveFire {
    SemiAuto,
    Burst,
    FullAuto
}

public class BaseWeapon : MonoBehaviour {
    [Header("Weapon Settings")]
    public List<SelectiveFire> selectorType = new List<SelectiveFire>();
    [SerializeField]
    private SelectiveFire _currentSelecter = SelectiveFire.SemiAuto;
    public SelectiveFire Selector {
        get => _currentSelecter;
        set => _currentSelecter = value;
    }


    public float fireRate = 0.0f;
    private float lastFiredTick = 0.0f;




    public GameObject _magazine = null;



    public void Reload() {

    }

    private IEnumerator CoStartReload() {
        yield return null;
    }

    public void Fire() {

    }

    private IEnumerator CoStartFire() {
        while(true) {
            Fire();
            
        }
    }
}
