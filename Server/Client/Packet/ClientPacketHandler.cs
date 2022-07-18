using Google.Protobuf;
using Client.Session;
using ServerCore;
using Google.Protobuf.Protocol;
using System;

public static class PacketHandler {
    public static void S_DebugHandler(PacketSession s, IMessage packet) {
        S_Debug parsedPacket = (S_Debug)packet;
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

    public static void S_Player_ListHandler(PacketSession s, IMessage packet) {
        S_Player_List parsedPacket = (S_Player_List)packet;
        ServerSession session = (ServerSession)s;

        throw new NotImplementedException();
    }
    public static void S_Spawn_IndexHandler(PacketSession s, IMessage packet) {
        S_Spawn_Index parsedPacket = (S_Spawn_Index)packet;
        ServerSession session = (ServerSession)s;

        throw new NotImplementedException();
    }
}