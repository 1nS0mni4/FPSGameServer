using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class LoadingUIManager : BaseUI {
    public override pSceneType Type { get; protected set; } = pSceneType.Loading;

    [Header("UI Elements in Loading")]
    [SerializeField]
    private TextMeshProUGUI _loadingText = null;
    [SerializeField]
    private Slider _loadingBar = null;


    public override void Start() {
        base.Start();
        

    }
}
