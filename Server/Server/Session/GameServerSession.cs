using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Server.Session {
    public class GameServerSession : PacketSession {
        public int AuthCode { get; set; }

        public pAreaType AreaType { get; set; }
        public bool CanAccess { get; set; } = false;
        public long ServerIP { get; set; }
        public int ServerPort { get; set; }

        public override void OnConnect(EndPoint endPoint) {
            //TODO: 서버 초기화 데이터 전송하기. ex) AreaType 지정해주기 등등...
        }

        public override void OnDisconnect(EndPoint endPoint) {
            Console.WriteLine($"AuthCode: {AuthCode} AreaType: {AreaType} Server Disconnected");
        }

        public override void OnRecvPacket(ArraySegment<byte> segment) {
            PacketManager.Instance.OnRecvPacket(this, segment);
        }

        public override void OnSend(int numOfBytes) {

        }
    }
}
