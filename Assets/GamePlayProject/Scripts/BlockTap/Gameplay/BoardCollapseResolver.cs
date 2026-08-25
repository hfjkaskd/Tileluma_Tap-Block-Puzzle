using System.Collections.Generic;
using System.Linq;

namespace BlockTap.Gameplay
{
    public class BoardCollapseResolver
    {
        public Dictionary<int, List<int>> BuildEliminateMap(IEnumerable<BlockTapBlockView> selection)
        {
            var eliminateMap = new Dictionary<int, List<int>>();
            if (selection == null)
            {
                return eliminateMap;
            }

            foreach (var block in selection)
            {
                if (block == null)
                {
                    continue;
                }

                var column = block.Coordinate.Column;
                if (!eliminateMap.TryGetValue(column, out var rows))
                {
                    rows = new List<int>();
                    eliminateMap[column] = rows;
                }

                rows.Add(block.Coordinate.Row);
            }

            foreach (var pair in eliminateMap)
            {
                pair.Value.Sort();
            }

            return eliminateMap;
        }

        public int GetDroppedRow(int sourceRow, IReadOnlyList<int> eliminatedRows)
        {
            if (eliminatedRows == null || eliminatedRows.Count == 0)
            {
                return sourceRow;
            }

            var clearedBelow = 0;
            for (var index = 0; index < eliminatedRows.Count; index++)
            {
                if (sourceRow > eliminatedRows[index])
                {
                    clearedBelow++;
                }
            }

            return sourceRow - clearedBelow;
        }

        public IEnumerable<int> GetAffectedColumns(Dictionary<int, List<int>> eliminateMap)
        {
            return eliminateMap.Keys.OrderBy(column => column);
        }
    }
}
