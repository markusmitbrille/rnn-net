namespace Autrage.RNN.NET
{
    public abstract class Sensor : IStimulator
    {
        #region Properties

        public double State { get; private set; }

        #endregion Properties

        #region Constructors

        public Sensor()
        {
        }

        #endregion Constructors

        #region Methods

        public void Activate() => State = Fetch();

        protected abstract double Fetch();

        #endregion Methods
    }
}