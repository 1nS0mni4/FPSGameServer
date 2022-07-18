using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public abstract class BaseScene : MonoBehaviour {
    
    public abstract pSceneType SceneType { get; protected set; }
    protected void Awake() {
        Managers.Scene.Manager = this;
        InitScene();
    }

    public abstract void InitScene();
    public abstract void ClearScene();
    private void OnDestroy() {
        ClearScene();
    }
}
