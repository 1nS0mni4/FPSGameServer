using Google.Protobuf.Protocol;
using Server.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Contents {
    public class FieldmapManager {

        #region Singleton
        private static FieldmapManager _instance;
        public static FieldmapManager Instance { get { return _instance; } }
        static FieldmapManager() {
            _instance = new FieldmapManager();
        }
        #endregion

        private Dictionary<int, Fieldmap> _fields = new Dictionary<int, Fieldmap>();
        private object l_fields = new object();
        private int _roomID = 0;
        private List<System.Timers.Timer> _timer = new List<System.Timers.Timer>();
        public List<System.Timers.Timer> Timer { get => _timer; }
        public Fieldmap Generate() {
            Fieldmap field = new Fieldmap(_roomID++);
            field.Init();
            lock(l_fields) {
                _fields.Add(field.RoomCode, field);
            }

            return field;
        }

        public void DestroyField(Fieldmap room) {
            if(room == null)
                return;

            bool success = false;

            lock(l_fields) {
                success = _fields.Remove(room.RoomCode);
            }
            Console.WriteLine($"GameRoom{room.RoomCode} Removing Result: {success}");
        }

        public bool TryEnterField(ClientSession session = null, pAreaType prevArea = pAreaType.Hideout, pAreaType destArea = pAreaType.Hideout) {
            if(session == null)
                return false;

            if(Enum.IsDefined(typeof(pAreaType), destArea) == false)
                return false;

            if(destArea == pAreaType.Hideout)
                return false;

            Fieldmap tryable = null;
            lock(l_fields) {
                foreach(Fieldmap room in _fields.Values) {
                    if(room.CanAccept) {
                        tryable = room;
                        break;
                    }
                }

                if(tryable == null)
                    tryable = Generate();
            }

            tryable.Push(() => { tryable.Enter(session, prevArea, destArea); });

            return true;
        }
    }
}
