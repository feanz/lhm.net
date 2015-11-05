using System;
using System.Collections.Generic;

namespace lhm.net
{
    public class TableMigration
    {
        public TableMigration(Table origin, Table destination, string dateTimeStamp = null, IEnumerable<RenameMap> renameMappings = null)
        {
            Origin = origin;
            Destination = destination;
            DateTimeStamp = dateTimeStamp ?? DateTime.UtcNow.ToString(Constants.DateTimeStampFormat);
            Intersection = new Intersection(Origin, Destination, renameMappings);
        }

        public Table Origin { get; }

        public Table Destination { get; }

        public string ArchiveName => $"lhm_{DateTimeStamp}_{Origin.Name}".Truncate(128);

        public string DateTimeStamp { get; }

        public Intersection Intersection { get; }
    }
}