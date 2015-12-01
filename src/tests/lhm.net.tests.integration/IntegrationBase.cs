using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;

namespace lhm.net.tests.integration
{
    public class IntegrationBase
    {
        protected static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["TestDataBase"].ToString();
        protected readonly ILhmConnection Connection = new LhmConnection(new SqlConnection(ConnectionString));

        protected void Execute(string sql)
        {
            Connection.Execute(sql);
        }

        protected Table CreateTable(string tableName)
        {
            string dropTable = $@"IF EXISTS(SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID('{tableName}'))
                                BEGIN;
                                    DROP TABLE [{tableName}];
                                END;";

            var sql = $"{dropTable}\n{Fixture(tableName)}";

            try
            {
                Connection.Execute(sql);
            }
            catch
            {
                //for some reason this still sometimes give you there is already an item called x
            }

            return ReadTable(tableName);
        }

        protected Table ReadTable(string tableName)
        {
            return Table.Parse(tableName, Connection);
        }

        protected bool TableExists(string tableName)
        {
            return Connection.ExecuteScalar<bool>($@"IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = '{tableName}'))
                    BEGIN
                        select 1
                    END
                    ELSE
                    BEGIN
                        select 0
                    END");
        }

        protected int CountAll(string tableName)
        {
            return Connection.ExecuteScalar<int>($"select count(*) from {tableName}");
        }

        protected int Count(string table, string column, string value)
        {
            return Connection.ExecuteScalar<int>($"select count(*) from {table} where {column} = '{value}'");
        }

        protected bool IndexOnColumn(string tableName, string column, bool isUnique = false)
        {
            return IndexOnColumn(tableName, new List<string> { column }, isUnique);
        }

        private string Fixture(string tableName)
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);
            return File.ReadAllText(Path.Combine(dirPath, "Fixtures", $"{tableName}.sql"));
        }

        protected bool IndexOnColumn(string tableName, List<string> columns, bool isUnique = false)
        {
            var indexInfo = TableIndexes(tableName);

            var cleanColumnName = columns.Select(s => s.ToLowerInvariant());

            var indexesOnOurColumns = indexInfo.Where(info => cleanColumnName.Contains(info.ColumnName.ToLowerInvariant()))
                .ToList();

            var groupByIndexName = indexesOnOurColumns.Where(x => x.IsUnique == isUnique)
                .GroupBy(x => x.IndexName);

            var indexOnColumn = false;
            foreach (var g in groupByIndexName.Where(g => g.Count() == columns.Count))
            {
                indexOnColumn = g.All(x => cleanColumnName.Contains(x.ColumnName.ToLowerInvariant()));
            }

            return indexOnColumn;
        }

        protected bool HasIndex(string tableName, string myIndexName)
        {
            return TableIndexes(tableName).Any(x => x.IndexName.ToLowerInvariant() == myIndexName.ToLowerInvariant());
        }

        private IEnumerable<IndexHelper> TableIndexes(string tableName)
        {
            var sql = $@"SELECT                            
                            IndexName = ind.name,
                            ColumnName = col.name,
                            IsUnique = is_unique
                        FROM 
                             sys.indexes ind 
                        INNER JOIN 
                             sys.index_columns ic ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id 
                        INNER JOIN 
                             sys.columns col ON ic.object_id = col.object_id and ic.column_id = col.column_id 
                        INNER JOIN 
                             sys.tables t ON ind.object_id = t.object_id 
                        WHERE 
                             t.Name = '{
                    tableName
                    }'
                        ORDER BY 
                             t.name, ind.name, ind.index_id, ic.index_column_id";

            var indexInfo = Connection.Query<IndexHelper>(sql)
                .ToList();

            return indexInfo;
        }

        private class IndexHelper
        {
            public string IndexName { get; set; }

            public string ColumnName { get; set; }

            public bool IsUnique { get; set; }
        }
    }
}
