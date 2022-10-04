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

    public static uint ServerTick { get; private set; }

    public string LoginServerHostName = "";
    public int LoginServerPort = 0;
    public pAreaType areaType = pAreaType.Gamestart;

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

#if !UNITY_EDITOR
        if(args.Length < 2) {
            Console.WriteLine("Args Required: [ProcessName] [-batchmode] [-nographics] [LoginServerIP] [LoginServerPort] [pAreaType]");
            Application.Quit();
            return;
        }


        LoginServerHostName = args[3];

        if(int.TryParse(args[4], out LoginServerPort) == false) {
            Console.WriteLine("Args [LoginServerPort] not Parsed as int");
            Application.Quit();
            return;
        }

        areaType = (pAreaType)Enum.Parse(typeof(pAreaType), args[6]);
#endif

        IPHostEntry ipHost = Dns.GetHostEntry(LoginServerHostName);

        Network.ConnectTo(ipHost.AddressList[0], LoginServerPort);

        SceneManager.LoadSceneAsync((int)areaType, LoadSceneMode.Single);
    }

    private void Update() {
        if(PacketQueue.Instance.PacketCount == 0 )
            return;

        List<PacketModel> list = PacketQueue.Instance.PopAll();
        for(int i = 0; i < list.Count; i++) {
            Action<PacketSession, IMessage> action = null;

            action = PacketManager.Instance.GetPacketHandler(list[i].packetID);
            if(null != action) {
                action.Invoke(list[i].session, list[i].packet);
            }
        }
    }

    private void FixedUpdate() { ServerTick++; }

    public void CloseServer() {

    }
}
