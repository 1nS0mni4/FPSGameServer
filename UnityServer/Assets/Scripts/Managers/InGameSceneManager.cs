using Extensions;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Session;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public class ClientObject {
    public ClientSession _session { get; set; }
    public Player _player { get; set; }
}

public class InGameSceneManager : MonoBehaviour {
    #region Singleton
    private static InGameSceneManager _instance;
    public static InGameSceneManager Instance { get => _instance; }
    #endregion

    public pAreaType _areaType;

    public List<Transform> _spawnPoint = new List<Transform>();

    [HideInInspector]
    public CharacterPooler _playerPooler;


    private List<uint> _playerAuths = new List<uint>();
    private Dictionary<uint, Player> _players = new Dictionary<uint, Player>();
    private Dictionary<uint, InteractableObject> _fieldObjects = new Dictionary<uint, InteractableObject>();


    private bool f_Load_FieldData = false;
    private bool f_Load_Items = true;

    private void Awake() {
        if(_instance != null) {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    void Start() {
        Physics.autoSimulation = false;
        StartCoroutine(CoCheckLoadFinished());
        LoadFieldData();
        LoadItems();
    }

    private void LoadFieldData() {

        f_Load_FieldData = true;
    }

    private void LoadItems() {
        
        f_Load_Items = true;
    }

    private IEnumerator CoCheckLoadFinished() {
        while(true) {
            if(f_Load_FieldData & f_Load_Items == true)
                break;

            yield return null;
        }

        ServerManager.Network.Listen();
        yield break;
    }

    public bool RegisterUserAuth(uint authCode) {
        if(_playerAuths.Contains(authCode))
            return false;

        _playerAuths.Add(authCode);
        return true;
    }

    /// <summary>
    /// 유저가 서버에 접근을 시도할 때 유효성을 체크합니다.
    /// </summary>
    /// <param name="session"></param>
    /// <param name="authCode"></param>
    /// <returns>UserAuth에 등록되어있지 않거나, 이미 캐릭터가 생성되어있을 경우 false, 나머지는 true</returns>
    public bool EnterGame(ClientSession session, uint authCode) {   
        if(_playerAuths.Contains(authCode) == false || _players.ContainsKey(authCode))
            return false;

        Player player = _playerPooler.Get();
        
        Transform randSpawn = _spawnPoint[UnityEngine.Random.Range(0, _spawnPoint.Count - 1)];
        _spawnPoint.Remove(randSpawn);
        player.gameObject.SetActive(true);
        player.transform.position = randSpawn.position;
        player.Session = session;

        _players.Add(authCode, player);

        S_Broadcast_Player_Spawn spawn = new S_Broadcast_Player_Spawn();
        spawn.Info.AuthCode = authCode;
        spawn.Info.Position = player.transform.position.TopVector3();
        spawn.Info.Rotation = player.transform.rotation.TopQuaternion();

        //TODO: Broadcast기능 추가.
        ServerManager.Network.Broadcast(spawn);

        return true;
    }

    public void PlayerMove(uint authCode, pVector3 velocity) {

    }

    public void GameEnd() {

    }

    public void Broadcast(IMessage packet) {

    }
}
