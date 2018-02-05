using Autrage.LEX.NET.Serialization;

namespace Autrage.RNN.NET
{
    [DataContract]
    public abstract class Sensor : IStimulator
    {
        #region Properties

        [DataMember]
        public double State { get; private set; }

        #endregion Properties

        #region Methods

        public void Activate() => State = Fetch();

        protected abstract double Fetch();

        #endregion Methods
    }
}