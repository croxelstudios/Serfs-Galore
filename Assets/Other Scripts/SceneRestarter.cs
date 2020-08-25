using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneRestarter : MonoBehaviour
{
    public void SceneRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
