using System.Text.RegularExpressions;
using lhm.net.Logging;

namespace lhm.net
{
    /// <summary>
    /// Alter the orgin tables DDL to allow it to be 
    /// </summary>
    public class Targeter
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();
        private readonly Table _origin;
        private readonly ILhmConnection _connection;
        private readonly MigrationDateTimeStamp _migrationDateTimeStamp;
        private string _buildScript;

        public Targeter(Table origin, ILhmConnection connection, MigrationDateTimeStamp migrationDateTimeStamp)
        {
            _origin = origin;
            _connection = connection;
            _migrationDateTimeStamp = migrationDateTimeStamp;
            _buildScript = origin.Ddl;
        }

        public void Run()
        {
            Logger.Info($"Creating destination table:{_origin.DestinationName}");

            HandleCreateTable();
            HandlePrimaryKey();
            HandleAlterTable();
            HandleForeignKey();
            HandleIndexes();

            _connection.Execute(_buildScript);
        }

        private void HandleForeignKey()
        {
            _buildScript = Regex.Replace(_buildScript, "WITH NOCHECK ADD  CONSTRAINT \\[(.*?)\\]", MatchIndexKey);
            _buildScript = Regex.Replace(_buildScript, "WITH CHECK ADD  CONSTRAINT \\[(.*?)\\]", MatchIndexKey);
            _buildScript = Regex.Replace(_buildScript, "CHECK CONSTRAINT \\[(.*?)\\]", MatchIndexKey);
            _buildScript = Regex.Replace(_buildScript, "REFERENCES \\[(.*?)\\].\\[(.*?)\\]", TableName);
        }

        private void HandleIndexes()
        {
            _buildScript = Regex.Replace(_buildScript, "CREATE UNIQUE NONCLUSTERED INDEX \\[(.*?)\\].\\[(.*?)\\]", NewTableName);
            _buildScript = Regex.Replace(_buildScript, "CREATE NONCLUSTERED INDEX \\[(.*?)\\].\\[(.*?)\\]", NewTableName);
        }

        private void HandleCreateTable()
        {
            _buildScript = Regex.Replace(_buildScript, "CREATE TABLE \\[(.*?)\\].\\[(.*?)\\]", NewTableName);
        }

        private void HandlePrimaryKey()
        {
            _buildScript = Regex.Replace(_buildScript, "CONSTRAINT \\[(.*?)\\] PRIMARY KEY CLUSTERED", MatchIndexKey);
        }

        private void HandleAlterTable()
        {
            _buildScript = Regex.Replace(_buildScript, "ALTER TABLE \\[(.*?)\\].\\[(.*?)\\]", NewTableName);
        }

        private string NewTableName(Match m)
        {
            if (m.Groups.Count < 3)
            {
                return m.Value;
            }

            var full = m.Groups[0].Value;
            var tableName = m.Groups[2].Value;

            return full.Replace(tableName, _origin.DestinationName);
        }

        private string TableName(Match m)
        {
            if (m.Groups.Count < 3)
            {
                return m.Value;
            }

            var full = m.Groups[0].Value;
            var tableName = m.Groups[2].Value;

            if (tableName.Contains("lhm_"))
            {
                var splitResult = Regex.Split(tableName, "(.*)_");
                return full.Replace(tableName, $"{splitResult[0]}");
            }
            
            return full;
        }

        private string MatchIndexKey(Match m)
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
            return _migrationDateTimeStamp.Stamp(primaryKey);
        }
    }
}