using System.Threading;

namespace lhm.net.Throttler
{
    public class TimeThrottler : IThrottler
    {
        private readonly int _delayMiliseconds;

        public TimeThrottler(int stride, int delayMiliseconds = 100)
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