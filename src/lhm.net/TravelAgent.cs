using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace lhm.net
{
    public class TravelAgent
    {
        private readonly ILhmConnection _connection;
        private readonly string _dateTimeStamp;
        private string _buildScript;

        public TravelAgent(Table origin, ILhmConnection connection, string dateTimeStamp)
        {
            _connection = connection;
            _dateTimeStamp = dateTimeStamp;
            _buildScript = origin.Ddl;
        }

        public void PrepareDestination()
        {
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

        private static string NewTableName(Match m)
        {
            if (m.Groups.Count < 3)
            {
                return m.Value;
            }

            var full = m.Groups[0].Value;
            var tableName = m.Groups[2].Value;

            return full.Replace(tableName, $"{tableName}_lhm");
        }

        private string TableName(Match m)
        {
            if (m.Groups.Count < 3)
            {
                return m.Value;
            }

            var full = m.Groups[0].Value;
            var tableName = m.Groups[2].Value;

            if (tableName.Contains("_lhm"))
            {
                var splitResult = Regex.Split(tableName, "_(.*)");
                return full.Replace(tableName, $"{splitResult[0]}");
            }
            
            return full.Replace(tableName, string.Format("{0}", tableName));
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
            var timeStampedKey = IsKeyAlreadyTimeStamped(primaryKey) ?
                primaryKey.Remove(primaryKey.Length - Constants.DateTimeStampLength) + _dateTimeStamp :
                $"{primaryKey}_{_dateTimeStamp}";
            return timeStampedKey;
        }

        private static bool IsKeyAlreadyTimeStamped(string primaryKey)
        {
            if (primaryKey.Length < Constants.DateTimeStampLength)
            {
                return false;
            }

            var dateTimeStamp = primaryKey.GetLast(Constants.DateTimeStampLength);

            DateTime temp;
            var provider = CultureInfo.InvariantCulture;
            return DateTime.TryParseExact(dateTimeStamp, Constants.DateTimeStampFormat, provider, DateTimeStyles.None, out temp);
        }
    }
}