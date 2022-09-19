using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerManager : MonoBehaviour {
    private static ServerManager _instance;
    public static ServerManager Instance { get => _instance; }

    private NetworkManager _network = new NetworkManager();
    public static NetworkManager Network { get => _instance._network; }

    private void Awake() {
        if(_instance != null)
            Destroy(_instance.gameObject);

        DontDestroyOnLoad(gameObject);
        _instance = this;

        PacketManager.Instance.CustomHandler = PacketQueue.Instance.Push;
    }

    private void Start() {
        string[] args = Environment.GetCommandLineArgs();
        Console.WriteLine($"args Count: {args.Length}");
        Console.WriteLine($"args[0]: {args[0]}");

        if(args.Length < 2) {
            Console.WriteLine("Args Required: [ProcessName] [-batchmode] [-nographics] [LoginServerIP] [LoginServerPort] [pAreaType]");
            //Application.Quit();
            //return;
        }

        string hostString = args[3];

        int port;
        if(int.TryParse(args[4], out port) == false) {
            Console.WriteLine("Args [LoginServerPort] not Parsed as int");
            //Application.Quit();
            //return;
        }

        pAreaType areaType = (pAreaType)Enum.Parse(typeof(pAreaType), args[6]);

        foreach(string argument in args) {
            Console.WriteLine($"{argument}");
        }

        IPHostEntry ipHost = Dns.GetHostEntry(hostString);

        Network.ConnectTo(ipHost.AddressList[0], port);

        SceneManager.LoadSceneAsync((int)areaType, LoadSceneMode.Single);
    }

    private void Update() {
        if(PacketQueue.Instance.PacketCount == 0 )
            return;

        List<PacketModel> list = PacketQueue.Instance.PopAll();
        for(int i = 0; i < list.Count; i++) {
            Action<PacketSession, IMessage> action = null;

            action = PacketManager.Instance.GetPacketHandler(list[i].packetID);
            if(action != null) {
                //action.Invoke(SessionManager.Instance.Find(list[i].sessionID), list[i].packet);
            }
        }
    }

    public void CloseServer() {

    }
}
