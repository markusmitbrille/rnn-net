using Autrage.LEX.NET.Extensions;
using Autrage.LEX.NET.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.RNN.NET
{
    [DataContract]
    public sealed class NeuralNetwork
    {
        #region Fields

        [DataMember]
        private Genome genome;

        [DataMember]
        private IList<INeuralLayer> layers;

        [DataMember]
        private int currentLayer;

        #endregion Fields

        #region Constructors

        internal NeuralNetwork(Genome genome, IList<INeuralLayer> layers)
        {
            this.genome = genome ?? throw new ArgumentNullException(nameof(genome));
            this.layers = layers ?? throw new ArgumentNullException(nameof(layers));

            AddLayerListeners();
        }

        private NeuralNetwork()
        {
        }

        #endregion Constructors

        #region Methods

        public static NeuralNetwork Create(int complexity) => new Genome(complexity).Instantiate();

        public NeuralNetwork Replicate() => new Genome(genome).Instantiate();

        public NeuralNetwork Replicate(double mutationChance = 0, double complexificationChance = 0, double simplificationChance = 0, int maxMutations = 0, int maxComplexifications = 0, int maxSimplifications = 0)
            => new Genome(genome, mutationChance, complexificationChance, simplificationChance, maxMutations, maxComplexifications, maxSimplifications).Instantiate();

        public void Pulse()
        {
            if (layers.Count == 0) return;
            if (currentLayer < layers.Count)
            {
                layers[currentLayer].Pulse();
            }
            else
            {
                currentLayer = 0;
            }
        }

        private void AddLayerListeners()
        {
            foreach (INeuralLayer layer in layers)
            {
                layer.Completed += OnLayerCompleted;
            }
        }

        private void OnLayerCompleted(object sender, EventArgs e) => currentLayer = ++currentLayer % layers.Count;

        #endregion Methods

        #region Classes

        public class Serializer : ReferenceTypeSerializer
        {
            #region Methods

            public override bool CanHandle(Type type) => typeof(NeuralNetwork).IsAssignableFrom(type);

            protected override bool SerializePayload(Stream stream, object instance)
            {
                NeuralNetwork network = (NeuralNetwork)instance;

                Marshaller.Serialize(stream, network.genome);

                stream.Write(network.layers.Count);
                foreach (INeuralLayer layer in network.layers)
                {
                    Marshaller.Serialize(stream, layer);
                }

                return true;
            }

            protected override bool DeserializePayload(Stream stream, object instance)
            {
                NeuralNetwork network = (NeuralNetwork)instance;

                network.genome = Marshaller.Deserialize<Genome>(stream);

                if (stream.ReadInt() is int layerCount)
                {
                    DeserializeLayers(stream, network, layerCount);
                    network.AddLayerListeners();
                }
                else
                {
                    Warning("Could not read network layer count!");
                    return false;
                }

                return true;
            }

            private void DeserializeLayers(Stream stream, NeuralNetwork network, int layerCount)
            {
                network.layers = new List<INeuralLayer>(layerCount);
                for (int i = 0; i < layerCount; i++)
                {
                    network.layers.Add(Marshaller.Deserialize<INeuralLayer>(stream));
                }
            }

            #endregion Methods
        }

        #endregion Classes
    }
}