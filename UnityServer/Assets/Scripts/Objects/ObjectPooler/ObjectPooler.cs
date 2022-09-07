using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler<T> : MonoBehaviour where T: MonoBehaviour {
    private Transform _transform = null;
    private List<T> _objects = new List<T>();
    private object l_objects = new object();

    private void Awake() {
        _transform = GetComponent<Transform>();
        if(_transform == null)
            return;

        if(_transform.childCount <= 0)
            return;

        for(int i = 0; i < _transform.childCount; i++) {
            T child = _transform.GetChild(i).GetComponent<T>();
            child.gameObject.SetActive(false);
            _objects.Add(child);
        }
    }

    public T Get() {
        if(_transform == null)
            return null;

        lock(l_objects) {
            T go = null;

            for(int i = 0; i < _objects.Count; i++) {
                if(_objects[i].gameObject.activeSelf == false) {
                    go = _objects[i];
                    break;
                }
            }

            return go;
        }
    }

    public void Destroy(T obj) {
        if(_objects.Contains(obj) == false)
            return;

        obj.gameObject.SetActive(false);
    }
}