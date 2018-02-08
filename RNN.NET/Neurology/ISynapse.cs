namespace Autrage.RNN.NET
{
    public interface ISynapse
    {
        #region Properties

        double Signal { get; }
        IStimulator Stimulator { get; }

        #endregion Properties
    }
}