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

    #region Primitive Managers
    private NetworkManager _network = new NetworkManager();
    public static NetworkManager Network { get => Instance._network; }

    private InputManager _input = new InputManager();
    public static InputManager Input { get => Instance._input; }

    #endregion

    private void BatchRegister() {
        Register<NetworkManager>(_network);
        Register<InputManager>(_input);
    }


    #region MonoBehaviour Managers
    private SceneController _scene;
    public static SceneController Scene { get => Instance._scene; }

    //private BaseUI _ui;
    //public static BaseUI UI { get => Instance._ui;
    //    set {
    //        Instance._ui = value;
    //    }
    //}

    //private ObjectManager _object;
    //public static ObjectManager Object { get => Instance._object; set {
    //        Instance._object = value;
    //    }
    //}

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
