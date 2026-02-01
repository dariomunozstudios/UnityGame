using System;
using System.Collections.Generic;

namespace GameTest.DarioMunoz.Domain.Board
{
    public class BoardModel
    {
        public int Width { get; }
        public int Height { get; }

        private readonly CellModel[,] _cells;

        public BoardModel(int width, int height)
        {
            Width = width;
            Height = height;

            _cells = new CellModel[width, height];

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    _cells[x, y] = new CellModel(x, y);
        }

        public CellModel GetCell(int x, int y)
        {
            if (!IsInside(x, y))
                throw new ArgumentOutOfRangeException($"Cell ({x},{y}) out of bounds");

            return _cells[x, y];
        }

        public bool IsInside(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }

        public void SetPiece(int x, int y, PieceModel piece)
        {
            var cell = GetCell(x, y);
            cell.SetPiece(piece);
        }

        public void ClearCells(IEnumerable<CellModel> cells)
        {
            foreach (var cell in cells)
                cell.Clear();
        }
    }
}
