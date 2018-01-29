using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autrage.RNN.NET
{
    public interface INeuralNetwork
    {
        IList<IStimulator> Stimulators { get; }
        IList<IStimuland> Stimulands { get; }
        IList<INeuralLayer> Layers { get; }

        void Pulse();
    }
}
