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
      _makeFunc.Add((ushort)PacketID.CDebug, MakePacket<C_Debug>);
        _handler.Add((ushort)PacketID.CDebug, PacketHandler.C_DebugHandler);
      _makeFunc.Add((ushort)PacketID.CAccess, MakePacket<C_Access>);
        _handler.Add((ushort)PacketID.CAccess, PacketHandler.C_AccessHandler);
      _makeFunc.Add((ushort)PacketID.CRegister, MakePacket<C_Register>);
        _handler.Add((ushort)PacketID.CRegister, PacketHandler.C_RegisterHandler);
      _makeFunc.Add((ushort)PacketID.CDisconnect, MakePacket<C_Disconnect>);
        _handler.Add((ushort)PacketID.CDisconnect, PacketHandler.C_DisconnectHandler);
      _makeFunc.Add((ushort)PacketID.CExtractTo, MakePacket<C_Extract_To>);
        _handler.Add((ushort)PacketID.CExtractTo, PacketHandler.C_Extract_ToHandler);
      _makeFunc.Add((ushort)PacketID.CSpawnResponse, MakePacket<C_Spawn_Response>);
        _handler.Add((ushort)PacketID.CSpawnResponse, PacketHandler.C_Spawn_ResponseHandler);
      _makeFunc.Add((ushort)PacketID.CRequestOnline, MakePacket<C_Request_Online>);
        _handler.Add((ushort)PacketID.CRequestOnline, PacketHandler.C_Request_OnlineHandler);
      _makeFunc.Add((ushort)PacketID.CMove, MakePacket<C_Move>);
        _handler.Add((ushort)PacketID.CMove, PacketHandler.C_MoveHandler);
      _makeFunc.Add((ushort)PacketID.CJump, MakePacket<C_Jump>);
        _handler.Add((ushort)PacketID.CJump, PacketHandler.C_JumpHandler);
      _makeFunc.Add((ushort)PacketID.CTransformSync, MakePacket<C_Transform_Sync>);
        _handler.Add((ushort)PacketID.CTransformSync, PacketHandler.C_Transform_SyncHandler);
      _makeFunc.Add((ushort)PacketID.CLookRotation, MakePacket<C_Look_Rotation>);
        _handler.Add((ushort)PacketID.CLookRotation, PacketHandler.C_Look_RotationHandler);
      _makeFunc.Add((ushort)PacketID.CTimeCheckResponse, MakePacket<C_Time_Check_Response>);
        _handler.Add((ushort)PacketID.CTimeCheckResponse, PacketHandler.C_Time_Check_ResponseHandler);

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