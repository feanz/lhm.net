using System.CodeDom.Compiler;
using lhm.net.Logging;
using lhm.net.Throttler;
using Microsoft.SqlServer.Management.Common;

namespace lhm.net
{
    /// <summary>
    /// Copies an origin table to an altered destination table. Live activity is
    /// synchronized into the destination table using triggers.
    /// Once the origin and destination tables have converged, origin is archived
    /// and replaced by destination.
    /// </summary>
    public class Invoker
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private readonly Table _origin;
        private readonly ILhmConnection _connection;
        private readonly MigrationDateTimeStamp _migrationDateTimeStamp;

        public Invoker(Table origin, ILhmConnection connection)
        {
            _origin = origin;
            _connection = connection;
            _migrationDateTimeStamp = new MigrationDateTimeStamp();
            Migrator = new Migrator(_origin, _connection, _migrationDateTimeStamp);
        }

        public Migrator Migrator { get; }

        public void Run(MigrationOptions options)
        {
            Logger.Info($"Starting LHM run on table {_origin.Name}");

            options = ConfigureOptions(options);

            var targeter = new Targeter(_origin, _connection, _migrationDateTimeStamp);
            targeter.Run();
            
            var migration = Migrator.Run();

            var entangler = new Entangler(migration, _connection);
            entangler.Run();

            var chunker = new Chunker(migration, _connection, options.Throttler);
            chunker.Run();

            if (options.UseAtomicSwitcher)
            {
                var switcher = new AtomicSwitcher(migration, _connection);
                switcher.Run();
            }

            Logger.Info($"Finished LHM run on table {_origin.DestinationName}");
        }

        private static MigrationOptions ConfigureOptions(MigrationOptions options)
        {
            if (options == null)
            {
                options = new MigrationOptions();
            }

            if (options.Throttler == null)
            {
                if (Lhm.Throttler != null)
                {
                    options.Throttler = Lhm.Throttler;
                }

                //use the default throttler todo create throttler factory
                options.Throttler = new TimeThrottler();
            }

            return options;
        }
    }
}