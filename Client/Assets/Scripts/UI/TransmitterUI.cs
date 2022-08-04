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

    public void Button_TransmitAccess(int authCode) {
        if(_transmitter == null)
            return;

        _transmitter.CallTrainTo(authCode);
    }

    public void Button_PassTrain() {
        if(_transmitter == null)
            return;

        _transmitter.PassTrain();
    }

    private void RefreshSessionList(object packet) {
        S_Request_Online_Response response = packet as S_Request_Online_Response;

        if(response == null)
            return;

        StopCoroutine(CoStartLoading());
        _loadingIcon.gameObject.SetActive(false);

        for(int i = 0; i < response.OnlineUsers.Count; i++) {
            SessionPanel panel = _sessionPooler.Get();
            panel.Setup(response.OnlineUsers[i].AuthCode, response.OnlineUsers[i].Name);
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
        Managers.Input.CanInput = false;
        StartCoroutine(CoStartLoading());

        Managers.Network.MessageWait.Add(typeof(S_Request_Online_Response), RefreshSessionList);
        C_Request_Online request = new C_Request_Online();
        Managers.Network.Send(request);
    }

    protected override void OnDisabled() {
        Managers.Network.MessageWait.Remove(typeof(S_Request_Online_Response));
        Managers.Input.CanInput = true;

        StopCoroutine(CoStartLoading());
        _loadingIcon.gameObject.SetActive(false);
    }

    #endregion
}
