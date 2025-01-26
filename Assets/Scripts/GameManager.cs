using UnityEngine;

public class LifeManager : MonoBehaviour
{
    public GameObject[] redLives;
    public GameObject[] blueLives;

    private int currentRedIndex = 0;
    private int currentBlueIndex = 0;

    public void LoseRedLife()
    {
        if (currentRedIndex <= redLives.Length-1)
        {
            redLives[currentRedIndex].SetActive(false);
            currentRedIndex++;
        }
        else
        {
            Debug.Log("No more Red lives left!");
        }
    }

    public void LoseBlueLife()
    {
        if (currentBlueIndex < blueLives.Length)
        {
            blueLives[currentBlueIndex].SetActive(false);
            currentBlueIndex++;
        }
        else
        {
            Debug.Log("No more Blue lives left!");
        }
    }


    //reset lives

   
}
