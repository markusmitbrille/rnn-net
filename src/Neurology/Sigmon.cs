using Autrage.LEX.NET.Serialization;
using System;

namespace Autrage.RNN.NET
{
    [DataContract]
    internal class Sigmon : Neuron
    {
        protected override double ActivationFunction(double stimulus) => 1 / (1 + Math.Exp(-stimulus));
    }
}