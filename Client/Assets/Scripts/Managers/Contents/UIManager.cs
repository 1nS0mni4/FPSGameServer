using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public abstract class UIManager : MonoBehaviour{
    private static UIManager _instance = null;

    public virtual void Awake() {
        if(_instance != null && _instance != this) 
            Destroy(_instance.gameObject);

        _instance = this;
    }

    public static T GetManager<T>() where T: UIManager {
        return _instance as T;
    }
}
