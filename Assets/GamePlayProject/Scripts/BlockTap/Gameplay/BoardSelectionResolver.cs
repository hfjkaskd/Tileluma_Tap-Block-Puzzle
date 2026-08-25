using System.Collections.Generic;

namespace BlockTap.Gameplay
{
    public class BoardSelectionResolver
    {
        private readonly int columns;
        private readonly int rows;

        public BoardSelectionResolver(int columns, int rows)
        {
            this.columns = columns;
            this.rows = rows;
        }

        public List<BlockTapBlockView> Resolve(BlockTapBlockView[,] grid, BlockTapBlockView origin)
        {
            var result = new List<BlockTapBlockView>();
            if (origin == null)
            {
                return result;
            }

            var seen = new HashSet<BlockTapBlockView>();
            ResolveRecursive(grid, origin, origin.ColorIndex, seen, result);
            return result;
        }

        private void ResolveRecursive(
            BlockTapBlockView[,] grid,
            BlockTapBlockView pivot,
            int colorIndex,
            HashSet<BlockTapBlockView> seen,
            List<BlockTapBlockView> result)
        {
            if (pivot == null || !seen.Add(pivot))
            {
                return;
            }

            result.Add(pivot);

            AddLine(grid, pivot.Coordinate.Column, pivot.Coordinate.Row + 1, 0, 1, colorIndex, seen, result);
            AddLine(grid, pivot.Coordinate.Column, pivot.Coordinate.Row - 1, 0, -1, colorIndex, seen, result);
            AddLine(grid, pivot.Coordinate.Column + 1, pivot.Coordinate.Row, 1, 0, colorIndex, seen, result);
            AddLine(grid, pivot.Coordinate.Column - 1, pivot.Coordinate.Row, -1, 0, colorIndex, seen, result);
        }

        private void AddLine(
            BlockTapBlockView[,] grid,
            int startColumn,
            int startRow,
            int columnStep,
            int rowStep,
            int colorIndex,
            HashSet<BlockTapBlockView> seen,
            List<BlockTapBlockView> result)
        {
            var column = startColumn;
            var row = startRow;

            while (column >= 0 && column < columns && row >= 0 && row < rows)
            {
                var candidate = grid[column, row];
                if (candidate == null || candidate.ColorIndex != colorIndex)
                {
                    break;
                }

                if (!seen.Contains(candidate))
                {
                    ResolveRecursive(grid, candidate, colorIndex, seen, result);
                }

                column += columnStep;
                row += rowStep;
            }
        }
    }
}
