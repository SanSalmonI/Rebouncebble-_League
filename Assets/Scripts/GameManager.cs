using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LifeManager : MonoBehaviour
{
    public static LifeManager instance;
    public GameObject[] redLives;
    public GameObject[] blueLives;

    private int currentRedIndex = 0;
    private bool redWins = false;
    private int currentBlueIndex = 0;
    private bool blueWins = false;

    [SerializeField] private Image redWinsPanel;
    [SerializeField] private Image blueWinsPanel;
    [SerializeField] private Button restartButton;

    private bool restart = false;

    private void Start()
    {
        restartButton.onClick.AddListener(RestartGame);
    }
    private void Update()
    {
        CheckWin();
    }
    public void LoseRedLife()
    {
        if (currentRedIndex < redLives.Length -1)
        {
            redLives[currentRedIndex].SetActive(false);
            currentRedIndex++;
        }
        else
        {
            blueWins = true;
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
            redWins = true;
            Debug.Log("No more Blue lives left!");
        }
    }

    void CheckWin()
    {
        if (redWins || blueWins)
        {

            if (redWins)
            {
                redWinsPanel.gameObject.SetActive(true);

            }
            else if (blueWins)
            {
                blueWinsPanel.gameObject.SetActive(true);

            }
            restartButton.gameObject.SetActive(true);
            restart = true;
            // check if the player wants to restart the game
        }
    }

    public void RestartGame()
    {
        if (restart)
        {
            redWins = false;
            blueWins = false;
            currentRedIndex = 0;
            currentBlueIndex = 0;
            redWinsPanel.gameObject.SetActive(false);
            blueWinsPanel.gameObject.SetActive(false);
            restartButton.gameObject.SetActive(false);
            foreach (GameObject life in redLives)
            {
                life.SetActive(true);
            }
            foreach (GameObject life in blueLives)
            {
                life.SetActive(true);
            }
            restart = false;
        }
    }
}
