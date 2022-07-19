using Google.Protobuf;
using Client.Session;
using ServerCore;
using Google.Protobuf.Protocol;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using static Define;

public static class PacketHandler {
    public static void S_DebugHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Debug response = (S_Debug)packet;

        Debug.Log($"Response: {response.Message}");
    }

    public static void S_Access_ResponseHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Access_Response response = (S_Access_Response)packet;

        Debug.Log($"ErrorCode: {response.ErrorCode}, SessionID: {response.SessionID}");

        LoginUIManager ui = UIManager.GetUIManager<LoginUIManager>();
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
                Managers.Network.SessionID = response.SessionID;
                Managers.Scene.ChangeSceneTo(pSceneType.Hideout);
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

        LoginUIManager ui = UIManager.GetUIManager<LoginUIManager>();
        if(ui == null)
            return;

        Debug.Log("Register Success!");
    }

    public static void S_Player_ListHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Player_List response = (S_Player_List)packet;

        throw new NotImplementedException();
    }

    public static void S_Spawn_IndexHandler(PacketSession s, IMessage packet) {
        ServerSession session = (ServerSession)s;
        S_Spawn_Index response = (S_Spawn_Index)packet;

        FieldmapSceneManager manager = Managers.Scene.GetManager<FieldmapSceneManager>();

        if(manager == null)
            return;

        manager.SpawnPlayerWithPoint(response.SessionID, response.SpawnIndex);
    }
}