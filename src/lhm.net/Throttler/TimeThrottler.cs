using System.Threading;

namespace lhm.net.Throttler
{
    public class TimeThrottler : IThrottler
    {
        private readonly int _stride;
        private readonly int _delayMiliseconds;

        public TimeThrottler(int stride, int delayMiliseconds)
        {
            _stride = stride;
            _delayMiliseconds = delayMiliseconds;
        }

        public int Stride
        {
            get { return _stride; }
        }

        public void Run()
        {
            Thread.Sleep(_delayMiliseconds);
        }
    }
}