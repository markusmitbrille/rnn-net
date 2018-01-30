using System.Collections.Generic;

namespace Autrage.RNN.NET
{
    public interface IStimuland
    {
        #region Properties

        IList<ISynapse> Synapses { get; }

        #endregion Properties

        #region Methods

        void Stimulate();

        #endregion Methods
    }
}