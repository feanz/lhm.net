using System.Linq;
using Should.Fluent;
using Xunit;

namespace lhm.net.tests.integration
{
    public class TableTests : IntegrationBase
    {
        private readonly Table _table;

        public TableTests()
        {
            _table = CreateTable(Tables.Users);
        }

        [Fact]
        public void Should_parse_table_name()
        {
            _table.Name.Should().Equal("Users");
        }

        [Fact]
        public void Should_parse_primary_key()
        {
            _table.PrimaryKey.Should().Equal("ID");
        }

        [Fact]
        public void Should_parse_column_types()
        {
            _table.Columns.Single(info => info.Name == "Username").DataType
                .Should().Equal("nvarchar");
        }

        [Fact]
        public void Should_parse_column_meta_data()
        {
            _table.Columns.Single(info => info.Name == "Username").IsNullable
                .Should().Be.False();
        }
    }
}