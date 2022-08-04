using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using static Define;

public class ExtractionArea : MonoBehaviour {
    [Header("ExtractionArea Controller Object")]
    public ExtractionObjectController _controller;

    [Header("Extraction Destination")]
    [SerializeField]
    private pAreaType _destination;
    [HideInInspector]
    public int _roomCode = -1;

    [Header("Extraction Check Collider")]
    [SerializeField]
    private BoxCollider _collider = null;
    public BoxCollider Collider { get => _collider; }

    [Header("Extraction Time")]
    public float _extractionLimit = -1.0f;
    [SerializeField]
    private float _extractionRemaining = 0.0f;

    [SerializeField]
    private InGameSceneManager _sceneManager = null;
    [SerializeField]
    private ExtractionUI _extractionUI = null;
    public Action<bool> ExtractionProgress = null;

    private string _playerTag = "MyPlayer";
    private bool _isExtracting = false;
    public bool IsExtracting { get => _isExtracting && _collider.enabled; }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag(_playerTag) == false)
            return;

        if(_extractionUI != null)
            _extractionUI.gameObject.SetActive(true);

        _isExtracting = true;
        _extractionRemaining = _extractionLimit;
        StartCoroutine(CoStartCountExtraction());
    }

    private void OnTriggerExit(Collider other) {
        if(_isExtracting == false)
            return;

        if(other.CompareTag(_playerTag) == false)
            return;

        _isExtracting = false;
        _extractionRemaining = 0.0f;
        if(_extractionUI != null)
            _extractionUI.gameObject.SetActive(false);
        StopCoroutine(CoStartCountExtraction());
    }

    public IEnumerator CoStartCountExtraction() {
        while(_isExtracting) {
            _extractionRemaining -= Time.deltaTime;
            if(_extractionUI != null)
                _extractionUI.SetExecTime(_extractionRemaining);

            if(_extractionRemaining <= 0.0f) {
                ExtractionSucess();
                break;
            }

            yield return null;
        }

        yield break;
    }

    private void ExtractionSucess() {
        C_Extract_To extPacket = new C_Extract_To();
        extPacket.RoomCode = _roomCode;
        extPacket.PrevArea = Managers.CurArea;
        extPacket.DestArea = _destination;

        Managers.Network.Send(extPacket);

        if(ExtractionProgress != null) {
            ExtractionProgress.Invoke(false);
        }

        if(_sceneManager != null) {
            Managers.CurArea = _sceneManager.AreaType;
            Interlocked.MemoryBarrier();
            Managers.Scene.ChangeSceneTo(_destination == pAreaType.Hideout ? pAreaType.Hideout : pAreaType.Fieldmap);
        }
    }
}
