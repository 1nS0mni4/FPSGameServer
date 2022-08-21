using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public abstract class InteractableObject : MonoBehaviour {
    [SerializeField] protected InteractableUI _interactableUI = null;
                     public InteractType[] _interactTypes;

    public abstract void ShowInteractType();
    public abstract void Interact(InteractType type);
}