using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private SceneReference sceneToLoad = null;
    [SerializeField]
    private LoadSceneMode mode = LoadSceneMode.Single;
    [Header("Asynchronous")]
    [SerializeField]
    private bool async = false;
    [SerializeField]
    private bool activateSceneOnLoad = false;
    [SerializeField]
    private bool loadOnStart = false;
    private AsyncOperation asyncOperation;
    public UnityEvent onSceneLoaded;
    public FloatEvent onLoadingProgressChange;

    private float progress = 0;

    private void Start()
    {
        if (loadOnStart)
        {
            LoadScene();
        }
    }

    private void Update()
    {
        


        if (async && asyncOperation!=null)
        {
            if (asyncOperation.progress >= progress)
            {
                onLoadingProgressChange.Invoke(progress);
                progress = asyncOperation.progress;
            }

            if (!activateSceneOnLoad)
                if (asyncOperation.progress >= 0.9f)
                    SceneLoaded(asyncOperation);

        }
    }

    [ContextMenu("LoadScene")]
    public void LoadScene()
    {
        if (async)
        {
            asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad, mode);
            asyncOperation.allowSceneActivation = activateSceneOnLoad;
            if (activateSceneOnLoad)
                asyncOperation.completed += SceneLoaded;
        }
        else
            SceneManager.LoadScene(sceneToLoad);
    }

    private void SceneLoaded(AsyncOperation obj)
    {
        onSceneLoaded?.Invoke();
    }

    [ContextMenu("GoToScene")]
    public void GoToScene()
    {
        if (async)
            GoToAsyncLoadedScene();
        else
            SceneManager.LoadScene(sceneToLoad);
    }

    [ContextMenu("GoToAsyncLoadedScene")]
    public void GoToAsyncLoadedScene()
    {
        if (asyncOperation != null)
        {
            asyncOperation.allowSceneActivation = true;
        }
    }

    public float GetLoadProgress()
    {
        return asyncOperation.progress;
    }

    [Serializable]
    public class FloatEvent : UnityEvent<float>
    {

    }

}


