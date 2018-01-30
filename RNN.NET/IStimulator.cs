namespace Autrage.RNN.NET
{
    public interface IStimulator
    {
        #region Properties

        double State { get; }

        #endregion Properties

        #region Methods

        void Activate();

        #endregion Methods
    }
}