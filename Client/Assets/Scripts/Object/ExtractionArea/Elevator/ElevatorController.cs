using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ElevatorController : ExtractionAreaController {
    [SerializeField]
    private GameObject _doorLeft = null;
    [SerializeField]
    private GameObject _doorRight = null;

    private Vector3 _originLPos;
    private Vector3 _originRPos;
    Vector3 _rDoorTargetPos, _lDoorTargetPos;

    protected override void Start() {
        base.Start();
        _originLPos = _doorLeft.transform.position;
        _originRPos = _doorRight.transform.position;
    }

    protected override void TriggerAction(bool isStart) {
        _rDoorTargetPos = isStart ? _originRPos + _doorRight.transform.right : _originRPos;
        _lDoorTargetPos = isStart ? _originLPos - _doorLeft.transform.right : _originLPos;

        StartCoroutine(CoDoorAction());
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

            if( (_doorLeft.transform.position - _lDoorTargetPos).magnitude <= 0.01f &&
                ( _doorRight.transform.position - _rDoorTargetPos ).magnitude <= 0.01f) {

                _doorLeft.transform.position = _lDoorTargetPos;
                _doorRight.transform.position = _rDoorTargetPos;

                break;
            }
            yield return null;
        }

        yield break;
    }

    private void OnDestroy() {
        StopCoroutine(CoDoorAction());
    }
}
