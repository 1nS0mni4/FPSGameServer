using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Define;

public class SceneController : MonoBehaviour {
    public MSceneManager Manager { get; set; }
    private bool _isLoading = false;
    private float _fakeLoadingTime = 3.0f;

    private WaitForSeconds loadingWaitSeconds = new WaitForSeconds(0.1f);
    public bool IsLoading {
        get { return _isLoading == true; }
        set {
            if(value != false)
                return;

            _isLoading = value;

            while(Completed.Count > 0) {
                Action action = Completed.Dequeue();
                action.Invoke();
            }
        }
    }
    public Queue<Action> Completed = new Queue<Action>();
    public void ChangeSceneTo(pAreaType type) {
        _isLoading = true;
        Managers.Input.CanInput = false;

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

            Debug.Log($"Progress: {progress}");
            Debug.Log($"LoadingTime: {realLoadingTime}");

            if(progress >= 1.0f && realLoadingTime >= _fakeLoadingTime)
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
