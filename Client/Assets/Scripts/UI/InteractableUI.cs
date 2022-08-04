using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

[System.Serializable]
public struct InteractUIFormat {
    public InteractType _type;
    public GameObject _ui;
}

public class InteractableUI : MonoBehaviour {
    [Header("InteractableUI Panel Object")]
    [SerializeField] private GameObject                                  _panel               = null;

    [Header("InteractUIFormats")]
    [SerializeField] private List<InteractUIFormat>                      _uiFormats           = new List<InteractUIFormat>();
                     private Dictionary<InteractType, GameObject>        _formats             = new Dictionary<InteractType, GameObject>();

                     private List<InteractType>                          _activatedFormats    = new List<InteractType>();
                     private InteractableObject                          _curObject           = null;
                     private InteractType                                _curType             = InteractType.None;

                     private bool                                        _isOpen              = false;

    public bool IsOpen { get => _isOpen; }

    private void Awake() {
        if(_panel == null)        throw new MissingComponentException("InteractableUI - _panel Component is null");
        if(_uiFormats.Count <= 0) throw new MissingComponentException("InteractableUI - _uiFormats not Implemented");

        for(int i = 0; i < _uiFormats.Count; i++) {
            _formats.Add(_uiFormats[i]._type, _uiFormats[i]._ui);
            _uiFormats[i]._ui.gameObject.SetActive(false);
        }

        _panel.SetActive(false);
    }

    public void CloseInteractType() {
        _panel.SetActive(false);

        _isOpen = false;

        for(int i = 0; i < _uiFormats.Count; i++) {
            _uiFormats[i]._ui.gameObject.SetActive(false);
        }

        _panel.SetActive(false);
        _curObject = null;
        _curType = InteractType.None;
        _activatedFormats.Clear();
    }

    public void ShowInteractType(InteractableObject obj, InteractType[] types) {
        if(types.Length <= 0)
            return;

        _isOpen = true;

        for(int i = 0; i < _uiFormats.Count; i++) {
            _uiFormats[i]._ui.gameObject.SetActive(false);
        }

        GameObject ui = null;

        for(int i = 0; i < types.Length; i++) {
            if(_formats.TryGetValue(types[i], out ui)) {
                ui.SetActive(true);
                _activatedFormats.Add(types[i]);

                if(_curType == InteractType.None)
                    _curType = types[i];
            }
        }

        _curObject = obj;
        _panel.SetActive(true);
    }

    public void ScrollInteractType(int direction) {
        int index = _activatedFormats.IndexOf(_curType);
        index = Mathf.Clamp(index + direction, 0, _activatedFormats.Count);

        //TODO: 이전 InteractTypeUI Highlighting 제거
        _curType = _activatedFormats[index];

        //TODO: 현재 InteractTypeUI Highlighting 추가

    }

    public void Interact() {
        if(_curType == InteractType.None)
            return;

        _curObject.Interact(_curType);
        _panel.SetActive(false);
    }

    private void OnDisable() {
        _isOpen = false;

        for(int i = 0; i < _uiFormats.Count; i++) {
            _uiFormats[i]._ui.gameObject.SetActive(false);
        }

        _panel.SetActive(false);
        _curObject = null;
        _curType = InteractType.None;
        _activatedFormats.Clear();
    }
}
