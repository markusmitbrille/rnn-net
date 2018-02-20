using Autrage.LEX.NET.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace Autrage.RNN.NET
{
    [DataContract]
    internal abstract class Neuron : INeuron
    {
        [DataMember]
        private double stimulus;

        [DataMember]
        public double State { get; private set; }

        [DataMember]
        public double Bias { get; set; }

        [DataMember]
        public IList<ISynapse> Synapses { get; private set; } = new List<ISynapse>();

        public void Activate() => State = ActivationFunction(stimulus);

        public void Stimulate() => stimulus = Bias + Synapses.Sum(synapse => synapse.Signal);

        protected abstract double ActivationFunction(double stimulus);
    }
}