using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("In-Game UI")]
    [SerializeField] private TextMeshProUGUI player1ScoreText;
    [SerializeField] private TextMeshProUGUI player2ScoreText;
    [SerializeField] private TextMeshProUGUI player1LivesText;
    [SerializeField] private TextMeshProUGUI player2LivesText;
    [SerializeField] private TextMeshProUGUI currentTurnText;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private TextMeshProUGUI finalScoresText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    [SerializeField] private GameManager gameManager;

    private void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);

        // Find the GameManager if not assigned
        if (gameManager == null)
            gameManager = FindAnyObjectByType<GameManager>();
    }

    public void UpdateScores(int player1Score, int player2Score)
    {
        if (player1ScoreText != null)
            player1ScoreText.text = $"P1 Score: {player1Score}";

        if (player2ScoreText != null)
            player2ScoreText.text = $"P2 Score: {player2Score}";
    }

    public void UpdateLives(int player1Lives, int player2Lives)
    {
        if (player1LivesText != null)
            player1LivesText.text = $"P1 Lives: {player1Lives}";

        if (player2LivesText != null)
            player2LivesText.text = $"P2 Lives: {player2Lives}";
    }

    public void UpdateCurrentTurn(int playerTurn)
    {
        if (currentTurnText != null)
            currentTurnText.text = $"Player {playerTurn}'s Turn";
    }

    public void ShowGameOverScreen(int winningPlayer, int player1Score, int player2Score)
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (winnerText != null)
            winnerText.text = $"Player {winningPlayer} Wins!";

        if (finalScoresText != null)
            finalScoresText.text = $"Final Scores\nPlayer 1: {player1Score}\nPlayer 2: {player2Score}";
    }

    private void RestartGame()
    {
        if (gameManager == null)
            gameManager = FindAnyObjectByType<GameManager>();

        if (gameManager != null)
            gameManager.RestartGame();
    }

    private void ReturnToMainMenu()
    {
        if (gameManager == null)
            gameManager = FindAnyObjectByType<GameManager>();

        if (gameManager != null)
            gameManager.ReturnToMainMenu();
    }
}

