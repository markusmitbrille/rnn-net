using System;

namespace Autrage.RNN.NET
{
    class Sigmon : Neuron
    {
        protected override double ActivationFunction(double stimulus) => 1 / (1 + Math.Exp(-stimulus));
    }
}
