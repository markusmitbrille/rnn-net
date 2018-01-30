using System;
using System.Collections.Generic;
using System.Linq;

namespace Autrage.RNN.NET
{
    public abstract class Muscle : IStimuland
    {
        public IList<ISynapse> Synapses { get; } = new List<ISynapse>();

        public Muscle()
        {
        }

        public void Stimulate() => Move(Synapses.Sum(synapse => synapse.Signal));

        protected abstract void Move(double stimulus);
    }
}
