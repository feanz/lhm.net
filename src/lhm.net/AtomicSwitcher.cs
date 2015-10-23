using System.Data;
using Dapper;

namespace lhm.net
{
    public class AtomicSwitcher
    {
        private readonly TableMigration _migration;
        private readonly IDbConnection _connection;

        public AtomicSwitcher(TableMigration migration, IDbConnection connection)
        {
            _migration = migration;
            _connection = connection;
        }


        public void Run()
        {
            var sql = string.Format(@"DECLARE @TranName VARCHAR(20);
                                      SELECT @TranName = 'LHM_Rename_Table';
                                      BEGIN TRANSACTION @TranName;

                                        exec sp_rename [{0}], [{1}];
                                        exec sp_rename [{2}], [{0}] 

                                      COMMIT TRANSACTION @TranName;", _migration.Origin.Name, _migration.ArchiveName, _migration.Destination.Name);
            
            _connection.Execute(sql);
        }
    }
}