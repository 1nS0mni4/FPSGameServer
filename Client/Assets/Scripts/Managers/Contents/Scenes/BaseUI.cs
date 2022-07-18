using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public abstract class BaseUI : MonoBehaviour{
    public abstract pSceneType Type { get; protected set; }

    public virtual void Start() {
        //TODO: 여기서 Managers가 가지는 UIManager를 현재 UIManager로 수정한다.
        if(Managers.UI != null)
            Destroy(Managers.UI);

        Managers.UI = this;
    }

    public T GetUIManager<T>() where T: BaseUI {
        return Managers.UI as T;
    }
}
