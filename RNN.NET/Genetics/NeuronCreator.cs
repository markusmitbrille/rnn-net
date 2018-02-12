using Autrage.LEX.NET.Extensions;
using Autrage.LEX.NET.Serialization;
using System;

namespace Autrage.RNN.NET
{
    [DataContract]
    internal class NeuronCreator
    {
        #region Fields

        [DataMember]
        private int type = Rnd.Int();

        [DataMember]
        private double bias = Rnd.Double();

        #endregion Fields

        #region Methods

        public Neuron Create()
        {
            const int count = 2;
            switch (type % count)
            {
                case 0:
                    return new Perceptron() { Bias = bias };

                case 1:
                    return new Sigmon() { Bias = bias };

                default:
                    throw new ArgumentException($"Could not get neuron from {type}!", nameof(type));
            }
        }

        #endregion Methods
    }
}