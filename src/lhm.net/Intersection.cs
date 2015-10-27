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
        private readonly List<ColumnInfoMap> _intersect;

        public Intersection(Table origin, Table destination)
        {
            _origin = origin;
            _destination = destination;
            _intersect = PopulateIntersects(new List<RenameMap>());
        }

        public Intersection(Table origin, Table destination, IEnumerable<RenameMap> renameMaps)
        {
            _origin = origin;
            _destination = destination;

            _intersect = PopulateIntersects(renameMaps);
        }

        public string InsertForOrigin
        {

            get { return string.Join(", ", _intersect.Select(info => string.Format("[{0}]", info.OriginColumnInfo.Name))); }
        }

        public string InsertForDestination
        {

            get { return string.Join(", ", _intersect.Select(info => string.Format("[{0}]", info.DestinationColumnInfo.Name))); }
        }

        public string UpdatesForOrigin
        {
            get
            {
                var statement = string.Join("\n", _intersect.Where(info => info.OriginColumnInfo.IsIdentity == false).Select(info => string.Format("[{0}].[{1}] = INSERTED.{1},", _origin.Name, info.OriginColumnInfo.Name)));
                return statement.TrimEnd(',');
            }
        }

        public string UpdatesForDestination
        {
            get
            {
                var statement = string.Join("\n", _intersect.Where(info => info.DestinationColumnInfo.IsIdentity == false).Select(info => string.Format("[{0}].[{1}] = INSERTED.{2},", _destination.Name, info.DestinationColumnInfo.Name, info.OriginColumnInfo.Name)));
                return statement.TrimEnd(',');
            }
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