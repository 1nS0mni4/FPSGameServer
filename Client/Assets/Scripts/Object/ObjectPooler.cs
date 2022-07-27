using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public GameObject _prefab = null;
    private List<GameObject> _objects = new List<GameObject>();
    private object l_objects = new object();

    private void Awake() {
        if(_prefab == null) {
            for(int i = 0; i < transform.childCount; i++) {
                GameObject child = transform.GetChild(i).gameObject;
                child.SetActive(false);
                _objects.Add(child);
            }
        }
        else {
            InstantiateObject();
        }
    }

    private GameObject InstantiateObject(int count = 5) {
        if(_prefab == null)
            return null;

        lock(l_objects) {
            for(int i = 0; i < count; i++) {
                GameObject go = Instantiate(_prefab);
                go.SetActive(false);
                go.transform.SetParent(transform);
            }
        }

        return _objects[_objects.Count - 1];
    }

    public GameObject Get() {
        lock(l_objects) {
            GameObject go = null;

            for(int i = 0; i < _objects.Count; i++) {
                if(_objects[i].active == false) {
                    _objects[i].SetActive(true);
                    go = _objects[i];
                    break;
                }
            }

            if(go == null)
                go = InstantiateObject();

            return go;
        }
    }

    public void Destroy(GameObject obj) {
        if(_objects.Contains(obj) == false)
            return;

        obj.SetActive(false);
    }
}
