namespace lhm.net.Throttler
{
    public interface IThrottler
    {
        int Stride { get;}

        void Run();
    }
}