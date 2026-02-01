using System.Collections.Generic;
using UnityEngine;
using GameTest.DarioMunoz.Domain.Board;

namespace GameTest.DarioMunoz
{
    /// <summary>
    /// Handles the visual representation of the board.
    /// Uses object pooling to avoid Instantiate/Destroy overhead on replay.
    /// </summary>
    public class BoardVisualizer
    {
        // Cell size in Unity units (128x112 pixels at 100 PPU)
        private const float CellWidth = 1.28f;
        private const float CellHeight = 1.12f;

        // Total cells in the grid (5x6 = 30), used for pre-warming the pool
        private const int TotalCells = 30;

        private readonly Transform _boardRoot;
        private readonly GameObjectPool _cellPool;
        private readonly Dictionary<CellModel, CellVisual> _cellVisuals;

        // Track active GameObjects to return them to the pool
        private readonly List<GameObject> _activeObjects;

        public BoardVisualizer(GameObject cellPrefab, Transform boardRoot)
        {
            _boardRoot = boardRoot;
            _cellVisuals = new Dictionary<CellModel, CellVisual>();
            _activeObjects = new List<GameObject>();

            // Pre-warm pool with exactly the number of cells we need
            _cellPool = new GameObjectPool(cellPrefab, boardRoot, TotalCells);
        }

        /// <summary>
        /// Creates the visual grid. Gets objects from pool instead of instantiating.
        /// </summary>
        public void CreateGrid(BoardModel board)
        {
            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    var cellModel = board.GetCell(x, y);
                    var cellObj = _cellPool.Get();

                    // Position
                    float posX = x * CellWidth;
                    float posY = y * CellHeight;
                    cellObj.transform.localPosition = new Vector3(posX, posY, 0);

                    // Sorting: upper rows render on top to cover LEGO plugs
                    var spriteRenderer = cellObj.GetComponentInChildren<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.sortingOrder = y;
                    }

                    var cellVisual = cellObj.GetComponent<CellVisual>();
                    if (cellVisual == null)
                    {
                        cellVisual = cellObj.AddComponent<CellVisual>();
                    }

                    cellVisual.Initialize(cellModel);
                    _cellVisuals[cellModel] = cellVisual;
                    _activeObjects.Add(cellObj);
                }
            }
        }

        public void ClearCells(List<CellModel> cells)
        {
            foreach (var cell in cells)
            {
                if (_cellVisuals.TryGetValue(cell, out var visual))
                {
                    visual.PlayClearEffect();
                }
            }
        }

        public void RefreshBoard(BoardModel board)
        {
            foreach (var kvp in _cellVisuals)
            {
                kvp.Value.Refresh();
            }
        }

        /// <summary>
        /// Returns all active cells to the pool instead of destroying them.
        /// </summary>
        public void DestroyGrid()
        {
            foreach (var obj in _activeObjects)
            {
                if (obj != null)
                    _cellPool.Return(obj);
            }

            _activeObjects.Clear();
            _cellVisuals.Clear();
        }
    }
}
