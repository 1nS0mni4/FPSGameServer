using Extensions;
using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class InGameSceneManager : MSceneManager {
    [SerializeField] private InGameUIManager _uiManager = null;

    [Header("Spawn Point")]
    [SerializeField] private List<SpawnPointFormat> _spawnPoints = null;

    [Header("Player")]
    [SerializeField] private MyPlayer _myPlayer = null;
                     public Dictionary<int, Character> _players = new Dictionary<int, Character>();
                     public CharacterPooler _characterPooler = null;

                     private WaitForSeconds loadWaitTime = new WaitForSeconds(0.5f);

    /****************************************************
     *                  Loading Flags                   *
     ****************************************************/

    [HideInInspector]public bool f_Loaded_Player    = true;
    [HideInInspector]public bool f_Loaded_Item      = true;
    [HideInInspector]public bool f_Loaded_Field     = true;

    public override void InitScene() {

    }

    protected override void Start() {
        base.Start();
        StartCoroutine(CoCheckDataLoaded());
    }

    public override void ClearScene() {
        
    }

    public void SpawnPlayerInSpawnPoint(int authID, pAreaType prevArea, pAreaType destArea) {
        if(_players.ContainsKey(authID))
            return;

        Vector3 pos = Vector3.zero;
        Quaternion rot = Quaternion.Euler(Vector3.zero);

        for(int i = 0; i < _spawnPoints.Count; i++) {
            if(prevArea != _spawnPoints[i].fromArea)
                continue;

            if(destArea != _spawnPoints[i].toArea)
                continue;

            pos = _spawnPoints[i].transform.position;
            rot = _spawnPoints[i].transform.rotation;
            break;
        }

        if(authID == Managers.Network.AuthCode) {
            _myPlayer.gameObject.transform.position = pos;
            _myPlayer.gameObject.transform.rotation = rot;

            {
                C_Spawn_Response spawn = new C_Spawn_Response();
                spawn.Position = _myPlayer.gameObject.transform.position.TopVector3();
                spawn.Rotation = _myPlayer.gameObject.transform.rotation.TopQuaternion();
                Managers.Network.Send(spawn);
            }

            _players.Add(authID, _myPlayer);
            _myPlayer.gameObject.SetActive(true);
        }
        else {
            Character player  = _characterPooler.Get();
            player.gameObject.transform.position = pos;
            player.gameObject.transform.rotation = rot;
            player.gameObject.SetActive(true);

            _players.Add(authID, player);
        }
    }

    public void SpawnPlayerInPosition(int authID, pVector3 position, pQuaternion rotation) {
        if(authID == Managers.Network.AuthCode)
            return;

        if(_players.ContainsKey(authID))
            return;

        Character player = _characterPooler.Get();
        if(player == null)
            return;

        player.Position = position.ToUnityVector3();
        player.RotateDir = rotation.ToUnityQuaternion();

        player.gameObject.SetActive(true);

        _players.Add(authID, player);
    }

    public void SyncObjectInPosition(int authID, pVector3 position, pQuaternion rotation) {
        Character player = null;

        if(_players.TryGetValue(authID, out player)) {
            player.Position = position.ToUnityVector3();
            player.RotateDir = rotation.ToUnityQuaternion();
        }
    }

    public void SyncPlayerRotation(int authID, pQuaternion rot) {
        Character player = null;

        if(_players.TryGetValue(authID, out player)) {
            player.RotateDir = rot.ToUnityQuaternion();
        }
    }

    public void RemovePlayer(int authCode) {
        if(_players.ContainsKey(authCode)) {
            _characterPooler.Destroy(_players[authCode]);
        }
    }

    public override void LoadCompleted() {
        Managers.Input.CanInput = true;
        _uiManager.Fade.FadeControlTo(false);
    }

    private IEnumerator CoCheckDataLoaded() {
        bool loading = true;
        while(loading) {
            loading = !(f_Loaded_Field  & f_Loaded_Item & _myPlayer.gameObject.activeSelf);
            yield return loadWaitTime;
        }

        LoadCompleted();
        yield break;
    }
}

[Serializable]
public struct SpawnPointFormat {
    public pAreaType fromArea;
    public pAreaType toArea;
    public Transform transform;
}