using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Contents.Objects.Player;
using Server.Contents.Sessions.Base;
using Server.Session;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Contents.Sessions {
    public class Fieldmap : GameRoom {
        public Fieldmap(int roomCode) {
            RoomCode = roomCode;
        }

        #region GameRoom Management Functions

        public override void Init() {
            base.Init();
        }

        public override void DestroyRoom() {
            for(int i = 0; i < _timerList.Count; i++) {
                _timerList[i].Dispose();
            }
            _timerList.Clear();
            _jobQueue.Clear();
            _jobQueue = null;
            _sessions = null;

            Console.WriteLine($"Try to Destory GameRoom {RoomCode}");
            FieldmapManager.Instance.DestroyField(this);
        }

        public override void Enter(ClientSession session, pAreaType prevArea, pAreaType destArea) {
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

        public override void Leave(int authCode) {
            bool result = _sessions.Remove(authCode);
            if(result == false) {
                //    S_Error_Packet errorPacket = new S_Error_Packet();
                //    errorPacket.ErrorCode = NetworkError.InvalidAccess;
                //    session.Send(errorPacket);
                return;
            }

            //TODO: Broadcast Someone's Leave

            S_Player_Leave playerLeave = new S_Player_Leave();
            playerLeave.AuthCode = authCode;

            Push(() => Broadcast(playerLeave));

            if(_sessions.Count == 0)
                Push(() => DestroyRoom());
        }

        public override void Update() {
            foreach(int key in _players.Keys) {
                if(_sessions.ContainsKey(key))
                    _players[key].Update(_sessions[key].TimeDelay);
            }

            //TODO: 다른 Update가 필요한 오브젝트 컨테이너를 여기서 호출
        }
        #endregion
    }
}
