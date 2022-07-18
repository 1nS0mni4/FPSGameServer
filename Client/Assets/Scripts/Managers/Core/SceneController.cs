using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Define;

public class SceneController : MonoBehaviour
{
    public pSceneType CurrentScene { get; private set; }
    public float progress;
    public BaseScene Manager { private get; set; }
    public void ChangeSceneTo(pSceneType type) {
        StartCoroutine(CoStartChangeSceneTo(type));
    }

    private IEnumerator CoStartChangeSceneTo(pSceneType type) {
        Scene prevScene = SceneManager.GetActiveScene();

        AsyncOperation task = SceneManager.LoadSceneAsync(type.ToString());
        task.allowSceneActivation = false;

        while(task.isDone == false) {
            progress = task.progress + 0.1f;
            Debug.Log($"Loading Progress: {task.progress}");

            if(progress >= 1.0f)
                break;

            yield return null;
        }
        
        CurrentScene = type;
        progress = 1.0f;

        C_Changed_Scene_To scenePacket = new C_Changed_Scene_To();
        scenePacket.SceneType = type;
        
        Managers.Network.Send(scenePacket);
        task.allowSceneActivation = true;
        //SceneManager.UnloadScene(prevScene);
        yield break;
    }
    public T GetManager<T>() where T: BaseScene {
        return Manager as T;
    }
}
