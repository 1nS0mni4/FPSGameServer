using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolManager {
    [SerializeField]
    private Transform _playerParent = null;
    [SerializeField]
    private Transform _projectileParent = null;
    [SerializeField]
    private Transform _fieldObjectParent = null;

    private Dictionary<pObjectType, ObjectPool> _poolers = null;

    public void Init() {
        _poolers = new Dictionary<pObjectType, ObjectPool>();
    }

    public void RegisterPoolObject(pObjectType type, GameObject prefab) {
        if(_poolers.ContainsKey(type))
            return;

        ObjectPool pool = new ObjectPool();
        switch(type) {
            case pObjectType.Player:
            case pObjectType.Myplayer: {
                pool.Init(_playerParent, prefab);
            }break;

            case pObjectType.Projectile: {
                pool.Init(_projectileParent, prefab);
            }break;

            case pObjectType.Fieldobject: {
                pool.Init(_fieldObjectParent, prefab);
            }break;
        }

        _poolers.Add(type, pool);
    }

    public void RegisterPoolObject(List<PoolableFormat> formats) {
        for(int i = 0; i < formats.Count; i++) {
            RegisterPoolObject(formats[i]._type, formats[i].obj);
        }
    }

    public GameObject InstantiateObjects(pObjectType type, ushort count = 5) {
        if(_poolers.ContainsKey(type) == false)
            return null;

        GameObject go = _poolers[type].InstantiateObject(count);
        return go;
    }

    public GameObject GetPoolObject(pObjectType type) {
        if(_poolers.ContainsKey(type) == false)
            return null;

        GameObject go = _poolers[type].GetPoolObject();
        return go;
    }

    public void DestroyObject(pObjectType type, GameObject obj) {
        if(_poolers.ContainsKey(type) == false)
            return;

        _poolers[type].DeactiveObject(obj);
    }

    public bool DestroyPool() {
        _fieldObjectParent = null;
        _playerParent = null;
        _projectileParent = null;

        foreach(pObjectType key in _poolers.Keys) {
            _poolers[key].DestroyPool();
        }
        _poolers = null;

        return true;
    }
}

public class ObjectPool{
    private Transform _parent = null;
    private GameObject _prefab = null;
    private List<GameObject> _pools = new List<GameObject>();

    public void Init(Transform parent, GameObject prefab) {
        _parent = parent;
        _prefab = prefab;
    }
    public GameObject InstantiateObject(ushort count = 5) {
        for(ushort i = 0; i < count; i++) {
            GameObject go = Object.Instantiate(_prefab);
            go.transform.SetParent(_parent);
        }

        return _pools[_pools.Count - 1];
    }

    public GameObject GetPoolObject() {
        GameObject go = null;

        for(ushort i = 0; i < _pools.Count; i++) {
            if(_pools[i].active == false) {
                go = _pools[i];
                break;
            }
        }

        if(go == null)
            go = InstantiateObject();

        return go;
    }

    public void DeactiveObject(GameObject go) {
        if(_pools.Contains(go) == false)
            return;

        go.SetActive(false);
    }

    public void DestroyPool() {
        _pools = null;
        _prefab = null;
        _parent = null;
    }
}