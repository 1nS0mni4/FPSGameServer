using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


#region Manager Interfaces

public interface IManagerStart { public void Start(); }
public interface IManagerUpdate { public void Update(); }
public interface IManagerFixedUpdate { public void FixedUpdate(); }
public interface IManagerOnApplicationPause { public void OnApplicationPause(bool pause); }
public interface IManagerOnApplicationQuit { public void OnApplicationQuit(); }

#endregion

public class Manager { }

public class Managers : MonoBehaviour {
    #region Singleton
    private static Managers _instance;
    public static Managers Instance {
        get { return _instance; }
    }
    #endregion

    #region Manager Event Functions
    private Action ManagerStart;
    private Action ManagerUpdate;
    private Action ManagerFixedUpdate;
    private Action<bool> ManagerOnApplicationPause;
    private Action ManagerOnApplicationQuit;

    public List<string> AddedManager = new List<string>();
    #endregion

    #region Game Instances
    public static pAreaType CurArea { get => _instance._scene.Manager.AreaType; }

    private bool _canInput = false;
    public static bool CanInput { get => _instance._canInput; set { _instance._canInput = value; } }

    #endregion

    #region Primitive Managers
    private NetworkManager _network = new NetworkManager();
    public static NetworkManager Network { get => _instance._network; }

    private void BatchRegister() {
        Register<NetworkManager>(_network);
    }

    #endregion

    #region MonoBehaviour Managers
    private SceneController _scene;
    public static SceneController Scene { get => _instance._scene; }

    private InGameSceneManager _inGame = null;
    public static InGameSceneManager InGame { get => _instance._inGame; }

    #endregion

    #region Unity Event Functions
    private void Awake() {
        #region Singleton
        if(_instance != null) {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        #endregion

        _scene = GetComponent<SceneController>();
        BatchRegister();
    }

    private void Update() {
        ManagerUpdate?.Invoke();
    }

    private void FixedUpdate() {
        ManagerFixedUpdate?.Invoke();
    }

    private void OnApplicationPause(bool pause) {
        ManagerOnApplicationPause?.Invoke(pause);
    }

    private void OnApplicationQuit() {
        ManagerOnApplicationQuit?.Invoke();
    }

    #endregion

    private void Register<T>(T manager) {
        AddedManager.Add(manager.GetType().Name);

        IManagerStart start = manager as IManagerStart;
        if(start != null)
            start.Start();

        IManagerUpdate update = manager as IManagerUpdate;
        if(update != null) {
            ManagerUpdate -= update.Update;
            ManagerUpdate += update.Update;
        }

        IManagerFixedUpdate fixedUpdate = manager as IManagerFixedUpdate;
        if(fixedUpdate != null) {
            ManagerFixedUpdate -= fixedUpdate.FixedUpdate;
            ManagerFixedUpdate += fixedUpdate.FixedUpdate;
        }

        IManagerOnApplicationPause pause = manager as IManagerOnApplicationPause;
        if(pause != null) {
            ManagerOnApplicationPause -= pause.OnApplicationPause;
            ManagerOnApplicationPause += pause.OnApplicationPause;
        }

        IManagerOnApplicationQuit quit = manager as IManagerOnApplicationQuit;
        if(quit != null) {
            ManagerOnApplicationQuit -= quit.OnApplicationQuit;
            ManagerOnApplicationQuit += quit.OnApplicationQuit;
        }
    }
}