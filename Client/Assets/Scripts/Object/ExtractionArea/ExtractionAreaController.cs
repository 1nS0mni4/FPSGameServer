using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtractionAreaController : MonoBehaviour
{
    [SerializeField]
    protected ExtractionArea _extractionArea = null;

    protected virtual void Start() {
        _extractionArea.Completed = TriggerAction;
    }

    protected virtual void OnTriggerEnter(Collider other) {
        _extractionArea.Collider.enabled = true;
        TriggerAction(true);

    }

    protected virtual void OnTriggerExit(Collider other) {
        if(_extractionArea.IsExtracting)
            return;
        TriggerAction(false);
        _extractionArea.Collider.enabled = false;
    }

    protected virtual void TriggerAction(bool isStart) { }
}
