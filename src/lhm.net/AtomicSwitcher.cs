using System.Collections.Generic;
using System.Linq;
using System.Runtime;
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
            SwitchTables();
            RemoveForeignKeyReferences();
        }

        /// <summary>
        /// Switch the origin table with the migrated talbe. The original table is archived.  We need to drop any Foreign Keys that have a dependency on this table first as 
        /// sp_rename will update them to point at the archive. Once the rename is complete we can add the constaint back in.  
        /// </summary>
        private void SwitchTables()
        {
            Logger.Info($"Renaming origin table {_migration.Origin.Name} to archive table {_migration.ArchiveName}");

            var fkeys = _connection.Query<FKeyInfo>($"EXEC sp_fkeys '{_migration.Origin.Name}'")
                .ToList();

            var sql = $@"DECLARE @TranName VARCHAR(20);
                        SELECT @TranName = 'LHM_Rename_Table';
                        BEGIN TRANSACTION @TranName;
                        
                        {CreateDropFKeys(fkeys)}
                        
                        exec sp_rename [{_migration.Origin.Name}], [{_migration.ArchiveName}];                                                
                        exec sp_rename [{_migration.Destination.Name}], [{_migration.Origin.Name}]                         

                        {CreateAddFKeys(fkeys)}

                        COMMIT TRANSACTION @TranName;";

            _connection.Execute(sql);
        }

        /// <summary>
        /// We need to remove any foreign keys from the archived tables pointing at any existing tables in the database. 
        /// </summary>
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

        private static string CreateDropFKeys(List<FKeyInfo> fkeys)
        {
            string dropKeys = null;
            fkeys.ForEach(fkey => { dropKeys += $"\nALTER TABLE [{fkey.FKTABLE_NAME}] DROP CONSTRAINT [{fkey.FK_NAME}]"; });
            return dropKeys;
        }
        
        private static string CreateAddFKeys(List<FKeyInfo> fkeys)
        {
            string addKeys = null;
            fkeys.ForEach(fkey =>
            {
                addKeys += "\n" + $@"ALTER TABLE [{fkey.FKTABLE_NAME}]  WITH CHECK ADD  CONSTRAINT [{fkey.FK_NAME}] FOREIGN KEY([{fkey.FKCOLUMN_NAME}])
                                REFERENCES [{fkey.PKTABLE_NAME}] ([{fkey.PKCOLUMN_NAME}])";
            });
            return addKeys;
        }
    }
}