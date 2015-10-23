using lhm.net.Throttler;

namespace lhm.net
{
    public class MigrationOptions
    {
        public MigrationOptions()
        {
            UseAtomicSwitcher = true;
        }

        public bool UseAtomicSwitcher { get; set; }

        public IThrottler Throttler { get; set; }
    }
}