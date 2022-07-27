using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Define;

public class SceneController : MonoBehaviour
{
    public pAreaType CurrentScene { get; private set; }
    public float progress;
    public MSceneManager Manager { get; set; }
    private int _isLoading = 0;
    public bool IsLoading { get { return _isLoading == 1; } set { 
            Interlocked.Exchange(ref _isLoading, 0); 

            while(Completed.Count > 0) {
                Action action = Completed.Dequeue();
                action.Invoke();
            }
        } 
    }
    public Queue<Action> Completed = new Queue<Action>();
    public void ChangeSceneTo(pAreaType type) {
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
        Interlocked.Exchange(ref _isLoading, 1);
        Scene prevScene = SceneManager.GetActiveScene();

        AsyncOperation task = SceneManager.LoadSceneAsync(type.ToString());
        task.allowSceneActivation = false;

        while(task.isDone == false) {
            progress = task.progress + 0.1f;

            if(progress >= 1.0f)
                break;

            yield return null;
        }
        
        CurrentScene = type;
        progress = 1.0f;

        C_Extract_To scenePacket = new C_Extract_To();
        scenePacket.PrevArea = Managers.CurArea;
        scenePacket.DestArea = type;

        Managers.Network.Send(scenePacket);
        task.allowSceneActivation = true;
        //SceneManager.UnloadSceneAsync(prevScene);
                
        yield break;
    }
    public T GetManager<T>() where T: MSceneManager {
        return Manager as T;
    }
}
