using System.Linq;
using Should.Fluent;
using Xunit;

namespace lhm.net.tests.integration
{
    public class AtomicSwitcherTests : IntegrationBase
    {
        public AtomicSwitcherTests()
        {
            CreateTable(Tables.Origin);
            CreateTable(Tables.Destination);
        }

        [Fact]
        public void Should_rename_origin_to_archive()
        {
            var tableMigration = new TableMigration(
                new Table(Tables.Origin),
                new Table(Tables.Destination));

            var sut = new AtomicSwitcher(tableMigration, Connection);

            sut.Run();

            TableExists(Tables.Origin).Should().Be.True();

            ReadTable(tableMigration.ArchiveName)
                .Columns
                .Any(info => info.Name.ToLowerInvariant() == "origin")
                .Should().Be.True();
        }

        [Fact]
        public void Should_rename_destination_to_origin()
        {
            var tableMigration = new TableMigration(
             new Table(Tables.Origin),
             new Table(Tables.Destination));

            var sut = new AtomicSwitcher(tableMigration, Connection);

            sut.Run();

            TableExists(Tables.Destination).Should().Be.False();

            ReadTable(Tables.Origin)
                .Columns
                .Any(info => info.Name.ToLowerInvariant() == "destination");
        }
    }
}