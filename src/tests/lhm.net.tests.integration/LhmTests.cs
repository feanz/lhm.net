using System;
using System.Collections.Generic;
using System.Linq;
using Should.Fluent;
using Xunit;

namespace lhm.net.tests.integration
{
    public class LhmTests : IntegrationBase, IDisposable
    {
        public LhmTests()
        {
            Lhm.Setup(ConnectionString);
            CreateTable("Users");
        }

        [Fact]
        public void Should_add_a_column()
        {
            Lhm.ChangeTable(Tables.Users, migrator =>
            {
                migrator.AddColumn("Login", "int");
            });

            ReadTable(Tables.Users)
                .Columns
                .Any(info => info.Name.ToLowerInvariant() == "login")
                .Should().Be.True();
        }

        [Fact]
        public void Should_copy_all_rows()
        {
            11.Times(n => Execute($"insert into users (application, username, reference, createdat) values ('application{n}', 'username{n}', {n}, getdate())"));

            Lhm.ChangeTable(Tables.Users, migrator =>
            {
                migrator.AddColumn("Login", "int");
            });

            CountAll(Tables.Users).Should().Equal(11);
        }

        [Fact]
        public void Should_remove_a_column()
        {
            Lhm.ChangeTable(Tables.Users, migrator =>
            {
                migrator.RemoveColumn("comment");
            });

            ReadTable(Tables.Users)
              .Columns
              .Any(info => info.Name.ToLowerInvariant() == "comment")
              .Should().Be.False();
        }

        [Fact]
        public void Should_add_an_index()
        {
            Lhm.ChangeTable(Tables.Users, migrator =>
            {
                migrator.AddIndex("reference");
            });

            IndexOnColumn(Tables.Users, "reference").Should().Be.True();
        }

        [Fact]
        public void Should_add_an_to_multiple_columns()
        {
            Lhm.ChangeTable(Tables.Users, migrator =>
            {
                migrator.AddIndex(new List<string>
                {
                    "username",
                    "createdat"
                });
            });

            IndexOnColumn(Tables.Users, new List<string>
            {
                "username",
                "createdat"
            }).Should().Be.True();
        }


        [Fact]
        public void Should_add_an_index_with_custom_name()
        {
            var myIndexName = "my_index_name";

            Lhm.ChangeTable(Tables.Users, migrator =>
            {
                migrator.AddIndex("reference", myIndexName);
            });

            HasIndex(Tables.Users, myIndexName).Should().Be.True();
        }

        [Fact]
        public void Should_add_a_unique_index()
        {
            Lhm.ChangeTable(Tables.Users, migrator =>
            {
                migrator.AddIndex("reference", isUnique: true);
            });

            IndexOnColumn(Tables.Users, "reference", isUnique: true).Should().Be.True();
        }

        [Fact]
        public void Should_remnove_an_index()
        {
            Lhm.ChangeTable(Tables.Users, migrator =>
            {
                migrator.RemoveIndex("IX_Username");
            });

            IndexOnColumn(Tables.Users, "username").Should().Be.False();
        }

        [Fact]
        public void Should_rename_a_column()
        {
            Lhm.ChangeTable(Tables.Users, migrator =>
            {
                migrator.RenameColumn("description", "extrawords");
            });

            var table = ReadTable(Tables.Users);

            table.Columns
                .Count(x => x.Name.ToLowerInvariant() == "description")
                .Should().Equal(0);

            table.Columns
                .Count(x => x.Name.ToLowerInvariant() == "extrawords")
                .Should().Equal(1);
        }

        [Fact]
        public void Should_apply_a_dll_statement()
        {
            Lhm.ChangeTable(Tables.Users, migrator =>
            {
                migrator.Ddl("alter table {0} add flag tinyint", migrator.Destination);
            });

            var table = ReadTable(Tables.Users);

            table.Columns
                .Single(x => x.Name.ToLowerInvariant() == "flag")
                .Name.Should().Equal("flag");

            table.Columns
                .Single(x => x.Name.ToLowerInvariant() == "flag")
                .DataType.Should().Equal("tinyint");
        }

        public void Dispose()
        {
            Lhm.CleanUp(true);
        }
    }
}