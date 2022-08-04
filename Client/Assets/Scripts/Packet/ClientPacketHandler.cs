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

    public static void S_Access_ResponseHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Access_Response response = (S_Access_Response)packet;

        Debug.Log($"ErrorCode: {response.ErrorCode}, SessionID: {response.AuthCode}");

        LoginUIManager ui = UIManager.GetManager<LoginUIManager>();
        if(ui == null)
            return;

        switch(response.ErrorCode) {
            case NetworkError.Noaccount: {
                ui.DisplayError(NetworkError.Noaccount);
            }break;

            case NetworkError.Overlap: {
                ui.DisplayError(NetworkError.Overlap);
            }break;

            case NetworkError.Success: {
                Debug.Log("Success!");
                Managers.Network.AuthCode = response.AuthCode;
                Managers.Scene.ChangeSceneTo(pAreaType.Hideout);
            }break;
            default:break;
        }
    }

    public static void S_Register_ResponseHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Register_Response response = (S_Register_Response)packet;

        if(response.ErrorCode == false) {
            Debug.Log("Register Failed!");
            return;
        }

        LoginUIManager ui = UIManager.GetManager<LoginUIManager>();
        if(ui == null)
            return;

        Debug.Log("Register Success!");
    }

    public static void S_Load_PlayersHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Load_Players response = (S_Load_Players)packet;

        if(Managers.Scene.IsLoading) {
            Managers.Scene.Completed.Enqueue(() => {

            });
        }
    }

    public static void S_Load_ItemsHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Load_Items response = (S_Load_Items)packet;

        InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();

        if(manager == null)
            return;

        //TODO: 맵의 아이템에 대한 모든 정보 기입

        manager.f_Loaded_Item = true;
    }

    public static void S_Load_FieldsHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Load_Fields response = (S_Load_Fields)packet;

        InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();

        if(manager == null)
            return;

        manager.f_Loaded_Field = true;
    }

    public static void S_Player_InterpolHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Player_Interpol response = (S_Player_Interpol)packet;

        InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();

        if(manager == null)
            return;

        manager.SynchObjectInPosition(response.AuthCode, response.Transform);

        manager.f_Loaded_Player = true;
    }

    public static void S_SpawnHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Spawn response = (S_Spawn)packet;

        if(Managers.Scene.IsLoading) {
            Managers.Scene.Completed.Enqueue(() => {
                InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();

                if(manager == null)
                    return;

                manager.SpawnPlayerInSpawnPoint(response.AuthCode, response.PrevArea);
            });
        }
    }

    public static void S_Player_LeaveHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Player_Leave response = (S_Player_Leave)packet;

        InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();

        if(manager == null)
            return;

        manager.RemovePlayer(response.AuthCode);
    }


    public static void S_Request_Online_ResponseHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Request_Online_Response response = (S_Request_Online_Response)packet;

        Action<object> requester = null;

        if(Managers.Network.MessageWait.TryGetValue(typeof(S_Request_Online_Response), out requester)) {
            requester.Invoke(response);
            Managers.Network.MessageWait.Remove(typeof(S_Request_Online_Response));
        }
    }


    public static void S_Move_BroadcastHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Move_Broadcast response = (S_Move_Broadcast)packet;

        if(Managers.Network.AuthCode == response.AuthCode)
            return;

        InGameSceneManager manager = Managers.Scene.Manager as InGameSceneManager;
        if(manager == null)
            return;

        Player player = null;
        if(manager._players.TryGetValue(response.AuthCode, out player)){
            //player.Movement.MoveTo(response.Dir, response.Stance);
            player.MoveDir = response.Dir.toVector3();
            player.Stance = response.Stance;
        }
    }

    public static void S_Jump_BroadcastHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Jump_Broadcast response = (S_Jump_Broadcast)packet;

        if(Managers.Network.AuthCode == response.AuthCode)
            return;

        InGameSceneManager manager = Managers.Scene.Manager as InGameSceneManager;
        if(manager == null)
            return;

        Player player = null;
        if(manager._players.TryGetValue(response.AuthCode, out player)) {
            player.Jump();
        }
    }
}