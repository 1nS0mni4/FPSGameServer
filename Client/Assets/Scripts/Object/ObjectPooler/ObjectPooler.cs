using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pooling Object는 컴포넌트를 대상으로 사용한다.
/// Pooling Object를 가지는 오브젝트의 Transform을 가져와야한다.
/// </summary>
/// <typeparam name="T"></typeparam>
[System.Serializable]
public class ObjectPooler<T> : MonoBehaviour where T: MonoBehaviour {
    public Transform _transform = null;
    private List<T> _objects = new List<T>();
    private object l_objects = new object();

    private void Awake() {
        if(_transform == null)
            _transform = transform;

        for(int i = 0; i < _transform.childCount; i++) {
            T child = _transform.GetChild(i).GetComponent<T>();
            child.gameObject.SetActive(false);
            _objects.Add(child);
        }
    }

    public T Get() {
        lock(l_objects) {
            T go = null;

            for(int i = 0; i < _objects.Count; i++) {
                if(_objects[i].gameObject.activeSelf == false) {
                    _objects[i].gameObject.SetActive(true);
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