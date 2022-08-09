using Google.Protobuf;
using Server.Session;
using ServerCore;
using Google.Protobuf.Protocol;
using System;
using Server.DB;
using Server.Contents.Sessions;
using Server.Contents.Sessions.Base;
using UnityEngine;

public static class PacketHandler {
    public static void C_DebugHandler(PacketSession s, IMessage packet) {
        C_Debug parsedPacket = (C_Debug)packet;
        ClientSession session = (ClientSession)s;

        Console.WriteLine(parsedPacket.Message);
    }

    public static async void C_AccessHandler(PacketSession s, IMessage packet) {
        C_Access parsedPacket = (C_Access)packet;
        ClientSession session = (ClientSession)s;

        pAuthResult result = await Task.Run(() => DbCommands.AccountValidationCheck(parsedPacket));
        session.AuthCode = result.AuthCode;

        S_Access_Response response = new S_Access_Response(){};
        response.ErrorCode = result.ErrorCode;
        response.AuthCode = result.AuthCode;

        session.Send(response);

        if(response.ErrorCode == NetworkError.Success) {
            S_Spawn spawn = new S_Spawn();
            spawn.AuthCode = session.AuthCode;
            spawn.PrevArea = pAreaType.Gamestart;
            spawn.DestArea = pAreaType.Hideout;

            session.Send(spawn);
        }
    }

    public static async void C_RegisterHandler(PacketSession s, IMessage packet) {
        C_Register parsedPacket = (C_Register)packet;
        ClientSession session = (ClientSession)s;

        bool result = await Task.Run(() => DbCommands.AccountCreate(parsedPacket));

        S_Register_Response response = new S_Register_Response();
        response.ErrorCode = result;
        session.Send(response);
    }

    public static async void C_DisconnectHandler(PacketSession s, IMessage packet) {
        C_Disconnect parsedPacket = (C_Disconnect)packet;
        ClientSession session = (ClientSession)s;

        bool result = await Task.Run(() => DbCommands.Disconnect(session.AuthCode));
    }

    public static async void C_Extract_ToHandler(PacketSession s, IMessage packet) {
        C_Extract_To parsedPacket = (C_Extract_To)packet;
        ClientSession session = (ClientSession)s;

        bool result = true;

        switch(parsedPacket.DestArea) {
            case pAreaType.Hideout: {
                GameRoom section = session.Section;
                if(section != null) {
                    section.Push(() => section.Leave(session.AuthCode));
                    section = null;
                }

                if(parsedPacket.RoomCode > 0) {   //방 코드가 있을 경우
                    result = await Task.Run(() => { 
                        return HideoutManager.Instance.TryEnterRoom(parsedPacket.RoomCode, session, parsedPacket.PrevArea, parsedPacket.DestArea); 
                    });
                }
                else {
                    S_Spawn spawn = new S_Spawn();
                    spawn.AuthCode = session.AuthCode;
                    spawn.PrevArea = parsedPacket.PrevArea;
                    spawn.DestArea = parsedPacket.DestArea;

                    session.Send(spawn);
                }

                //TODO: 개인 창고 데이터 전송
            }break;

            case pAreaType.Cityhall:
            case pAreaType.Residential:
            case pAreaType.Industrial:
            case pAreaType.Commerce: {
                result = parsedPacket.PrevArea == pAreaType.Hideout ?
                    await Task.Run(() => {
                        GameRoom section = session.Section;
                        if(section != null)
                            section.Push(() => section.Leave(session.AuthCode));

                        return FieldmapManager.Instance.TryEnterField(session, parsedPacket.PrevArea, parsedPacket.DestArea);
                    }) :
                    await Task.Run(() => {
                        GameRoom section = session.Section;
                        if(section == null)
                            return false;
                        return true;
                    });

                //TODO: 인벤토리 데이터 전송
            }break;
        }

        if(result == false) {
            S_Error_Packet errPacket = new S_Error_Packet();
            errPacket.ErrorCode = NetworkError.InvalidAccess;
            session.Send(errPacket);
            session.Disconnect();
        }
    }

    public static void C_Spawn_ResponseHandler(PacketSession s, IMessage packet) {
        C_Spawn_Response parsedPacket = (C_Spawn_Response)packet;
        ClientSession session = (ClientSession)s;

        GameRoom section = session.Section;
        if(section == null)
            return;

        section.Push(() => section.Spawn_Player(session.AuthCode, parsedPacket.Position));
    }

    public static void C_Request_OnlineHandler(PacketSession s, IMessage packet) {
        C_Request_Online parsedPacket = (C_Request_Online)packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }

    public static void C_MoveHandler(PacketSession s, IMessage packet) {
        C_Move parsedPacket = (C_Move)packet;
        ClientSession session = (ClientSession)s;

        GameRoom section = session.Section;
        if(section == null)
            return;

        section.Push(() => section.HandleMove(session, parsedPacket));
    }

    public static void C_JumpHandler(PacketSession s, IMessage packet) {
        C_Move parsedPacket = (C_Move)packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }

    public static void C_Transform_SyncHandler(PacketSession s, IMessage packet) {
        C_Transform_Sync parsedPacket = (C_Transform_Sync)packet;
        ClientSession session = (ClientSession)s;

        GameRoom section = session.Section;
        if(section == null)
            return;

        section.Push(() => section.Sync_PlayerPosition(session.AuthCode, parsedPacket.Position, false));
    }
    public static void C_Look_RotationHandler(PacketSession s, IMessage packet) {
        C_Look_Rotation parsedPacket = (C_Look_Rotation)packet;
        ClientSession session = (ClientSession)s;

        GameRoom section = session.Section;
        if(section == null)
            return;

        section.Push(() => section.Sync_PlayerRotation(session.AuthCode, parsedPacket.Rotation));
    }
}