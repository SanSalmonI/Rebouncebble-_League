using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    [SerializeField] private int startingLives = 3;

    [Header("Scoring")]
    [SerializeField] private int successfulBumpPoints = 10;
    [SerializeField] private int wrongTurnPenalty = -5;

    [Header("UI References")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameObject gameOverScreen;

    [Header("Game Objects")]
    [SerializeField] private GameObject bubble;

    // Game state
    private int player1Lives;
    private int player2Lives;
    private int player1Score;
    private int player2Score;
    private int currentPlayerTurn = 1; // 1 for Player 1, 2 for Player 2
    private bool gameOver = false;
    private int lastPlayerToBump = 0; // 0 means no one has bumped yet

    // Static instance
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeGame();
    }

    public void InitializeGame()
    {
        player1Lives = startingLives;
        player2Lives = startingLives;
        player1Score = 0;
        player2Score = 0;
        currentPlayerTurn = 1;
        lastPlayerToBump = 0;
        gameOver = false;

        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);

        UpdateUI();
    }

    public void BubbleHitByPlayer(int playerNumber)
    {
        if (gameOver) return;

        // Check if it's the correct player's turn
        if (playerNumber == currentPlayerTurn)
        {
            // Correct player bumped the bubble
            if (playerNumber == 1)
                player1Score += successfulBumpPoints;
            else
                player2Score += successfulBumpPoints;

            // Switch turns
            currentPlayerTurn = (currentPlayerTurn == 1) ? 2 : 1;
            lastPlayerToBump = playerNumber;

            Debug.Log($"Player {playerNumber} successfully bumped the bubble! Turn switches to Player {currentPlayerTurn}");
        }
        else
        {
            // Wrong player bumped the bubble
            if (playerNumber == 1)
                player1Score += wrongTurnPenalty;
            else
                player2Score += wrongTurnPenalty;

            Debug.Log($"Player {playerNumber} bumped out of turn! Loses {Mathf.Abs(wrongTurnPenalty)} points");
        }

        UpdateUI();
    }

    public void BubbleHitGround()
    {
        if (gameOver) return;

        // The player whose turn it is loses a life
        if (currentPlayerTurn == 1)
        {
            player1Lives--;
            Debug.Log($"Player 1 lost a life! Remaining lives: {player1Lives}");

            if (player1Lives <= 0)
            {
                EndGame(2); // Player 2 wins
                return;
            }
        }
        else
        {
            player2Lives--;
            Debug.Log($"Player 2 lost a life! Remaining lives: {player2Lives}");

            if (player2Lives <= 0)
            {
                EndGame(1); // Player 1 wins
                return;
            }
        }

        UpdateUI();
        ResetBubblePosition();
    }

    private void ResetBubblePosition()
    {
        if (bubble != null)
        {
            // Reset bubble to a position above the center of the play area
            bubble.transform.position = new Vector3(0, 5, 0);

            // Reset velocity
            Rigidbody rb = bubble.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    private void EndGame(int winningPlayer)
    {
        gameOver = true;
        Debug.Log($"Game Over! Player {winningPlayer} wins!");

        if (uiManager != null)
        {
            uiManager.ShowGameOverScreen(winningPlayer, player1Score, player2Score);
        }
        else if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }
    }

    private void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateLives(player1Lives, player2Lives);
            uiManager.UpdateScores(player1Score, player2Score);
            uiManager.UpdateCurrentTurn(currentPlayerTurn);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0); // Assuming main menu is scene 0
    }
}
