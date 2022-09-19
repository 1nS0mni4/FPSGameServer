using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using static Define;

public class ExtractionArea : MonoBehaviour {
    [Header("Extraction Destination")]
    [SerializeField]
    private pAreaType _destination;

    [Header("Extraction Check Collider")]
    [SerializeField]
    private BoxCollider _collider = null;
    public BoxCollider Collider { get => _collider; }

    [Header("Extraction Time")]
    public float _extractionLimit = -1.0f;
    [SerializeField]
    private float _extractionRemaining = 0.0f;

    [SerializeField]
    private ExtractionUI _extractionUI = null;
    public Action<bool> ExtractionSuccessEvent = null;

    private string _playerTag = "MyPlayer";
    private bool _isExtracting = false;
    public bool IsExtracting { get => _isExtracting && _collider.enabled; }  
    public pAreaType Destination { get => _destination; }

    /// <summary>
    /// Ȱ��ȭ�� ExtractionArea�� ������ �������� �� ����Ǵ� �Լ�
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag(_playerTag) == false)
            return;

        if(_extractionUI != null)
            _extractionUI.gameObject.SetActive(true);

        _isExtracting = true;
        _extractionRemaining = _extractionLimit;
        StartCoroutine(CoStartCountExtraction());
    }

    /// <summary>
    /// ������ ExtractionArea ������ ����� �� ����Ǵ� �Լ�
    /// </summary>
    /// <param name="other"></param>
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


    /// <summary>
    /// ������ Extraction�� �õ� ���� �� ��� üũ�ϴ� �Լ�
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// ������ Extraction�� �������� �� ����Ǵ� �Լ�
    /// </summary>
    private void ExtractionSucess() {
        if(ExtractionSuccessEvent != null) {
            ExtractionSuccessEvent.Invoke(false);
        }

        InGameSceneManager manager = Managers.Scene.GetManager<InGameSceneManager>();

        if(manager == null)
            return;

        Managers.Scene.ChangeSceneTo(_destination);

        switch(_destination) {
            case pAreaType.Hideout: {   //���� �ʵ忡�� Ż���� ��
                //TODO: ����(�ʵ�, ģ�� ���̵�ƿ�)�� ���� �� C_Game_Leave ����, ģ�� ������ �� �� ���� ����.
            }break;
            default: { //���� �ʵ忡 ������ ��
                C_Game_Try_Enter tryEnter = new C_Game_Try_Enter();
                tryEnter.AuthCode = Managers.Network.AuthCode;

                Managers.Network.Send(tryEnter);
            }
            break;
        }
    }
}
