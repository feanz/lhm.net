using System;
using System.Collections.Generic;
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
            Logger.Info(string.Format("Creating triggers between: {0} and {1}", _origin.Name, _destination.Name));

            foreach (var entangle in Entanglers)
            {
                _connection.Execute(entangle());
            }

            Logger.Info(string.Format("Finished creating triggers between {0} and {1}", _origin.Name, _destination.Name));
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
            return string.Format(@"CREATE TRIGGER [{0}_Insert_lhm_{1}] ON [{0}] 
                                    AFTER INSERT 
                                    AS 
                                    BEGIN
                                        SET IDENTITY_INSERT [{2}] ON
                                        Insert into {2} ({3}) select {4} from inserted
                                    END", _origin.Name, _timestamp, _destination.Name, _intersection.InsertForDestination, _intersection.InsertForOrigin);
        }

        private string CreateUpdateTrigger()
        {
            return string.Format(@"CREATE TRIGGER [{0}_Update_lhm_{1}] ON [{0}] 
                                    AFTER Update 
                                    AS 
                                    BEGIN 
                                        Update [{2}] SET 
                                        {3}
                                        FROM [{2}]
                                        INNER JOIN INSERTED ON [{2}].[{4}] = INSERTED.[{4}]
                                    END", _origin.Name, _timestamp, _destination.Name, _intersection.UpdatesForDestination, _destination.PrimaryKey);
        }

        private string CreateDeleteTrigger()
        {
            return string.Format(@"CREATE TRIGGER [{0}_Delete_lhm_{1}] ON [{0}] 
                                    AFTER DELETE 
                                    AS 
                                    BEGIN
                                        DELETE FROM [{2}] WHERE {3} IN (SELECT {3} FROM DELETED)
                                    END", _origin.Name, _timestamp, _destination.Name, _destination.PrimaryKey);
        }
    }
}