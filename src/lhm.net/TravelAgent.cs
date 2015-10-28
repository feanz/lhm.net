using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using Dapper;

namespace lhm.net
{
    public class TravelAgent
    {
        private readonly IDbConnection _connection;
        private readonly string _dateTimeStamp;
        private string _buildScript;

        public TravelAgent(Table origin, IDbConnection connection, string dateTimeStamp)
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
            HandleForiegnKey();
            HandleIndexes();

            _connection.Execute(_buildScript);
        }

        private void HandleForiegnKey()
        {
            _buildScript = Regex.Replace(_buildScript, "WITH CHECK ADD  CONSTRAINT \\[(.*?)\\]", MatchIndexKey);
            _buildScript = Regex.Replace(_buildScript, "CHECK CONSTRAINT \\[(.*?)\\]", MatchIndexKey);
        }

        private void HandleIndexes()
        {
            _buildScript = Regex.Replace(_buildScript, "CREATE UNIQUE NONCLUSTERED INDEX \\[(.*?)\\]", MatchIndexKey);
            _buildScript = Regex.Replace(_buildScript, "CREATE UNIQUE NONCLUSTERED INDEX \\[(.*?)\\].\\[(.*?)\\]", NewTableName);
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

            return full.Replace(tableName, string.Format("{0}_lhm", tableName));
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
                primaryKey.Remove(primaryKey.Length - 19) + _dateTimeStamp :
                string.Format("{0}_{1}", primaryKey, _dateTimeStamp);
            return timeStampedKey;
        }

        private static bool IsKeyAlreadyTimeStamped(string primaryKey)
        {
            if (primaryKey.Length < 19)
            {
                return false;
            }

            var dateTimeStamp = primaryKey.GetLast(19);

            DateTime temp;
            var provider = CultureInfo.InvariantCulture;
            return DateTime.TryParseExact(dateTimeStamp, Constants.DateFormat, provider, DateTimeStyles.None, out temp);
        }
    }
}