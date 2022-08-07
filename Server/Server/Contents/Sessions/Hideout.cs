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
    public class Hideout : GameRoom {
        public Hideout(int roomCode) {
            RoomCode = roomCode;
        }

        #region Hideout Management Functions

        public override void Init() {
            AddJobTimer(Update, 250f);
        }

        public override void DestroyRoom() {
            for(int i = 0; i < _timerList.Count; i++) {
                _timerList[i].Stop();
            }
            _timerList.Clear();
            _sessions = null;
            _jobQueue.Clear();
            _jobQueue = null;

            Console.WriteLine($"Try to Destory Hideout {RoomCode}");
            HideoutManager.Instance.DestroyRoom(this);
        }
        #endregion
    }
}
