using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static Define;

public class HideoutSceneManager : MSceneManager {
    //TODO: 오브젝트풀, 플레이어풀등 GameScene에만 존재하는 오브젝트 부모객체 저장

    public override void InitScene() {
        
        //TODO: 맵에 존재하는 오브젝트들 중 서버에서 데이터를 받아와야하는 것들은 여기서 비동기로 작업
    }

    public override void ClearScene() {

    }

    public override void LoadCompleted() {
        throw new NotImplementedException();
    }
}
