using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Define;

public class ExtractionArea : MonoBehaviour
{
    [Header("Elevator Destination")]
    [SerializeField]
    private pSceneType destination;

    [Header("Extraction Check Collider")]
    [SerializeField]
    private BoxCollider _collider = null;

    [Header("Extraction Time")]
    public float extractionLimit = -1.0f;
    [SerializeField]
    private float extractionRemaining = 0.0f;

    public UnityAction<float> ExecutionTimeListener;

    private InGameSceneManager _sceneManager = null;
    private InGameUIManager _uiManager = null;
    private ExtractionUI extraction = null;

    private string _playerTag = "MyPlayer";
    private bool canExtract = false;
    private bool isExtracting = false;

    private void OnEnable() {
        _sceneManager = MSceneManager.GetManager<InGameSceneManager>();
        _uiManager = UIManager.GetManager<InGameUIManager>();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag(_playerTag) == false)
            return;
        
        if(_uiManager != null)
            extraction = _uiManager.ExtracUI;
        
        isExtracting = true;
        extractionRemaining = extractionLimit;
        extraction.gameObject.SetActive(true);
        StartCoroutine(CoStartCountExtraction());
    }


    private void OnTriggerExit(Collider other) {
        if(other.CompareTag(_playerTag) == false)
            return;

        isExtracting = false;
        extractionRemaining = 0.0f;
        extraction.gameObject.SetActive(false);
        StopCoroutine(CoStartCountExtraction());
    }

    public IEnumerator CoStartCountExtraction() {
        while(isExtracting) {
            extractionRemaining -= Time.deltaTime;
            extraction.SetExecTime(extractionRemaining);

            if(extractionRemaining <= 0.0f) {
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
        //    }
        //    break;
        //    case SceneType.Hideout: {
        //        C_Execute_To packet = new C_Execute_To(){Dest = SceneType.Hideout };
        //        Managers.Network.Send(packet);
        //        Debug.Log("Sended Execute To Hideout Packet");
        //    }
        //    break;
        //    default: break;
        //}

        if(_sceneManager != null) {
            Managers.PrevScene = _sceneManager.SceneType;
            Managers.Scene.ChangeSceneTo(pSceneType.Hideout);
        }

        if(_uiManager != null) {
            _uiManager.Fade.FadeControlTo(true);
        }
    }
}
