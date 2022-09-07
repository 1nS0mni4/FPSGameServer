using Google.Protobuf;
using Client.Session;
using ServerCore;
using Google.Protobuf.Protocol;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using static Define;
using Extensions;

public static class PacketHandler {
    public static void S_Error_PacketHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Error_Packet response = (S_Error_Packet)packet;

        Debug.Log(response.ErrorCode.ToString());
    }

    public static void S_Response_AccessHandler(PacketSession s, IMessage packet) {
        Debug.Log("S_Access_Response Received!");
        ServerSession session = (ServerSession)s;
        S_Response_Access response = (S_Response_Access)packet;

        LoginUIManager ui = UIManager.GetManager<LoginUIManager>();
        if(ui == null)
            return;

        switch(response.ErrorCode) {
            case NetworkError.Noaccount: {
                ui.DisplayError(NetworkError.Noaccount);
            }
            break;

            case NetworkError.Overlap: {
                ui.DisplayError(NetworkError.Overlap);
            }
            break;

            case NetworkError.Success: {
                Managers.Network.AuthCode = response.AuthCode;
                Managers.Scene.ChangeSceneTo(pAreaType.Hideout);
                Managers.Scene.Completed.Enqueue(() => {
                    InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();

                    if(manager == null)
                        return;

                    manager.SpawnPlayer(response.AuthCode, new pVector3() { X = 0f, Y = 2f, Z = -20f }, new pQuaternion() { X = 0f, Y = 0f, Z = 0f, W = 0f });
                });
            }
            break;
            default: break;
        }
    }

    public static void S_Response_RegisterHandler(PacketSession s, IMessage packet) {
        Debug.Log("S_Register_Response Received!");
        ServerSession session = (ServerSession)s;
        S_Response_Register response = (S_Response_Register)packet;

        if(response.ErrorCode == false) {
            return;
        }

        LoginUIManager ui = UIManager.GetManager<LoginUIManager>();
        if(ui == null)
            return;
    }

    public static void S_Load_PlayersHandler(PacketSession s, IMessage packet) {
        Debug.Log($"S_Load_Players Received!");
        ServerSession session = (ServerSession)s;
        S_Load_Players response = (S_Load_Players)packet;

        Action loadPlayer = delegate() {
            InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();

            if(manager == null)
                return;

            for(int i = 0; i < response.ObjectList.Count; i++) {
                manager.SpawnPlayer(response.ObjectList[i].AuthCode, response.ObjectList[i].Position, response.ObjectList[i].Rotation);
                Debug.Log($"AuthCode: {response.ObjectList[i].AuthCode}");
            }

            manager.f_Loaded_Player = true;
        };

        if(Managers.Scene.IsSceneChanging) 
            Managers.Scene.Completed.Enqueue(loadPlayer);
        else
            loadPlayer.Invoke();
    }

    public static void S_Load_ItemsHandler(PacketSession s, IMessage packet) {
        Debug.Log("S_Load_Items Received!");
        ServerSession session = (ServerSession)s;
        S_Load_Items response = (S_Load_Items)packet;

        Action loadItem = delegate () {
            InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();

            if(manager == null)
                return;

            //TODO: 맵의 아이템에 대한 모든 정보 기입

            manager.f_Loaded_Item = true;
        };

        if(Managers.Scene.IsSceneChanging) 
            Managers.Scene.Completed.Enqueue(loadItem);
        else
            loadItem.Invoke();
    }

    public static void S_Load_FieldHandler(PacketSession s, IMessage packet) {
        Debug.Log("S_Load_Fields Received!");
        ServerSession session = (ServerSession)s;
        S_Load_Field response = (S_Load_Field)packet;

        Action loadField = delegate () {
            InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();

            if(manager == null)
                return;

            manager.f_Loaded_Field = true;
        };

        if(Managers.Scene.IsSceneChanging) 
            Managers.Scene.Completed.Enqueue(loadField);
        else
            loadField.Invoke();
    }

    public static void S_Broadcast_Player_SpawnHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Broadcast_Player_Spawn response = (S_Broadcast_Player_Spawn)packet;

        Action spawnUser = () => {
            InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();

            if(manager == null)
                return;

            manager.SpawnPlayer(response.Info.AuthCode, response.Info.Position, response.Info.Rotation);
        };

        if(Managers.Scene.IsSceneChanging)   //로딩 중 내 캐릭터 생성을 명령 받았을 때
            Managers.Scene.Completed.Enqueue(spawnUser);
        else
            spawnUser.Invoke();

        Debug.Log($"S_Spawn Received! {response.Info.AuthCode}");
    }

    public static void S_Broadcast_Player_LeaveHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Broadcast_Player_Leave response = (S_Broadcast_Player_Leave)packet;

        InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();

        if(manager == null)
            return;

        manager.RemovePlayer(response.AuthCode);
        Debug.Log($"S_Player_Leave Received! {response.AuthCode}");
    }

    public static void S_Response_Request_OnlineHandler(PacketSession s, IMessage packet) {
        Debug.Log("S_Request_Online_Response Received!");
        ServerSession session = (ServerSession)s;
        S_Response_Request_Online response = (S_Response_Request_Online)packet;

        InGameUIManager uiManager = UIManager.GetManager<InGameUIManager>();
        if(uiManager == null)
            return;

        uiManager.Transmitter.RefreshSessionList(response);
    }

    public static void S_Response_Request_Game_SessionHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Response_Request_Game_Session response = (S_Response_Request_Game_Session)packet;

        Managers.Network.Connect_Game(response.EndPoint.IpAddress, response.EndPoint.Port);
    }

    public static void S_Broadcast_Player_MoveHandler(PacketSession s, IMessage packet) {
        Debug.Log("S_Move_Broadcast Received!");
        ServerSession session = (ServerSession)s;
        S_Broadcast_Player_Move response = (S_Broadcast_Player_Move)packet;

        if(Managers.Network.AuthCode == response.AuthCode)
            return;

        InGameSceneManager manager = Managers.Scene.Manager as InGameSceneManager;
        if(manager == null)
            return;

        manager.SyncPlayerMove(response.AuthCode, response.Velocity);
    }

    public static void S_Sync_Player_TransformHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Sync_Player_Transform response = (S_Sync_Player_Transform)packet;

        InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();
        if(manager == null)
            return;

        for(int i = 0; i < response.Players.Count; i++) {
            manager.SyncObjectInPosition(response.Players[i].AuthCode, response.Players[i].Position, response.Players[i].Rotation);
            Debug.Log($"S_Sync_Player_Transform Received! {response.Players[i].AuthCode}");
        }
    }

    public static void S_Broadcast_Player_RotationHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Broadcast_Player_Rotation response = (S_Broadcast_Player_Rotation)packet;

        InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();
        if(manager == null)
            return;

        manager.SyncPlayerRotation(response.AuthCode, response.Rotation);

        Debug.Log($"S_Broadcast_Look_Rotation Received! {response.AuthCode}");
    }
}