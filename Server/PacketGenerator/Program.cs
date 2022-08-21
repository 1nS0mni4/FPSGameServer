using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketGenerator {
    public enum NameType {
        WithUnder,
        WithoutUnder,
    }

    public class Program {
        public static string clientManager = "";
        public static string clientManagerRegister= "";
        public static string loginServerManager= "";
        public static string loginServerManagerRegister = "";
        public static string gameServerManager = "";
        public static string gameServerManagerRegister = "";

        //public static string clientHandler = "";
        //public static string clientHandlerRegister = "";
        //public static string feServerHandler = "";
        //public static string feServerHandlerRegister = "";
        //public static string beServerHandler = "";
        //public static string beServerHandlerRegister = "";

        public static void Main(string[] args) {
            string filePath = "../../../../../Common/protoc-3.12.3-win64/bin/Protocol.proto";
            if(args.Length >= 1) {
                filePath = args[0];
            }

            bool startParsing = false;
            foreach(string line in File.ReadAllLines(filePath)) {
                if(startParsing == false && line.Contains("enum PacketID{")) {
                    startParsing = true;
                    continue;   //다음 줄 읽기 위해 사용
                }

                if(startParsing == false)
                    continue;

                if(line.StartsWith('}')) {
                    startParsing = false;
                    break;
                }

                char[] charToTrim = new char[]{' ', ';', '\t'};
                string name = line.Trim(charToTrim).Split("=")[0];
                string[] rName = RefineName(name);

                switch(rName[(int)NameType.WithUnder].First()) {
                    case 'c': case 'C': {
                        loginServerManagerRegister += string.Format(PacketFormat.managerRegisterFormat, rName[(int)NameType.WithoutUnder], rName[(int)NameType.WithUnder]);
                        //feServerHandlerRegister += string.Format(PacketFormat.handlerFunctionFormat, rName[(int)NameType.WithUnder], "Client");
                        gameServerManagerRegister += string.Format(PacketFormat.managerRegisterFormat, rName[(int)NameType.WithoutUnder], rName[(int)NameType.WithUnder]);
                        //beServerHandlerRegister += string.Format(PacketFormat.handlerFunctionFormat, rName[(int)NameType.WithUnder], "Client");
                    }
                    break;
                    case 's': case 'S': {
                        
                        //clientHandlerRegister += string.Format(PacketFormat.handlerFunctionFormat, rName[(int)NameType.WithUnder], "Server");
                        string serverType = rName[(int)NameType.WithUnder].Split("_")[1];

                        if(serverType.Equals("L2G")) {
                            gameServerManagerRegister += string.Format(PacketFormat.managerRegisterFormat, rName[(int)NameType.WithoutUnder], rName[(int)NameType.WithUnder]);
                            //beServerHandlerRegister += string.Format(PacketFormat.handlerFunctionFormat, rName[(int)NameType.WithUnder], "Client");
                        }
                        else if(serverType.Equals("G2L")) {
                            loginServerManagerRegister += string.Format(PacketFormat.managerRegisterFormat, rName[(int)NameType.WithoutUnder], rName[(int)NameType.WithUnder]);
                            //feServerHandlerRegister += string.Format(PacketFormat.handlerFunctionFormat, rName[(int)NameType.WithUnder], "Client");
                        }
                        else if(serverType.Equals("Common")) {
                            loginServerManagerRegister += string.Format(PacketFormat.managerRegisterFormat, rName[(int)NameType.WithoutUnder], rName[(int)NameType.WithUnder]);
                            //feServerHandlerRegister += string.Format(PacketFormat.handlerFunctionFormat, rName[(int)NameType.WithUnder], "Client");
                            gameServerManagerRegister += string.Format(PacketFormat.managerRegisterFormat, rName[(int)NameType.WithoutUnder], rName[(int)NameType.WithUnder]);
                            //beServerHandlerRegister += string.Format(PacketFormat.handlerFunctionFormat, rName[(int)NameType.WithUnder], "Client");
                        }
                        else {
                            clientManagerRegister += string.Format(PacketFormat.managerRegisterFormat, rName[(int)NameType.WithoutUnder], rName[(int)NameType.WithUnder]);
                        }
                    }
                    break;
                }
            }

            loginServerManager = string.Format(PacketFormat.managerFormat, loginServerManagerRegister);
            clientManager = string.Format(PacketFormat.managerFormat, clientManagerRegister);
            //feServerHandler = string.Format(PacketFormat.handlerFormat, "Server", feServerHandlerRegister);
            //clientHandler = string.Format(PacketFormat.handlerFormat, "Client", clientHandlerRegister);

            File.WriteAllText("LoginServerPacketManager.cs", loginServerManager);
            File.WriteAllText("ClientPacketManager.cs", clientManager);
            File.WriteAllText("GameServerPacketManager.cs", gameServerManager);
            //File.WriteAllText("ServerPacketHandler.cs", feServerHandler);
            //File.WriteAllText("ClientPacketHandler.cs", clientHandler);
        }

        private static string[] RefineName(string message) {
            message = message.Trim();
            string[] splitted = message.Split('_');
            string[] refined = new string[2];
            foreach(string split in splitted) {
                refined[(int)NameType.WithUnder] += '_' + split.First().ToString().ToUpper() + split.Substring(1).ToLower();
                refined[(int)NameType.WithoutUnder] += split.First().ToString().ToUpper() + split.Substring(1).ToLower();
            }

            

            refined[(int)NameType.WithUnder] = refined[(int)NameType.WithUnder].Trim('_');
            refined[(int)NameType.WithoutUnder] = refined[(int)NameType.WithoutUnder].Trim('_');
            return refined;
        }
    }
}