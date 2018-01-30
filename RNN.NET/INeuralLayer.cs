using System.Collections.Generic;

namespace Autrage.RNN.NET
{
    public interface INeuralLayer : IList<INeuron>
    {
        #region Methods

        void Activate();

        void Stimulate();

        #endregion Methods
    }
}