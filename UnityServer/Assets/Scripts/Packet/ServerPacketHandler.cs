using Google.Protobuf;
using Server.Session;
using ServerCore;
using Google.Protobuf.Protocol;
using System;
using Client.Session;

public static class PacketHandler {
    public static void C_Common_DebugHandler(PacketSession s, IMessage packet) {
        C_Common_Debug parsedPacket = (C_Common_Debug)packet;
        ClientSession session = (ClientSession)s;

        if(s == null)
            return;

        throw new NotImplementedException();
    }
    public static void C_Common_DisconnectHandler(PacketSession s, IMessage packet) {
        C_Game_Move parsedPacket = (C_Game_Move)packet;
        ClientSession session = (ClientSession)s;

        if(s == null)
            return;

        throw new NotImplementedException();
    }
    public static void C_Game_MoveHandler(PacketSession s, IMessage packet) {
        C_Game_Move parsedPacket = (C_Game_Move)packet;
        ClientSession session = (ClientSession)s;

        if(s == null)
            return;

        //InGameSceneManager
    }
    public static void C_Game_RotationHandler(PacketSession s, IMessage packet) {
        C_Game_Rotation parsedPacket = (C_Game_Rotation)packet;
        ClientSession session = (ClientSession)s;

        if(s == null)
            return;

        throw new NotImplementedException();
    }
    public static void C_Game_Transform_SyncHandler(PacketSession s, IMessage packet) {
        C_Game_Transform_Sync parsedPacket = (C_Game_Transform_Sync)packet;
        ClientSession session = (ClientSession)s;

        if(s == null)
            return;

        throw new NotImplementedException();
    }

    public static void C_Game_InteractHandler(PacketSession s, IMessage packet) {
        C_Game_Interact parsedPacket = (C_Game_Interact)packet;
        ClientSession session = (ClientSession)s;

        if(s == null)
            return;

        throw new NotImplementedException();
    }

    public static void C_Game_Try_EnterHandler(PacketSession s, IMessage packet) {
        C_Game_Try_Enter parsedPacket = (C_Game_Try_Enter)packet;
        ClientSession session = s as ClientSession;

        if(session == null)
            return;

        session.AuthCode = parsedPacket.AuthCode;

        bool result = InGameSceneManager.Instance.EnterGame(session, session.AuthCode);
        if(result == false)
            session.Disconnect();
    }

    public static void S_Game_User_AccessHandler(PacketSession s, IMessage packet) {
        S_Game_User_Access parsedPacket = (S_Game_User_Access)packet;
        ServerSession session = (ServerSession)s;

        for(int i = 0; i < parsedPacket.UserCount; i++) {
            InGameSceneManager.Instance.RegisterUserAuth(parsedPacket.AuthCode[i]);
        }
    }
}