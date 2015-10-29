using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using lhm.net.Logging;

namespace lhm.net
{
    /// <summary>
    ///  Copies existing schema and applies changes using alter on the empty table.
    ///  `run` returns a Migration which can be used for the remaining process.
    /// </summary>
    public class Migrator
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();
        private readonly Table _origin;
        private readonly ILhmConnection _connection;
        private readonly List<string> _statements;
        private readonly List<RenameMap> _renameMaps; 
        private readonly string _dateTimeStamp;

        public Migrator(Table origin, ILhmConnection connection = null)
        {
            _origin = origin;
            _connection = connection;
            _statements = new List<string>();
            _renameMaps = new List<RenameMap>();
            _dateTimeStamp = DateTime.UtcNow.ToString(Constants.DateFormat);
        }

        public string Destination
        {
            get { return _origin.DestinationName; }
        }

        public List<string> Statements
        {
            get
            {
                return _statements;
            }
        }

        public void AddColumn(string columnName, string type)
        {
            Ddl("ALTER TABLE [{0}] Add [{1}] [{2}]", Destination, columnName, type);
        }

        public void RemoveColumn(string columnName)
        {
            Ddl("ALTER TABLE {0} DROP COLUMN {1}", Destination, columnName);
        }

        public void RenameColumn(string oldColumnName, string newColumnName)
        {
            Ddl("EXEC sp_rename '{0}.{1}', '{2}', 'COLUMN'", Destination, oldColumnName, newColumnName);
            _renameMaps.Add(new RenameMap(oldColumnName, newColumnName));
        }

        public void AddIndex(string indexName, bool isUnique, IndexDef indexDef)
        {
            AddIndex(indexName, isUnique, indexDef.ToString());
        }

        public void AddIndex(string indexName, bool isUnique, params IndexDef[] indexInfo)
        {
            AddIndex(indexName, isUnique, String.Join(", ", indexInfo.Select(ii => ii.ToString())));
        }

        public void RemoveIndex(string indexName)
        {
            Ddl("DROP INDEX {0} ON {1}", indexName, Name);
        }

        private void AddIndex(string indexName, bool isUnique, string columnDefinition)
        {
            if (isUnique)
            {
                Ddl("CREATE UNIQUE INDEX {0} ON {1} ({2})", indexName, Name, columnDefinition);
            }
            else
            {
                Ddl("CREATE INDEX {0} ON {1} ({2})", indexName, Name, columnDefinition);
            }
        }

        private void Ddl(string format, params object[] args)
        {
            _statements.Add(string.Format(format, args));
        }

        public TableMigration Run()
        {
            CreateDestinationTables();

            Logger.Info(string.Format("Applying migrations to table:{0}", Destination));

            using (var transaction = new SqlTransaction(_connection))
            {
                foreach (var migration in _statements)
                {
                    Logger.InfoFormat(string.Format("Applying migration to table:{0} Migration:{1}", Destination,
                        migration));

                    _connection.Execute(migration, transaction: transaction);
                }

                transaction.Commit();
            }

            if (_renameMaps.Any())
            {
                return new TableMigration(_origin, ReadDestination(), _dateTimeStamp, _renameMaps);
            }

            return new TableMigration(_origin, ReadDestination(), _dateTimeStamp);
        }

        private void CreateDestinationTables()
        {
            Logger.Info(string.Format("Creating destination table:{0}", Destination));

            var builder = new TravelAgent(_origin, _connection, _dateTimeStamp);

            builder.PrepareDestination();
        }

        private Table ReadDestination()
        {
            return Table.Parse(_origin.DestinationName, _connection);
        }
    }
}
