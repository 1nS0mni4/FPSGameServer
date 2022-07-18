using Google.Protobuf.Protocol;
using Server.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Contents {
    public class GameRoomManager {
        #region Singleton
        private static GameRoomManager _instance;
        public static GameRoomManager Instance { get { return _instance; } }
        static GameRoomManager() {
            _instance = new GameRoomManager();
        }
        #endregion

        private Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>();
        private object l_rooms = new object();
        private int _roomID = 0;
        private List<System.Timers.Timer> _timer = new List<System.Timers.Timer>();
        public List<System.Timers.Timer> Timer { get => _timer; }
        public GameRoom Generate() {
            GameRoom room = null;
            lock(l_rooms) {
                room = new GameRoom(){RoomID = _roomID++};
                room.Init();
                _rooms.Add(room.RoomID, room);
            }

            return room;
        }

        public void DestroyRoom(GameRoom room) {
            if(room == null)
                return;
            bool success = false;

            lock(l_rooms) {
                success = _rooms.Remove(room.RoomID);
            }
            Console.WriteLine($"GameRoom{room.RoomID} Removing Result: {success}");
        }

        public void TryEnterRoom(int roomID = -1, ClientSession session = null) {
            if(session == null)
                return;

            if(roomID == -1) {
                GameRoom tryable = null;
                lock(l_rooms) {
                    foreach(GameRoom room in _rooms.Values) {
                        if(room.CanAccept) {
                            tryable = room;
                            break;
                        }
                    }

                    if(tryable == null)
                        tryable = Generate();
                }

                tryable.Push(() => { tryable.EnterRoom(session); });
                session.Room = tryable;
            }
            else {
                lock(l_rooms) {
                    GameRoom room = null;

                    if(_rooms.TryGetValue(roomID, out room)) {
                        room.Push(() => room.EnterRoom(session));
                        session.Room = room;
                    }
                }
            }
        }

        //public List<GameRoomInfo> GetRoomsInfo() {
        //    List<GameRoomInfo> roomInfos = new List<GameRoomInfo>();

        //    foreach(GameRoom room in _rooms.Values) {
        //        roomInfos.Add(new GameRoomInfo() {
        //            RoomID = room.RoomID,
        //            MaxCapacity = room.MaxCapacity,
        //            ConnectedCount = room.ConnectedCount
        //        });
        //    }

        //    return roomInfos;
        //}
    }
}
