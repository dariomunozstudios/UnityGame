using System.Collections.Generic;
using GameTest.DarioMunoz.Domain.Board;

namespace GameTest.DarioMunoz
{
    /// <summary>
    /// Performs flood-fill algorithm to find all connected blocks of the same color.
    /// When a player taps a block, this system finds all adjacent blocks (up, down, left, right)
    /// that share the same color type.
    /// </summary>
    public class FloodFillCollector
    {
        // Directions: up, down, left, right
        private static readonly (int dx, int dy)[] Directions = new[]
        {
            (0, 1),   // Up
            (0, -1),  // Down
            (-1, 0),  // Left
            (1, 0)    // Right
        };

        /// <summary>
        /// Finds all cells connected to the starting cell that share the same piece type.
        /// Uses iterative flood-fill to avoid stack overflow on large groups.
        /// </summary>
        public List<CellModel> FindConnectedCells(BoardModel board, CellModel startCell)
        {
            var result = new List<CellModel>();

            // Can't collect from empty cells
            if (startCell.IsEmpty)
                return result;

            var targetType = startCell.Piece.Type;
            var visited = new HashSet<CellModel>();
            var queue = new Queue<CellModel>();

            // Start flood-fill from the clicked cell
            queue.Enqueue(startCell);
            visited.Add(startCell);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                result.Add(current);

                // Check all 4 directions
                foreach (var (dx, dy) in Directions)
                {
                    int newX = current.X + dx;
                    int newY = current.Y + dy;

                    // Skip if out of bounds
                    if (!board.IsInside(newX, newY))
                        continue;

                    var neighbor = board.GetCell(newX, newY);

                    // Skip if already visited
                    if (visited.Contains(neighbor))
                        continue;

                    // Skip if empty or different color
                    if (neighbor.IsEmpty || neighbor.Piece.Type != targetType)
                        continue;

                    // Add to queue for processing
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }

            return result;
        }
    }
}
