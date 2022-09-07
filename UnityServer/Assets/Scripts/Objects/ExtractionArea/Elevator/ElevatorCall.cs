using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorCall : InteractableObject {
    [Header("Elevator Objects")]
    [SerializeField] private ElevatorController _elevController = null;
    [SerializeField] private ExtractionArea _extraction = null;

    private void Awake() {
#if UNITY_CLIENT_FPS
        if(_interactableUI == null) throw new MissingComponentException("ElevatorCall - _interactableUI Component is null");
#endif
        if(_elevController == null) throw new MissingComponentException("ElevatorCall - _elevController Component is null");
        if(_extraction == null) throw new MissingComponentException("ElevatorCall - _extraction Component is null");
    }

    public override void Interact(pInteractType type) {
        switch(type) {
            case pInteractType.ObjectUse: {
                _elevController.ActivateExtraction(true);
            }break;
        }
    }
}
