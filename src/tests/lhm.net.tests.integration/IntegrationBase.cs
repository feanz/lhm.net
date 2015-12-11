using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
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

        private string Fixture(string tableName)
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);
            return File.ReadAllText(Path.Combine(dirPath, "Fixtures", $"{tableName}.sql"));
        }
    }
}
