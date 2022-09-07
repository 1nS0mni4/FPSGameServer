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

    public Action<ushort, IMessage, int> CustomHandler { get; set; } = null;

    private void Register() {
        _makeFunc.Add((ushort)PacketID.CCommonDebug, MakePacket<C_Common_Debug>);
        _handler.Add((ushort)PacketID.CCommonDebug, PacketHandler.C_Common_DebugHandler);
        _makeFunc.Add((ushort)PacketID.CCommonDisconnect, MakePacket<C_Common_Disconnect>);
        _handler.Add((ushort)PacketID.CCommonDisconnect, PacketHandler.C_Common_DisconnectHandler);
        _makeFunc.Add((ushort)PacketID.SGameUserAccess, MakePacket<S_Game_User_Access>);
        _handler.Add((ushort)PacketID.SGameUserAccess, PacketHandler.S_Game_User_AccessHandler);
        _makeFunc.Add((ushort)PacketID.CGameTryEnter, MakePacket<C_Game_Try_Enter>);
        _handler.Add((ushort)PacketID.CGameTryEnter, PacketHandler.C_Game_Try_EnterHandler);
        _makeFunc.Add((ushort)PacketID.CGameMove, MakePacket<C_Game_Move>);
        _handler.Add((ushort)PacketID.CGameMove, PacketHandler.C_Game_MoveHandler);
        _makeFunc.Add((ushort)PacketID.CGameRotation, MakePacket<C_Game_Rotation>);
        _handler.Add((ushort)PacketID.CGameRotation, PacketHandler.C_Game_RotationHandler);
        _makeFunc.Add((ushort)PacketID.CGameTransformSync, MakePacket<C_Game_Transform_Sync>);
        _handler.Add((ushort)PacketID.CGameTransformSync, PacketHandler.C_Game_Transform_SyncHandler);
        _makeFunc.Add((ushort)PacketID.CGameInteract, MakePacket<C_Game_Interact>);
        _handler.Add((ushort)PacketID.CGameInteract, PacketHandler.C_Game_InteractHandler);

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
            CustomHandler.Invoke(msgID, pkt, session.SessionID);
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