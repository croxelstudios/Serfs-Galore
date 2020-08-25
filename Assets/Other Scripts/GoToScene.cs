using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToScene : MonoBehaviour
{
    [SerializeField]
    SceneReference scene = null;

    public void GoScene()
    {
        SceneManager.LoadScene(scene);
    }
}
