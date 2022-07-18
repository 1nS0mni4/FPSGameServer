using Google.Protobuf;
using Server.Session;
using ServerCore;
using Google.Protobuf.Protocol;
using System;
using Server.Contents;
using Server.DB;

public static class PacketHandler {

    public static void C_DebugHandler(PacketSession s, IMessage packet) {
        C_Debug parsedPacket = (C_Debug)packet;
        ClientSession session = (ClientSession)s;

        Console.WriteLine(parsedPacket.Message);
    }

    public static async void C_AccessHandler(PacketSession s, IMessage packet) {
        C_Access parsedPacket = (C_Access)packet;
        ClientSession session = (ClientSession)s;

        UserAccount res = null;

        Task<pAuthResult> accountAsync = DbCommands.AccountValidationCheck(parsedPacket);

        //TODO: DB에서 계정 존재여부 확인
        pAuthResult result = await accountAsync;
        session.AuthCode = result.AuthCode;

        S_Access_Response response = new S_Access_Response(){};
        response.ErrorCode = result.ErrorCode;
        response.SessionID = session.SessionID;

        switch(response.ErrorCode) {
            case NetworkError.Noaccount:        { Console.WriteLine("No Account");          }break;
            case NetworkError.InvalidPassword:  { Console.WriteLine("Invalid Password");    }break;
            case NetworkError.Overlap:          { Console.WriteLine("Overlap");             }break;
            case NetworkError.Success:          { Console.WriteLine("Success");             }break;
        }
        
        session.Send(response);
    }

    public static async void C_RegisterHandler(PacketSession s, IMessage packet) {
        C_Register parsedPacket = (C_Register)packet;
        ClientSession session = (ClientSession)s;

        Task<bool> accRegister = DbCommands.AccountCreate(parsedPacket);

        bool result = await accRegister;

        S_Register_Response response = new S_Register_Response();

        response.ErrorCode = result;
        session.Send(response);
    }
    
    public static async void C_DisconnectHandler(PacketSession s, IMessage packet) {
        C_Disconnect parsedPacket = (C_Disconnect)packet;
        ClientSession session = (ClientSession)s;

        Task<bool> accDisconnect = DbCommands.Disconnect(session.AuthCode);

        bool result = await accDisconnect;
    }

    public static void C_Changed_Scene_ToHandler(PacketSession s, IMessage packet) {
        C_Changed_Scene_To parsedPacket = (C_Changed_Scene_To)packet;
        ClientSession session = (ClientSession)s;

        switch(parsedPacket.SceneType) {
            case pSceneType.Hideout: {

            }break;

            case pSceneType.Fieldmap: {
                GameRoom room = session.Room;

                if(room == null)
                    return;

                //TODO: 필드맵 오브젝트 구현 시 해당 방에 존재하는 오브젝트들에 대한 데이터도 전송 필요
                room.Push(() => room.SendUserObjectSynch());
            }break;
        }
    }

    public static void C_Spawn_PointHandler(PacketSession s, IMessage packet) {
        C_Spawn_Point parsedPacket = (C_Spawn_Point)packet;
        ClientSession session = (ClientSession)s;

        GameRoom room = session.Room;
        if(room == null)
            return;

        room.Push(() => room.UserInterpolation(session.SessionID, parsedPacket.Position));
    }

}