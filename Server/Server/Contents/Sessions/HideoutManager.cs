using Google.Protobuf.Protocol;
using Server.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Contents.Sessions {
    public class HideoutManager {
        #region Singleton
        private static HideoutManager _instance;
        public static HideoutManager Instance { get { return _instance; } }
        static HideoutManager() {
            _instance = new HideoutManager();
        }
        #endregion

        private Dictionary<int, Hideout> _hideouts = new Dictionary<int, Hideout>();
        private object l_rooms = new object();
        private List<System.Timers.Timer> _timer = new List<System.Timers.Timer>();
        public List<System.Timers.Timer> Timer { get => _timer; }


        public Hideout Generate(int roomCode) {
            Hideout room = new Hideout(roomCode);
            room.Init();

            lock(l_rooms) {
                _hideouts.Add(room.RoomCode, room);
            }

            return room;
        }

        public void DestroyRoom(Hideout hideout) {
            if(hideout == null)
                return;
            bool success = false;

            lock(l_rooms) {
                success = _hideouts.Remove(hideout.RoomCode);
            }
            Console.WriteLine($"Hideout{hideout.RoomCode} Removing Result: {success}");
        }

        public bool TryEnterRoom(int roomID = -1, ClientSession session = null, pAreaType prevArea = pAreaType.Hideout, pAreaType destArea = pAreaType.Hideout) {
            if(roomID == -1)
                return false;

            if(session == null)
                return false;

            lock(l_rooms) {
                Hideout hideout = null;

                if(_hideouts.TryGetValue(roomID, out hideout)) {
                    hideout.Push(() => hideout.Enter(session, prevArea, destArea));
                    return true;
                }
            }

            return false;
        }
    }
}
