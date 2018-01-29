using System;
using System.Collections.Generic;
using System.Linq;

namespace Autrage.RNN.NET
{
    public sealed class Receptor : IStimuland
    {
        public IList<ISynapse> Synapses { get; } = new List<ISynapse>();

        private double stimulus;
        public double Stimulus => stimulus;

        private Action<double> Act { get; }

        public Receptor(Action<double> act) => Act = act;

        public void Stimulate()
        {
            stimulus = Synapses.Sum(synapse => synapse.Signal);
            Act?.Invoke(stimulus);
        }
    }
}
