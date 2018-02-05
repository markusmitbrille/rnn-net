using Autrage.LEX.NET.Serialization;

namespace Autrage.RNN.NET
{
    [DataContract]
    internal class Perceptron : Neuron
    {
        #region Methods

        protected override double ActivationFunction(double stimulus) => stimulus > 0 ? 1 : -1;

        #endregion Methods
    }
}