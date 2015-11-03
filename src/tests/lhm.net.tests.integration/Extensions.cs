using System;

namespace lhm.net.tests.integration
{
    public static class Extensions
    {
        public static void Times(this int times, Action<int> action)
        {
            for (int i = 0; i < times; i++)
            {
                action(i);
            }
        }
    }
}