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
      _makeFunc.Add((ushort)PacketID.SDebug, MakePacket<S_Debug>);
        _handler.Add((ushort)PacketID.SDebug, PacketHandler.S_DebugHandler);
      _makeFunc.Add((ushort)PacketID.SAccessResponse, MakePacket<S_Access_Response>);
        _handler.Add((ushort)PacketID.SAccessResponse, PacketHandler.S_Access_ResponseHandler);
      _makeFunc.Add((ushort)PacketID.SRegisterResponse, MakePacket<S_Register_Response>);
        _handler.Add((ushort)PacketID.SRegisterResponse, PacketHandler.S_Register_ResponseHandler);
      _makeFunc.Add((ushort)PacketID.SPlayerList, MakePacket<S_Player_List>);
        _handler.Add((ushort)PacketID.SPlayerList, PacketHandler.S_Player_ListHandler);
      _makeFunc.Add((ushort)PacketID.SSpawnIndex, MakePacket<S_Spawn_Index>);
        _handler.Add((ushort)PacketID.SSpawnIndex, PacketHandler.S_Spawn_IndexHandler);

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