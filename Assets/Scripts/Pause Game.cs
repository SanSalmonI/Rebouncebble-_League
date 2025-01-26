using UnityEngine;

public class PauseGame : MonoBehaviour
{


    bool isPaused=false;
    private void Update()
    {
        
        if (isPaused == false&&Input.GetKeyDown(KeyCode.Escape)) { 
            isPaused = true;
            Time.timeScale = 0;
        }
        else if (isPaused == true&& Input.GetKeyDown(KeyCode.Escape)) {
            isPaused= false;
            Time.timeScale = 1;
        } 
 
    }


}
