using Should.Fluent;
using Xunit;

namespace lhm.net.tests.unit
{
    public class TableTests
    {
        [Fact]
        public void Should_name_the_destination()
        {
            var sut = new Table("Foo");

            sut.DestinationName.Should()
                .Equal("lhm_Foo");
        }
    }
}