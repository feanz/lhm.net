using System;

namespace lhm.net
{
    public class TableMigration
    {
        private readonly Table _origin;
        private readonly Table _destination;
        private readonly string _dateTimeStamp;

        public TableMigration(Table origin, Table destination, string dateTimeStamp)
        {
            _origin = origin;
            _destination = destination;
            _dateTimeStamp = dateTimeStamp;
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
            get { return string.Format("{0}_lhma_{1}", _origin.Name, _dateTimeStamp); }
        }

        public Intersection Intersection
        {
            get
            {
                return new Intersection(Origin, Destination);
            }
        }
    }
}