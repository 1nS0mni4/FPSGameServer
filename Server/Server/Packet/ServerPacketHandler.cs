using Google.Protobuf;
using Server.Session;
using ServerCore;
using Google.Protobuf.Protocol;
using System;
using Server.DB;
using Server.Contents.Manager;

public static class PacketHandler {
    public static void C_Common_DebugHandler(PacketSession s, IMessage packet) {
        C_Common_Debug parsedPacket = (C_Common_Debug)packet;
        ClientSession session = (ClientSession)s;

        Console.WriteLine(parsedPacket.Message);
    }

    public static async void C_Login_AccessHandler(PacketSession s, IMessage packet) {
        C_Login_Access parsedPacket = (C_Login_Access)packet;
        ClientSession session = (ClientSession)s;

        pAuthResult result = await Task.Run(() => DbCommands.AccountValidationCheck(parsedPacket));
        session.AuthCode = result.AuthCode;

        S_Response_Access response = new S_Response_Access(){};
        response.ErrorCode = result.ErrorCode;
        response.AuthCode = result.AuthCode;

        session.Send(response);
    }

    public static async void C_Login_RegisterHandler(PacketSession s, IMessage packet) {
        C_Login_Register parsedPacket = (C_Login_Register)packet;
        ClientSession session = (ClientSession)s;

        bool result = await Task.Run(() => DbCommands.AccountCreate(parsedPacket));

        S_Response_Register response = new S_Response_Register();
        response.ErrorCode = result;
        session.Send(response);
    }

    public static async void C_Common_DisconnectHandler(PacketSession s, IMessage packet) {
        C_Common_Disconnect parsedPacket = (C_Common_Disconnect)packet;
        ClientSession session = (ClientSession)s;

        bool result = await Task.Run(() => DbCommands.Disconnect(session.AuthCode));
    }

    public static async void C_Login_Request_Game_SessionHandler(PacketSession s, IMessage packet) {
        C_Login_Request_Game_Session parsedPacket = (C_Login_Request_Game_Session)packet;
        ClientSession session = (ClientSession)s;

        GameServerManager.Instance.TryEnterRoom(session, parsedPacket.AreaType, parsedPacket.UserCount);
    }

    public static void C_Login_Request_OnlineHandler(PacketSession s, IMessage packet) {
        C_Login_Request_Online parsedPacket = (C_Login_Request_Online)packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }

    public static async void S_Login_Game_StandbyHandler(PacketSession s, IMessage packet) {
        S_Login_Game_Standby parsedPacket = (S_Login_Game_Standby)packet;
        GameServerSession session = (GameServerSession)s;

        GameServerManager.Instance.GameServerStandby(session, parsedPacket);
        Console.WriteLine("Server Standby!");
    }
     
    public static async void S_Login_Notify_Server_InfoHandler(PacketSession s, IMessage packet) {
        S_Login_Notify_Server_Info parsedPacket = (S_Login_Notify_Server_Info)packet;
        GameServerSession session = (GameServerSession)s;

        GameServerManager.Instance.GameServerUpdate(parsedPacket);
    }
}