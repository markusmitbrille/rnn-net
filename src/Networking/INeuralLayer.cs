using System;
using System.Collections;

namespace Autrage.RNN.NET
{
    internal interface INeuralLayer : IEnumerable
    {
        event EventHandler Completed;

        void Pulse();
    }
}