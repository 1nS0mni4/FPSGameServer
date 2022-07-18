using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class FieldmapSceneManager : BaseScene {
    [SerializeField]
    private Transform _playerParent = null;
    [SerializeField]
    private GameObject m_MyPlayerPrefab = null;
    [SerializeField]
    private GameObject m_PlayerPrefab = null;

    [SerializeField]
    private List<Transform> _spawnPoints = new List<Transform>();

    [SerializeField]
    public Dictionary<int, GameObject> _players = new Dictionary<int, GameObject>();

    public override pSceneType SceneType { get; protected set; } = pSceneType.Fieldmap;

    public override void InitScene() {

    }

    public override void ClearScene() {
        for(int i = 0; i < _playerParent.childCount; i++) {
            Destroy(_playerParent.GetChild(i).gameObject);
        }
    }

    public void SpawnPlayerWithPoint(int sessionID, int spawnPointIndex) {
        GameObject go = Instantiate(m_PlayerPrefab);
        go.transform.position = _spawnPoints[spawnPointIndex].position;

        if(sessionID != Managers.Network.SessionID)
            go.AddComponent<Player>();
        else{
            go.AddComponent<MyPlayer>();
            Vector3 pos = go.transform.position;

            C_Spawn_Point spawnPacket = new C_Spawn_Point(){Position = new pVector3() { X = pos.x, Y = pos.y, Z = pos.z } };
            Managers.Network.Send(spawnPacket);
        }
    }
}
