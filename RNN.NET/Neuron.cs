using System.Collections.Generic;
using System.Linq;

namespace Autrage.RNN.NET
{
    abstract class Neuron : INeuron
    {
        public double Stimulus { get; private set; }
        public double State { get; private set; }

        public double Bias { get; set; }
        public IList<ISynapse> Synapses { get; } = new List<ISynapse>();

        public void Stimulate() => Stimulus = Bias + Synapses.Sum(synapse => synapse.Signal);
        public void Activate() => State = ActivationFunction(Stimulus);

        protected abstract double ActivationFunction(double stimulus);
    }
}
