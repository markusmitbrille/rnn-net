using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autrage.RNN.NET
{
    public interface INeuron : IStimuland, IStimulator
    {
        double Bias { get; }
    }
}
