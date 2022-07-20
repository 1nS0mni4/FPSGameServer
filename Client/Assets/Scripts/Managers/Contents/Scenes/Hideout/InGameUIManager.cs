using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class InGameUIManager : UIManager {
    public override pSceneType Type { get => _type; protected set => _type = value; }
    [SerializeField]
    private ExtractionUI _execUI = null;
    public ExtractionUI ExtracUI { get => _execUI; }

    [SerializeField]
    private FadeUI _fade = null;
    public FadeUI Fade { get => _fade; }
}
