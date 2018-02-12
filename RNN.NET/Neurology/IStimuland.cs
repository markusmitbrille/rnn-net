using System.Collections.Generic;

namespace Autrage.RNN.NET
{
    public interface IStimuland
    {
        IList<ISynapse> Synapses { get; }

        void Stimulate();
    }
}