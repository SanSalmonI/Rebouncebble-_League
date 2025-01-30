using UnityEngine;
using UnityEngine.SceneManagement;


public class RestartGame : MonoBehaviour
{
   public void RestartLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
