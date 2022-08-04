using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Panel : MonoBehaviour {
    protected virtual void OnEnabled()  { }
    protected virtual void OnDisabled() { }
    private void OnEnable()  { Cursor.lockState = CursorLockMode.None;   OnEnabled(); }
    private void OnDisable() { Cursor.lockState = CursorLockMode.Locked; OnDisabled(); }
}
