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
    }public static void C_Changed_Scene_ToHandler(PacketSession s, IMessage packet) {
        C_Changed_Scene_To parsedPacket = (C_Changed_Scene_To)packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }public static void C_Spawn_PointHandler(PacketSession s, IMessage packet) {
        C_Spawn_Point parsedPacket = (C_Spawn_Point)packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }
}