using System.Linq;
using lhm.net.Logging;

namespace lhm.net
{
    public class AtomicSwitcher
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private readonly TableMigration _migration;
        private readonly ILhmConnection _connection;

        public AtomicSwitcher(TableMigration migration, ILhmConnection connection)
        {
            _migration = migration;
            _connection = connection;
        }

        public void Run()
        {
            RenameArchiveTable();
            RemoveForeignKeyReferences();
        }

        private void RenameArchiveTable()
        {
            Logger.Info($"Renaming origin table {_migration.Origin.Name} to archive table {_migration.ArchiveName}");

            var sql = $@"DECLARE @TranName VARCHAR(20);
                        SELECT @TranName = 'LHM_Rename_Table';
                        BEGIN TRANSACTION @TranName;

                        exec sp_rename [{_migration.Origin.Name}], [{_migration.ArchiveName}];
                        exec sp_rename [{_migration.Destination.Name}], [{_migration.Origin.Name}] 

                        COMMIT TRANSACTION @TranName;";

            _connection.Execute(sql);
        }

        private void RemoveForeignKeyReferences()
        {
            Logger.Info($"Removing foreign keys from archive table {_migration.ArchiveName}");

            string sql = $@"SELECT obj.name AS FK_NAME
                    FROM sys.foreign_key_columns fkc
                    INNER JOIN sys.objects obj
                    ON obj.object_id = fkc.constraint_object_id
                    INNER JOIN sys.tables table1
                    ON table1.object_id = fkc.parent_object_id
                    WHERE table1.name = '{_migration.ArchiveName}'";
            var foriegnKeys = _connection.Query<string>(sql).ToList();

            foriegnKeys.ForEach(fk =>
            {
                sql = $"ALTER TABLE {_migration.ArchiveName} DROP CONSTRAINT \"{fk}\"";

                _connection.Execute(sql);
            });
        }
    }
}