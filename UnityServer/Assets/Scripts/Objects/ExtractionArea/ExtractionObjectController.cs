using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtractionObjectController : MonoBehaviour {
    [SerializeField] protected ExtractionArea _extractionArea = null;
                     protected bool _isActivated = false;

    public bool IsActivated { get => _isActivated; }

    private void Awake() {
        if(_extractionArea == null) throw new System.NotImplementedException();
    }

    protected virtual void Start() {
        _extractionArea.ExtractionSuccessEvent = ExtractionEffects;
    }

    public virtual void ActivateExtraction(bool isActivate) {
        if(isActivate == true) {
            _extractionArea.Collider.enabled = true;
            ExtractionEffects(true);
        }
        else {
            if(_extractionArea.IsExtracting)
                return;
            ExtractionEffects(false);
            _extractionArea.Collider.enabled = false;
        }
    }


    public virtual void ExtractionEffects(bool isStart) { }
}
