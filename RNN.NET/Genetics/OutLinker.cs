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
    internal class OutLinker : Gene
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
            if (!network.Stimulands.Any())
            {
                return;
            }
            if (!network.Nodes.Any())
            {
                return;
            }

            network.Stimulands[stimuland % network.Stimulands.Count].Synapses.Add(new Synapse(network.Nodes[stimulator % network.Nodes.Count]) { Weight = weight });
        }

        public override void Mutate()
        {
            stimulator = Singleton<Random>.Instance.Next();
            stimuland = Singleton<Random>.Instance.Next();
            weight = Singleton<Random>.Instance.NextDouble();
        }

        public override Gene Replicate() => new OutLinker() { stimulator = stimulator, stimuland = stimuland, weight = weight };

        #endregion Methods

        #region Classes

        internal class Serializer : ReferenceTypeSerializer
        {
            #region Methods

            public override bool CanHandle(Type type) => typeof(OutLinker).IsAssignableFrom(type);

            protected override bool SerializePayload(Stream stream, object instance)
            {
                OutLinker gene = (OutLinker)instance;

                stream.Write(gene.stimuland);
                stream.Write(gene.stimulator);
                stream.Write(gene.weight);

                return true;
            }

            protected override bool DeserializePayload(Stream stream, object instance)
            {
                OutLinker gene = (OutLinker)instance;

                if (stream.ReadInt() is int stimuland)
                {
                    gene.stimuland = stimuland;
                }
                else
                {
                    Warning("Could not read out-linker stimuland!");
                    return false;
                }

                if (stream.ReadInt() is int stimulator)
                {
                    gene.stimulator = stimulator;
                }
                else
                {
                    Warning("Could not read out-linker stimulator!");
                    return false;
                }

                if (stream.ReadDouble() is double weight)
                {
                    gene.weight = weight;
                }
                else
                {
                    Warning("Could not read out-linker weight!");
                    return false;
                }

                return true;
            }

            #endregion Methods
        }

        #endregion Classes
    }
}