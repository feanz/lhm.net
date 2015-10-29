using Should.Fluent;
using Xunit;

namespace lhm.net.tests.unit
{
    public class MigratorTests
    {
        public class Destination
        {
            [Fact]
            public void Should_have_correct_name()
            {
                var sut = new Migrator(new Table("Users"));

                sut.Destination.Should().Equal("Users_lhm");
            }
        }

        public class AddColumn
        {
            [Fact]
            public void Should_add_a_column()
            {
                var sut = new Migrator(new Table("Users"));

                sut.AddColumn("Logins", "int");

                sut.Statements.Should().Contain.One("ALTER TABLE [Users_lhm] Add [Logins] [int]");
            }
        }
    }
}