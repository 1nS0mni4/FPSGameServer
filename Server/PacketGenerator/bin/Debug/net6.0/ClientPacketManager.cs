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
      _makeFunc.Add((ushort)MsgID.SConnectResponse      , MakePacket<S_Connect_Response      >);
        _handler.Add((ushort)MsgID.SConnectResponse      , PacketHandler.S_Connect_Response      Handler);
      _makeFunc.Add((ushort)MsgID.SRoomInfos            , MakePacket<S_Room_Infos            >);
        _handler.Add((ushort)MsgID.SRoomInfos            , PacketHandler.S_Room_Infos            Handler);
      _makeFunc.Add((ushort)MsgID.SCreateRoomResponse  , MakePacket<S_Create_Room_Response  >);
        _handler.Add((ushort)MsgID.SCreateRoomResponse  , PacketHandler.S_Create_Room_Response  Handler);
      _makeFunc.Add((ushort)MsgID.SEnterRoomResponse   , MakePacket<S_Enter_Room_Response   >);
        _handler.Add((ushort)MsgID.SEnterRoomResponse   , PacketHandler.S_Enter_Room_Response   Handler);
      _makeFunc.Add((ushort)MsgID.SBroadcastMove        , MakePacket<S_Broadcast_Move        >);
        _handler.Add((ushort)MsgID.SBroadcastMove        , PacketHandler.S_Broadcast_Move        Handler);

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