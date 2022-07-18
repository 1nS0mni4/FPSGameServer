using Google.Protobuf;
using Server.Session;
using ServerCore;
using Google.Protobuf.Protocol;
using System;

public static class PacketHandler {
    public static void C_Connect               Handler(PacketSession s, IMessage packet) {
        C_Connect                parsedPacket = (C_Connect               )packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }public static void C_Room_Info_Refresh     Handler(PacketSession s, IMessage packet) {
        C_Room_Info_Refresh      parsedPacket = (C_Room_Info_Refresh     )packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }public static void C_Create_Room           Handler(PacketSession s, IMessage packet) {
        C_Create_Room            parsedPacket = (C_Create_Room           )packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }public static void C_Enter_Room            Handler(PacketSession s, IMessage packet) {
        C_Enter_Room             parsedPacket = (C_Enter_Room            )packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }public static void C_Move                  Handler(PacketSession s, IMessage packet) {
        C_Move                   parsedPacket = (C_Move                  )packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }
}