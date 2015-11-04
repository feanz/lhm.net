using System.Collections.Generic;
using System.Linq;
using Should.Fluent;
using Xunit;

namespace lhm.net.tests.unit
{
    public class IntersectionTests
    {
        public class Common
        {
            [Fact]
            public void Should_not_contain_dropped_columns()
            {
                var originColumns = new List<ColumnInfo>
                {
                    new ColumnInfo {Name = "Droppped"},
                    new ColumnInfo {Name = "Retained"}
                };
                var destinationColumns = new List<ColumnInfo>
                {
                    new ColumnInfo {Name = "Retained"}
                };

                var sut = new Intersection(new Table("Origin", columns: originColumns),
                    new Table("Destination", columns: destinationColumns));

                sut.Common.Should()
                    .Not
                    .Contain
                    .One(map => map.DestinationColumns.Name == "Droppped");
            }

            [Fact]
            public void Should_contain_new_columns()
            {
                var originColumns = new List<ColumnInfo>
                {
                    new ColumnInfo {Name = "Retained"}
                };
                var destinationColumns = new List<ColumnInfo>
                {
                    new ColumnInfo {Name = "Retained"},
                    new ColumnInfo {Name = "Added"}
                };

                var sut = new Intersection(new Table("Origin", columns: originColumns),
                    new Table("Destination", columns: destinationColumns));

                sut.Common.Should()
                    .Not
                    .Contain
                    .One(map => map.DestinationColumns.Name == "Droppped");
            }

            [Fact]
            public void Should_not_contain_renamed_columns()
            {
                var originColumns = new List<ColumnInfo>
                {
                    new ColumnInfo {Name = "Retained"},
                    new ColumnInfo {Name = "Original"}
                };
                var destinationColumns = new List<ColumnInfo>
                {
                    new ColumnInfo {Name = "Retained"},
                    new ColumnInfo {Name = "Renamed"}
                };

                var renameMaps = new List<RenameMap> { new RenameMap("Original", "Renamed") };

                var sut = new Intersection(new Table("Origin", columns: originColumns),
                    new Table("Destination", columns: destinationColumns),
                    renameMaps);

                sut.Common.Should()
                   .Not
                   .Contain
                   .One(map => map.OriginColumns.Name == "Renamed");

                sut.Common.Should()
                    .Not
                    .Contain
                    .One(map => map.DestinationColumns.Name == "Original");
            }

            [Fact]
            public void Should_contain_renamed_column_name()
            {
                var originColumns = new List<ColumnInfo>
                {
                    new ColumnInfo {Name = "Retained"},
                    new ColumnInfo {Name = "Original"}
                };
                var destinationColumns = new List<ColumnInfo>
                {
                    new ColumnInfo {Name = "Retained"},
                    new ColumnInfo {Name = "Renamed"}
                };

                var renameMaps = new List<RenameMap> { new RenameMap("Original", "Renamed") };

                var sut = new Intersection(new Table("Origin", columns: originColumns),
                    new Table("Destination", columns: destinationColumns),
                    renameMaps);

                sut.Common.Should()
                    .Contain
                    .One(map => map.OriginColumns.Name == "Original");

                sut.Common.Should()
                    .Contain
                    .One(map => map.DestinationColumns.Name == "Renamed");
            }
        }

        public class InsertDestinationColumns
        {
            [Fact]
            public void Should_not_have_dropped_columns()
            {
                var originColumns = new List<ColumnInfo>
                    {
                        new ColumnInfo {Name = "Droppped"},
                        new ColumnInfo {Name = "Retained"}
                    };
                var destinationColumns = new List<ColumnInfo>
                    {
                        new ColumnInfo {Name = "Retained"}
                    };

                var sut = new Intersection(new Table("Origin", columns: originColumns),
                    new Table("Destination", columns: destinationColumns));

                sut.DestinationColumns
                    .Should()
                    .Not
                    .Contain("Droppped");
            }
        }

        public class InsertOriginColumns
        {
            [Fact]
            public void Should_not_contain_dropped_columns()
            {
                var originColumns = new List<ColumnInfo>
                    {
                        new ColumnInfo {Name = "Droppped"},
                        new ColumnInfo {Name = "Retained"}
                    };
                var destinationColumns = new List<ColumnInfo>
                    {
                        new ColumnInfo {Name = "Retained"}
                    };

                var sut = new Intersection(new Table("Origin", columns: originColumns),
                    new Table("Destination", columns: destinationColumns));

                sut.OriginColumns
                    .Should()
                    .Not
                    .Contain("Droppped");
            }

            [Fact]
            public void Should_not_contain_added_columns()
            {
                var originColumns = new List<ColumnInfo>
                    {
                        new ColumnInfo {Name = "Retained"}
                    };
                var destinationColumns = new List<ColumnInfo>
                    {
                        new ColumnInfo {Name = "Retained"},
                        new ColumnInfo {Name = "Added"}
                    };

                var sut = new Intersection(new Table("Origin", columns: originColumns),
                    new Table("Destination", columns: destinationColumns));

                sut.OriginColumns
                    .Should()
                    .Not
                    .Contain("Added");
            }
        }
    }
}