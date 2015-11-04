using System.Collections;
using System.Collections.Generic;
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

                var sut = CreateSut(connection);

                sut.Run();

                connection.Verify(
                    lhmConnection =>
                        lhmConnection.Execute(It.Is<string>(sql => sql.Contains("RowNumber > 0 AND RowNumber <= 1")),
                            It.IsAny<object>(),
                            It.IsAny<IDbTransaction>()), Times.Once);
            }

            [Fact]
            public void Should_chunk_the_result_according_to_the_stride_size()
            {
                var connection = new Mock<ILhmConnection>();

                var rowsEffected = new Queue<int>();
                rowsEffected.Enqueue(2);
                rowsEffected.Enqueue(2);
                rowsEffected.Enqueue(0);

                connection.Setup(lhmConnection => lhmConnection.Execute(It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<IDbTransaction>()))
                    .Returns(rowsEffected.Dequeue);

                var strideLength = 2;

                var sut = CreateSut(connection, strideLength);
                
                sut.Run();

                connection.Verify(
                    lhmConnection =>
                        lhmConnection.Execute(It.Is<string>(sql => sql.Contains("RowNumber > 0 AND RowNumber <= 2")),
                            It.IsAny<object>(),
                            It.IsAny<IDbTransaction>()), Times.Once);

                connection.Verify(
                    lhmConnection =>
                        lhmConnection.Execute(It.Is<string>(sql => sql.Contains("RowNumber > 2 AND RowNumber <= 4")),
                            It.IsAny<object>(),
                            It.IsAny<IDbTransaction>()), Times.Once);

                connection.Verify(
                    lhmConnection =>
                        lhmConnection.Execute(It.Is<string>(sql => sql.Contains("RowNumber > 4 AND RowNumber <= 6")),
                            It.IsAny<object>(),
                            It.IsAny<IDbTransaction>()), Times.Once);
            }

            private static Chunker CreateSut(IMock<ILhmConnection> connection, int strideLength = 1)
            {
                return new Chunker(
                    new TableMigration(
                        new Table("foo"),
                        new Table("bar")),
                    connection.Object,
                    new TimeThrottler(strideLength, 10));
            }
        }
    }
}