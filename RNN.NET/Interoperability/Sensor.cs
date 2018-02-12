using Autrage.LEX.NET.Serialization;

namespace Autrage.RNN.NET
{
    [DataContract]
    public abstract class Sensor : IStimulator
    {
        [DataMember]
        public NeuralNetwork Network { get; internal set; }

        [DataMember]
        public double State { get; private set; }

        public void Activate() => State = Fetch();

        protected abstract double Fetch();
    }
}