namespace Autrage.RNN.NET
{
    public interface INeuron : IStimuland, IStimulator
    {
        #region Properties

        double Bias { get; }

        #endregion Properties
    }
}