﻿using Google.Protobuf.Protocol;
using Server.Contents.Room;
using Server.Session;
using Server.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.Contents.Manager {
    public class GameServerManager : Singleton<GameServerManager> {
        public static string ServerProgramPath = "";

        private int _serverID = 0;
        private Dictionary<int, GameServer> _cityhallServers = new Dictionary<int, GameServer>();
        private object l_cityhall = new object();
        private Dictionary<int, GameServer> _residentialServers = new Dictionary<int, GameServer>();
        private object l_residential = new object();
        private Dictionary<int, GameServer> _industrialServers = new Dictionary<int, GameServer>();
        private object l_industrial = new object();
        private Dictionary<int, GameServer> _commerceServers = new Dictionary<int, GameServer>();
        private object l_commerce = new object();

        private List<ClientSession> _cityhallWaiting    = new List<ClientSession>();
        private object l_cityhallWaiting       = new object();
        private List<ClientSession> _residentialWaiting = new List<ClientSession>();
        private object l_residentialWaiting    = new object();
        private List<ClientSession> _industrialWaiting  = new List<ClientSession>();
        private object l_industrialWaiting  = new object();
        private List<ClientSession> _commerceWaiting    = new List<ClientSession>();
        private object l_commerceWaiting    = new object();

        /// <summary>
        /// 새로운 서버 프로세스를 실행시키고 필수값들을 전달한다.
        /// </summary>
        /// <param name="areaType"></param>
        public void Generate(pAreaType areaType) {
            GameServer server = new GameServer();
            server.ServerID = Interlocked.Exchange(ref _serverID, _serverID + 1);
            server.AreaType = areaType;

            switch(server.AreaType) {
                case pAreaType.Cityhall:    { lock(l_cityhall)   { _cityhallServers.Add(server.ServerID, server);    } } break;
                case pAreaType.Residential: { lock(l_residential){ _residentialServers.Add(server.ServerID, server); } } break;
                case pAreaType.Industrial:  { lock(l_industrial) { _industrialServers.Add(server.ServerID, server);  } } break;
                case pAreaType.Commerce:    { lock(l_commerce)   { _commerceServers.Add(server.ServerID, server);    } } break;
                default: break;
            }

            ProcessStartInfo info = new ProcessStartInfo($"{ServerProgramPath}UnityServer.exe");
            info.CreateNoWindow = true;
            info.ArgumentList.Add("-batchmode");
            info.ArgumentList.Add("-nographics");
            info.ArgumentList.Add(Program.IpAddr.ToString());
            info.ArgumentList.Add(Program.ServerPort.ToString());
            info.ArgumentList.Add(server.ServerID.ToString());
            info.ArgumentList.Add(((int)areaType).ToString());
            //info.UserName = $"{nameof(areaType)}_Server({server.ServerID})";

            Process.Start(info);
        }

        /// <summary>
        /// 유저가 요청한 타입의 서버에 접속 가능여부를 체크하고, 가능하다면 해당 서버의 EndPoint를 전달한다.
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="userCount"></param>
        /// <returns></returns>
        public void TryEnterRoom(ClientSession session, pAreaType areaType, int userCount) {
            GameServer server = null;

            switch(areaType) {
                case pAreaType.Cityhall: {
                    lock(l_cityhall) {
                        foreach(GameServer cityhall in _cityhallServers.Values) {
                            if(cityhall.CanAccess)
                                server = cityhall;
                        }
                    }
                }
                break;
                case pAreaType.Residential: {
                    lock(l_residential) {
                        foreach(GameServer residential in _residentialServers.Values) {
                            if(residential.CanAccess)
                                server = residential;
                        }
                    }
                }
                break;
                case pAreaType.Industrial: {
                    lock(l_industrial) {
                        foreach(GameServer industrial in _industrialServers.Values) {
                            if(industrial.CanAccess)
                                server = industrial;
                        }
                    }
                }
                break;
                case pAreaType.Commerce: {
                    lock(l_commerce) {
                        foreach(GameServer commerce in _commerceServers.Values) {
                            if(commerce.CanAccess)
                                server = commerce;
                        }
                    }
                }
                break;
                default: break;
            }

            if(server == null) {
                Generate(areaType);
                switch(areaType) {
                    case pAreaType.Cityhall:    { lock(l_cityhallWaiting)    { _cityhallWaiting.Add(session);    } } break;
                    case pAreaType.Residential: { lock(l_residentialWaiting) { _residentialWaiting.Add(session); } } break;
                    case pAreaType.Industrial:  { lock(l_industrialWaiting)  { _industrialWaiting.Add(session);  } } break;
                    case pAreaType.Commerce:    { lock(l_commerceWaiting)    { _commerceWaiting.Add(session);    } } break;
                    default: break;
                }

                return;
            }

            S_Response_Request_Game_Session request = new S_Response_Request_Game_Session();
            request.EndPoint = server.EndPoint;
            session.Send(request);
        }

        /// <summary>
        /// 새로운 서버 프로세스가 실행되고 연결되었을 때 해당 세션을 AreaType에 맞게 Initialize하고 AreaType별 Dictionary에 저장한다.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="serverInfo"></param>
        public void GameServerStandby(GameServerSession session, S_Login_Game_Standby serverInfo) {
            if(session == null)
                return;

            GameServer server = null;

            switch(serverInfo.AreaType) {
                case pAreaType.Cityhall: {
                    lock(l_cityhall) {
                        _cityhallServers.TryGetValue(serverInfo.ServerID, out server);
                    }
                }break;
                case pAreaType.Residential: {
                    lock(l_residential) {
                        _residentialServers.TryGetValue(serverInfo.ServerID, out server);
                    }
                }break;
                case pAreaType.Industrial: {
                    lock(l_industrial) {
                        _industrialServers.TryGetValue(serverInfo.ServerID, out server);
                    }
                }break;
                case pAreaType.Commerce: {
                    lock(l_commerce) {
                        _commerceServers.TryGetValue(serverInfo.ServerID, out server);
                    }
                }break;
                default:return;
            }

            if(server == null)
                return;

            server.InitializeServer(session, serverInfo);

            S_Response_Request_Game_Session request = new S_Response_Request_Game_Session();
            request.EndPoint = server.EndPoint;

            switch(server.AreaType) {
                case pAreaType.Cityhall: {
                    lock(l_cityhallWaiting) { 
                        foreach(ClientSession s in _cityhallWaiting) { s.Send(request); }
                        _cityhallWaiting.Clear();
                    }
                } break;
                case pAreaType.Residential: {
                    lock(l_residentialWaiting) {
                        foreach(ClientSession s in _residentialWaiting) { s.Send(request); }
                        _residentialWaiting.Clear();
                    }
                } break;
                case pAreaType.Industrial: {
                    lock(l_industrialWaiting) {
                        foreach(ClientSession s in _industrialWaiting) { s.Send(request); }
                        _industrialWaiting.Clear();
                    }
                } break;
                case pAreaType.Commerce: {
                    lock(l_commerceWaiting) {
                        foreach(ClientSession s in _commerceWaiting) { s.Send(request); }
                        _commerceWaiting.Clear();
                    }
                } break;
                default: return;
            }
        }

        public void GameServerUpdate(S_Login_Game_Info_Notify notify) {
            //TODO: 패킷 만들기, 패킷 데이터에 따라 유저 수 업데이트
        }
    }
}