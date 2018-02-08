using Autrage.LEX.NET;
using Autrage.LEX.NET.Extensions;
using Autrage.LEX.NET.Serialization;
using System;
using System.IO;
using System.Linq;
using static Autrage.LEX.NET.DebugUtils;

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

        #region Classes

        internal class Serializer : ReferenceTypeSerializer
        {
            #region Methods

            public override bool CanHandle(Type type) => typeof(NodeLinker).IsAssignableFrom(type);

            protected override bool SerializePayload(Stream stream, object instance)
            {
                NodeLinker gene = (NodeLinker)instance;

                stream.Write(gene.stimuland);
                stream.Write(gene.stimulator);
                stream.Write(gene.weight);

                return true;
            }

            protected override bool DeserializePayload(Stream stream, object instance)
            {
                NodeLinker gene = (NodeLinker)instance;

                if (stream.ReadInt() is int stimuland)
                {
                    gene.stimuland = stimuland;
                }
                else
                {
                    Warning("Could not read node linker stimuland!");
                    return false;
                }

                if (stream.ReadInt() is int stimulator)
                {
                    gene.stimulator = stimulator;
                }
                else
                {
                    Warning("Could not read node linker stimulator!");
                    return false;
                }

                if (stream.ReadDouble() is double weight)
                {
                    gene.weight = weight;
                }
                else
                {
                    Warning("Could not read node linker weight!");
                    return false;
                }

                return true;
            }

            #endregion Methods
        }

        #endregion Classes
    }
}