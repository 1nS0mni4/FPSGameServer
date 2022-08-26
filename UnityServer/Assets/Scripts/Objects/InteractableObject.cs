using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public abstract class InteractableObject : MonoBehaviour, NetworkObject {
    public int AuthCode { get; set; }

#if UNITY_CLIENT_FPS
    [SerializeField] protected InteractableUI _interactableUI = null;

    public InteractType[] _interactTypes;

    public void ShowInteractType() {
        _interactableUI.ShowInteractType(this, _interactTypes);
    }
#endif
    public abstract void Interact(InteractType type);
}