using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class LobbyUIManager : BaseUI {
    public override pSceneType Type { get; protected set; } = pSceneType.Lobby;

    [Header("UI Elements in Lobby")]
    [SerializeField]
    private RoomListUI _roomListUI = null;
    [SerializeField]
    private Button _buttonCreate = null;
    [SerializeField]
    private Button _buttonRefresh = null;

    private WaitForSeconds autoRefresh = new WaitForSeconds(3.0f);

    public RoomListUI RoomListUI { get => _roomListUI; }


    public override void Start() {
        base.Start();
        RefreshList(automatic: true);
    }


    #region Button Actions

    private IEnumerator CoRefreshAutomatically() {
        while(true) {
            yield return autoRefresh;
            RefreshList(automatic: true);
        }
    }

    public void CreateRoom() {

    }

    public void RefreshList(bool automatic = false) {

    }

    private IEnumerator CoCountRefreshInterval() {
        _buttonRefresh.interactable = false;
        yield return new WaitForSeconds(3.0f);
        _buttonRefresh.interactable = true;
    }

    #endregion

    private void OnDestroy() {
        StopCoroutine("CoRefreshAutomatically");
        StopCoroutine("CoCountRefreshInterval");
    }
}
