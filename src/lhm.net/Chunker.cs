using System.Data;
using Dapper;
using lhm.net.Logging;
using lhm.net.Throttler;

namespace lhm.net
{
    public class Chunker
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private readonly TableMigration _migration;
        private readonly IDbConnection _connection;
        private readonly IThrottler _throttler;

        public Chunker(TableMigration migration, IDbConnection connection, MigrationOptions options)
        {
            _migration = migration;
            _connection = connection;
            _throttler = options.Throttler;
        }

        public void Run()
        {
            var start = 0;
            var nextToInsert = start;
            var stride = _throttler.Stride;

            int rowsAffected;

            Logger.Info(string.Format("Starting to copy data from: {0} to {1}", _migration.Origin.Name, _migration.Destination.Name));

            do
            {
                rowsAffected = Copy(nextToInsert, stride);

                if (rowsAffected > 0)
                {
                    _throttler.Run();
                }

                Logger.Info(string.Format("Copied batch of {0} rows", rowsAffected));
                nextToInsert += stride;

            } while (rowsAffected > 0);

            Logger.Info(string.Format("Finsihed copying data from: {0} to {1} rows copied:{2}", _migration.Origin.Name, _migration.Destination.Name, rowsAffected));
        }

        private int Copy(int skip, int take)
        {
            var sql = string.Format(
                @"SET IDENTITY_INSERT [{0}] ON
                   INSERT INTO {0} ({1}) 
                    SELECT {2} FROM [{3}]
                    ORDER BY {4} 
                    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;
                  SELECT @@RowCount", _migration.Destination.Name, _migration.Intersection.InsertForDestination, _migration.Intersection.InsertForOrigin, _migration.Origin.Name, _migration.Origin.PrimaryKey);

            return _connection.Execute(sql, new { skip, take });
        }
    }
}