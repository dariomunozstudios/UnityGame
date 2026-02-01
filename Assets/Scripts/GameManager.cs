using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using GameTest.DarioMunoz.Domain.Board;


namespace GameTest.DarioMunoz
{
    /// <summary>
    /// Main Game controller: Task 3 | Building the puzzle mechanic
    /// <summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Game Configuration")]
        [SerializeField] private int gridWidth = 5;
        [SerializeField] private int gridHeight = 6;
        [SerializeField] private int initialMoves = 5;

        [Header("UI References")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text movesText;
        [SerializeField] private GameObject gameOverScreen;

        [Header("Board References")]
        [SerializeField] private Transform boardRoot;
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private Camera mainCamera;

        // Game state
        private int _currentScore;
        private int _remainingMoves;
        private bool _isGameOver;
        private bool _isProcessing;

        // Board systems
        private BoardModel _board;
        private BoardRefillSystem _refillSystem;
        private FloodFillCollector _floodFillCollector;
        private BoardVisualizer _boardVisualizer;

        void Start()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            // Systems created once, reused on replay
            _refillSystem = new BoardRefillSystem();
            _floodFillCollector = new FloodFillCollector();
            _boardVisualizer = new BoardVisualizer(cellPrefab, boardRoot);

            StartNewGame();
        }
        void Update()
        {
            HandleInput();
        }

        /// <summary>
        /// Detects clicks/taps using the New Input System and raycasts to find the cell.
        /// </summary>
        private void HandleInput()
        {
            if (_isProcessing || _isGameOver)
                return;

            // New Input System: detect mouse click
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 screenPos = Mouse.current.position.ReadValue();
                ProcessTap(screenPos);
                return;
            }

            // New Input System: detect touch
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                Vector2 screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
                ProcessTap(screenPos);
            }
        }

        /// <summary>
        /// Converts screen position to world position and raycasts to find the clicked cell.
        /// </summary>
        private void ProcessTap(Vector2 screenPos)
        {
            Vector2 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider == null)
            {
                Debug.Log($"Raycast don't hit anything in {worldPos}");
                return;
            }

            var cellVisual = hit.collider.GetComponent<CellVisual>();
            if (cellVisual == null || cellVisual.Model == null)
            {
                Debug.Log("Hitted object don't have CellVisual");
                return;
            }

            int x = cellVisual.Model.X;
            int y = cellVisual.Model.Y;

            Debug.Log($"Nothing clicked in ({x}, {y})");
            OnCellClicked(x, y);
        }

        /// <summary>
        /// Called when a cell is clicked. Performs flood-fill collection.
        /// </summary>
        public void OnCellClicked(int x, int y)
        {
            if (_isProcessing || _isGameOver)
                return;

            if (!_board.IsInside(x, y))
                return;

            var cell = _board.GetCell(x, y);
            if (cell.IsEmpty)
                return;

            var connectedCells = _floodFillCollector.FindConnectedCells(_board, cell);

            Debug.Log($"Find {connectedCells.Count} connected blocks of color: {cell.Piece.Type}");

            if (connectedCells.Count == 0)
                return;

            StartCoroutine(ProcessCollection(connectedCells));
        }

        private IEnumerator ProcessCollection(List<CellModel> cells)
        {
            _isProcessing = true;

            // Score: +1 for 1 block, +2 for 2, etc.
            _currentScore += cells.Count;
            UpdateScoreUI();

            _remainingMoves--;
            UpdateMovesUI();

            // Clear
            _boardVisualizer.ClearCells(cells);
            foreach (var cell in cells)
                cell.Clear();

            // Wait 1 second then refill
            yield return new WaitForSeconds(1f);

            _refillSystem.Refill(_board);
            _boardVisualizer.RefreshBoard(_board);

            _isProcessing = false;

            if (_remainingMoves <= 0)
                ShowGameOver();
        }

        /// <summary>
        /// Resets only game state and board data. Reuses existing systems and pool.
        /// </summary>
        private void StartNewGame()
        {
            _board = new BoardModel(gridWidth, gridHeight);
            _refillSystem.Refill(_board);

            _boardVisualizer.DestroyGrid();
            _boardVisualizer.CreateGrid(_board);

            _currentScore = 0;
            _remainingMoves = initialMoves;
            _isGameOver = false;
            _isProcessing = false;

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
