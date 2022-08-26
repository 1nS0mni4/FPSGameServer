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
      _makeFunc.Add((ushort)PacketID.SResponseAccess, MakePacket<S_Response_Access>);
        _handler.Add((ushort)PacketID.SResponseAccess, PacketHandler.S_Response_AccessHandler);
      _makeFunc.Add((ushort)PacketID.SResponseRegister, MakePacket<S_Response_Register>);
        _handler.Add((ushort)PacketID.SResponseRegister, PacketHandler.S_Response_RegisterHandler);
      _makeFunc.Add((ushort)PacketID.SResponseRequestGameSession, MakePacket<S_Response_Request_Game_Session>);
        _handler.Add((ushort)PacketID.SResponseRequestGameSession, PacketHandler.S_Response_Request_Game_SessionHandler);
      _makeFunc.Add((ushort)PacketID.SSpawn, MakePacket<S_Spawn>);
        _handler.Add((ushort)PacketID.SSpawn, PacketHandler.S_SpawnHandler);
      _makeFunc.Add((ushort)PacketID.SResponseRequestOnline, MakePacket<S_Response_Request_Online>);
        _handler.Add((ushort)PacketID.SResponseRequestOnline, PacketHandler.S_Response_Request_OnlineHandler);
      _makeFunc.Add((ushort)PacketID.SLoadPlayers, MakePacket<S_Load_Players>);
        _handler.Add((ushort)PacketID.SLoadPlayers, PacketHandler.S_Load_PlayersHandler);
      _makeFunc.Add((ushort)PacketID.SLoadItems, MakePacket<S_Load_Items>);
        _handler.Add((ushort)PacketID.SLoadItems, PacketHandler.S_Load_ItemsHandler);
      _makeFunc.Add((ushort)PacketID.SLoadFields, MakePacket<S_Load_Fields>);
        _handler.Add((ushort)PacketID.SLoadFields, PacketHandler.S_Load_FieldsHandler);
      _makeFunc.Add((ushort)PacketID.SBroadcastPlayerLeave, MakePacket<S_Broadcast_Player_Leave>);
        _handler.Add((ushort)PacketID.SBroadcastPlayerLeave, PacketHandler.S_Broadcast_Player_LeaveHandler);
      _makeFunc.Add((ushort)PacketID.SBroadcastPlayerMove, MakePacket<S_Broadcast_Player_Move>);
        _handler.Add((ushort)PacketID.SBroadcastPlayerMove, PacketHandler.S_Broadcast_Player_MoveHandler);
      _makeFunc.Add((ushort)PacketID.SBroadcastPlayerRotation, MakePacket<S_Broadcast_Player_Rotation>);
        _handler.Add((ushort)PacketID.SBroadcastPlayerRotation, PacketHandler.S_Broadcast_Player_RotationHandler);
      _makeFunc.Add((ushort)PacketID.SBroadcastPlayerJump, MakePacket<S_Broadcast_Player_Jump>);
        _handler.Add((ushort)PacketID.SBroadcastPlayerJump, PacketHandler.S_Broadcast_Player_JumpHandler);
      _makeFunc.Add((ushort)PacketID.SSyncPlayerTransform, MakePacket<S_Sync_Player_Transform>);
        _handler.Add((ushort)PacketID.SSyncPlayerTransform, PacketHandler.S_Sync_Player_TransformHandler);

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