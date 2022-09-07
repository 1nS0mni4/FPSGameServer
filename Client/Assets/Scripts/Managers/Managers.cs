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
public interface IManagerOnApplicationPause { public void OnApplicationPause(bool pause); }
public interface IManagerOnApplicationQuit { public void OnApplicationQuit(); }

#endregion

public class Manager { }

public class Managers : MonoBehaviour {
    #region Singleton_Unity
    private static Managers _instance;
    private static Managers Instance {
        get {
            if(_instance == null)
                throw new System.Exception("Managers\' Instance not Instantiated!");

            return _instance;
        }
    }
    #endregion

    #region Manager Instances
    public Action ManagerStart;
    public Action ManagerUpdate;
    public Action<bool> ManagerOnApplicationPause;
    public Action ManagerOnApplicationQuit;

    public List<string> AddedManager = new List<string>();
    #endregion

    #region Game Instances
    private volatile pAreaType _prevScene = pAreaType.Hideout;
    public static pAreaType CurArea { get => _instance._prevScene; set => _instance._prevScene = value; }

    #endregion


    #region Primitive Managers
    private NetworkManager _network = new NetworkManager();
    public static NetworkManager Network { get => _instance._network; }

#if UNITY_CLIENT_FPS
    private InputManager _input = new InputManager();
    public static InputManager Input { get => _instance._input; }
#endif

    #endregion

    private void BatchRegister() {
        Register<NetworkManager>(_network);

#if UNITY_CLIENT_FPS
        Register<InputManager>(_input);
#endif

    }


    #region MonoBehaviour Managers
    private SceneController _scene;
    public static SceneController Scene { get => _instance._scene; }

    private InGameSceneManager _inGame = null;
    public static InGameSceneManager InGame { get => _instance._inGame; }
    #endregion


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
        if(ManagerUpdate != null)
            ManagerUpdate.Invoke();


    }

    private void OnApplicationPause(bool pause) {
        if(ManagerOnApplicationPause != null)
            ManagerOnApplicationPause.Invoke(pause);
    }

    private void OnApplicationQuit() {
        if(ManagerOnApplicationQuit != null)
            ManagerOnApplicationQuit.Invoke();
    }

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