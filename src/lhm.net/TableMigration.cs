using System;
using System.Collections.Generic;

namespace lhm.net
{
    public class TableMigration
    {
        public TableMigration(Table origin, Table destination, MigrationDateTimeStamp migrationDateTimeStamp = null, IEnumerable<RenameMap> renameMappings = null)
        {
            Origin = origin;
            Destination = destination;
            MigrationDateTimeStamp = migrationDateTimeStamp ?? new MigrationDateTimeStamp();
            Intersection = new Intersection(Origin, Destination, renameMappings);
        }

        public Table Origin { get; }

        public Table Destination { get; }

        public string ArchiveName => $"lhm_{MigrationDateTimeStamp}_{Origin.Name}".Truncate(128);

        public MigrationDateTimeStamp MigrationDateTimeStamp { get; }

        public Intersection Intersection { get; }
    }
}