using System.Collections.Generic;
using System.Linq;

namespace Autrage.RNN.NET
{
    abstract class Neuron : INeuron
    {
        private double stimulus;
        public double Stimulus => stimulus;

        private double state;
        public double State => state;

        public double Bias { get; set; }
        public IList<ISynapse> Synapses { get; } = new List<ISynapse>();

        public void Stimulate() => stimulus = Bias + Synapses.Sum(synapse => synapse.Signal);
        public void Activate() => state = ActivationFunction(stimulus);

        protected abstract double ActivationFunction(double stimulus);
    }
}
