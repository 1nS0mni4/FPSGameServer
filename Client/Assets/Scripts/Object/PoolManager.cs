using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolManager<T>{
    private Transform _parent = null;
    private Dictionary<T, ObjectPool> _poolers = new Dictionary<T, ObjectPool>();

    public void SetParent(Transform parent) { _parent = parent; }

    public void RegisterPoolObject(T type, GameObject prefab) {
        if(_poolers.ContainsKey(type))
            return;

        ObjectPool pool = new ObjectPool();
        pool.Init(prefab, _parent);
        _poolers.Add(type, pool);

        InstantiateObject(type);
    }

    public void InstantiateObject(T type) {
        if(_poolers.ContainsKey(type) == false)
            return;

        _poolers[type].InstantiateObject();
    }

    public GameObject GetPoolObject(T type) {
        if(_poolers.ContainsKey(type) == false)
            return null;

        GameObject go = _poolers[type].GetPoolObject();
        return go;
    }

    public void DestroyObject(T type, GameObject obj) {
        if(_poolers.ContainsKey(type) == false)
            return;

        _poolers[type].DeactiveObject(obj);
    }
}

public class ObjectPool{
    private Transform _parent = null;
    private GameObject _prefab = null;
    private List<GameObject> _pools = new List<GameObject>();
    
    public void Init(GameObject prefab, Transform parent = null) {
        if(parent != null)
            _parent = parent;

        Poolable poolable = prefab.GetComponent<Poolable>();
        if(poolable == null)
            poolable = prefab.AddComponent<Poolable>();

        _prefab = prefab;
        poolable._destroyEvent -= DeactiveObject;
        poolable._destroyEvent += DeactiveObject;
    }
    public GameObject InstantiateObject(ushort count = 5) {
        for(ushort i = 0; i < count; i++) {
            GameObject go = Object.Instantiate(_prefab);
            go.SetActive(false);
            if(_parent != null)
                go.transform.SetParent(_parent);

            _pools.Add(go);
        }

        return _pools[_pools.Count - 1];
    }

    public GameObject GetPoolObject() {
        GameObject go = null;

        for(ushort i = 0; i < _pools.Count; i++) {
            if(_pools[i].activeSelf == false) {
                go = _pools[i];
                go.SetActive(true);
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
}

[System.Serializable]
public struct ParentPerType<T> {
    public T type;
    public Transform parent;
}

[System.Serializable]
public struct PoolableFormat<T> {
    public T type;
    public GameObject obj;
}