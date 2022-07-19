using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public abstract class UIManager : MonoBehaviour{
    public abstract pSceneType Type { get; protected set; }
    private static UIManager _instance = null;

    public virtual void Awake() {
        //TODO: 여기서 Managers가 가지는 UIManager를 현재 UIManager로 수정한다.
        if(_instance != null) 
            Destroy(_instance.gameObject);

        _instance = this;
    }

    public static T GetUIManager<T>() where T: UIManager {
        return _instance.GetComponent<T>();
    }
}
