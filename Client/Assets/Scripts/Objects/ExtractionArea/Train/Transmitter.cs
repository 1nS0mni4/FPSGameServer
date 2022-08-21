using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class Transmitter : InteractableObject {
    [Header("TransmitterUI Object")]
    [SerializeField] private TransmitterUI      _transmitterUI      = null;

    [Header("InHierarchy Objects")]
    [SerializeField] private TrainController    _trainController    = null;
    [SerializeField] private ExtractionArea     _extraction         = null;

    private void Awake() {
        if(_interactableUI  == null)    throw new MissingComponentException("Transmitter - _interactableUI Component is null");
        if(_trainController == null)    throw new MissingComponentException("Transmitter - _trainController Component is null");
        if(_transmitterUI   == null)    throw new MissingComponentException("Transmitter - _transmitterUI Component is null");
        if(_extraction      == null)    throw new MissingComponentException("Transmitter - _extraction Component is null");
    }

    #region Override Functions
    public override void ShowInteractType() {
        _interactableUI.ShowInteractType(this, _interactTypes);
    }

    public override void Interact(InteractType type) {
        switch(type) {
            case InteractType.Object_Use: {
                _transmitterUI.gameObject.SetActive(true);
            } break;
        }
    }
    #endregion


    #region Original Functions
    public void CallTrainTo(int authCode) {
        _extraction._roomCode = authCode;
        _trainController.StandbyTrain(true);
    }

    public void PassTrain() {
        _extraction._roomCode = -1;
        _trainController.StandbyTrain(false);
    }

    #endregion
}
