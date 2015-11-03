using System;
using System.IO;
using Should.Fluent;
using Xunit;

namespace lhm.net.tests.integration
{
    public class CleanUpTests : IntegrationBase, IDisposable
    {
        public CleanUpTests()
        {
            CreateTable(Tables.Origin);
            Lhm.Setup(ConnectionString);
            Lhm.ChangeTable(Tables.Origin, migrator =>
            {
                migrator.AddColumn("NewColumn","int");
            });
        }

        [Fact]
        public void Should_show_archive_tables()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                Lhm.CleanUp();

                var output = sw.ToString();

                output.Should().Contain("Existing LHM backup tables");
                output.Should().Contain("Origin_lhm");
            }
        }

        [Fact]

        public void Should_cleanup_archive_tables()
        {
            Lhm.CleanUp(true);

            var countArchiveTables = "SELECT COUNT(*) FROM information_schema.tables WHERE TABLE_NAME Like '%_lhm_%'";

            Connection.ExecuteScalar<int>(countArchiveTables)
                .Should()
                .Equal(0);
        }

        public void Dispose()
        {
            Lhm.CleanUp(true);
        }
    }
}