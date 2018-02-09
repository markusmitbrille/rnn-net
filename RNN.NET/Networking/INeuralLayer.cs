using System;
using System.Collections;

namespace Autrage.RNN.NET
{
    internal interface INeuralLayer : IList
    {
        #region Events

        event EventHandler Completed;

        #endregion Events

        #region Methods

        void Pulse();

        #endregion Methods
    }
}