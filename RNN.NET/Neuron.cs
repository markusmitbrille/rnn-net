using System.Collections.Generic;
using System.Linq;

namespace Autrage.RNN.NET
{
    internal abstract class Neuron : INeuron
    {
        #region Properties

        public double Bias { get; set; }
        public double State { get; private set; }
        public IList<ISynapse> Synapses { get; } = new List<ISynapse>();
        private double Stimulus { get; set; }

        #endregion Properties

        #region Methods

        public void Activate() => State = ActivationFunction(Stimulus);

        public void Stimulate() => Stimulus = Bias + Synapses.Sum(synapse => synapse.Signal);

        protected abstract double ActivationFunction(double stimulus);

        #endregion Methods
    }
}