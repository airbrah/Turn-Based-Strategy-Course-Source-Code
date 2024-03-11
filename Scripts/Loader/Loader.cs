using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    private class LoadingMonoBehavior : MonoBehaviour { } //Dumby class

    //SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    //SceneManager.MoveGameObjectToScene(GameObject.Find("Player"), SceneManager.GetSceneByName(_scene.ToString()));

    public enum Scene
    {
        NONE,
        Loading,
        EndGameScene,
        Scene0,

    }

    public enum Door
    {
        NONE,
        Door0
    }

    private static Action onLoaderCallback;
    private static AsyncOperation loadingAsyncOperation;

    public static void Load(Scene scene)
    {
        //Set loader callback to load desired scene
        onLoaderCallback = () =>
        {
            GameObject loadingGameObject = new GameObject("Loading Game Object");
            loadingGameObject.AddComponent<LoadingMonoBehavior>().StartCoroutine(LoadSceneAsync(scene));
        };
        
        // Load loading scene
        SceneManager.LoadScene(Scene.Loading.ToString()); //Loading screen
    } // Set the loader callback action to load the target scene
    private static IEnumerator LoadSceneAsync(Scene scene)
    {
        yield return null;

        loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString());
        

        while (!loadingAsyncOperation.isDone)
        {
            yield return null;
        }
    }
    public static float GetLoadingProgress()
    {
        if(loadingAsyncOperation != null)
        {
            return loadingAsyncOperation.progress;
        }
        else
        {
            return 1f;
        }
    }
    public static void LoaderCallback()
    {
        // Triggered after the first Update which lets the screen refresh
        // Execute the loader callback action which will load the target scene
        if(onLoaderCallback != null)
        {
            onLoaderCallback();
            onLoaderCallback = null;
        }
    }
}
