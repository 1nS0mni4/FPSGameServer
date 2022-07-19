using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class InGameUIManager : UIManager {
    [SerializeField]
    private ExecutionUI _execUI = null;
    public ExecutionUI ExecUI { get => _execUI; }
    public override pSceneType Type { get; protected set; } = pSceneType.Hideout;

}
