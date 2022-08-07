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
                spawn.Transform = _myPlayer.gameObject.transform.TopTransform();
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

    public void SpawnPlayerInPosition(int authID, pTransform tran) {
        if(authID == Managers.Network.AuthCode)
            return;

        if(_players.ContainsKey(authID))
            return;

        Character player = _characterPooler.Get();
        if(player == null)
            return;

        Vector3 pos = new Vector3(tran.Position.X, tran.Position.Y, tran.Position.Z);
        Vector3 rot = new Vector3(tran.Rotation.X, tran.Rotation.Y, tran.Rotation.Z);
        player.Position = pos;
        player.RotateDir = rot;
        player.gameObject.SetActive(true);

        _players.Add(authID, player);
    }

    public void SyncObjectInPosition(int authID, pTransform tran) {
        Character player = null;

        if(_players.TryGetValue(authID, out player)) {
            Vector3 pos = new Vector3(tran.Position.X, tran.Position.Y, tran.Position.Z);
            Vector3 rot = new Vector3(tran.Rotation.X, tran.Rotation.Y, tran.Rotation.Z);
            player.Position = pos;
            player.RotateDir = rot;
        }
    }

    public void SyncPlayerRotation(int authID, pVector3 rot) {
        Character player = null;

        if(_players.TryGetValue(authID, out player)) {
            player.RotateDir = rot.toVector3();
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