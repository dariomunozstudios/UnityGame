using TMPro;
using UnityEngine;


namespace GameTest.DarioMunoz
{
    /// <summary>
    /// Main Game controller: Task 2 | Data Handling : Implement a C# script to manage score and moves.
    /// <summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Game Configuration")]
        [SerializeField] private int initialMoves = 5;

        [Header("UI References")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text movesText;
        [SerializeField] private GameObject gameOverScreen;

        // Game state
        private int _currentScore;
        private int _remainingMoves;
        private bool _isGameOver;

        void Start()
        {
            StartNewGame();
        }
        void Update()
        {

        }

        private void StartNewGame()
        {
            _currentScore = 0;
            _remainingMoves = initialMoves;
            _isGameOver = false;

            UpdateScoreUI();
            UpdateMovesUI();
            gameOverScreen.SetActive(false);
        }

        private void ShowGameOver()
        {
            _isGameOver = true;
            gameOverScreen.SetActive(true);
        }

        // DEV ONLY: Assign to a UI button to drain moves one by one
        public void ButtonTestCount()
        {
            if (_isGameOver) return;

            _remainingMoves--;
            _currentScore += 10;
            UpdateScoreUI();
            UpdateMovesUI();

            if (_remainingMoves <= 0)
                ShowGameOver();
        }

        public void ReplayGame()
        {
            StartNewGame();
        }

        private void UpdateScoreUI()
        {
            scoreText.text = _currentScore.ToString();
        }

        private void UpdateMovesUI()
        {
            movesText.text = _remainingMoves.ToString();
        }
    }
}
