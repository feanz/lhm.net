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
            var identityStatement = string.Format("SET IDENTITY_INSERT [{0}] ON", _migration.Destination.Name);
            var insertStatement = string.Format(
                @"INSERT INTO {0} ({1}) 
                    SELECT {2} FROM [{3}]
                    ORDER BY {4} 
                    OFFSET {5} ROWS FETCH NEXT {6} ROWS ONLY;
                  SELECT @@RowCount", _migration.Destination.Name,
                                    _migration.Intersection.InsertForDestination,
                                    _migration.Intersection.InsertForOrigin,
                                    _migration.Origin.Name,
                                    _migration.Origin.PrimaryKey,
                                    skip,
                                    take);


            var sql = insertStatement;

            if (_migration.Destination.Columns.Any(cl => cl.IsIdentity))
            {
                sql = identityStatement + Environment.NewLine + insertStatement;
            }

            return _connection.Execute(sql, new { skip, take });
        }
    }
}
