using Google.Protobuf;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public struct PacketModel {
    public ushort packetID;
    public IMessage packet;
}
public class PacketQueue {
    #region Singleton
    private static PacketQueue _instance = new PacketQueue();
    public static PacketQueue Instance { get => _instance; }

    #endregion
    private Queue<PacketModel> _queue = new Queue<PacketModel>();
    private object l_queue = new object();

    public int PacketCount { get => _queue.Count; }

    public void Push(ushort packetID, IMessage packet) {
        lock(l_queue) {
            _queue.Enqueue(new PacketModel() { packetID = packetID, packet = packet});
        }
    }

    public List<PacketModel> PopAll() {
        List<PacketModel> list = new List<PacketModel>();

        lock(l_queue) {
            while(_queue.Count > 0) {
                list.Add(_queue.Dequeue());
            }
        }

        return list;
    }

    public void Clear() {
        lock(l_queue) {
            _queue.Clear();
        }
    }
}
