namespace Autrage.RNN.NET
{
    public abstract class Sensor : IStimulator
    {
        #region Constructors

        public Sensor()
        {
        }

        #endregion Constructors

        #region Properties

        public double State { get; private set; }

        #endregion Properties

        #region Methods

        public void Activate() => State = Fetch();

        protected abstract double Fetch();

        #endregion Methods
    }
}