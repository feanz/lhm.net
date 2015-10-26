using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using Dapper;

namespace lhm.net
{
    /// <summary>
    ///  Copies existing schema and applies changes using alter on the empty table.
    ///  `run` returns a Migration which can be used for the remaining process.
    /// </summary>
    public class Migrator
    {
        private readonly Table _origin;
        private readonly IDbConnection _connection;
        private readonly List<string> _statements;
        private List<ColumnMapping> _columnMappings; 
        private readonly string _dateTimeStamp;

        public Migrator(Table origin, IDbConnection connection)
        {
            _origin = origin;
            _connection = connection;
            _statements = new List<string>();
            _columnMappings = new List<ColumnMapping>();
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
            _columnMappings.Add(new ColumnMapping{OldColumnName = oldColumnName, NewColumnName = newColumnName});
        }

        private void Ddl(string format, params object[] args)
        {
            _statements.Add(string.Format(format, args));
        }

        public TableMigration Run()
        {
            CreateDestinationTables();

            foreach (var statement in _statements)
            {
                _connection.Execute(statement);
            }

            return new TableMigration(_origin, ReadDestination(), _dateTimeStamp);
        }

        private void CreateDestinationTables()
        {
            var builder = new TravelAgent(_origin, _connection, _dateTimeStamp);

            builder.PrepareDestination();
        }

        private Table ReadDestination()
        {
            return Table.Parse(_origin.DestinationName, _connection);
        }
    }
}