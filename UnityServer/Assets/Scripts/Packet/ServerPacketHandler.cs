using Google.Protobuf;
using Server.Session;
using ServerCore;
using Google.Protobuf.Protocol;
using System;
using Client.Session;
using Extensions;
using System.Threading.Tasks;
using UnityEngine;

public static class PacketHandler {
    public static async void C_Common_DebugHandler(PacketSession s, IMessage packet) {
        C_Common_Debug parsedPacket = (C_Common_Debug)packet;
        ClientSession session = s as ClientSession;

        if(session == null)
            return;

        throw new NotImplementedException();
    }
    public static async void C_Common_DisconnectHandler(PacketSession s, IMessage packet) {
        C_Game_Input parsedPacket = (C_Game_Input)packet;
        ClientSession session = s as ClientSession;

        if(session == null)
            return;

        await Task.Run(() => InGameSceneManager.Instance.Disconnect(session.AuthCode));
    }
    public static async void C_Game_Try_EnterHandler(PacketSession s, IMessage packet) {
        Debug.Log("C_Game_Try_Enter Received!");
        C_Game_Try_Enter parsedPacket = (C_Game_Try_Enter)packet;
        ClientSession session = s as ClientSession;

        if(session == null)
            return;

        session.AuthCode = parsedPacket.AuthCode;

        bool result = await Task.Run(() => InGameSceneManager.Instance.EnterGame(session));
        if(result == false)
            session.Disconnect();
    }
    public static void C_Game_InputHandler(PacketSession s, IMessage packet) {
        C_Game_Input parsedPacket = (C_Game_Input)packet;
        ClientSession session = s as ClientSession;

        if(null == session)
            return;

        if(null == session.Character)
            return;

        parsedPacket.Inputs.CopyTo(session.Character.Inputs, 0);
        session.Character.RotateTo(parsedPacket.CamFront.ToUnityQuaternion());
    }
    public static void C_Game_InteractHandler(PacketSession s, IMessage packet) {
        C_Game_Interact parsedPacket = (C_Game_Interact)packet;
        ClientSession session = s as ClientSession;

        if(session == null)
            return;

        //TOOD: 상호작용 대상 데이터 전달 필요할 경우 전송
        throw new NotImplementedException();
    }
    public static void S_Game_User_AccessHandler(PacketSession s, IMessage packet) {
        S_Game_User_Access parsedPacket = (S_Game_User_Access)packet;
        ServerSession session = s as ServerSession;

        if(session == null)
            return;

        for(int i = 0; i < parsedPacket.UserCount; i++) {
            InGameSceneManager.Instance.RegisterUserAuth(parsedPacket.AuthCode[i]);
        }
    }
}