using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace lhm.net
{
    public class Table
    {
        private readonly string _name;
        private readonly string _pk;
        private readonly string _ddl;

        private readonly List<ColumnInfo> _columns;

        public Table(string name, string pk = "Id", List<ColumnInfo> columns = null, string ddl = null)
        {
            _name = name;
            _columns = columns ?? new List<ColumnInfo>();
            _pk = pk;
            _ddl = ddl;
        }

        public string DestinationName
        {
            get { return string.Format("{0}_lhm", _name); }
        }

        public string Ddl
        {
            get { return _ddl; }
        }

        public string Name
        {
            get { return _name; }
        }

        public List<ColumnInfo> Columns
        {
            get { return _columns; }
        }

        public string PrimaryKey
        {
            get { return _pk; }
        }

        public static Table Parse(string tableName, ILhmConnection connection)
        {
            return new Parser(tableName, connection).Parse();
        }

        private class Parser
        {
            private readonly string _tableName;
            private readonly ILhmConnection _connection;

            public Parser(string tableName, ILhmConnection connection)
            {
                _tableName = tableName;
                _connection = connection;
            }

            public Table Parse()
            {
                var indiciesInformation = ReadIndicesInformation();

                var table = new Table(_tableName, ReadPrimaryKey(indiciesInformation), ReadColumnInformation(), BuildDdl());

                return table;
            }

            private string BuildDdl()
            {
                //todo possible schema parameter not just hard code 'dbo' and way to not have to cast to sql connection
                var commands = new Server(new ServerConnection((SqlConnection)_connection.DbConnection))
                    .Databases[_connection.DbConnection.Database]
                    .Tables[_tableName, "dbo"]
                    .Script(new ScriptingOptions
                    {
                        SchemaQualify = true,
                        DriAll = true,
                        Indexes = true
                    })
                    .Cast<string>()
                    .Select(s => s + "\n")
                    .ToList();


                return string.Join("\n", commands);
            }

            private List<IndexInfo> ReadIndicesInformation()
            {
                var sql = @"SELECT c.COLUMN_NAME as Name, 
                                i.is_primary_key as IsPrimaryKey
                                FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE as c
                                INNER JOIN sys.indexes as i on c.CONSTRAINT_NAME = i.Name
                                WHERE TABLE_NAME = @table";

                return _connection.Query<IndexInfo>(sql, new { table = _tableName })
                    .ToList();
            }

            private static string ReadPrimaryKey(IEnumerable<IndexInfo> indiciesInformation)
            {
                return indiciesInformation.Single(info => info.IsPrimaryKey).Name;
            }

            private List<ColumnInfo> ReadColumnInformation()
            {
                var sql = @"SELECT Table_Catalog as Catalog,                            
                                Column_Name as Name,   
							    Table_Schema as [Schema],
                                Data_Type as DataType,
							    CASE IS_NULLABLE
								    WHEN 'yes' THEN 1 
								    WHEN 'no' THEN 0
							    END as IsNullable,
                                Character_Maximum_Length as [MaxLength],
                                Ordinal_Position OrdinalPostion,
                                COLUMNPROPERTY(object_id(TABLE_NAME), COLUMN_NAME, 'IsIdentity') as IsIdentity
                            FROM information_schema.columns 
                            WHERE table_name = @table                            
                            ORDER BY Ordinal_Position";

                return _connection.Query<ColumnInfo>(sql, new { table = _tableName })
                    .ToList();
            }
        }
    }
}