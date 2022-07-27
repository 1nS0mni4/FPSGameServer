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
        protected const int max_capacity = 5;
        protected JobQueue _jobQueue = new JobQueue();

        protected Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();

        protected Dictionary<int, Player> _players = new Dictionary<int, Player>();

        protected int SpawnIndexCount { get; set; } = 4;

        protected System.Timers.Timer _timer = null;

        public int RoomCode { get; set; }
        public int ConnectedCount { get => _sessions.Count; }
        public bool CanAccept { get { return max_capacity - _sessions.Count > 0; } }

        public void Init() {
            AddJobTimer(Update, 250f);
        }

        protected void AddJobTimer(Action action, float times) {
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
            Console.WriteLine($"Try to Destory GameRoom {RoomCode}");
            GameRoomManager.Instance.DestroyRoom(this);
        }

        public void Push(Action action) {
            _jobQueue.Push(action);
        }

        public void Enter(ClientSession session, pAreaType prevArea, pAreaType destArea) {
            if(session == null)
                return;
            if(_sessions.ContainsKey(session.AuthCode))
                return;

            _sessions.Add(session.AuthCode, session);
            session.Section = this;

            Console.WriteLine($"Client{session.AuthCode} Entered Room{RoomCode}");

            //TODO: 오브젝트 데이터 패킷으로 신규 유저에게 전송
            {
                S_Load_Players playerList = GetpUserInGameDatas(session.AuthCode);
                session.Send(playerList);
                S_Load_Items itemList = new S_Load_Items();
                session.Send(itemList);
                S_Load_Fields fieldList = new S_Load_Fields();
                session.Send(fieldList);
            }

            S_Spawn spawn = new S_Spawn();
            spawn.AuthCode = session.AuthCode;
            spawn.PrevArea = prevArea;
            spawn.DestArea = destArea;

            Push(() => Broadcast(spawn));
            AddJobTimer(SendUserObjectSynch, 500f);
        }

        public void Leave(ClientSession session) {
            if(session == null)
                return;

            _sessions.Remove(session.SessionID);
            //TODO: Broadcast Someone's Leave

            S_Player_Leave playerLeave = new S_Player_Leave();
            playerLeave.AuthCode = session.AuthCode;

            Push(() => Broadcast(playerLeave));

            if(_sessions.Count == 0)
                Push(() => DestroyRoom());
        }

        public void Update() {
            foreach(int key in _players.Keys) {
                _players[key].Update();
            }

            //TODO: 다른 Update가 필요한 오브젝트 컨테이너를 여기서 호출
        }
        public void UserInterpolation(int authCode, pTransform transform) {
            Player player = null;

            if(_players.TryGetValue(authCode, out player)) {
                player.transform = transform;
                player.isInterpolated = true;
            }
            else {
                player = new Player();
                player.AuthCode = authCode;
                player.transform = transform;
                _players.Add(player.AuthCode, player);
            }

            S_Player_Interpol interpol = new S_Player_Interpol();
            interpol.AuthCode = player.AuthCode;
            interpol.Transform = player.transform;

            Push(() => Broadcast(interpol));
        }

        //public void MapObjectInitialization() { }

        public void SendUserObjectSynch() {
            S_Load_Players playerList = new S_Load_Players();

            foreach(int key in _players.Keys) {
                pObjectData data = new pObjectData();
                data.AuthCode = _players[key].AuthCode;
                data.Transform = _players[key].transform;

                playerList.ObjectList.Add(data);
            }

            Push(() => Broadcast(playerList));
        }

        public S_Load_Players GetpUserInGameDatas(int exceptAuthCode = -1) {
            S_Load_Players list = new S_Load_Players();
            
            foreach(int authCode in _players.Keys) {
                if(authCode == exceptAuthCode)
                    continue;
                pObjectData data = new pObjectData();
                data.AuthCode = _players[authCode].AuthCode;
                data.Transform = _players[authCode].transform;
                list.ObjectList.Add(data);
            }

            return list;
        }
    }
}
