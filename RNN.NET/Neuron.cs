using System.Collections.Generic;
using System.Linq;

namespace Autrage.RNN.NET
{
    internal abstract class Neuron : INeuron
    {
        #region Fields

        private double stimulus;

        #endregion Fields

        #region Properties

        public double State { get; private set; }

        public double Bias { get; set; }

        public IList<ISynapse> Synapses { get; } = new List<ISynapse>();

        #endregion Properties

        #region Methods

        public void Activate() => State = ActivationFunction(stimulus);

        public void Stimulate() => stimulus = Bias + Synapses.Sum(synapse => synapse.Signal);

        protected abstract double ActivationFunction(double stimulus);

        #endregion Methods
    }
}