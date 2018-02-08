using System;

namespace Autrage.RNN.NET
{
    internal interface INeuralLayer
    {
        #region Events

        event EventHandler Completed;

        #endregion Events

        #region Methods

        void Pulse();

        #endregion Methods
    }
}