﻿using System;
using System.Data;
using System.Data.SqlClient;
using lhm.net.Throttler;
using Serilog;
using Serilog.Configuration;
using YourRootNamespace.Logging;

namespace lhm.net
{
    public class Lhm
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private static string _connectionString;
        private static IDbConnection _connection;

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

        private static IDbConnection Connection
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
                    _connection = new SqlConnection(_connectionString);
                    return _connection;
                }
                return _connection;
            }
        }
    }
}