using Autrage.LEX.NET.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace Autrage.RNN.NET
{
    [DataContract]
    internal abstract class Neuron : INeuron
    {
        #region Fields

        [DataMember]
        private double stimulus;

        #endregion Fields

        #region Properties

        [DataMember]
        public double State { get; private set; }

        [DataMember]
        public double Bias { get; set; }

        [DataMember]
        public IList<ISynapse> Synapses { get; } = new List<ISynapse>();

        #endregion Properties

        #region Methods

        public void Activate() => State = ActivationFunction(stimulus);

        public void Stimulate() => stimulus = Bias + Synapses.Sum(synapse => synapse.Signal);

        protected abstract double ActivationFunction(double stimulus);

        #endregion Methods
    }
}