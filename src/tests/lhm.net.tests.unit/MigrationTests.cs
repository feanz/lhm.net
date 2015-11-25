using Should.Fluent;
using Xunit;

namespace lhm.net.tests.unit
{
    public class TableMigrationTests
    {
        [Fact]
        public void Should_name_the_archive()
        {
            var sut = new TableMigration(new Table("Origin"), new Table("Destination"));

            sut.ArchiveName.Should()
                .Contain("lhm_");

            sut.ArchiveName.Should()
                .Contain("Origin");
        }

        [Fact]
        public void Should_name_the_archive_with_provided_timeStamp()
        {
            var migrationDateTime = "2015_01_01_15_12_14_876";
            var sut = new TableMigration(new Table("Origin"), new Table("Destination"), new MigrationDateTimeStamp(migrationDateTime));

            sut.ArchiveName.Should()
                .Contain($"lhm_{migrationDateTime}_Origin");
        }

        [Fact]
        public void Should_limit_the_name_to_128_characters()
        {
            var sut = new TableMigration(new Table("a_very_very_very_very_very_very_very_very_very_very_very_long_table_name_that_should_make_the_LHMA_table_go_over_128_chars"), new Table("lhm_Foo"));

            sut.ArchiveName.Length.Should().Equal(128);
        }
    }
}