using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public abstract class MSceneManager : MonoBehaviour {
    private static MSceneManager _instance = null;
    public static bool IsInitialized { get; protected set; } = false;

    public abstract pSceneType SceneType { get; protected set; }
    protected void Awake() {
        if(_instance != null) {
            _instance.ClearScene();
            Destroy(_instance.gameObject);
        }

        _instance = this;
        InitScene();
    }

    public abstract void InitScene();
    public abstract void ClearScene();
    public static T GetManager<T>() where T: MSceneManager {
        return _instance as T;
    }
    private void OnDestroy() {
        ClearScene();
    }
}
