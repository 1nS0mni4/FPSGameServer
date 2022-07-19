using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(ObjectPoolManager))]
public class InGameSceneManager : MSceneManager {
    public override pSceneType SceneType { get; protected set; }

    [Header("Map Prefab")]
    public Transform _mapParent = null;
    [SerializeField]
    private GameObject _mapPrefab = null;

    [Header("Field Objects (ex. SpawnPoint, Door, Box...)")]
    public Transform _fieldObjectParent = null;
    [SerializeField]
    private List<FIeldObjectFormat> _fieldObjects = new List<FIeldObjectFormat>();

    [Space]
    [SerializeField]
    private ObjectPoolManager _objectPool = new ObjectPoolManager();
    public ObjectPoolManager Pool { get => _objectPool; }
    public List<PoolableFormat> _poolableObjects = new List<PoolableFormat>();

    private void LoadScene() {
        Instantiate(_mapPrefab);

        for(int i = 0; i < _fieldObjects.Count; i++) {
            GameObject fo = Instantiate(_fieldObjects[i].obj);
            fo.transform.position = _fieldObjects[i].position;
            fo.transform.rotation = Quaternion.Euler(_fieldObjects[i].rotation);
            fo.transform.localScale = _fieldObjects[i].scale;
        }

        Pool.RegisterPoolObject(_poolableObjects);

        GameObject go = Instantiate(new GameObject());
        go.AddComponent<Camera>();
        go.name = name;
        go.tag = name;
        go.transform.position = new Vector3(0, 8.5f, -10);
        go.transform.rotation = Quaternion.Euler(new Vector3(25, 0, 0));
    }

    public override void InitScene() {
        string name = "MainCamera";


        StartCoroutine(CoStartInitializing());
    }

    public override void ClearScene() {


    }

    private IEnumerator CoStartInitializing() {
        IsInitialized = false;

        LoadScene();

        yield break;
    }
}
[System.Serializable]
public struct FIeldObjectFormat {
    public GameObject obj;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
}

[System.Serializable]
public struct PoolableFormat {
    public pObjectType _type;
    public GameObject obj;
}