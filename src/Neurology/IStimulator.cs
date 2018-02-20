namespace Autrage.RNN.NET
{
    public interface IStimulator
    {
        double State { get; }

        void Activate();
    }
}