using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour {
    [Header("Object Parents")]
    public Transform _userParent = null;
    private List<GameObject> _userPool = new List<GameObject>();

    public Transform _projectileParent = null;
    private List<GameObject> _projectilePool = new List<GameObject>();

    public Transform _objectParent = null;
    private List<GameObject> _objectPool = new List<GameObject>();

    [Header("Object Prefabs")]
    [SerializeField]
    private GameObject _userPrefab = null;
    [SerializeField]
    private GameObject _projectilePrefab = null;
    [SerializeField]
    private GameObject[] _objectsPrefab = null;

    private void Awake() {
        Managers.Object = this;
        //PoolObjects();
    }

    public void PoolObjects(pObjectType objectType) {
        GameObject go = null;

        for(int i = 0; i < 5; i++) {
            go = Instantiate(_userPrefab);
            go.transform.SetParent(_userParent);
            go.SetActive(false);
            _userPool.Add(go);
        }

        for(int i = 0; i < 10; i++) {
            go = Instantiate(_projectilePrefab);
            go.transform.SetParent(_projectileParent);
            go.SetActive(false);
            _projectilePool.Add(go);
        }
    }

    public void SpawnObject(pObjectType objectType) {
        GameObject go = GetFreeObject(objectType);

        if(go == null) {

        }

        switch(objectType) {
            case pObjectType.Player: {
                
            }break;
            case pObjectType.Myplayer: {

            }break;
            case pObjectType.Projectile: {

            }break;
            case pObjectType.Fieldobject: {

            }break;
            default:break;
        }
    }

    private GameObject GetFreeObject(pObjectType objectType) {
        switch(objectType) {
            case pObjectType.Player: {

            }
            break;
            case pObjectType.Myplayer: {

            }
            break;
            case pObjectType.Projectile: {

            }
            break;
            case pObjectType.Fieldobject: {

            }
            break;
            default: break;
        }

        return null;
    }
}
