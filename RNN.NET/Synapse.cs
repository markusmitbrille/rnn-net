using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autrage.RNN.NET
{
    class Synapse : ISynapse
    {
        public IStimulator Stimulator { get; }
        public double Weight { get; set; }

        public double Signal => Weight * Stimulator?.State ?? 0;

        public Synapse(IStimulator stimulator)
        {
            Stimulator = stimulator;
        }
    }
}
