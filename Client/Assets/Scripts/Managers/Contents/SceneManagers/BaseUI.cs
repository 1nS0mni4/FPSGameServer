using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public abstract class BaseUI : MonoBehaviour{
    public abstract pSceneType Type { get; protected set; }

    public virtual void Start() {
        //TODO: ���⼭ Managers�� ������ UIManager�� ���� UIManager�� �����Ѵ�.
        if(Managers.UI != null)
            Destroy(Managers.UI);

        Managers.UI = this;
    }

    public T GetUIManager<T>() where T: BaseUI {
        return Managers.UI as T;
    }
}
