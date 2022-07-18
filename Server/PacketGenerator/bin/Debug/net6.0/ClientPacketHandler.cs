using Google.Protobuf;
using Client.Session;
using ServerCore;
using Google.Protobuf.Protocol;
using System;

public static class PacketHandler {
    public static void S_Connect_Response      Handler(PacketSession s, IMessage packet) {
        S_Connect_Response       parsedPacket = (S_Connect_Response      )packet;
        ServerSession session = (ServerSession)s;

        throw new NotImplementedException();
    }public static void S_Room_Infos            Handler(PacketSession s, IMessage packet) {
        S_Room_Infos             parsedPacket = (S_Room_Infos            )packet;
        ServerSession session = (ServerSession)s;

        throw new NotImplementedException();
    }public static void S_Create_Room_Response  Handler(PacketSession s, IMessage packet) {
        S_Create_Room_Response   parsedPacket = (S_Create_Room_Response  )packet;
        ServerSession session = (ServerSession)s;

        throw new NotImplementedException();
    }public static void S_Enter_Room_Response   Handler(PacketSession s, IMessage packet) {
        S_Enter_Room_Response    parsedPacket = (S_Enter_Room_Response   )packet;
        ServerSession session = (ServerSession)s;

        throw new NotImplementedException();
    }public static void S_Broadcast_Move        Handler(PacketSession s, IMessage packet) {
        S_Broadcast_Move         parsedPacket = (S_Broadcast_Move        )packet;
        ServerSession session = (ServerSession)s;

        throw new NotImplementedException();
    }
}