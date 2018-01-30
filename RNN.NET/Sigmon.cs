using System;

namespace Autrage.RNN.NET
{
    internal class Sigmon : Neuron
    {
        #region Methods

        protected override double ActivationFunction(double stimulus) => 1 / (1 + Math.Exp(-stimulus));

        #endregion Methods
    }
}