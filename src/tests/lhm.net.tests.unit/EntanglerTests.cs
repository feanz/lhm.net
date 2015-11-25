using System.Collections.Generic;
using System.Linq;
using Moq;
using Should.Fluent;
using Xunit;

namespace lhm.net.tests.unit
{
    public class EntanglerTests : TestBase
    {
        [Fact]
        public void Should_create_insert_trigger_to_desination_table()
        {
            var sut = CreateSut();

            var ddl = @"CREATE TRIGGER [origin_Insert_lhm_2015_01_01_01_01_01_111] ON [origin] 
                        AFTER INSERT 
                        AS 
                        BEGIN 
                            SET IDENTITY_INSERT [destination] ON 
                            Insert into destination ([info], [tags]) select [info], [tags] from inserted 
                        END";

            sut.Entanglers.Any(s => Strip(s) == Strip(ddl))
                .Should().Be.True();
        }

        [Fact]
        public void Should_create_update_trigger_to_desination_table()
        {
            var sut = CreateSut();

            var ddl = @"CREATE TRIGGER [origin_Update_lhm_2015_01_01_01_01_01_111] ON [origin]
                        AFTER Update
                        AS 
						BEGIN
                            Update [destination] SET
						    [destination].[info] = INSERTED.info, 
                            [destination].[tags] = INSERTED.tags
                            FROM [destination]
                            INNER JOIN INSERTED ON [destination].[Id] = INSERTED.[Id]
                        END";

            sut.Entanglers.Any(s => Strip(s) == Strip(ddl))
                .Should().Be.True();
        }

        [Fact]
        public void Should_create_delete_trigger_to_desination_table()
        {
            var sut = CreateSut();

            var ddl = @"CREATE TRIGGER [origin_Delete_lhm_2015_01_01_01_01_01_111] ON [origin] 
                           AFTER DELETE 
                           AS 
                           BEGIN 
                               DELETE FROM [destination] 
                               WHERE Id IN (SELECT Id FROM DELETED) 
                           END";

            sut.Entanglers.Any(s => Strip(s) == Strip(ddl))
              .Should().Be.True();
        }

        private static Entangler CreateSut()
        {
            var connection = new Mock<ILhmConnection>();

            var sut = new Entangler(new TableMigration(
                new Table("origin", columns: new List<ColumnInfo>
                {
                    new ColumnInfo {Name = "info"},
                    new ColumnInfo {Name = "tags"}
                }),
                new Table("destination", columns: new List<ColumnInfo>
                {
                    new ColumnInfo {Name = "info"},
                    new ColumnInfo {Name = "tags"}
                }),
                "2015_01_01_01_01_01_111"
                ),
                connection.Object);
            return sut;
        }
    }
}