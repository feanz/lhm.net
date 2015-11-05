using System.Collections.Generic;
using System.Linq;

namespace lhm.net
{
    /// <summary>
    ///  Determine and format columns common to origin and destination.
    /// </summary>
    public class Intersection
    {
        private readonly Table _origin;
        private readonly Table _destination;

        public Intersection(Table origin, Table destination, IEnumerable<RenameMap> renameMaps = null)
        {
            _origin = origin;
            _destination = destination;
            Common = PopulateIntersects(renameMaps ?? new List<RenameMap>());
        }

        public List<ColumnInfoMap> Common { get; }

        public string OriginColumns
        {

            get { return string.Join(", ", Common.Select(info => $"[{info.OriginColumns.Name}]")); }
        }

        public string DestinationColumns
        {

            get { return string.Join(", ", Common.Select(info => $"[{info.DestinationColumns.Name}]")); }
        }

        private List<ColumnInfoMap> PopulateIntersects(IEnumerable<RenameMap> renameMaps)
        {
            var intersect = _origin.Columns.Intersect(_destination.Columns).SelectMany(x =>
            {
                var destColumn = _destination.Columns.Single(y => y.Name == x.Name);
                var origColumn = _origin.Columns.Single(y => y.Name == x.Name);
                var columnInfoMapping = new ColumnInfoMap(origColumn, destColumn);

                return new List<ColumnInfoMap> { columnInfoMapping };

            }).ToList();

            intersect.AddRange(from renameMap in renameMaps
                               let destColumn = _destination.Columns.Single(y => y.Name == renameMap.NewColumnName)
                               let origColumn = _origin.Columns.Single(y => y.Name == renameMap.OldColumnName)
                               select new ColumnInfoMap(origColumn, destColumn));

            return intersect;
        }
    }
}