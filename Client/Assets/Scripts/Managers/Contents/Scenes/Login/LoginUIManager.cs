using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginUIManager : UIManager {
    [Header("UI Panels in Login Scene")]
    [SerializeField]
    private GameObject _loginPanel = null;
    [SerializeField]
    private GameObject _registerPanel = null;
    [SerializeField]
    private GameObject _errorPanel = null;

    [Header("UI Elements in Login")]
    [SerializeField]
    private Button _buttonEnter = null;

    [Header("UI Elements in Register")]
    [SerializeField]
    private Button _buttonRegister = null;

    [Header("UI Elements in Error")]
    [SerializeField]
    private TextMeshProUGUI _textError = null;
    [SerializeField]
    private Button _buttonOk = null;

    [Header("UI Elements in Common")]
    [SerializeField]
    private TextMeshProUGUI _textID = null;
    [SerializeField]
    private TextMeshProUGUI _textPW = null;

    private string idInput = null;
    private string pwInput = null;
    private bool isRegister = false;

    public override void Awake() {
        base.Awake();
        _loginPanel.SetActive(!isRegister);
        _registerPanel.SetActive(isRegister);
    }

    public void ChangeUI() {
        isRegister = !isRegister;
        _loginPanel.SetActive(!isRegister);
        _registerPanel.SetActive(isRegister);
    }

    public void DisplayError(NetworkError errorType) {
        switch(errorType) {
            case NetworkError.Noaccount: {
                _errorPanel.SetActive(true);
                _textError.text = "Account is NOT Exists! Try Regster First!";
                ChangeUI();
            } break;
            case NetworkError.InvalidPassword: {
                _errorPanel.SetActive(true);
                _textError.text = "Invalid Password!";
            }
            break;
            case NetworkError.Overlap: {
                _errorPanel.SetActive(true);
                _textError.text = "This Account is already Connected. System Close.";
                //System.Environment.Exit(0);
            } break;

            default:break;
        }
    }

    public void ButtonEnterPushed() {
        idInput = _textID.text;
        pwInput = _textPW.text;

        C_Login_Access access = new C_Login_Access();
        access.Id = idInput;
        access.Pw = pwInput;

        Managers.Network.Send(access);
        idInput = pwInput = null;
    }

    public void ButtonRegisterPushed() {
        idInput = _textID.text;
        pwInput = _textPW.text;

        C_Login_Register register = new C_Login_Register(){
            Id = idInput,
            Pw = pwInput
        };

        Managers.Network.Send(register);
        idInput = pwInput = null;
        ChangeUI();
    }

    public void ButtonOKPushed() {
        _errorPanel.SetActive(false);
    }
}
