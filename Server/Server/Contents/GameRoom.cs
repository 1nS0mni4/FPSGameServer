using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Contents.Objects.Player;
using Server.Session;
using ServerCore;

namespace Server.Contents {
    //public struct GameRoomInfo {
    //    public int RoomID;
    //    public int MaxCapacity;
    //    public int ConnectedCount;
    //}
    public class GameRoom : IJobQueue {
        private const int max_capacity = 5;
        private JobQueue _jobQueue = new JobQueue();

        private Dictionary<int, ServerCore.Session> _sessions = new Dictionary<int, ServerCore.Session>();

        private Dictionary<int, Player> _players = new Dictionary<int, Player>();

        private int SpawnIndexCount { get; set; } = 4;

        private System.Timers.Timer _timer = null;

        public int RoomID { get; set; }
        public int MaxCapacity {
            get => max_capacity;
        }
        public int ConnectedCount { get => _sessions.Count; }
        public bool CanAccept { get { return MaxCapacity > 0; } }

        public void Init() {
            AddJobTimer(Update, 250f);
        }

        private void AddJobTimer(Action action, float times) {
            if(action == null)
                return;

            _timer = new System.Timers.Timer();
            _timer.Interval = times;
            _timer.Elapsed += (s, e) => action.Invoke();
            _timer.AutoReset = true;
            _timer.Enabled = true;

            GameRoomManager.Instance.Timer.Add(_timer);
        }

        public void Broadcast(IMessage packet) {
            if(_sessions.Count == 0)
                return;

            foreach(ClientSession session in _sessions.Values) {
                session.Send(packet);
            }
        }

        public void DestroyRoom() {
            _sessions = null;
            _jobQueue = null;
            GameRoomManager.Instance.Timer.Remove(_timer);
            Console.WriteLine($"Try to Destory GameRoom {RoomID}");
            GameRoomManager.Instance.DestroyRoom(this);
        }

        public void Push(Action action) {
            _jobQueue.Push(action);
        }

        public void EnterRoom(ClientSession session) {
            if(session == null)
                return;
            if(_sessions.ContainsKey(session.SessionID))
                return;

            _sessions.Add(session.SessionID, session);
            _players.Add(session.SessionID, new Player());

            Console.WriteLine($"Client{session.SessionID} Entered Room{RoomID}");

            //TODO: 오브젝트 데이터 패킷으로 신규 유저에게 전송
            {
                S_Player_List list = new S_Player_List();
                foreach(int sessionID in _players.Keys) {
                    if(sessionID == session.SessionID)
                        continue;
                    list.UserList.Add(new pUserInGameData() { SessionID = sessionID, Position = _players[sessionID].position });
                }
                session.Send(list);
            }

            S_Spawn_Index spawnIndex = new S_Spawn_Index(){ SpawnIndex = GetRandomSpawnIndex() , SessionID = session.SessionID};
            Push(() => Broadcast(spawnIndex));
            AddJobTimer(SendUserObjectSynch, 250f);
        }

        public void LeaveRoom(ClientSession session) {
            if(session == null)
                return;

            _sessions.Remove(session.SessionID);
            //TODO: Broadcast Someone's Leave

            if(_sessions.Count == 0)
                Push(() => DestroyRoom());
        }

        public void Update() {
            foreach(int key in _players.Keys) {
                _players[key].Update();
            }

            //TODO: 다른 Update가 필요한 오브젝트 컨테이너를 여기서 호출
        }

        public int GetRandomSpawnIndex() {
            return Random.Shared.Next(0, SpawnIndexCount);
        }

        public void UserInterpolation(int sessionID, pVector3 position) {
            Player player = null;

            if(_players.TryGetValue(sessionID, out player)) {
                player.position = position;
            }
        }

        //public void MapObjectInitialization() { }

        public void SendUserObjectSynch() {
            List<pUserInGameData> list = new List<pUserInGameData>();

            foreach(int key in _players.Keys) {
                list.Add(new pUserInGameData() { SessionID = key, Position = _players[key].position });
            }

            S_Player_List playerList = new S_Player_List();
            
            for(int i = 0; i < list.Count; i++) {
                playerList.UserList.Add(list[i]);
            }

            Push(() => Broadcast(playerList));
        }
    }
}
