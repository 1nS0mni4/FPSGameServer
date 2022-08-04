using Google.Protobuf;
using Server.Session;
using ServerCore;
using Google.Protobuf.Protocol;
using System;

public static class PacketHandler {
    public static void C_DebugHandler(PacketSession s, IMessage packet) {
        C_Debug parsedPacket = (C_Debug)packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }public static void C_AccessHandler(PacketSession s, IMessage packet) {
        C_Access parsedPacket = (C_Access)packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }public static void C_RegisterHandler(PacketSession s, IMessage packet) {
        C_Register parsedPacket = (C_Register)packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }public static void C_DisconnectHandler(PacketSession s, IMessage packet) {
        C_Disconnect parsedPacket = (C_Disconnect)packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }public static void C_Extract_ToHandler(PacketSession s, IMessage packet) {
        C_Extract_To parsedPacket = (C_Extract_To)packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }public static void C_Spawn_ResponseHandler(PacketSession s, IMessage packet) {
        C_Spawn_Response parsedPacket = (C_Spawn_Response)packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }public static void C_Request_OnlineHandler(PacketSession s, IMessage packet) {
        C_Request_Online parsedPacket = (C_Request_Online)packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }public static void C_MoveHandler(PacketSession s, IMessage packet) {
        C_Move parsedPacket = (C_Move)packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }public static void C_JumpHandler(PacketSession s, IMessage packet) {
        C_Jump parsedPacket = (C_Jump)packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }
}