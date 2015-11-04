using lhm.net.Throttler;
using Should.Fluent;
using Xunit;

namespace lhm.net.tests.integration
{
    public class ChunkerTests : IntegrationBase
    {
        private readonly TableMigration _migration;
        private readonly Table _destination;

        public ChunkerTests()
        {
            var origin = CreateTable(Tables.Origin);
            _destination = CreateTable(Tables.Destination);
            _migration = new TableMigration(origin, _destination);
        }

        [Fact]
        public void Should_copy_23_rows_from_origin_to_destination_with_time_based_throttle()
        {
            23.Times(n => Execute($"insert into Origin (origin) values ({n*n + 23})"));

            var chunker = new Chunker(_migration, Connection, new TimeThrottler(100));
            chunker.Run();

            CountAll(_destination.Name).Should().Equal(23);
        }
    }
}