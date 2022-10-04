using Extensions;
using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class InGameSceneManager : MSceneManager {
    #region Components & GameObjects
    [SerializeField] private InGameUIManager             _uiManager        = null;
    [SerializeField] private MyPlayer                    _myPlayer         = null;
                     public  Dictionary<uint, Character> _characters       = new Dictionary<uint, Character>();
                     public  CharacterPooler             _characterPooler  = null;

    #endregion

    #region Variables
    [HideInInspector]public  bool           f_Loaded_Player = true;
    [HideInInspector]public  bool           f_Loaded_Item   = true;
    [HideInInspector]public  bool           f_Loaded_Field  = true;

    private WaitForSeconds loadWaitTime    = new WaitForSeconds(0.5f);

    #endregion

    #region Unity Event Functions & Override Functions
    /// <summary>
    /// Call by MSceneManager's UnityEvent.Awake().
    /// </summary>
    public override void OnAwakeEvent() {

    }

    /// <summary>
    /// Call by MSceneManager's UnityEvent.Start().
    /// </summary>
    public override void OnStartEvent() {
        StartCoroutine(CoCheckDataLoaded());
    }

    /// <summary>
    /// Call by MSceneManager's UnityEvent.OnDestroy().
    /// </summary>
    public override void OnDestroyEvent() {

    }

    /// <summary>
    /// Call when Game Scene is all loaded.
    /// </summary>
    public override void OnLoadCompleted() {
        Managers.CanInput = true;
        _uiManager.Fade.FadeControlTo(false);
    }

    #endregion

    public void SpawnPlayer(uint authCode, pVector3 position = null, pQuaternion rotation = null) {
        if(_characters.ContainsKey(authCode))
            return;

        Vector3 pos;
        if(position == null) pos = new Vector3(0, 2, -20);
        else                 pos = position.toVector3();

        Quaternion rot;
        if(rotation == null) rot = Quaternion.Euler(new Vector3(0, 0, 0));
        else                 rot = rotation.ToQuaternion();

        Character character = null;

        if(authCode == Managers.Network.AuthCode) { character = _myPlayer; }
        else {
            character  = _characterPooler.Get();
            _characters.Add(authCode, character);
        }

        character.AuthCode = authCode;
        character.gameObject.transform.position = pos;
        character.gameObject.transform.rotation = rot;
        character.gameObject.SetActive(true);
    }

    public void HandleCharacterMove(uint authCode, uint serverTick, pVector3 newPosition, pQuaternion camFront) {
        if(_characters.TryGetValue(authCode, out Character character)) {
            character.Move(serverTick, newPosition.toVector3());
            character.Rotate(camFront.ToQuaternion());
        }
    }

    public void RemovePlayer(uint authCode) {
        if(_characters.Remove(authCode)) {
            _characterPooler.Destroy(_characters[authCode]);
        }
    }

    private IEnumerator CoCheckDataLoaded() {
        bool loading = true;
        while(loading) {
            loading = !( f_Loaded_Field & f_Loaded_Item & _myPlayer.gameObject.activeSelf );
            yield return loadWaitTime;
        }

        OnLoadCompleted();
        yield break;
    }
}