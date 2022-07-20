using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public abstract class UIManager : MonoBehaviour{
    [SerializeField]
    protected pSceneType _type;
    public abstract pSceneType Type { get; protected set; }
    private static UIManager _instance = null;

    public virtual void Awake() {
        if(_instance != null) 
            Destroy(_instance.gameObject);

        _instance = this;
    }

    public static T GetManager<T>() where T: UIManager {
        return _instance as T;
    }
}
