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
        public static string serverManager= "";
        public static string serverManagerRegister = "";

        public static string clientHandler = "";
        public static string clientHandlerRegister = "";
        public static string serverHandler = "";
        public static string serverHandlerRegister = "";
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

                switch(name.First()) {
                    case 'c': case 'C': {
                        serverManagerRegister += string.Format(PacketFormat.managerRegisterFormat, rName[(int)NameType.WithoutUnder], rName[(int)NameType.WithUnder]);
                        serverHandlerRegister += string.Format(PacketFormat.handlerFunctionFormat, rName[(int)NameType.WithUnder], "Client");
                    }
                    break;
                    case 's': case 'S': {
                        clientManagerRegister += string.Format(PacketFormat.managerRegisterFormat, rName[(int)NameType.WithoutUnder], rName[(int)NameType.WithUnder]);
                        clientHandlerRegister += string.Format(PacketFormat.handlerFunctionFormat, rName[(int)NameType.WithUnder], "Server");
                    }
                    break;
                }
            }

            serverManager = string.Format(PacketFormat.managerFormat, serverManagerRegister);
            clientManager = string.Format(PacketFormat.managerFormat, clientManagerRegister);
            serverHandler = string.Format(PacketFormat.handlerFormat, "Server", serverHandlerRegister);
            clientHandler = string.Format(PacketFormat.handlerFormat, "Client", clientHandlerRegister);

            File.WriteAllText("ServerPacketManager.cs", serverManager);
            File.WriteAllText("ClientPacketManager.cs", clientManager);
            File.WriteAllText("ServerPacketHandler.cs", serverHandler);
            File.WriteAllText("ClientPacketHandler.cs", clientHandler);
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