using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Define;

public class ExecutionArea : MonoBehaviour
{
    [Header("Elevator Destination")]
    [SerializeField]
    private pSceneType destination;

    [Header("Execution Check Collider")]
    [SerializeField]
    private BoxCollider _collider = null;

    [Header("Execution Time")]
    public float executionLimit = -1.0f;
    [SerializeField]
    private float executionRemaining = 0.0f;

    public UnityAction<float> ExecutionTimeListener;

    private ExecutionUI execution = null;

    private MyPlayer _player = null;
    private bool isExecuting = false;

    private void Start() {
        //TODO: ExecutionTimeListener에 HideoutUIManager의 ExecutionText 수정함수 넣어주기
    }


    private void OnTriggerEnter(Collider other) {
        _player = other.GetComponent<MyPlayer>();

        execution = ( Managers.UI as InGameUIManager ).ExecUI;
        if(execution == null)
            return;

        if(_player != null) {
            isExecuting = true;
            executionRemaining = executionLimit;
            execution.gameObject.SetActive(true);
            StartCoroutine(CoStartCountExecution());
            //Debug.Log("Execution Started!");
        }
    }

    private void OnTriggerExit(Collider other) {
        if(_player == null)
            return;

        if(_player.CompareTag(other.tag) == false)
            return;

        isExecuting = false;
        executionRemaining = 0.0f;
        execution.gameObject.SetActive(false);
        StopCoroutine(CoStartCountExecution());
        //Debug.Log("Execution Canceled!");
    }

    public IEnumerator CoStartCountExecution() {
        while(isExecuting) {
            executionRemaining -= Time.deltaTime;
            //Debug.Log($"Execution Time : {executionRemaining}");
            execution.SetExecTime(executionRemaining);

            if(executionRemaining <= 0.0f) {
                ExecutionSuccess();
                break;
            }

            yield return null;
        }

        yield break;
    }

    private void ExecutionSuccess() {
        //switch(destination) {
        //    case SceneType.Fieldmap: {
        //        C_Execute_To packet = new C_Execute_To(){Dest = SceneType.Fieldmap };
        //        Managers.Network.Send(packet);
        //        Debug.Log("Sended Execute To Fieldmap Packet");
        //    }break;
        //    case SceneType.Hideout: {
        //        C_Execute_To packet = new C_Execute_To(){Dest = SceneType.Hideout };
        //        Managers.Network.Send(packet);
        //        Debug.Log("Sended Execute To Hideout Packet");
        //    }
        //    break;
        //    default:break;
        //}
    }
}
