using System.Collections.Generic;
using System.Linq;

namespace Autrage.RNN.NET
{
    public interface IStimuland
    {
        IList<ISynapse> Synapses { get; }

        void Stimulate();
    }
}
