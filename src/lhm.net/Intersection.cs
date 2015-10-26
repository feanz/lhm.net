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
        private readonly List<ColumnInfo> _intersect;

        public Intersection(Table origin, Table destination)
        {
            //todo this will need to hold column renames as well
            _origin = origin;
            _destination = destination;
            _intersect = _origin.Columns.Intersect(_destination.Columns).ToList();
        }

        public Intersection(Table origin, Table destination, List<ColumnMapping> columnMappings)
        {
            //todo this will need to hold column renames as well
            _origin = origin;
            _destination = destination;
            _intersect = _origin.Columns.Intersect(_destination.Columns).ToList();
            columnMappings.ForEach(cm => _intersect.AddRange(_origin.Columns.Where(cl => cl.Name == cm.OldColumnName)));
        }

        public string Insert
        {
            get { return string.Join(", ", _intersect.Select(info => string.Format("[{0}]", info.Name))); }
        }

        public string Updates
        {
            get
            {
                var statement = string.Join("\n", _intersect.Where(info => info.IsIdentity == false).Select(info => string.Format("[{0}].[{1}] = INSERTED.{1},", _destination.Name, info.Name)));
                return statement.TrimEnd(',');
            }
        }
    }
}