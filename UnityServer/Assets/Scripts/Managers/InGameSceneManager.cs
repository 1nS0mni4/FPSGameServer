using Extensions;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Session;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    public CharacterPooler _playerPooler;

    private Dictionary<uint, Player> _players = new Dictionary<uint, Player>();
    private object l_players = new object();
    private Dictionary<uint, ClientSession> _sessions = new Dictionary<uint, ClientSession>();
    private object l_sessions = new object();
    private Dictionary<uint, InteractableObject> _fieldObjects = new Dictionary<uint, InteractableObject>();


    private bool f_Load_FieldData = true;
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
    private void OnLoadCompleted() {
        ServerManager.Network.Listen();
    }

    private IEnumerator CoCheckLoadFinished() {
        while(true) {
            if(f_Load_FieldData & f_Load_Items == true)
                break;

            yield return null;
        }

        OnLoadCompleted();

#if UNITY_EDITOR
        S_Login_Debug_Game_Standby standby = new S_Login_Debug_Game_Standby();

        pEndPoint endPoint = new pEndPoint();
        endPoint.HostString = ServerManager.Network.LocalHostName;
        endPoint.Port = ServerManager.Network.LocalPort;

        standby.EndPoint = endPoint;
        standby.AreaType = _areaType;
        standby.Capacity = 5;
        
        //TODO: 필드 별 인원 최대 수용량 설정하기.
#else
        S_Login_Game_Standby standby = new S_Login_Game_Standby();
        pEndPoint endPoint = new pEndPoint();
        {
            endPoint.HostString = ServerManager.Network.LocalHostName;
            endPoint.Port = ServerManager.Network.LocalPort;
        }

        standby.EndPoint = endPoint;
        standby.AreaType = _areaType;

#endif
        ServerManager.Network.SendToLoginServer(standby);
        yield break;
    }

    public bool RegisterUserAuth(uint authCode) {
        if(_players.ContainsKey(authCode))
            return false;

        Player player = _playerPooler.Get();

        Transform randSpawn = _spawnPoint[UnityEngine.Random.Range(0, _spawnPoint.Count)];
        if(randSpawn == null) {

        }
        _spawnPoint.Remove(randSpawn);
        player.gameObject.SetActive(true);
        player.transform.position = randSpawn.position;
        player.SetAuth(authCode);

        lock(l_players) {
            _players.Add(authCode, player);
        }
        return true;
    }

    /// <summary>
    /// 유저가 서버에 접근을 시도할 때 유효성을 체크합니다.
    /// </summary>
    /// <param name="session"></param>
    /// <param name="authCode"></param>
    /// <returns>UserAuth에 등록되어있지 않거나, 이미 캐릭터가 생성되어있을 경우 false, 나머지는 true</returns>
    public bool EnterGame(ClientSession session) {   
        if(_sessions.ContainsKey(session.AuthCode))
            return false;

        Player player;
        if(_players.TryGetValue(session.AuthCode, out player) == false)
            return false;

        session.Character = player;
        lock(l_sessions) {
            _sessions.Add(session.AuthCode, session);
        }

        S_Broadcast_Player_Spawn spawn = new S_Broadcast_Player_Spawn();
        spawn.Info.AuthCode = session.AuthCode;
        spawn.Info.Position = player.transform.position.TopVector3();
        spawn.Info.Rotation = player.transform.rotation.TopQuaternion();

        Broadcast(spawn);
        return true;
    }

    public bool Disconnect(uint authCode) {
        bool success = true;
        if(_players.TryGetValue(authCode, out Player player)) {
            lock(l_players) {
                _players.Remove(authCode);
            }
            _playerPooler.Destroy(player);
        }
        else
            success &= false;

        if(_sessions.TryGetValue(authCode, out ClientSession session)) {
            lock(l_sessions) {
                _sessions.Remove(session.AuthCode);
            }
            session.Disconnect();
        }
        else
            success &= false;

        return success;
    }

    public void PlayerMove(uint authCode, bool[] inputs, pQuaternion camFront) {
        if(_players.TryGetValue(authCode, out Player player)) {
            
        }
    }

    public void GameEnd() {

    }

    public async Task Broadcast(IMessage packet) {
        await Task.Factory.StartNew(() => {
            lock(l_sessions) {
                foreach(var session in _sessions.Values) {
                    session.Send(packet);
                }
            }
        });
    }
}
