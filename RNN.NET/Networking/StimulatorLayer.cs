using Autrage.LEX.NET.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Autrage.RNN.NET
{
    [DataContract]
    internal class StimulatorLayer : INeuralLayer, IEnumerable<IStimulator>, IEnumerable
    {
        [DataMember]
        private List<IStimulator> stimulators;

        [DataMember]
        private int current;

        public event EventHandler Completed;

        public StimulatorLayer(IEnumerable<IStimulator> collection) => stimulators = new List<IStimulator>(collection);

        private StimulatorLayer()
        {
        }

        public void Pulse()
        {
            if (stimulators.Count == 0) return;
            if (current < stimulators.Count)
            {
                stimulators[current].Activate();
            }
            else
            {
                current = 0;
                Completed?.Invoke(this, EventArgs.Empty);
            }
        }

        public IEnumerator<IStimulator> GetEnumerator() => stimulators.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => stimulators.GetEnumerator();
    }
}