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
                     public Dictionary<uint, Player> _players = new Dictionary<uint, Player>();
                     public CharacterPooler _characterPooler = null;

                     private WaitForSeconds loadWaitTime = new WaitForSeconds(0.5f);

    /****************************************************
     *                  Loading Flags                   *
     ****************************************************/

    [HideInInspector]public bool f_Loaded_Player    = true;
    [HideInInspector]public bool f_Loaded_Item      = true;
    [HideInInspector]public bool f_Loaded_Field     = true;

    public override void InitScene() {
        //Physics.autoSimulation = false;
    }

    protected override void Start() {
        base.Start();
        StartCoroutine(CoCheckDataLoaded());
    }

    public override void ClearScene() {
        
    }

    public void SpawnPlayer(uint authCode, pVector3 position = null, pQuaternion rotation = null) {
        if(_players.ContainsKey(authCode))
            return;

        Vector3 pos = position.ToUnityVector3();
        if(position == null)
            pos = new Vector3(0, 2, -20);
        
        Quaternion rot = rotation.ToUnityQuaternion();
        if(rotation == null)
            rot = Quaternion.Euler(new Vector3(0, 0, 0));

        if(authCode == Managers.Network.AuthCode) {
            _myPlayer.AuthCode = authCode;
            _myPlayer.gameObject.transform.position = pos;
            _myPlayer.gameObject.transform.rotation = rot;

            _myPlayer.gameObject.SetActive(true);
        }
        else {
            Player player  = _characterPooler.Get();
            player.AuthCode = authCode;
            player.gameObject.transform.position = pos;
            player.gameObject.transform.rotation = rot;
            player.gameObject.SetActive(true);

            _players.Add(authCode, player);
        }
    }

    public void SyncObjectInPosition(uint authCode, pVector3 position, pQuaternion rotation) {
        Player player = null;

        if(_players.TryGetValue(authCode, out player)) {
            player.Position = position.ToUnityVector3();
            player.RotateDir = rotation.ToUnityQuaternion();
        }
    }

    public void SyncPlayerRotation(uint authCode, pQuaternion rot) {
        Player player = null;

        if(_players.TryGetValue(authCode, out player)) {
            player.RotateDir = rot.ToUnityQuaternion();
        }
    }

    public void SyncPlayerMove(uint authCode, pVector3 velocity) {
        Player player = null;

        if(_players.TryGetValue(authCode, out player)) {
            player.Velocity = velocity.ToUnityVector3();

            float deltaTime = Managers.Network.PingTime;
            while(deltaTime > Time.fixedDeltaTime) {
                deltaTime -= Time.fixedDeltaTime;
                Physics.Simulate(Time.fixedDeltaTime);
            }
        }
    }

    public void RemovePlayer(uint authCode) {
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