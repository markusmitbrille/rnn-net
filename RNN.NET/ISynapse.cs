namespace Autrage.RNN.NET
{
    public interface ISynapse
    {
        IStimulator Stimulator { get; }
        double Signal { get; }
    }
}