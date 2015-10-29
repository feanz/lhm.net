using System;
using System.Linq;
using lhm.net.Logging;
using lhm.net.Throttler;

namespace lhm.net
{
    public class Chunker
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private readonly TableMigration _migration;
        private readonly ILhmConnection _connection;
        private readonly IThrottler _throttler;

        public Chunker(TableMigration migration, ILhmConnection connection, IThrottler throttler)
        {
            _migration = migration;
            _connection = connection;
            _throttler = throttler;
        }

        public void Run()
        {
            var start = 0;
            var nextToInsert = start;
            var stride = _throttler.Stride;

            int rowsAffected;

            Logger.Info($"Starting to copy data from: {_migration.Origin.Name} to {_migration.Destination.Name}");

            do
            {
                rowsAffected = Copy(nextToInsert, stride);

                if (rowsAffected > 0)
                {
                    _throttler.Run();
                }

                Logger.Info($"Copied batch of {rowsAffected} rows");
                nextToInsert += stride;

            } while (rowsAffected > 0);

            Logger.Info($"Finsihed copying data from: {_migration.Origin.Name} to {_migration.Destination.Name} rows copied:{rowsAffected}");
        }

        private int Copy(int skip, int take)
        {
            var identityStatement = $"SET IDENTITY_INSERT [{_migration.Destination.Name}] ON";
            var insertStatement = $@"INSERT INTO [{_migration.Destination.Name}] ({_migration.Intersection.InsertForDestination})  
                        SELECT {_migration.Intersection.InsertForOrigin} 
                        FROM [{_migration.Origin.Name}] 
                        ORDER BY {_migration.Origin.PrimaryKey} OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY; 
                        SELECT @@RowCount";


            var sql = insertStatement;

            if (_migration.Destination.Columns.Any(cl => cl.IsIdentity))
            {
                sql = identityStatement + Environment.NewLine + insertStatement;
            }

            return _connection.Execute(sql, new { skip, take });
        }
    }
}
