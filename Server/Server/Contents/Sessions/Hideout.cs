using Server.Contents.Sessions.Base;
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
    }
}
