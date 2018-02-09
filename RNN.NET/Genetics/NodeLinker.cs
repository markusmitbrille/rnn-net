using Autrage.LEX.NET;
using Autrage.LEX.NET.Serialization;
using System;
using System.Linq;

namespace Autrage.RNN.NET
{
    [DataContract]
    internal class NodeLinker : Gene
    {
        #region Fields

        [DataMember]
        private int stimuland = Singleton<Random>.Instance.Next();

        [DataMember]
        private int stimulator = Singleton<Random>.Instance.Next();

        [DataMember]
        private double weight = Singleton<Random>.Instance.NextDouble();

        #endregion Fields

        #region Methods

        public override void ApplyTo(NetworkSkeleton network)
        {
            if (!network.Nodes.Any())
            {
                return;
            }

            network.Nodes[stimuland % network.Nodes.Count].Synapses.Add(new Synapse(network.Nodes[stimulator % network.Nodes.Count]) { Weight = weight });
        }

        public override void Mutate()
        {
            stimuland = Singleton<Random>.Instance.Next();
            stimulator = Singleton<Random>.Instance.Next();
            weight = Singleton<Random>.Instance.NextDouble();
        }

        public override Gene Replicate() => new NodeLinker() { stimulator = stimulator, stimuland = stimuland, weight = weight };

        #endregion Methods
    }
}