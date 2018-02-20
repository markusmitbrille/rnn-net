using Autrage.LEX.NET.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Autrage.RNN.NET
{
    [DataContract]
    internal class StimulandLayer : INeuralLayer, IEnumerable<IStimuland>, IEnumerable
    {
        [DataMember]
        private List<IStimuland> stimulands;

        [DataMember]
        private int current;

        public event EventHandler Completed;

        public StimulandLayer(IEnumerable<IStimuland> collection)
        {
            stimulands = new List<IStimuland>(collection);
        }

        private StimulandLayer()
        {
        }

        public void Pulse()
        {
            if (stimulands.Count == 0) return;
            if (current < stimulands.Count)
            {
                stimulands[current].Stimulate();
            }
            else
            {
                current = 0;
                Completed?.Invoke(this, EventArgs.Empty);
            }
        }

        public IEnumerator<IStimuland> GetEnumerator() => stimulands.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => stimulands.GetEnumerator();
    }
}