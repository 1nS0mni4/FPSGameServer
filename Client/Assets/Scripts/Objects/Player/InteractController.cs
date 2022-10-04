using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractController : MonoBehaviour {
    #region Raycast Variables
    [Header("Interaction Ray Implements")]
    public  float               _interactDistance       = 5.0f;
    private Ray                 _interactRay;
    public  LayerMask           _interactLayerMask;
    public  InteractableUI      _interactUI             = null;
    private InteractableObject  _curInteracting         = null;

    #endregion

    #region Properties
    public InteractableObject CurInteractacting { get => _curInteracting; }

    #endregion

    private void FixedUpdate() {
        _interactRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        Debug.DrawRay(_interactRay.origin, _interactRay.direction, Color.red, _interactDistance);
        if(Physics.Raycast(_interactRay, out RaycastHit hit, _interactDistance, _interactLayerMask)) {
            if(_curInteracting != null && _curInteracting.GetInstanceID().Equals(hit.colliderInstanceID))
                return;

            _curInteracting = hit.collider.GetComponent<InteractableObject>();
            _curInteracting.ShowInteractTypeUI();
        }
        else {
            if(_interactUI.IsOpen)
                _interactUI.CloseInteractTypeUI();

            _curInteracting = null;
        }
    }

    public void Interact() {
        _interactUI.Interact();
    }
}
