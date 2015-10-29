using lhm.net.Logging;
using lhm.net.Throttler;

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

        private readonly ILhmConnection _connection;
        private readonly Migrator _migrator;

        public Invoker(Table origin, ILhmConnection connection)
        {
            _connection = connection;
            _migrator = new Migrator(origin, connection);
        }

        public Migrator Migrator
        {
            get { return _migrator; }
        }

        public void Run(MigrationOptions options)
        {
            Logger.Info("Starting LHM run on table " + _migrator.Destination);

            options = ConfigureOptions(options);

            var migration = Migrator.Run();

            var entangler = new Entangler(migration, _connection);
            entangler.Run();

            var chunker = new Chunker(migration, _connection, options);
            chunker.Run();

            if (options.UseAtomicSwitcher)
            {
                var switcher = new AtomicSwitcher(migration, _connection);
                switcher.Run();
            }
            else
            {
                //todo create alternate switcher
            }

            Logger.Info("Finished LHM run on table " + _migrator.Destination);
        }

        private MigrationOptions ConfigureOptions(MigrationOptions options)
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
                options.Throttler = new TimeThrottler(10, 100);
            }

            return options;
        }
    }
}