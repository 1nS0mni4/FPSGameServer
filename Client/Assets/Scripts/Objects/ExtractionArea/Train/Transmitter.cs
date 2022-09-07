using Google.Protobuf.Protocol;
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

    public override void Interact(pInteractType type) {
        switch(type) {
            case pInteractType.ObjectUse: {
                _transmitterUI.gameObject.SetActive(true);
                //TODO: TransmitterUI에게 지금 작동 중인 Transmitter가 본인임을 전달해야 함
            } break;
        }
    }
    #endregion


    #region Original Functions
    //TODO: IPEndPoint 받아서 세션을 따로 연결해야됨! authCode로 하는게 아니라!
    public void CallTrainTo(uint authCode) {
        _trainController.ActivateExtraction(true);
    }

    public void PassTrain() {
        _trainController.ActivateExtraction(false);
    }

    #endregion
}
