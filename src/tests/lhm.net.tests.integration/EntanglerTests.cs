using Should.Fluent;
using Xunit;

namespace lhm.net.tests.integration
{
    public class EntanglerTests : IntegrationBase
    {
        public EntanglerTests()
        {
            var origin = CreateTable("origin");
            var destination = CreateTable("destination");

            var migration = new TableMigration(origin, destination);
            var entangler = new Entangler(migration, Connection);
            entangler.Run();
        }

        [Fact]
        public void Should_replay_insert_from_origin_to_destination()
        {
            Execute("insert into origin (common) values ('inserted')");

            Count("destination", "common", "inserted").Should().Equal(1);
        }
        
        [Fact]
        public void Should_replay_deletes_from_origin_to_destination()
        {
            Execute("insert into origin (common) values ('inserted')");

            Execute("delete from origin where common = 'inserted'");

            Count("destination", "common", "inserted").Should().Equal(0);
        }

        [Fact]
        public void Should_replay_update_from_origin_to_destination()
        {
            Execute("insert into origin (common) values ('inserted')");

            Execute("update origin set common = 'updated'");

            Count("destination", "common", "inserted").Should().Equal(0);
            Count("destination", "common", "updated").Should().Equal(1);
        }
    }
}