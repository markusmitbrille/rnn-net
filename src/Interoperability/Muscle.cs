﻿using Autrage.LEX.NET.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace Autrage.RNN.NET
{
    [DataContract]
    public abstract class Muscle : IStimuland
    {
        [DataMember]
        public NeuralNetwork Network { get; internal set; }

        [DataMember]
        public IList<ISynapse> Synapses { get; private set; } = new List<ISynapse>();

        public void Stimulate() => Move(Synapses.Sum(synapse => synapse.Signal));

        protected abstract void Move(double stimulus);
    }
}