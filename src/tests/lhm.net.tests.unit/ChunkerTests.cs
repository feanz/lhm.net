using System.Data;
using lhm.net.Throttler;
using Moq;
using Xunit;

namespace lhm.net.tests.unit
{
    public class ChunkerTests
    {
        public class Run
        {
            [Fact]
            public void Should_correctly_copy_single_record_table()
            {
                var connection = new Mock<ILhmConnection>();

                var sut = new Chunker(
                    new TableMigration(
                        new Table("foo"),
                        new Table("bar")),
                        connection.Object,
                        new TimeThrottler(1, 10));

                sut.Run();

                connection.Verify(lhmConnection => lhmConnection.Execute(It.Is<string>(sql => sql.Contains("OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;")),
                    It.IsAny<object>(),
                    It.IsAny<IDbTransaction>()));
            }
        }
    }
}