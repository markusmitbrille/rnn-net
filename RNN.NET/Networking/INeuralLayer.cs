using System;
using System.Collections;

namespace Autrage.RNN.NET
{
    internal interface INeuralLayer : IEnumerable
    {
        #region Events

        event EventHandler Completed;

        #endregion Events

        #region Methods

        void Pulse();

        #endregion Methods
    }
}