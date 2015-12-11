using System;
using Xunit;

namespace lhm.net.tests.unit
{
    public class LhmTests
    {
        [Fact]
        public void Should_throw_exception_if_no_connection_is_setup()
        {
            Assert.Throws<Exception>(() => Lhm.ChangeTable("foo", migrator =>
            {
                migrator.AddColumn("bar", "int");
            }));
        }
    }
}