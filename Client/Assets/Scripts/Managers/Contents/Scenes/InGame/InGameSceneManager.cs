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
                     public Dictionary<int, Player> _players = new Dictionary<int, Player>();
                     public ObjectPooler<Player> _playerPooler = null;

                     private WaitForSeconds loadWaitTime = new WaitForSeconds(0.5f);

    /****************************************************
     *                  Loading Flags                   *
     ****************************************************/

    [HideInInspector]public bool f_Loaded_Player    = true;
    [HideInInspector]public bool f_Loaded_Item      = true;
    [HideInInspector]public bool f_Loaded_Field     = true;


    public override void InitScene() {
        Managers.Scene.IsLoading = false;
    }

    private void Start() {
        StartCoroutine(CoCheckDataLoaded());
    }

    public override void ClearScene() {
        
    }

    public void SpawnPlayerInSpawnPoint(int authID, pAreaType prevArea) {
        Vector3 pos = new Vector3(0, 0, 0);
        Quaternion rot = Quaternion.EulerAngles(new Vector3(0, 0, 0));

        for(int i = 0; i < _spawnPoints.Count; i++) {
            if(prevArea != _spawnPoints[i].areaType)
                continue;

            pos = _spawnPoints[i].transform.position;
            rot = _spawnPoints[i].transform.rotation;
            break;
        }

        if(authID == Managers.Network.AuthCode) {
            _myPlayer.transform.position = pos;
            _myPlayer.transform.rotation = rot;
            Managers.CurArea = AreaType;

            {
                C_Spawn_Response spawn = new C_Spawn_Response();
                spawn.Transform = _myPlayer.transform.TopTransform();
                Managers.Network.Send(spawn);
            }
            //_players.Add(authID, _myPlayer);
            _myPlayer.gameObject.SetActive(true);
        }
        else {
            //TODO: 플레이어 풀러에서 객체를 하나 받아 AuthCode->Key의 _player 데이터 Add 후 초기화 및 SetActive(true);
            Player player = _playerPooler.Get();
            player.transform.position = pos;
            player.transform.rotation = rot;

            _players.Add(authID, player);
        }
    }

    public void SpawnPlayerInPosition(int authID, pTransform tran) {
        //TODO: 플레이어 풀러에서 객체를 하나 받아 AuthCode->Key의 _player 데이터 Add 후 초기화 및 SetActive(true);

        Player player = _playerPooler.Get();
        Vector3 pos = new Vector3(tran.Position.X, tran.Position.Y, tran.Position.Z);
        Vector3 rot = new Vector3(tran.Rotation.X, tran.Rotation.Y, tran.Rotation.Z);
        player.transform.position = pos;
        player.transform.rotation = Quaternion.EulerAngles(rot);

        _players.Add(authID, player);
    }

    public void SynchObjectInPosition(int authID, pTransform tran) {
        Player player = null;

        if(_players.TryGetValue(authID, out player)) {
            Vector3 pos = new Vector3(tran.Position.X, tran.Position.Y, tran.Position.Z);
            Vector3 rot = new Vector3(tran.Rotation.X, tran.Rotation.Y, tran.Rotation.Z);
            player.transform.position = pos;
            player.transform.rotation = Quaternion.EulerAngles(rot);
        }
        else {
            SpawnPlayerInPosition(authID, tran);
        }
    }

    public void RemovePlayer(int authCode) {
        if(_players.ContainsKey(authCode)) {
            _playerPooler.Destroy(_players[authCode]);
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
    public pAreaType areaType;
    public Transform transform;
}