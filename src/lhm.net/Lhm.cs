using System;
using System.Data.SqlClient;
using System.Linq;
using lhm.net.Logging;
using lhm.net.Throttler;
using Serilog;

namespace lhm.net
{
    public class Lhm
    {
        private static readonly ILog Logger;

        private static string _connectionString;
        private static ILhmConnection _connection;

        private static IThrottler _defaultThrottler;

        public static IThrottler Throttler
        {
            get { return _defaultThrottler; }
        }

        static Lhm()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo
                .LiterateConsole(outputTemplate: "{Timestamp:HH:MM} [{Level}] ({Name:l}){NewLine} {Message}{NewLine}{Exception}")
                .CreateLogger();

            Logger = LogProvider.GetCurrentClassLogger();
        }

        public static void Setup(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static void SetupThrottler(IThrottler throttler)
        {
            _defaultThrottler = throttler;
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
            var tables = Connection.Query<string>("SELECT TABLE_NAME FROM information_schema.tables WHERE TABLE_NAME Like '%_lhm_%'")
                .ToList();

            var triggers = Connection.Query<string>("SELECT sysobjects.name AS trigger_name FROM sysobjects WHERE sysobjects.type = 'TR' AND sysobjects.name like '%_lhm_%'")
                .ToList();

            if (run)
            {
                triggers.ForEach(s =>
                {
                    _connection.Execute(string.Format("DROP Trigger [{0}]", s));
                });

                tables.ForEach(s =>
                {
                    _connection.Execute(string.Format("DROP TABLE [{0}]", s));
                });
            }
            else if (!tables.Any() && !triggers.Any())
            {
                Logger.Info("Everything is clean. Nothing to do.");
            }
            else
            {
                Logger.Info(string.Format("Existing LHM backup tables: \n\n{0} \n", string.Join("\n", tables)));
                Logger.Info(string.Format("Existing LHM triggers: \n\n{0}\n", string.Join("\n", triggers)));
                Logger.Info("Run Lhm.cleanup(true) to drop them all.");
            }
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