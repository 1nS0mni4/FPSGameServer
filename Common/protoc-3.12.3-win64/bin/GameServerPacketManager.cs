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
      _makeFunc.Add((ushort)PacketID.CCommonDisconnect, MakePacket<C_Common_Disconnect>);
        _handler.Add((ushort)PacketID.CCommonDisconnect, PacketHandler.C_Common_DisconnectHandler);
      _makeFunc.Add((ushort)PacketID.CCommonExtractTo, MakePacket<C_Common_Extract_To>);
        _handler.Add((ushort)PacketID.CCommonExtractTo, PacketHandler.C_Common_Extract_ToHandler);
      _makeFunc.Add((ushort)PacketID.CGameMove, MakePacket<C_Game_Move>);
        _handler.Add((ushort)PacketID.CGameMove, PacketHandler.C_Game_MoveHandler);
      _makeFunc.Add((ushort)PacketID.CGameLookRotation, MakePacket<C_Game_Look_Rotation>);
        _handler.Add((ushort)PacketID.CGameLookRotation, PacketHandler.C_Game_Look_RotationHandler);
      _makeFunc.Add((ushort)PacketID.CGameJump, MakePacket<C_Game_Jump>);
        _handler.Add((ushort)PacketID.CGameJump, PacketHandler.C_Game_JumpHandler);
      _makeFunc.Add((ushort)PacketID.CGameTransformSync, MakePacket<C_Game_Transform_Sync>);
        _handler.Add((ushort)PacketID.CGameTransformSync, PacketHandler.C_Game_Transform_SyncHandler);
      _makeFunc.Add((ushort)PacketID.CCommonLeave, MakePacket<C_Common_Leave>);
        _handler.Add((ushort)PacketID.CCommonLeave, PacketHandler.C_Common_LeaveHandler);

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