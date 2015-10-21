using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
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
        private readonly string _dateTimeStamp;
        private readonly string _dateFormat = "yyyy_M_dd_hh_mm_ss";

        public Migrator(Table origin, IDbConnection connection)
        {
            _origin = origin;
            _connection = connection;
            _statements = new List<string>();
            _dateTimeStamp = DateTime.UtcNow.ToString(_dateFormat);
        }

        public string Name
        {
            get { return _origin.DestinationName; }
        }

        public void AddColumn(string columnName, string type)
        {
            Ddl("ALTER TABLE {0} Add {1} {2}", Name, columnName, type);
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
            var statement = BuildDestinationStatement();

            _connection.Execute(statement);
        }

        private string BuildDestinationStatement()
        {
            //todo move into it own statement builder class, name the regex groups
            var statement = Regex.Replace(_origin.Ddl, "CREATE TABLE \\[(.*?)\\].\\[(.*?)\\]", NewTableName);

            statement = Regex.Replace(statement, "CONSTRAINT \\[(.*?)\\] PRIMARY KEY CLUSTERED", MatchPrimaryKey);

            return statement;
        }

        public static string NewTableName(Match m)
        {
            if (m.Groups.Count < 3)
            {
                return m.Value;
            }

            var full = m.Groups[0].Value;
            var tableName = m.Groups[2].Value;

            return full.Replace(tableName, string.Format("{0}_lhm", tableName));
        }

        public string MatchPrimaryKey(Match m)
        {
            if (m.Groups.Count < 2)
            {
                return m.Value;
            }

            var full = m.Groups[0].Value;
            var primaryKey = m.Groups[1].Value;

            var timeStampedKey = CreateTimeStampedKey(primaryKey);

            return full.Replace(primaryKey, timeStampedKey);
        }

        private string CreateTimeStampedKey(string primaryKey)
        {
            var timeStampedKey = IsKeyAlreadyTimeStamped(primaryKey) ?
                primaryKey.Remove(primaryKey.Length - 19) + _dateTimeStamp :
                string.Format("{0}_{1}", primaryKey, _dateTimeStamp);
            return timeStampedKey;
        }

        private bool IsKeyAlreadyTimeStamped(string primaryKey)
        {
            if (primaryKey.Length < 19)
            {
                return false;
            }

            var dateTimeStamp = primaryKey.GetLast(19);

            DateTime temp;
            var provider = CultureInfo.InvariantCulture;
            return DateTime.TryParseExact(dateTimeStamp, _dateFormat, provider, DateTimeStyles.None, out temp);
        }

        private Table ReadDestination()
        {
            return Table.Parse(_origin.DestinationName, _connection);
        }
    }
}