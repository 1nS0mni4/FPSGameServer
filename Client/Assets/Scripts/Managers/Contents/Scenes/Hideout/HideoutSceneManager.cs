using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static Define;

public class HideoutSceneManager : BaseScene {
    public override pSceneType SceneType { get; protected set; } = pSceneType.Hideout;


    //TODO: ������ƮǮ, �÷��̾�Ǯ�� GameScene���� �����ϴ� ������Ʈ �θ�ü ����

    public override void InitScene() {
        
        //TODO: �ʿ� �����ϴ� ������Ʈ�� �� �������� �����͸� �޾ƿ;��ϴ� �͵��� ���⼭ �񵿱�� �۾�
    }

    public override void ClearScene() {

    }
}
