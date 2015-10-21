using System;
using System.Collections.Generic;
using System.Data;
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

        public Entangler(TableMigration migration, IDbConnection connection)
        {
            _connection = connection;
            _intersection = migration.Intersection;
            _origin = migration.Origin;
            _destination = migration.Destination;
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
            return string.Format(@"CREATE TRIGGER {0}_Insert ON [{0}] 
                                    AFTER INSERT 
                                    AS 
                                    BEGIN
                                        SET IDENTITY_INSERT [{1}] ON
                                        Insert into {1} ({2}) select {2} from inserted
                                    END", _origin.Name, _destination.Name, _intersection.Insert);
        }

        private string CreateUpdateTrigger()
        {
            return string.Format(@"CREATE TRIGGER {0}_Update ON [{0}] 
                                    AFTER Update 
                                    AS 
                                    BEGIN 
                                        Update [{1}] SET 
                                        {2}
                                        FROM [{1}]
                                        INNER JOIN INSERTED ON [{1}].[{3}] = INSERTED.[{3}]
                                    END", _origin.Name, _destination.Name, _intersection.Updates, _destination.PrimaryKey);
        }

        private string CreateDeleteTrigger()
        {
            return string.Format(@"CREATE TRIGGER [{0}_Delete] ON [{0}] 
                                    AFTER DELETE 
                                    AS 
                                    BEGIN
                                        DELETE FROM [{1}] WHERE {2} IN (SELECT {2} FROM DELETED)
                                    END", _origin.Name, _destination.Name, _destination.PrimaryKey);
        }
    }
}