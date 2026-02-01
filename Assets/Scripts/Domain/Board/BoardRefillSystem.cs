using System;

namespace GameTest.DarioMunoz.Domain.Board
{
    public class BoardRefillSystem
    {
        private readonly Random _random = new Random();

        /// <summary>
        /// Applies gravity (blocks fall down) and then fills empty cells at the top
        /// with new random blocks.
        /// </summary>
        public void Refill(BoardModel board)
        {
            ApplyGravity(board);
            FillEmptyCells(board);
        }

        /// <summary>
        /// For each column, shifts all existing blocks down to fill gaps.
        /// Scans from bottom to top: when an empty cell is found, pulls the
        /// nearest block above it down into that position.
        /// </summary>
        private void ApplyGravity(BoardModel board)
        {
            for (int x = 0; x < board.Width; x++)
            {
                // writeIndex tracks the next empty position to fill from the bottom
                int writeIndex = 0;

                // Collect all non-empty pieces in this column from bottom to top
                for (int y = 0; y < board.Height; y++)
                {
                    var cell = board.GetCell(x, y);
                    if (!cell.IsEmpty)
                    {
                        if (y != writeIndex)
                        {
                            // Move piece down to the lowest available position
                            var targetCell = board.GetCell(x, writeIndex);
                            targetCell.SetPiece(cell.Piece);
                            cell.Clear();
                        }
                        writeIndex++;
                    }
                }
            }
        }

        /// <summary>
        /// Fills any remaining empty cells (top of each column) with new random blocks.
        /// </summary>
        private void FillEmptyCells(BoardModel board)
        {
            for (int x = 0; x < board.Width; x++)
                for (int y = 0; y < board.Height; y++)
                {
                    var cell = board.GetCell(x, y);
                    if (cell.IsEmpty)
                    {
                        cell.SetPiece(new PieceModel(GetRandomType()));
                    }
                }
        }

        private PieceType GetRandomType()
        {
            var values = Enum.GetValues(typeof(PieceType));
            return (PieceType)values.GetValue(_random.Next(values.Length));
        }
    }
}
