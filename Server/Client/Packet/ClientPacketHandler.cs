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
    }

    public static void S_Access_ResponseHandler(PacketSession s, IMessage packet) {
        S_Access_Response parsedPacket = (S_Access_Response)packet;
        ServerSession session = (ServerSession)s;

        throw new NotImplementedException();
    }
    public static void S_Register_ResponseHandler(PacketSession s, IMessage packet) {
        S_Access_Response parsedPacket = (S_Access_Response)packet;
        ServerSession session = (ServerSession)s;

        throw new NotImplementedException();
    }

    public static void S_Spawn_IndexHandler(PacketSession s, IMessage packet) {
        S_Spawn_Index parsedPacket = (S_Spawn_Index)packet;
        ServerSession session = (ServerSession)s;

        throw new NotImplementedException();
    }
    public static void S_Load_PlayersHandler(PacketSession s, IMessage packet) {
        S_Spawn_Index parsedPacket = (S_Spawn_Index)packet;
        ServerSession session = (ServerSession)s;

        throw new NotImplementedException();
    }
    public static void S_Load_ItemsHandler(PacketSession s, IMessage packet) {
        S_Spawn_Index parsedPacket = (S_Spawn_Index)packet;
        ServerSession session = (ServerSession)s;

        throw new NotImplementedException();
    }
    public static void S_Load_FieldsHandler(PacketSession s, IMessage packet) {
        S_Spawn_Index parsedPacket = (S_Spawn_Index)packet;
        ServerSession session = (ServerSession)s;

        throw new NotImplementedException();
    }
}