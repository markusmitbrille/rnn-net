namespace Autrage.RNN.NET
{
    public interface INeuron : IStimuland, IStimulator
    {
        double Bias { get; }
    }
}