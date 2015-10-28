using System;
using System.Collections.Generic;
using System.Linq;

namespace lhm.net
{
    public class TableMigration
    {
        private readonly Table _origin;
        private readonly Table _destination;
        private readonly string _dateTimeStamp;
        private readonly List<RenameMap> _columnMappings;

        public TableMigration(Table origin, Table destination, string dateTimeStamp)
        {
            _origin = origin;
            _destination = destination;
            _dateTimeStamp = dateTimeStamp;
            _columnMappings = new List<RenameMap>();
        }
        public TableMigration(Table origin, Table destination, string dateTimeStamp, List<RenameMap> columnMappings)
        {
            _origin = origin;
            _destination = destination;
            _dateTimeStamp = dateTimeStamp;
            _columnMappings = columnMappings;
        }

        public Table Origin
        {
            get { return _origin; }
        }

        public Table Destination
        {
            get { return _destination; }
        }

        public string ArchiveName
        {
            get { return string.Format("{0}_lhm_{1}", _origin.Name, _dateTimeStamp); }
        }

        public string DateTimeStamp
        {
            get { return _dateTimeStamp; }
        }

        public Intersection Intersection
        {
            get
            {
                if (_columnMappings.Any())
                {
                    return new Intersection(Origin, Destination, _columnMappings);
                }
                
                return new Intersection(Origin, Destination);
            }
        }
    }
}