using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MSceneManager : MonoBehaviour {
    #region Reference-Type Variables
    private SceneController _sceneController;
    #endregion

    #region Value-Type Variables
    [SerializeField] private pAreaType _areaType;
                    
    #endregion

    #region Properties
    public pAreaType AreaType { get => _areaType; }
    #endregion

    #region Unity Event Functions
    protected void Awake() {
        _sceneController = Managers.Scene;
        if(_sceneController.Manager != null)
            Destroy(_sceneController.Manager);

        _sceneController.Manager = this;

        OnAwakeEvent();
    }

    protected virtual void Start() {
        _sceneController.IsSceneChanging = false;
        OnStartEvent();
    }

    private void OnDestroy() {
        OnDestroyEvent();
    }

    #endregion

    #region Abstract Functions
    public abstract void OnAwakeEvent();
    public abstract void OnStartEvent();
    public abstract void OnDestroyEvent();
    public abstract void OnLoadCompleted();
    #endregion

}
