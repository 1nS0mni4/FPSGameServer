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
      _makeFunc.Add((ushort)PacketID.SErrorPacket, MakePacket<S_Error_Packet>);
        _handler.Add((ushort)PacketID.SErrorPacket, PacketHandler.S_Error_PacketHandler);
      _makeFunc.Add((ushort)PacketID.SAccessResponse, MakePacket<S_Access_Response>);
        _handler.Add((ushort)PacketID.SAccessResponse, PacketHandler.S_Access_ResponseHandler);
      _makeFunc.Add((ushort)PacketID.SRegisterResponse, MakePacket<S_Register_Response>);
        _handler.Add((ushort)PacketID.SRegisterResponse, PacketHandler.S_Register_ResponseHandler);
      _makeFunc.Add((ushort)PacketID.SSpawn, MakePacket<S_Spawn>);
        _handler.Add((ushort)PacketID.SSpawn, PacketHandler.S_SpawnHandler);
      _makeFunc.Add((ushort)PacketID.SPlayerInterpol, MakePacket<S_Player_Interpol>);
        _handler.Add((ushort)PacketID.SPlayerInterpol, PacketHandler.S_Player_InterpolHandler);
      _makeFunc.Add((ushort)PacketID.SLoadPlayers, MakePacket<S_Load_Players>);
        _handler.Add((ushort)PacketID.SLoadPlayers, PacketHandler.S_Load_PlayersHandler);
      _makeFunc.Add((ushort)PacketID.SLoadItems, MakePacket<S_Load_Items>);
        _handler.Add((ushort)PacketID.SLoadItems, PacketHandler.S_Load_ItemsHandler);
      _makeFunc.Add((ushort)PacketID.SLoadFields, MakePacket<S_Load_Fields>);
        _handler.Add((ushort)PacketID.SLoadFields, PacketHandler.S_Load_FieldsHandler);
      _makeFunc.Add((ushort)PacketID.SPlayerLeave, MakePacket<S_Player_Leave>);
        _handler.Add((ushort)PacketID.SPlayerLeave, PacketHandler.S_Player_LeaveHandler);

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