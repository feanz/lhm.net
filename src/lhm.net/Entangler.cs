using System;
using System.Collections.Generic;
using System.Linq;
using lhm.net.Logging;

namespace lhm.net
{
    /// <summary>
    ///   Creates entanglement between two tables. All creates, updates and deletes
    ///   to origin will be repeated on the destination table.
    /// </summary>
    public class Entangler
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private readonly ILhmConnection _connection;
        private readonly Table _origin;
        private readonly Table _destination;
        private readonly Intersection _intersection;
        private readonly string _timestamp;

        public Entangler(TableMigration migration, ILhmConnection connection)
        {
            _connection = connection;
            _intersection = migration.Intersection;
            _origin = migration.Origin;
            _destination = migration.Destination;
            _timestamp = migration.DateTimeStamp;
        }

        public void Run()
        {
            Logger.Info($"Creating triggers between: {_origin.Name} and {_destination.Name}");

            foreach (var entangle in Entanglers)
            {
                _connection.Execute(entangle());
            }

            Logger.Info($"Finished creating triggers between {_origin.Name} and {_destination.Name}");
        }

        public IEnumerable<Func<string>> Entanglers
        {
            get
            {
                yield return CreateInsertTrigger;
                yield return CreateUpdateTrigger;
                yield return CreateDeleteTrigger;
            }
        }

        private string CreateInsertTrigger()
        {
            return $@"CREATE TRIGGER [{_origin.Name}_Insert_lhm_{_timestamp}] ON [{_origin.Name}] 
                        AFTER INSERT 
                        AS 
                        BEGIN 
                            SET IDENTITY_INSERT [{_destination.Name}] ON 
                            Insert into {_destination.Name} ({_intersection.DestinationColumns}) select {_intersection.OriginColumns} from inserted 
                        END";
        }

        private string CreateUpdateTrigger()
        {
            var updateDestinationColumns = string.Join("\n", _intersection.Common.Where(info => info.DestinationColumns.IsIdentity == false)
                   .Select(info => $"[{_destination.Name}].[{info.DestinationColumns.Name}] = INSERTED.{info.OriginColumns.Name},"))
                   .TrimEnd(',');

            return $@"CREATE TRIGGER [{_origin.Name}_Update_lhm_{_timestamp}] ON [{_origin.Name}] 
                        AFTER Update 
                        AS 
                        BEGIN 
                            Update [{_destination.Name}] SET 
                            {updateDestinationColumns}
                            FROM [{_destination.Name}]
                            INNER JOIN INSERTED ON [{_destination.Name}].[{_destination.PrimaryKey}] = INSERTED.[{_destination.PrimaryKey}]
                        END";
        }

        private string CreateDeleteTrigger()
        {
            return $@"CREATE TRIGGER [{_origin.Name}_Delete_lhm_{_timestamp}] ON [{_origin.Name}] 
                        AFTER DELETE 
                        AS 
                        BEGIN
                            DELETE FROM [{_destination.Name}] WHERE {_destination.PrimaryKey} IN (SELECT {_destination.PrimaryKey} FROM DELETED)
                        END";
        }
    }
}