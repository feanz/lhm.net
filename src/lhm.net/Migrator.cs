using System;
using System.Collections.Generic;
using System.Linq;
using lhm.net.Logging;

namespace lhm.net
{
    /// <summary>
    ///  Copies existing schema and applies changes using alter on the empty table.
    ///  `run` returns a TableMigration which can be used for the remaining process.
    /// </summary>
    public class Migrator
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();
        private readonly Table _origin;
        private readonly ILhmConnection _connection;
        private readonly List<string> _statements;
        private readonly List<RenameMap> _renameMaps;
        private readonly MigrationDateTimeStamp _migrationDateTimeStamp;

        public Migrator(Table origin, ILhmConnection connection = null, MigrationDateTimeStamp migrationDateTimeStamp = null)
        {
            _origin = origin;
            _connection = connection;
            _statements = new List<string>();
            _renameMaps = new List<RenameMap>();
            _migrationDateTimeStamp = migrationDateTimeStamp ?? new MigrationDateTimeStamp();
        }

        public string Source => _origin.Name;

        public string Destination => _origin.DestinationName;

        public List<string> Statements => _statements;

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
            AddIndex(indexName, isUnique, string.Join(", ", indexInfo.Select(ii => ii.ToString())));
        }

        public void RemoveIndex(string indexName)
        {
            Ddl("DROP INDEX {0} ON {1}", indexName, Destination);
        }

        private void AddIndex(string indexName, bool isUnique, string columnDefinition)
        {
            Ddl(!isUnique ? "CREATE INDEX {0} ON {1} ({2})" : "CREATE UNIQUE INDEX {0} ON {1} ({2})", indexName,
                Destination, columnDefinition);
        }

        private void Ddl(string format, params object[] args)
        {
            _statements.Add(string.Format(format, args));
        }

        public TableMigration Run()
        {
            Logger.Info($"Applying migrations to table:{Destination}");

            using (var transaction = _connection.BeginTransaction())
            {
                foreach (var migration in _statements)
                {
                    Logger.InfoFormat($"Applying migration to table:{Destination} Migration:{migration}");

                    _connection.Execute(migration, transaction: transaction);
                }

                transaction.Commit();
            }

            return new TableMigration(_origin, ReadDestination(), _migrationDateTimeStamp, _renameMaps);
        }

        private Table ReadDestination()
        {
            return Table.Parse(_origin.DestinationName, _connection);
        }
    }
}
