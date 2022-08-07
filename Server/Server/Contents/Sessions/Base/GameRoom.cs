using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using Server.Contents.Objects.Player;
using Server.Session;
using Server.Utils;
using ServerCore;
using UnityEngine;

namespace Server.Contents.Sessions.Base {
    //public struct GameRoomInfo {
    //    public int RoomID;
    //    public int MaxCapacity;
    //    public int ConnectedCount;
    //}
    public class GameRoom : IJobQueue {
        protected const int max_capacity = 5;
        protected JobQueue _jobQueue = new JobQueue();

        protected volatile Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();

        protected Dictionary<int, Player> _players = new Dictionary<int, Player>();

        protected List<System.Timers.Timer> _timerList = new List<System.Timers.Timer>();

        private WaitForSeconds _timeSyncWait = new WaitForSeconds(1.0f);


        public int RoomCode { get; set; }
        public bool CanAccept { get { return max_capacity - _sessions.Count > 0; } }

        #region GameRoom Management Functions

        public virtual void Init() {
            AddJobTimer(Update, 250f);
            AddJobTimer(SyncPlayerTransform, 500f);
        }

        protected void AddJobTimer(Action action, float times) {
            if(action == null)
                return;

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = times;
            timer.Elapsed += (s, e) => action.Invoke();
            timer.AutoReset = true;
            timer.Enabled = true;

            _timerList.Add(timer);
        }

        public virtual void Broadcast(IMessage packet) {
            if(_sessions.Count == 0)
                return;

            foreach(ClientSession session in _sessions.Values) {
                session.Send(packet);
            }
        }

        public virtual void DestroyRoom() {
            for(int i = 0; i < _timerList.Count; i++) {
                _timerList[i].Stop();
            }
            _timerList.Clear();
            _sessions = null;
            _jobQueue.Clear();
            _jobQueue = null;
            
            Console.WriteLine($"Try to Destory GameRoom {RoomCode}");
            GameRoomManager.Instance.DestroyRoom(this);
        }

        public void Push(Action action) {
            if(_jobQueue == null)
                return;

            _jobQueue.Push(action);
        }

        public virtual void Enter(ClientSession session, pAreaType prevArea, pAreaType destArea) {
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
        }

        public virtual void Leave(int authCode) {
            Console.WriteLine($"Player {authCode} Try to Leave");
            bool result = _sessions.Remove(authCode);
            if(result == false) {
                return;
            }

            //TODO: Broadcast Someone's Leave

            S_Player_Leave playerLeave = new S_Player_Leave();
            playerLeave.AuthCode = authCode;

            Push(() => Broadcast(playerLeave));

            if(_sessions.Count == 0)
                Push(() => DestroyRoom());
        }

        public virtual void Update() {
            foreach(int key in _players.Keys) {
                _players[key].Update();
            }

            //TODO: 다른 Update가 필요한 오브젝트 컨테이너를 여기서 호출
        }
        #endregion


        #region User Management Functions

        public void UserInterpolation(int authCode, pTransform transform, bool checkFlag) {
            Player player = null;

            if(_players.TryGetValue(authCode, out player)) {
                Console.WriteLine($"Transform: {transform.Position.X}, {transform.Position.Y}, {transform.Position.Z}");
                pTransform nextTransform = new pTransform();
                nextTransform.Position = new pVector3();

                nextTransform.Position.X = transform.Position.X - player.transform.Position.X;
                nextTransform.Position.Y = transform.Position.Y - player.transform.Position.Y;
                nextTransform.Position.Z = transform.Position.Z - player.transform.Position.Z;

                if(checkFlag == true && nextTransform.Position.Magnitude() > ( player.speed * Environment.TickCount64 ) + 3.0f) {
                    Push(() => Leave(player.AuthCode));
                    Console.WriteLine($"Player{player.AuthCode} Disconnected Due to Irregular Moving {nextTransform.Position.Magnitude()}");
                    Console.WriteLine($"Player Speed: {player.speed} TimeDelay: {Environment.TickCount64}");
                    return;
                }

                player.transform = transform;
                player.isInterpolated = true;
            }
            else {
                player = new Player();
                player.AuthCode = authCode;
                player.transform = transform;
                player.isInterpolated = true;

                _players.Add(player.AuthCode, player);
            }
        }

        /// <summary>
        /// Get All Users' Data in this Session. Include AI.
        /// </summary>
        /// <param name="excAuthCode">AuthCode for Exception.</param>
        /// <returns></returns>
        public S_Load_Players GetpUserInGameDatas(int excAuthCode = -1) {
            S_Load_Players list = new S_Load_Players();

            foreach(Player p in _players.Values) {
                if(p.AuthCode == excAuthCode)
                    continue;

                pObjectData data = new pObjectData();
                data.AuthCode = p.AuthCode;
                data.Transform = p.transform;
                list.ObjectList.Add(data);
            }

            return list;
        }

        public void HandleMove(ClientSession session, C_Move move) {
            Player player = null;

            if(_sessions.ContainsKey(session.AuthCode) == false)
                return;

            if(_players.TryGetValue(session.AuthCode, out player) == false)
                return;

            player.direction = move.Dir;
            player.stance = move.Stance;

            //TODO: 이동 검사 필요
            //TODO: 시간당 이동 계산 후 Broadcast 필요

            S_Move_Broadcast moveBroad = new S_Move_Broadcast();
            moveBroad.AuthCode = session.AuthCode;
            moveBroad.Dir = move.Dir;
            moveBroad.Stance = move.Stance;

            Push(() => Broadcast(moveBroad));
        }

        public void SyncPlayerTransform() {
            S_Sync_Player_Transform sync = new S_Sync_Player_Transform();
            pObjectData data = new pObjectData();

            foreach(Player p in _players.Values) {
                data = new pObjectData();
                data.AuthCode = p.AuthCode;
                data.Transform = p.transform;

                sync.PlayerTransforms.Add(data);
            }

            Push(() => Broadcast(sync));
        }

        #endregion

    }
}