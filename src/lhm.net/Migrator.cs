using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        /// <summary>
        /// Add column to table
        /// </summary>
        /// <param name="columnName">Name of the column</param>
        /// <param name="type">Sql data type of column</param>
        public void AddColumn(string columnName, string type)
        {
            DdlInternal("ALTER TABLE [{0}] Add [{1}] [{2}]", Destination, columnName, type);
        }

        /// <summary>
        /// Remove column from table
        /// </summary>
        /// <param name="columnName">Name of the column to remove</param>
        public void RemoveColumn(string columnName)
        {
            DdlInternal("ALTER TABLE {0} DROP COLUMN {1}", Destination, columnName);
        }

        /// <summary>
        /// Rename column in table
        /// </summary>
        /// <param name="currentColumnName">The current name of the column</param>
        /// <param name="newColumnName">The name the column should be changed to</param>
        public void RenameColumn(string currentColumnName, string newColumnName)
        {
            DdlInternal("EXEC sp_rename '{0}.{1}', '{2}', 'COLUMN'", Destination, currentColumnName, newColumnName);
            _renameMaps.Add(new RenameMap(currentColumnName, newColumnName));
        }

        /// <summary>
        /// Add an ascending index to column with a default name
        /// </summary>
        /// <param name="columnName">The name of the column to add the index to</param>
        /// <param name="indexName">Optional name of index</param>
        /// <param name="isUnique">Flag to indiciate if the index is unique</param>
        public void AddIndex(string columnName, string indexName = null, bool isUnique = false)
        {
            AddIndex(new List<string> { columnName }, indexName, isUnique);
        }

        /// <summary>
        /// Add index to table
        /// </summary>
        /// <param name="columns">Columns to add index to</param>
        /// <param name="indexName">Optional index name if non provided one will be generated</param>
        /// <param name="isUnique">Optional flag to indiciate if the index is unique</param>
        public void AddIndex(IEnumerable<string> columns, string indexName = null, bool isUnique = false)
        {
            var name = indexName ?? $"IX_{string.Join("_", columns)}";
            AddIndex(name, isUnique, columns.Select(s => new IndexDef(s, IndexOrder.ASC)).ToArray());
        }

        /// <summary>
        /// Add index to table 
        /// </summary>
        /// <param name="indexName">The name of the index to be created</param>
        /// <param name="isUnique">Flag to indiciate if the index is unique</param>
        /// <param name="indexInfo">Definations of the index to create (includes column names and sort order)</param>
        public void AddIndex(string indexName, bool isUnique, params IndexDef[] indexInfo)
        {
            AddIndex(indexName, isUnique, string.Join(", ", indexInfo.Select(ii => ii.ToString())));
        }

        /// <summary>
        /// Remove index from table
        /// </summary>
        /// <param name="indexName">The name of the index to remove</param>
        public void RemoveIndex(string indexName)
        {
            DdlInternal("DROP INDEX {0} ON {1}", indexName, Destination);
        }

        /// <summary>
        /// Custom DDL statements to apply
        /// </summary>
        /// <param name="statement">Custom DDL statement</param>
        /// <param name="args">Format Arguments</param>
        public void Ddl(string statement, params object[] args)
        {
            _statements.Add(string.Format(statement, args));
        }
        
        private void DdlInternal(string format, params object[] args)
        {
            _statements.Add(string.Format(format, args));
        }

        private void AddIndex(string indexName, bool isUnique, string columnDefinition)
        {
            DdlInternal(!isUnique ? "CREATE INDEX {0} ON {1} ({2})" : "CREATE UNIQUE INDEX {0} ON {1} ({2})", indexName,
                Destination, columnDefinition);
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
