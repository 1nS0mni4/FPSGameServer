using Google.Protobuf;
using Server.Session;
using ServerCore;
using Google.Protobuf.Protocol;
using System;
using Server.DB;
using Server.Contents.Manager;

public static class PacketHandler {
    public static void C_Login_DebugHandler(PacketSession s, IMessage packet) {
        C_Login_Debug parsedPacket = (C_Login_Debug)packet;
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

        if(response.ErrorCode == NetworkError.Success) {
            S_Spawn spawn = new S_Spawn();
            spawn.AuthCode = session.AuthCode;
            spawn.PrevArea = pAreaType.Gamestart;
            spawn.DestArea = pAreaType.Hideout;

            session.Send(spawn);
        }
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

    //TODO: 서버 적용 시 수정 필요
    public static async void C_Common_Extract_ToHandler(PacketSession s, IMessage packet) {
        C_Common_Extract_To parsedPacket = (C_Common_Extract_To)packet;
        ClientSession session = (ClientSession)s;

        {/*
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
                }
                break;

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
                }
                break;
            }

            if(result == false) {
                S_Error_Packet errPacket = new S_Error_Packet();
                errPacket.ErrorCode = NetworkError.InvalidAccess;
                session.Send(errPacket);
                session.Disconnect();
            }
        */}



        throw new NotImplementedException();
    }

    public static async void C_Common_LeaveHandler(PacketSession s, IMessage packet) {
        C_Common_Leave parsedPacket = (C_Common_Leave)packet;
        ClientSession session = (ClientSession)s;

        throw new NotImplementedException();
    }

    public static async void S_Login_Game_StandbyHandler(PacketSession s, IMessage packet) {
        S_Login_Game_Standby parsedPacket = (S_Login_Game_Standby)packet;
        GameServerSession session = (GameServerSession)s;

        GameServerManager.Instance.GameServerStandby(session, parsedPacket);
    }

    public static async void S_Login_Game_Info_NotifyHandler(PacketSession s, IMessage packet) {
        S_Login_Game_Info_Notify parsedPacket = (S_Login_Game_Info_Notify)packet;
        GameServerSession session = (GameServerSession)s;

        GameServerManager.Instance.GameServerUpdate(parsedPacket);
    }
}