using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Session;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientObject {
    public ClientSession _session { get; set; }
    public Player _player { get; set; }
}

public class InGameSceneManager : MonoBehaviour {
    private static InGameSceneManager _instance;
    public static InGameSceneManager Instance { get => _instance; }

    public pAreaType _areaType;

    public CharacterPooler _characterPooler;
    private List<uint> _playerAuths = new List<uint>();
    private Dictionary<uint, ClientObject> _players = new Dictionary<uint, ClientObject>();
    private Dictionary<uint, InteractableObject> _fieldObjects = new Dictionary<uint, InteractableObject>();


    private bool f_Load_FieldData = false;
    private bool f_Load_Items = true;

    private IEnumerator CoCheckLoadFinished() {
        while(true) {
            if(f_Load_FieldData & f_Load_Items)
                break;

            yield return null;
        }

        ServerManager.Network.Listen();
    }

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

        Player player = _characterPooler.Get();

        ClientObject cliObject = new ClientObject();
        cliObject._session = session;
        cliObject._player = player;
        
        _players.Add(authCode, cliObject);

        //TODO: 플레이어 생성 후 위치 전송

        return true;
    }

    public void PlayerMove(uint authCode, pVector3 velocity) {

    }
}
