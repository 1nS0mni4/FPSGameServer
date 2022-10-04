using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SessionPanelPooler))]
public class TransmitterUI : UI_Panel {
    public  Transmitter                       _transmitter    = null;
    public  Image                             _loadingIcon    = null;
    private SessionPanelPooler                _sessionPooler  = null;
    private WaitForSeconds                    _loadingWait    = new WaitForSeconds(0.5f);

    private void Awake() {
        _sessionPooler = GetComponent<SessionPanelPooler>();

        if(_transmitter == null)
            throw new MissingComponentException("TransmitterUI - _transmitter Component is null");

        if(_loadingIcon == null)
            throw new MissingComponentException("TransmitterUI - _loadingIcon Component is null");
    }

    public void Button_ClosePanel() {
        gameObject.SetActive(false);
    }

    public void Button_TransmitAccess(uint authCode) {
        if(_transmitter == null)
            return;

        _transmitter.CallTrainTo(authCode);
    }

    public void Button_PassTrain() {
        if(_transmitter == null)
            return;

        _transmitter.PassTrain();
    }

    public void RefreshSessionList(S_Response_Request_Online packet) {
        StopCoroutine(CoStartLoading());
        _loadingIcon.gameObject.SetActive(false);

        for(int i = 0; i < packet.OnlineUsers.Count; i++) {
            SessionPanel panel = _sessionPooler.Get();
            panel.Setup(packet.OnlineUsers[i].AuthCode, packet.OnlineUsers[i].Name);
        }
    }

    private IEnumerator CoStartLoading() {
        _loadingIcon.gameObject.SetActive(true);

        while(true) {
            _loadingIcon.rectTransform.Rotate(0, 0, 500f * Time.deltaTime);
            yield return _loadingWait;
        }
    }

    #region Override Functions

    protected override void OnEnabled() {
        Managers.CanInput = false;
        StartCoroutine(CoStartLoading());
    }

    protected override void OnDisabled() {
        //Managers.Network.MessageWait.Remove(typeof(S_Response_Request_Online));
        Managers.CanInput = true;

        StopCoroutine(CoStartLoading());
        _loadingIcon.gameObject.SetActive(false);
    }

    #endregion
}
