using Google.Protobuf;
using Client.Session;
using ServerCore;
using Google.Protobuf.Protocol;
using System;

public static class PacketHandler {
    public static void S_Error_PacketHandler(PacketSession s, IMessage packet) {
        S_Error_Packet parsedPacket = (S_Error_Packet)packet;
        ServerSession session = (ServerSession)s;

        throw new NotImplementedException();
    }public static void S_Access_ResponseHandler(PacketSession s, IMessage packet) {
        S_Access_Response parsedPacket = (S_Access_Response)packet;
        ServerSession session = (ServerSession)s;

        throw new NotImplementedException();
    }public static void S_Register_ResponseHandler(PacketSession s, IMessage packet) {
        S_Register_Response parsedPacket = (S_Register_Response)packet;
        ServerSession session = (ServerSession)s;

        throw new NotImplementedException();
    }public static void S_SpawnHandler(PacketSession s, IMessage packet) {
        S_Spawn parsedPacket = (S_Spawn)packet;
        ServerSession session = (ServerSession)s;

        throw new NotImplementedException();
    }public static void S_Player_InterpolHandler(PacketSession s, IMessage packet) {
        S_Player_Interpol parsedPacket = (S_Player_Interpol)packet;
        ServerSession session = (ServerSession)s;

        throw new NotImplementedException();
    }public static void S_Load_PlayersHandler(PacketSession s, IMessage packet) {
        S_Load_Players parsedPacket = (S_Load_Players)packet;
        ServerSession session = (ServerSession)s;

        throw new NotImplementedException();
    }public static void S_Load_ItemsHandler(PacketSession s, IMessage packet) {
        S_Load_Items parsedPacket = (S_Load_Items)packet;
        ServerSession session = (ServerSession)s;

        throw new NotImplementedException();
    }public static void S_Load_FieldsHandler(PacketSession s, IMessage packet) {
        S_Load_Fields parsedPacket = (S_Load_Fields)packet;
        ServerSession session = (ServerSession)s;

        throw new NotImplementedException();
    }public static void S_Player_LeaveHandler(PacketSession s, IMessage packet) {
        S_Player_Leave parsedPacket = (S_Player_Leave)packet;
        ServerSession session = (ServerSession)s;

        throw new NotImplementedException();
    }
}