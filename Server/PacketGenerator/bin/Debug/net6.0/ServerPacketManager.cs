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
      _makeFunc.Add((ushort)MsgID.CConnect               , MakePacket<C_Connect               >);
        _handler.Add((ushort)MsgID.CConnect               , PacketHandler.C_Connect               Handler);
      _makeFunc.Add((ushort)MsgID.CRoomInfoRefresh     , MakePacket<C_Room_Info_Refresh     >);
        _handler.Add((ushort)MsgID.CRoomInfoRefresh     , PacketHandler.C_Room_Info_Refresh     Handler);
      _makeFunc.Add((ushort)MsgID.CCreateRoom           , MakePacket<C_Create_Room           >);
        _handler.Add((ushort)MsgID.CCreateRoom           , PacketHandler.C_Create_Room           Handler);
      _makeFunc.Add((ushort)MsgID.CEnterRoom            , MakePacket<C_Enter_Room            >);
        _handler.Add((ushort)MsgID.CEnterRoom            , PacketHandler.C_Enter_Room            Handler);
      _makeFunc.Add((ushort)MsgID.CMove                  , MakePacket<C_Move                  >);
        _handler.Add((ushort)MsgID.CMove                  , PacketHandler.C_Move                  Handler);

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