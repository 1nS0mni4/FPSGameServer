using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public abstract class UIManager : MonoBehaviour{
    public abstract pSceneType Type { get; protected set; }
    private static UIManager _instance = null;

    public virtual void Awake() {
        //TODO: ���⼭ Managers�� ������ UIManager�� ���� UIManager�� �����Ѵ�.
        if(_instance != null) 
            Destroy(_instance.gameObject);

        _instance = this;
    }

    public static T GetUIManager<T>() where T: UIManager {
        return _instance.GetComponent<T>();
    }
}
