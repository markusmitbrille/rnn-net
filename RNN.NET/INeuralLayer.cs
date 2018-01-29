using System.Collections.Generic;

namespace Autrage.RNN.NET
{
    public interface INeuralLayer : IList<INeuron>
    {
        void Stimulate();
        void Activate();
    }
}
