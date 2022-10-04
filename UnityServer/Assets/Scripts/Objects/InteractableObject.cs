using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour, NetworkObject {
    public uint AuthCode { get; set; }

#if UNITY_CLIENT_FPS
    [SerializeField] protected InteractableUI _interactableUI = null;

    public pInteractType[] _interactTypes;

    public void ShowInteractType() {
        _interactableUI.ShowInteractType(this, _interactTypes);
    }
#endif
    public abstract void Interact(pInteractType type);
}