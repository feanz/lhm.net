using System;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Emit;
using lhm.net.Logging;
using lhm.net.Throttler;

namespace lhm.net
{
    public class Lhm
    {
        private static readonly ILog Logger;

        private static string _connectionString;
        private static ILhmConnection _connection;

        public static IThrottler Throttler { get; set; }

        static Lhm()
        {
            Logger = LogProvider.GetCurrentClassLogger();
        }

        public static void Setup(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static void ChangeTable(string tableName, Action<Migrator> configMigration,
            MigrationOptions options = null)
        {
            try
            {
                var origin = Table.Parse(tableName, Connection);
                var invoker = new Invoker(origin, Connection);
                configMigration.Invoke(invoker.Migrator);
                invoker.Run(options);
            }
            catch (Exception ex)
            {
                Logger.ErrorException(ex.Message, ex);
            }
            finally
            {
                Connection.Close();
            }
        }

        public static void CleanUp(bool run = false)
        {
            var tables = Connection.Query<string>("SELECT TABLE_NAME FROM information_schema.tables WHERE TABLE_NAME Like '%lhm_%'")
                .ToList();

            var triggers = Connection.Query<string>("SELECT sysobjects.name AS trigger_name FROM sysobjects WHERE sysobjects.type = 'TR' AND sysobjects.name like '%_lhm_%'")
                .ToList();

            if (run)
            {
                triggers.ForEach(s =>
                {
                    _connection.Execute($"DROP Trigger [{s}]");
                });
                
                tables.ForEach(s =>
                {
                    _connection.Execute($"DROP TABLE [{s}]");
                });
            }
            else if (!tables.Any() && !triggers.Any())
            {
                LogAndOutput("Everything is clean. Nothing to do.");
            }
            else
            {
                LogAndOutput($"Existing LHM backup tables: \n\n{string.Join("\n", tables)} \n");
                LogAndOutput($"Existing LHM triggers: \n\n{string.Join("\n", triggers)}\n");
                LogAndOutput("Run Lhm.cleanup(true) to drop them all.");
            }
        }

        private static void LogAndOutput(string message)
        {
            Console.WriteLine(message);
            Logger.Info(message);
        }

        private static ILhmConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    if (_connectionString.IsNothing())
                    {
                        throw new Exception("No connection string provided Please call LHM setup");
                    }
                    //todo add connection string validation
                    _connection = new LhmConnection(new SqlConnection(_connectionString));
                    return _connection;
                }
                return _connection;
            }
        }
    }
}