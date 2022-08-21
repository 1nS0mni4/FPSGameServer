using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(AudioSource))]
public class TrainController : ExtractionObjectController {
    [SerializeField] private GameObject     _doorLeft = null;
    [SerializeField] private GameObject     _doorRight = null;

    [SerializeField] private AudioSource    _trainAudioSource = null;
    [SerializeField] private AudioClip      _trainHorn = null;

    [SerializeField] private float _trainStandbyThreshold = 3.0f;

                     private Vector3 _originLPos,
                                     _originRPos;
                     
                     private Vector3 _rDoorTargetPos,
                                     _lDoorTargetPos;

    private void Awake() {
        _trainAudioSource = GetComponent<AudioSource>();
        
        if(_trainHorn != null)
            _trainAudioSource.clip = _trainHorn;
    }

    //TODO: 2022-07-28 작업 하던거 - 기차 트랜스미터 구현, 트랜스미터에서 기차 작동시키는거 구현

    protected override sealed void Start() {
        base.Start();
        _originLPos = _doorLeft.transform.position;
        _originRPos = _doorRight.transform.position;
    }

    #region Override Functions
    public override sealed void ExtractionEffects(bool isStart) {
        _rDoorTargetPos = isStart ? _originRPos + _doorRight.transform.right.normalized : _originRPos;
        _lDoorTargetPos = isStart ? _originLPos - _doorLeft.transform.right.normalized : _originLPos;

        StartCoroutine(CoDoorAction());
    }
    #endregion

    #region Original Functions

    public void StandbyTrain(bool isActivate) {
        StartCoroutine(CoActivateTrain(isActivate));
    }

    private IEnumerator CoActivateTrain(bool isActivate) {
        float timer = 0.0f;

        //TODO: 경적 소리 및 세팅
        //_trainAudioSource.PlayOneShot(_trainHorn);

        while(timer <= _trainStandbyThreshold) {
            timer += Time.deltaTime;
            yield return null;
        }

        //TODO: isActivate == true일 때 기차 정차 소리 및 세팅
        ActivateExtraction(isActivate);
        yield break;
    }

    private IEnumerator CoDoorAction() {
        Vector3 _value;

        while(true) {
            _value = _doorLeft.transform.position;
            _value.x = Mathf.Lerp(_doorLeft.transform.position.x, _lDoorTargetPos.x, 0.05f);
            _doorLeft.transform.position = _value;

            _value = _doorRight.transform.position;
            _value.x = Mathf.Lerp(_doorRight.transform.position.x, _rDoorTargetPos.x, 0.05f);
            _doorRight.transform.position = _value;

            if(( _doorLeft.transform.position - _lDoorTargetPos ).magnitude <= 0.01f &&
                ( _doorRight.transform.position - _rDoorTargetPos ).magnitude <= 0.01f) {

                _doorLeft.transform.position = _lDoorTargetPos;
                _doorRight.transform.position = _rDoorTargetPos;

                break;
            }

            yield return null;
        }

        yield break;
    }

    #endregion

    private void OnDestroy() {
        StopCoroutine(CoDoorAction());
        StopCoroutine("CoActivateTrain");
    }
}
