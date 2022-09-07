using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public abstract class InteractableObject : MonoBehaviour, NetworkObject {

    [SerializeField] protected InteractableUI _interactableUI = null;
                     public pInteractType[] _interactTypes;
                     private bool _interactable = true;
                     public bool Interactable { get => _interactable; }

    public uint AuthCode { get; set; }
    public abstract void ShowInteractType();
    public abstract void Interact(pInteractType type);
}