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
      _makeFunc.Add((ushort)PacketID.CChangedSceneTo, MakePacket<C_Changed_Scene_To>);
        _handler.Add((ushort)PacketID.CChangedSceneTo, PacketHandler.C_Changed_Scene_ToHandler);
      _makeFunc.Add((ushort)PacketID.CSpawnPoint, MakePacket<C_Spawn_Point>);
        _handler.Add((ushort)PacketID.CSpawnPoint, PacketHandler.C_Spawn_PointHandler);

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