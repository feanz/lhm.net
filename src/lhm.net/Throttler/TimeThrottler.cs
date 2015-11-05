using System.Threading;

namespace lhm.net.Throttler
{
    /// <summary>
    /// Throttle action based on a configurable delay and batch size
    /// </summary>
    public class TimeThrottler : IThrottler
    {
        private readonly int _delayMiliseconds;

        public TimeThrottler(int stride = 40000, int delayMiliseconds = 100)
        {
            Stride = stride;
            _delayMiliseconds = delayMiliseconds;
        }

        public int Stride { get; }

        public void Run()
        {
            Thread.Sleep(_delayMiliseconds);
        }
    }
}