using Google.Protobuf;
using Server.Session;
using ServerCore;
using Google.Protobuf.Protocol;
using System;

public static class PacketHandler {
    public static void C_Common_DisconnectHandler(PacketSession s, IMessage packet) {
        C_Game_Move parsedPacket = (C_Game_Move)packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }
    public static void C_Common_Extract_ToHandler(PacketSession s, IMessage packet) {
        C_Game_Move parsedPacket = (C_Game_Move)packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }
    public static void C_Game_MoveHandler(PacketSession s, IMessage packet) {
        C_Game_Move parsedPacket = (C_Game_Move)packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }
    public static void C_Game_Look_RotationHandler(PacketSession s, IMessage packet) {
        C_Game_Move parsedPacket = (C_Game_Move)packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }
    public static void C_Game_JumpHandler(PacketSession s, IMessage packet) {
        C_Game_Move parsedPacket = (C_Game_Move)packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }
    public static void C_Game_Transform_SyncHandler(PacketSession s, IMessage packet) {
        C_Game_Move parsedPacket = (C_Game_Move)packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }
    public static void C_Common_LeaveHandler(PacketSession s, IMessage packet) {
        C_Game_Move parsedPacket = (C_Game_Move)packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }
}