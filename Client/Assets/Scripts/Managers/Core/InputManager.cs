using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : Manager, IManagerStart, IManagerUpdate {
    private Action _keyInputHandler = null;
    private Action _mouseInputHandler = null;
    //public KeyOption _keySetting = null;
    public bool CanInput = false;

    public void AddKeyInputHandler(Action handler) {
        RemoveKeyInputHandler(handler);
        _keyInputHandler += handler;
    }
    public void RemoveKeyInputHandler(Action handler) {
        _keyInputHandler -= handler;
    }
    public void AddMouseInputHandler(Action handler) {
        _mouseInputHandler -= handler;
        _mouseInputHandler += handler;
    }
    public void RemoveMouseInputHandler(Action handler) {
        _mouseInputHandler -= handler;
    }

    public void Start() {
        //_keySetting = new KeyOption();
    }

    public void Update() {
        if(CanInput == false)
            return;

        if(_mouseInputHandler != null)
            _mouseInputHandler.Invoke();
        if(_keyInputHandler != null)
            _keyInputHandler.Invoke();
    }
}
