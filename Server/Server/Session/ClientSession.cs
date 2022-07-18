using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Contents;
using Server.DB;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.Session {
    public class ClientSession : PacketSession {
        public int AuthCode { get; set; }
        public GameRoom Room { get; set; }
        public override void OnConnect(EndPoint endPoint) {
            Console.WriteLine($"Connected To: {endPoint}");
        }

        public override void OnDisconnect(EndPoint endPoint) {
            Console.WriteLine($"Disconnected: {endPoint}");
            if(Room != null) {
                GameRoom room = Room;
                room.Push(() => { room.LeaveRoom(this); });
            }
        }

        public override void OnRecvPacket(ArraySegment<byte> segment) {
            PacketManager.Instance.OnRecvPacket(this, segment);
        }

        public override void OnSend(int numOfBytes) {
            
        }

        public void Send(IMessage packet) {
            ushort size = (ushort)(packet.CalculateSize() + 4);
            PacketID msgID = (PacketID)Enum.Parse(typeof(PacketID), packet.Descriptor.Name.Replace("_", string.Empty));

            byte[] buffer = new byte[size];
            Array.Copy(BitConverter.GetBytes(size), 0, buffer, 0, sizeof(ushort));
            Array.Copy(BitConverter.GetBytes((ushort)msgID), 0, buffer, 2, sizeof(ushort));
            Array.Copy(packet.ToByteArray(), 0, buffer, 4, packet.CalculateSize());

            Send(new ArraySegment<byte>(buffer, 0, size));
        }
    }
}
