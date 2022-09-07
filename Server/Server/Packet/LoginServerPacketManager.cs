using System;
using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;

public class PacketManager {
    #region Singleton
    public static PacketManager Instance { get; private set; } = new PacketManager();
    #endregion
    public PacketManager() {
        Register();
    }

    private Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _makeFunc = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
    private Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();

    public Action<ushort, IMessage> CustomHandler { get; set; } = null;

    private void Register() {
        _makeFunc.Add((ushort)PacketID.CCommonDebug, MakePacket<C_Common_Debug>);
        _handler.Add((ushort)PacketID.CCommonDebug, PacketHandler.C_Common_DebugHandler);
        _makeFunc.Add((ushort)PacketID.CLoginAccess, MakePacket<C_Login_Access>);
        _handler.Add((ushort)PacketID.CLoginAccess, PacketHandler.C_Login_AccessHandler);
        _makeFunc.Add((ushort)PacketID.CLoginRegister, MakePacket<C_Login_Register>);
        _handler.Add((ushort)PacketID.CLoginRegister, PacketHandler.C_Login_RegisterHandler);
        _makeFunc.Add((ushort)PacketID.CCommonDisconnect, MakePacket<C_Common_Disconnect>);
        _handler.Add((ushort)PacketID.CCommonDisconnect, PacketHandler.C_Common_DisconnectHandler);
        _makeFunc.Add((ushort)PacketID.CLoginRequestOnline, MakePacket<C_Login_Request_Online>);
        _handler.Add((ushort)PacketID.CLoginRequestOnline, PacketHandler.C_Login_Request_OnlineHandler);
        _makeFunc.Add((ushort)PacketID.CLoginRequestGameSession, MakePacket<C_Login_Request_Game_Session>);
        _handler.Add((ushort)PacketID.CLoginRequestGameSession, PacketHandler.C_Login_Request_Game_SessionHandler);
        _makeFunc.Add((ushort)PacketID.SLoginGameStandby, MakePacket<S_Login_Game_Standby>);
        _handler.Add((ushort)PacketID.SLoginGameStandby, PacketHandler.S_Login_Game_StandbyHandler);
        _makeFunc.Add((ushort)PacketID.SLoginNotifyServerInfo, MakePacket<S_Login_Notify_Server_Info>);
        _handler.Add((ushort)PacketID.SLoginNotifyServerInfo, PacketHandler.S_Login_Notify_Server_InfoHandler);
    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> segment) {
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        ushort msgID = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);

        Action<PacketSession, ArraySegment<byte>, ushort> action = null;
        if(_makeFunc.TryGetValue(msgID, out action)) {
            action.Invoke(session, segment, msgID);
        }
    }

    private void MakePacket<T>(PacketSession session, ArraySegment<byte> segment, ushort msgID) where T : IMessage, new() {
        T pkt = new T();
        pkt.MergeFrom(segment.Array, segment.Offset + 4, segment.Count - 4);

        if(CustomHandler != null) {
            CustomHandler.Invoke(msgID, pkt);
        }
        else {
            Action<PacketSession, IMessage> action = null;
            if(_handler.TryGetValue(msgID, out action))
                action.Invoke(session, pkt);
        }
    }

    public Action<PacketSession, IMessage> GetPacketHandler(ushort ID) {
        Action<PacketSession, IMessage> action = null;

        if(_handler.TryGetValue(ID, out action))
            return action;
        return null;
    }
}