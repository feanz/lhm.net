using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
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
        private readonly IDbConnection _connection;
        private readonly List<string> _statements;
        private List<RenameMap> _renameMaps; 
        private readonly string _dateTimeStamp;

        public Migrator(Table origin, IDbConnection connection)
        {
            _origin = origin;
            _connection = connection;
            _statements = new List<string>();
            _renameMaps = new List<RenameMap>();
            _dateTimeStamp = DateTime.UtcNow.ToString(Constants.DateFormat);
        }

        public string Name
        {
            get { return _origin.DestinationName; }
        }

        public void AddColumn(string columnName, string type)
        {
            Ddl("ALTER TABLE {0} Add {1} {2}", Name, columnName, type);
        }

        public void RemoveColumn(string columnName)
        {
            Ddl("ALTER TABLE {0} DROP COLUMN {1}", Name, columnName);
        }

        public void RenameColumn(string oldColumnName, string newColumnName)
        {
            Ddl("EXEC sp_rename '{0}.{1}', '{2}', 'COLUMN'", Name, oldColumnName, newColumnName);
            _renameMaps.Add(new RenameMap(oldColumnName, newColumnName));
        }

        public void AddIndex(string indexName, bool isUnique, string columnName)
        {
            if (isUnique)
            {
                Ddl("CREATE UNIQUE INDEX {0}_index_{1} ON {2} ({3})", indexName, _dateTimeStamp, Name, columnName);
            }
            else
            {
                Ddl("CREATE INDEX {0}_index_{1} ON {2} ({3})", indexName, _dateTimeStamp, Name, columnName);
            }
        }

        public void AddCompoundIndex(string indexName, bool isUnique, params string[] columnNames)
        {
            AddIndex(indexName, isUnique, String.Join(", ", columnNames));
        }

        private void Ddl(string format, params object[] args)
        {
            _statements.Add(string.Format(format, args));
        }

        public TableMigration Run()
        {
            CreateDestinationTables();

            Logger.Info(string.Format("Applying migrations to table:{0}", Name));

            foreach (var migration in _statements)
            {
                Logger.InfoFormat(string.Format("Applying migration to table:{0} Migration:{1}", Name, migration));

                _connection.Execute(migration);
            }

            if (_renameMaps.Any())
            {
                return new TableMigration(_origin, ReadDestination(), _dateTimeStamp, _renameMaps);
            }

            return new TableMigration(_origin, ReadDestination(), _dateTimeStamp);
        }

        private void CreateDestinationTables()
        {
            Logger.Info(string.Format("Creating destination table:{0}", Name));

            var builder = new TravelAgent(_origin, _connection, _dateTimeStamp);

            builder.PrepareDestination();
        }

        private Table ReadDestination()
        {
            return Table.Parse(_origin.DestinationName, _connection);
        }
    }
}
