using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtractionObjectController : MonoBehaviour {
    [SerializeField] protected ExtractionArea _extractionArea = null;
                     protected bool _isActivated = false;

    public bool IsActivated { get => _isActivated; }

    protected virtual void Start() {
        _extractionArea.ExtractionProgress = ExtractionAccess;
    }

    public virtual void OnExtractionObjectActivate(bool isActivate) {
        if(isActivate == true) {
            _extractionArea.Collider.enabled = true;
            ExtractionAccess(true);
        }
        else {
            if(_extractionArea.IsExtracting)
                return;
            ExtractionAccess(false);
            _extractionArea.Collider.enabled = false;
        }

    }

    protected virtual void ExtractionAccess(bool isStart) { }
}
