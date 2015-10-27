﻿using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using lhm.net.Logging;

namespace lhm.net
{
    /// <summary>
    ///  Copies existing schema and applies changes using alter on the empty table.
    ///  `run` returns a Migration which can be used for the remaining process.
    /// </summary>
    public class Migrator
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();
        private readonly Table _origin;
        private readonly IDbConnection _connection;
        private readonly List<string> _statements;
        private readonly string _dateTimeStamp;

        public Migrator(Table origin, IDbConnection connection)
        {
            _origin = origin;
            _connection = connection;
            _statements = new List<string>();
            _dateTimeStamp = DateTime.UtcNow.ToString(Constants.DateFormat);
        }

        public string Name
        {
            get { return _origin.DestinationName; }
        }

        public void AddColumn(string columnName, string type)
        {
            Ddl("ALTER TABLE {0} Add {1} {2}", Name, columnName, type);
        }

        public void RemoveColumn(string columnName)
        {
            Ddl("ALTER TABLE {0} DROP COLUMN {1}", Name, columnName);
        }

        private void Ddl(string format, params object[] args)
        {
            _statements.Add(string.Format(format, args));
        }

        public TableMigration Run()
        {
            CreateDestinationTables();

            Logger.Info(string.Format("Applying migrations to table:{0}", Name));

            foreach (var migration in _statements)
            {
                Logger.InfoFormat(string.Format("Applying migration to table:{0} Migration:{1}", Name, migration));

                _connection.Execute(migration);
            }

            return new TableMigration(_origin, ReadDestination(), _dateTimeStamp);
        }

        private void CreateDestinationTables()
        {
            Logger.Info(string.Format("Creating destination table:{0}", Name));

            var builder = new TravelAgent(_origin, _connection, _dateTimeStamp);

            builder.PrepareDestination();
        }

        private Table ReadDestination()
        {
            return Table.Parse(_origin.DestinationName, _connection);
        }
    }
}
