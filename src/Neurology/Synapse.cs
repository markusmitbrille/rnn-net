﻿using Autrage.LEX.NET.Serialization;

namespace Autrage.RNN.NET
{
    [DataContract]
    internal class Synapse : ISynapse
    {
        public double Signal => Weight * Stimulator?.State ?? 0;

        [DataMember]
        public IStimulator Stimulator { get; private set; }

        [DataMember]
        public double Weight { get; set; }

        public Synapse(IStimulator stimulator)
        {
            Stimulator = stimulator;
        }

        private Synapse()
        {
        }

        public static void Link(IStimulator stimulator, IStimuland stimuland, double weight)
        {
            stimuland.Synapses.Add(new Synapse(stimulator) { Weight = weight });
        }
    }
}