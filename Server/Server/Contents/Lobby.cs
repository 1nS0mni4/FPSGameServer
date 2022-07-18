using Server.Session;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.Protocol;

namespace Server.Contents {
    public class Lobby : IJobQueue{
        #region Singleton
        private static Lobby _instance = null;
        static Lobby() { _instance = new Lobby(); }
        public static Lobby Instance { get => _instance; }

        #endregion

        private Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();
        private JobQueue _jobQueue = new JobQueue();
        private object l_sessions = new object();

        public void EnterLobby(ClientSession session) {
            if(session == null)
                return;
            if(_sessions.ContainsKey(session.SessionID))
                return;

            _sessions.Add(session.SessionID, session);

            //TODO: 방 정보를 GameRoomManager에서 모아서 유저에게 전송한다.
            //List<GameRoomInfo> roomInfos = GameRoomManager.Instance.GetRoomsInfo();
            //S_Room_Infos roomInfoPacket = new S_Room_Infos();
            //roomInfoPacket.RoomInfos.AddRange(roomInfos);
            //session.Send(roomInfoPacket);
        }

        public void Push(Action action) {
            _jobQueue.Push(action);
        }
    }
}
