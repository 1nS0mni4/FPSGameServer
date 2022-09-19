using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorCall : InteractableObject {
    [Header("Elevator Objects")]
    [SerializeField] private ElevatorController _elevController = null;
    [SerializeField] private ExtractionArea _extraction = null;

    private WaitForSeconds _sleepForInGame = new WaitForSeconds(0.1f);

    private void Awake() {
        if(_interactableUI == null) throw new MissingComponentException("ElevatorCall - _interactableUI Component is null");
        if(_elevController == null) throw new MissingComponentException("ElevatorCall - _elevController Component is null");
        if(_extraction == null) throw new MissingComponentException("ElevatorCall - _extraction Component is null");
    }

    public override void Interact(pInteractType type) {
        switch(type) {
            case pInteractType.ObjectUse: {
                if(_elevController.IsActivated == false) {
                    C_Login_Request_Game_Session request = new C_Login_Request_Game_Session();
                    request.AreaType = _extraction.Destination;
                    request.AuthCode.Add(Managers.Network.AuthCode);
                    request.UserCount = 1;

                    Managers.Network.Send(request);
                    StartCoroutine(CoCheckGameConnected());
                }
                else {
                    _elevController.ActivateExtraction(false);
                    Managers.Network.Disconnect_Game();
                }
            }break;
        }
    }

    public override void ShowInteractType() {
        if(_elevController.IsInteractable == false)
            return;

        _interactableUI.ShowInteractType(this, _interactTypes);
    }

    private IEnumerator CoCheckGameConnected() {
        while(true) {
            if(Managers.Network.InGame)
                break;

            yield return _sleepForInGame;
        }

        _elevController.ActivateExtraction(true);
        yield break;
    }
}
