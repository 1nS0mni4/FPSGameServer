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
        Debug.Log("S_Error_Packet Received!");
        ServerSession session = (ServerSession)s;
        S_Error_Packet response = (S_Error_Packet)packet;

        Debug.Log(response.ErrorCode.ToString());
    }

    public static void S_Access_ResponseHandler(PacketSession s, IMessage packet) {
        Debug.Log("S_Access_Response Received!");
        ServerSession session = (ServerSession)s;
        S_Access_Response response = (S_Access_Response)packet;

        Debug.Log($"ErrorCode: {response.ErrorCode}, SessionID: {response.AuthCode}");

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
                Debug.Log("Success!");
                Managers.Network.AuthCode = response.AuthCode;
                Managers.Scene.ChangeSceneTo(pAreaType.Hideout);
            }
            break;
            default: break;
        }
    }

    public static void S_Register_ResponseHandler(PacketSession s, IMessage packet) {
        Debug.Log("S_Register_Response Received!");
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
        Debug.Log("S_Load_Players Received!");
        ServerSession session = (ServerSession)s;
        S_Load_Players response = (S_Load_Players)packet;

        if(Managers.Scene.IsLoading) {
            Managers.Scene.Completed.Enqueue(() => {
                InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();

                if(manager == null)
                    return;

                for(int i = 0; i < response.ObjectList.Count; i++) {
                    manager.SpawnPlayerInPosition(response.ObjectList[i].AuthCode, response.ObjectList[i].Transform);
                }

                manager.f_Loaded_Player = true;
            });
        }
    }

    public static void S_Load_ItemsHandler(PacketSession s, IMessage packet) {
        Debug.Log("S_Load_Items Received!");
        ServerSession session = (ServerSession)s;
        S_Load_Items response = (S_Load_Items)packet;

        if(Managers.Scene.IsLoading) {
            Managers.Scene.Completed.Enqueue(() => {
                InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();

                if(manager == null)
                    return;

                //TODO: ���� �����ۿ� ���� ��� ���� ����

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

    public static void S_Player_InterpolHandler(PacketSession s, IMessage packet) {
        Debug.Log("S_Player_Interpol Received!");
        ServerSession session = (ServerSession)s;
        S_Player_Interpol response = (S_Player_Interpol)packet;

        InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();

        if(manager == null)
            return;

        manager.SyncObjectInPosition(response.AuthCode, response.Transform);
    }

    public static void S_SpawnHandler(PacketSession s, IMessage packet) {
        Debug.Log("S_Spawn Received!");
        ServerSession session = (ServerSession)s;
        S_Spawn response = (S_Spawn)packet;

        Action spawnUser = () => {
            InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();

            if(manager == null)
                return;

            manager.SpawnPlayerInSpawnPoint(response.AuthCode, response.PrevArea, response.DestArea);
        };

        if(Managers.Scene.IsLoading)   //�ε� �� �� ĳ���� ������ ��� �޾��� ��
            Managers.Scene.Completed.Enqueue(spawnUser);
        else
            spawnUser.Invoke();
    }

    public static void S_Player_LeaveHandler(PacketSession s, IMessage packet) {
        Debug.Log("S_Player_Leave Received!");
        ServerSession session = (ServerSession)s;
        S_Player_Leave response = (S_Player_Leave)packet;

        InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();

        if(manager == null)
            return;

        manager.RemovePlayer(response.AuthCode);
    }

    public static void S_Request_Online_ResponseHandler(PacketSession s, IMessage packet) {
        Debug.Log("S_Request_Online_Response Received!");
        ServerSession session = (ServerSession)s;
        S_Request_Online_Response response = (S_Request_Online_Response)packet;

        Action<object> requester = null;

        if(Managers.Network.MessageWait.TryGetValue(typeof(S_Request_Online_Response), out requester)) {
            Managers.Network.MessageWait.Remove(typeof(S_Request_Online_Response));
            requester.Invoke(response);
        }
    }

    public static void S_Move_BroadcastHandler(PacketSession s, IMessage packet) {
        Debug.Log("S_Move_Broadcast Received!");
        ServerSession session = (ServerSession)s;
        S_Move_Broadcast response = (S_Move_Broadcast)packet;

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

    public static void S_Jump_BroadcastHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Jump_Broadcast response = (S_Jump_Broadcast)packet;

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
            manager.SyncObjectInPosition(response.PlayerTransforms[i].AuthCode, response.PlayerTransforms[i].Transform);
        }
    }

    public static void S_Broadcast_Look_RotationHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Broadcast_Look_Rotation response = (S_Broadcast_Look_Rotation)packet;

        InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();
        if(manager == null)
            return;

        manager.SyncPlayerRotation(response.AuthCode, response.Rotation);
    }


}