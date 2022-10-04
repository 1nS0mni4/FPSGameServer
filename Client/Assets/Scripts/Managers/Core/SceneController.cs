using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {
    #region Reference-Type Variables
    private WaitForSeconds  loadingWaitSeconds = new WaitForSeconds(0.1f);
    public Queue<Action>    Completed = new Queue<Action>();

    #endregion

    #region Value-Type Variables
    private bool _isSceneChanging = false;
    private float _fakeLoadingTime = 3.0f;

    #endregion

    #region Properties
    public MSceneManager Manager { get; set; }
    public bool IsSceneChanging {
        get { return _isSceneChanging == true; }
        set {
            if(value != false)
                return;

            _isSceneChanging = value;

            while(Completed.Count > 0) {
                Action action = Completed.Dequeue();
                action.Invoke();
            }
        }
    }
    #endregion

    public void ChangeSceneTo(pAreaType type) {
        Debug.Log($"ChangeSceneTo {type} Called!");
        _isSceneChanging = true;
        Managers.CanInput = false;

        InGameUIManager uiManager = UIManager.GetManager<InGameUIManager>();

        if(uiManager != null) {
            uiManager.Fade.Completed -= () => StartCoroutine(CoStartChangeSceneTo(type));
            uiManager.Fade.Completed += () => StartCoroutine(CoStartChangeSceneTo(type));
            uiManager.Fade.FadeControlTo(true);
            return;
        }
        else
            StartCoroutine(CoStartChangeSceneTo(type));
    }

    private IEnumerator CoStartChangeSceneTo(pAreaType type) {
        AsyncOperation task = SceneManager.LoadSceneAsync(type.ToString());
        task.allowSceneActivation = false;
        float realLoadingTime = 0.0f;

        while(task.isDone == false) {
            float progress = task.progress + 0.1f;
            realLoadingTime += 0.1f;

            //Debug.Log("Loading...");
            if(realLoadingTime >= _fakeLoadingTime)
                break;

            yield return loadingWaitSeconds;
        }

        task.allowSceneActivation = true;

        yield break;
    }
    public T GetManager<T>() where T : MSceneManager {
        return Manager as T;
    }
}
