using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtractionObjectController : MonoBehaviour {
    [SerializeField] protected ExtractionArea _extractionArea = null;
                     protected bool _isActivated = false;
                     protected bool _isInteractable = true;


    public bool IsActivated { get => _isActivated; }
    public bool IsInteractable { get => _isInteractable; }

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
            _extractionArea.Collider.enabled = false;
            ExtractionEffects(false);
        }

        _isActivated = isActivate;
    }

    protected virtual void ExtractionEffects(bool isStart) { }
}
