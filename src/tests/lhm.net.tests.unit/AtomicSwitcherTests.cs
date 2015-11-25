using System.Collections.Generic;
using System.Data;
using Moq;
using Xunit;

namespace lhm.net.tests.unit
{
    public class AtomicSwitcherTests : TestBase
    {
        [Fact]
        public void Should_perform_single_atomic_switch()
        {
            var connection = new Mock<ILhmConnection>();
            var sut = CreateSut(connection);

            sut.Run();

            var ddl = @"DECLARE @TranName VARCHAR(20);
                        SELECT @TranName = 'LHM_Rename_Table';
                        BEGIN TRANSACTION @TranName;

                        exec sp_rename[origin], [lhm_2015_01_01_01_01_01_111_origin];
                        exec sp_rename[destination], [origin]

                        COMMIT TRANSACTION @TranName;";

            connection.Verify(c => c.Execute(It.Is<string>(sql => EqualIgnoringWhiteSpace(sql, ddl)),
                It.IsAny<object>(),
                It.IsAny<IDbTransaction>()), Times.Once);
        }

        [Fact]
        public void Should_drop_any_foreign_keys_before_switch()
        {
            var connection = new Mock<ILhmConnection>();
            var sut = CreateSut(connection);

            connection.Setup(c => c.Query<FKeyInfo>(It.Is<string>(s => s == "EXEC sp_fkeys 'origin'"),
                It.IsAny<object>()))
                .Returns(new List<FKeyInfo>
                {
                    new FKeyInfo { FK_Name = "FKey", FKTable_Name = "FKeyTable"}
                }).Verifiable();

            sut.Run();

            var ddl = @"ALTER TABLE [FKeyTable] DROP CONSTRAINT [FKey]";

            connection.Verify(c => c.Execute(It.Is<string>(sql => sql.Contains(ddl)),
                It.IsAny<object>(),
                It.IsAny<IDbTransaction>()), 
                Times.Once);
        }

        [Fact]
        public void Should_add_any_foreign_keys_constraints_after_switch()
        {
            var connection = new Mock<ILhmConnection>();
            var sut = CreateSut(connection);

            connection.Setup(c => c.Query<FKeyInfo>(It.Is<string>(s => s == "EXEC sp_fkeys 'origin'"),
                It.IsAny<object>()))
                .Returns(new List<FKeyInfo>
                {
                    new FKeyInfo { FK_Name = "FKey", FKTable_Name = "FKeyTable", FKColumn_Name = "FKey", PKTable_Name = "origin", PKColumn_Name = "id"}
                }).Verifiable();

            sut.Run();

            var ddl = @"ALTER TABLE [FKeyTable]  WITH CHECK ADD  CONSTRAINT [FKey] FOREIGN KEY([FKey])
                                REFERENCES [origin] ([id])";

            connection.Verify(c => c.Execute(It.Is<string>(sql => Strip(sql).Contains(Strip(ddl))),
                It.IsAny<object>(),
                It.IsAny<IDbTransaction>()), 
                Times.Once);
        }

        private static AtomicSwitcher CreateSut(Mock<ILhmConnection> connection)
        {
            var sut = new AtomicSwitcher(new TableMigration(
                new Table("origin"),
                new Table("destination"),
                new MigrationDateTimeStamp("2015_01_01_01_01_01_111")), connection.Object);
            return sut;
        }
    }
}