using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using Dapper;

namespace lhm.net
{
    /// <summary>
    ///   Creates entanglement between two tables. All creates, updates and deletes
    ///   to origin will be repeated on the destination table.
    /// </summary>
    public class Entangler
    {
        private readonly IDbConnection _connection;
        private readonly Table _origin;
        private readonly Table _destination;
        private readonly Intersection _intersection;
        private readonly string _timestamp;

        public Entangler(TableMigration migration, IDbConnection connection)
        {
            _connection = connection;
            _intersection = migration.Intersection;
            _origin = migration.Origin;
            _destination = migration.Destination;
            _timestamp = migration.DateTimeStamp;

        }

        public void Run()
        {
            foreach (var entangle in Entanglers)
            {
                _connection.Execute(entangle());
            }
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
            return string.Format(@"CREATE TRIGGER {0}_Insert_{1} ON [{0}] 
                                    AFTER INSERT 
                                    AS 
                                    BEGIN
                                        SET IDENTITY_INSERT [{2}] ON
                                        Insert into {2} ({3}) select {3} from inserted
                                    END", _origin.Name, _timestamp, _destination.Name, _intersection.Insert);
        }

        private string CreateUpdateTrigger()
        {
            return string.Format(@"CREATE TRIGGER {0}_Update_{1} ON [{0}] 
                                    AFTER Update 
                                    AS 
                                    BEGIN 
                                        Update [{2}] SET 
                                        {3}
                                        FROM [{2}]
                                        INNER JOIN INSERTED ON [{2}].[{4}] = INSERTED.[{4}]
                                    END", _origin.Name, _timestamp, _destination.Name, _intersection.Updates, _destination.PrimaryKey);
        }

        private string CreateDeleteTrigger()
        {
            return string.Format(@"CREATE TRIGGER [{0}_Delete_{1}] ON [{0}] 
                                    AFTER DELETE 
                                    AS 
                                    BEGIN
                                        DELETE FROM [{2}] WHERE {3} IN (SELECT {3} FROM DELETED)
                                    END", _origin.Name, _timestamp, _destination.Name, _destination.PrimaryKey);
        }
    }
}