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

        if(Managers.Scene.IsSceneChanging) {
            Managers.Scene.Completed.Enqueue(() => {
                InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();

                if(manager == null)
                    return;

                for(int i = 0; i < response.ObjectList.Count; i++) {
                    manager.SpawnPlayerInPosition(response.ObjectList[i].AuthCode, response.ObjectList[i].Position, response.ObjectList[i].Rotation);
                    Debug.Log($"AuthCode: {response.ObjectList[i].AuthCode}");
                }

                manager.f_Loaded_Player = true;
            });
        }
    }

    public static void S_Load_ItemsHandler(PacketSession s, IMessage packet) {
        Debug.Log("S_Load_Items Received!");
        ServerSession session = (ServerSession)s;
        S_Load_Items response = (S_Load_Items)packet;

        if(Managers.Scene.IsSceneChanging) {
            Managers.Scene.Completed.Enqueue(() => {
                InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();

                if(manager == null)
                    return;

                //TODO: 맵의 아이템에 대한 모든 정보 기입

                manager.f_Loaded_Item = true;
            });
        }
    }

    public static void S_Load_FieldsHandler(PacketSession s, IMessage packet) {
        Debug.Log("S_Load_Fields Received!");
        ServerSession session = (ServerSession)s;
        S_Load_Fields response = (S_Load_Fields)packet;

        InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();

        if(manager == null)
            return;

        manager.f_Loaded_Field = true;
    }

    public static void S_Interpol_PlayerHandler(PacketSession s, IMessage packet) {

        ServerSession session = (ServerSession)s;
        S_Interpol_Player response = (S_Interpol_Player)packet;

        InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();

        if(manager == null)
            return;

        manager.SyncObjectInPosition(response.AuthCode, response.Position, response.Rotation);
        Debug.Log($"S_Player_Interpol Received! AuthCode: {response.AuthCode}");
    }

    public static void S_SpawnHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Spawn response = (S_Spawn)packet;

        Action spawnUser = () => {
            InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();

            if(manager == null)
                return;

            manager.SpawnPlayerInSpawnPoint(response.AuthCode, response.PrevArea, response.DestArea);
        };

        if(Managers.Scene.IsSceneChanging)   //로딩 중 내 캐릭터 생성을 명령 받았을 때
            Managers.Scene.Completed.Enqueue(spawnUser);
        else
            spawnUser.Invoke();

        Debug.Log($"S_Spawn Received! {response.AuthCode}");
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

        Action<object> requester = null;

        if(Managers.Network.MessageWait.TryGetValue(typeof(S_Response_Request_Online), out requester)) {
            Managers.Network.MessageWait.Remove(typeof(S_Response_Request_Online));
            requester.Invoke(response);
        }
    }

    public static void S_Response_Request_Game_SessionHandler(PacketSession s, IMessage packet) {
        Debug.Log("S_Request_Online_Response Received!");
        ServerSession session = (ServerSession)s;
        S_Response_Request_Online response = (S_Response_Request_Online)packet;


        throw new NotImplementedException();
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

        Character player = null;
        if(manager._players.TryGetValue(response.AuthCode, out player)) {
            player.Stance = response.Stance;
            player.MoveDir = response.Dir.toVector3();
        }
    }

    public static void S_Broadcast_Player_JumpHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Broadcast_Player_Jump response = (S_Broadcast_Player_Jump)packet;

        if(Managers.Network.AuthCode == response.AuthCode)
            return;

        InGameSceneManager manager = Managers.Scene.Manager as InGameSceneManager;
        if(manager == null)
            return;

        Character character = null;
        if(manager._players.TryGetValue(response.AuthCode, out character)) {
            Player player = character.GetComponent<Player>();
            player.Jump();
        }
    }

    public static void S_Sync_Player_TransformHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Sync_Player_Transform response = (S_Sync_Player_Transform)packet;

        InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();
        if(manager == null)
            return;

        for(int i = 0; i < response.PlayerTransforms.Count; i++) {
            manager.SyncObjectInPosition(response.PlayerTransforms[i].AuthCode, response.PlayerTransforms[i].Position, response.PlayerTransforms[i].Rotation);
            Debug.Log($"S_Sync_Player_Transform Received! {response.PlayerTransforms[i].AuthCode}");
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