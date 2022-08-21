using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MSceneManager : MonoBehaviour {
    [SerializeField] protected pAreaType _areaType = pAreaType.Hideout;
                     public pAreaType AreaType { get => _areaType; 
                                                 set { _areaType = value; } 
                     }
    protected void Awake() {
        MSceneManager manager = Managers.Scene.Manager;
        if(manager != null)
            manager.ClearScene();

        Managers.Scene.Manager = this;
        Managers.CurArea = AreaType;
        
        InitScene();
    }

    protected virtual void Start() {
        Managers.Scene.IsSceneChanging = false;
    }

    public abstract void InitScene();
    public abstract void ClearScene();
    public abstract void LoadCompleted();
    private void OnDestroy() {
        ClearScene();
    }
}
