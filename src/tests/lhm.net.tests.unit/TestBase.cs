using System.Text.RegularExpressions;

namespace lhm.net.tests.unit
{
    public class TestBase
    {
        protected static string Strip(string value)
        {
            var result = Regex.Replace(value, @"\t|\n|\r|\s+", "");
            return result;
        }
    }
}