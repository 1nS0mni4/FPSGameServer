using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class OnAccessClicked : UnityEvent<int> { }

public class SessionPanel : MonoBehaviour {
    [SerializeField] private Button             _access     = null;
    [SerializeField] private TextMeshProUGUI    _authCodeText   = null;
    [SerializeField] private TextMeshProUGUI    _userNameText   = null;

    public OnAccessClicked _onAccessClicked = new OnAccessClicked();

    private int _authCode = -1;
    private string _userName = "";

    public int AuthCode { get => _authCode; set {
            _authCode = value;
            _authCodeText.text = _authCode.ToString();
        } 
    }

    public string UserName { get => _userName; 
        set {
            _userName = value;
            _userNameText.text = _userName;
        } 
    }

    public void Setup(int authCode, string userName) {
        AuthCode = authCode;
        UserName = userName;
    }

    public void Button_Access() {
        if(_onAccessClicked.GetPersistentEventCount() > 0) {
            _onAccessClicked.Invoke(_authCode);
        }
    }

    private void OnDisable() {
        AuthCode = -1;
        UserName = string.Empty;
    }
}
