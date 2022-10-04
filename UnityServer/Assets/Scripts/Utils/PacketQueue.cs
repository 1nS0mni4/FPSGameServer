using Google.Protobuf;
using ServerCore;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public struct PacketModel {
    public PacketSession session;
    public ushort packetID;
    public IMessage packet;
}
public class PacketQueue {
    #region Singleton
    private static PacketQueue _instance;
    static PacketQueue() {
        _instance = new PacketQueue();
    }
    public static PacketQueue Instance { get => _instance; }

    #endregion
    private ConcurrentQueue<PacketModel> _queue = new ConcurrentQueue<PacketModel>();
    private object l_queue = new object();
    public int PacketCount { get => _queue.Count; }

    public void Push(PacketSession session, IMessage packet, ushort packetID) {
        lock(l_queue) {
            _queue.Enqueue(new PacketModel() { session = session, packetID = packetID, packet = packet });
        }
    }

    public List<PacketModel> PopAll() {
        List<PacketModel> list = new List<PacketModel>();

        while(_queue.Count > 0) {
            if(_queue.TryDequeue(out PacketModel model))
                list.Add(model);
        }

        return list;
    }

    public void Clear() {
        _queue.Clear();
    }
}
