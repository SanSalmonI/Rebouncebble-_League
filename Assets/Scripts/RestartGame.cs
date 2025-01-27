using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEditor.Build.Content;


public class RestartGame : MonoBehaviour
{
   public void RestartLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
