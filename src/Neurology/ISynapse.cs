namespace Autrage.RNN.NET
{
    public interface ISynapse
    {
        double Signal { get; }
        IStimulator Stimulator { get; }
    }
}