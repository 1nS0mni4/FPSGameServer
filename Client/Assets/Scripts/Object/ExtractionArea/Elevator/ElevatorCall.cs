using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorCall : InteractableObject {
    [Header("Elevator Objects")]
    [SerializeField] private ElevatorController _elevController = null;
    [SerializeField] private ExtractionArea _extraction = null;

    private void Awake() {
        if(_interactableUI == null) throw new MissingComponentException("ElevatorCall - _interactableUI Component is null");
        if(_elevController == null) throw new MissingComponentException("ElevatorCall - _elevController Component is null");
        if(_extraction == null) throw new MissingComponentException("ElevatorCall - _extraction Component is null");
    }

    public override void Interact(Define.InteractType type) {
        switch(type) {
            case Define.InteractType.Object_Use: {
                _elevController.ActivateExtraction(true);
            }break;
        }
    }

    public override void ShowInteractType() {
        _interactableUI.ShowInteractType(this, _interactTypes);
    }
}
