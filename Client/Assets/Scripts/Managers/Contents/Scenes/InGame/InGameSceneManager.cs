using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class InGameSceneManager : MSceneManager {
    public override pSceneType SceneType { get; protected set; }
    [SerializeField]
    private InGameUIManager _uiManager = null;

    [Header("Map")]
    public Transform _mapParent = null;
    [SerializeField]
    private GameObject _mapPrefab = null;

    [Header("Spawn Point")]
    [SerializeField]
    private List<PlayerInSceneFormat> _spawnPoints;

    [Header("Player")]
    [SerializeField]
    private GameObject _myPlayerPrefab = null;
    [SerializeField]
    private GameObject _playerPrefab = null;

    [Header("Field Objects (ex. ExtractionPoint, Door, Box...)")]
    public Transform _fieldObjectParent = null;
    [SerializeField]
    private List<FieldObjectFormat> _fieldObjects = new List<FieldObjectFormat>();

    [Space]
    [Header("----------------Object Pooling----------------")]
    [SerializeField]
    private PoolManager<pObjectType> _objectPool = new PoolManager<pObjectType>();
    public PoolManager<pObjectType> Pool { get => _objectPool; }
    public Transform _poolParent = null;
    public List<PoolableFormat<pObjectType>> _poolableObjects = new List<PoolableFormat<pObjectType>>();
    public override void InitScene() {
        LoadScene();
    }

    private void LoadScene() {
        _objectPool.SetParent(_poolParent);
        Instantiate(_mapPrefab, _mapParent);

        for(int i = 0; i < _fieldObjects.Count; i++) {
            GameObject fo = Instantiate(_fieldObjects[i].obj);
            fo.transform.position = _fieldObjects[i].position;
            fo.transform.rotation = Quaternion.Euler(_fieldObjects[i].rotation);
            fo.transform.localScale = _fieldObjects[i].scale;
            fo.transform.SetParent(_fieldObjectParent);
        }

        for(int i = 0; i < _poolableObjects.Count; i++) {
            _objectPool.RegisterPoolObject(_poolableObjects[i].type, _poolableObjects[i].obj);
        }

        IsInitialized = true;

        Interlocked.MemoryBarrier();

        SpawnMyPlayerInScene();

        C_Changed_Scene_To sceneChanged = new C_Changed_Scene_To();
        sceneChanged.SceneType = SceneType;
        Managers.Network.Send(sceneChanged);
    }

    public void SpawnMyPlayerInScene() {
        Vector3 pos = new Vector3(0, 2, 0);
        Vector3 rot = new Vector3(0, 0, 0);

        for(int i = 0; i < _spawnPoints.Count; i++) {
            if(_spawnPoints[i]._playerInScene == Managers.PrevScene) {
                pos = _spawnPoints[i].position;
                rot = _spawnPoints[i].rotation;
            }
        }

        GameObject go = Instantiate(_myPlayerPrefab, pos, Quaternion.EulerAngles(rot));

        _uiManager.Fade.FadeControlTo(false);
    }

    public override void ClearScene() {

    }
}
[System.Serializable]
public struct FieldObjectFormat {
    public GameObject obj;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
}

[Serializable]
public struct PlayerInSceneFormat {
    public pSceneType _playerInScene;
    public Vector3 position;
    public Vector3 rotation;
}