using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.SceneManagement;

public class DontDestroyUniqueTag : MonoBehaviour
{
    [SerializeField]
    bool destroyOnScenesWithoutIt = true;
    [SerializeField]
    UnityEvent onFirstLoad = null;
    [SerializeField]
    SceneEvent sceneLoaded = null;
    [SerializeField]
    SceneEvent sceneChanged = null;
    [SerializeField]
    UnityEvent sceneRestarted = null;

    [Serializable]
    class SceneEvent : UnityEvent<Scene> { }

    int currentScene;
    bool imChosen;
    bool duplicateDestroyed;
    bool isFirstLoad;
    bool sceneReset;

    void OnEnable()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
        if (objs.Length > 1)
        {
            foreach (GameObject obj in objs)
                if (obj != gameObject)
                {
                    DontDestroyUniqueTag ddut = obj.GetComponent<DontDestroyUniqueTag>();
                    if (ddut != null)
                    {
                        ddut.ChooseThisOne();
                        break;
                    }
                }
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += SceneLoaded;
            onFirstLoad?.Invoke();
            isFirstLoad = true;
            duplicateDestroyed = true;
            imChosen = true;
        }
    }

    void OnDisable()
    {
        if (imChosen)
        {
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
            SceneManager.sceneLoaded -= SceneLoaded;
            imChosen = false;
        }
    }

    void OnDestroy()
    {
        //Debug.Log("");
    }

    public void ChooseThisOne()
    {
        duplicateDestroyed = true;
    }

    void LateUpdate()
    {
        if (sceneReset)
            SceneManager.LoadScene(SceneManager.GetActiveScene().path);
        else
        {
            if (isFirstLoad) isFirstLoad = false;
            duplicateDestroyed = false;
        }
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isFirstLoad) isFirstLoad = false;
        else
        {
            if (destroyOnScenesWithoutIt && !duplicateDestroyed) SelfDestruct();
            else
            {
                sceneLoaded?.Invoke(scene);
                if (scene.buildIndex != currentScene)
                {
                    sceneChanged?.Invoke(scene);
                    currentScene = scene.buildIndex;
                }
                else sceneRestarted?.Invoke();
            }
        }
    }
    
    public void SelfDestruct()
    {
        Destroy(gameObject);
    }

    public void Restart() //TO DO: This could be done much better (as of right now, it loads the scene twice)
    {
        if (isActiveAndEnabled)
        {
            OnDisable();
            sceneReset = true;
        }
    }
}
