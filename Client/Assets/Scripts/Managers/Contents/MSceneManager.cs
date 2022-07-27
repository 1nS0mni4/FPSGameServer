using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MSceneManager : MonoBehaviour {
    private static MSceneManager _instance = null;

    [SerializeField]
    protected pAreaType _areaType = pAreaType.Hideout;
    public pAreaType AreaType { get => _areaType; set { _areaType = value; } }
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
    public abstract void LoadCompleted();
    public static T GetManager<T>() where T: MSceneManager {
        return _instance as T;
    }
    private void OnDestroy() {
        ClearScene();
    }
}
