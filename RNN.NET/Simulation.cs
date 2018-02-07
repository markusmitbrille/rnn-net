using Autrage.LEX.NET.Extensions;
using Autrage.LEX.NET.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.RNN.NET
{
    public class Simulation : List<NeuralNetwork>
    {
        #region Fields

        public Func<NeuralNetwork, double> Fitness;
        private double cutoffPercentile;

        #endregion Fields

        #region Properties

        public int Order { get; set; }
        public int Complexity { get; set; }

        public double CutoffPercentile
        {
            get => cutoffPercentile;
            set => cutoffPercentile = value.Clamp01();
        }

        public int CutoffCount => (int)(Count * cutoffPercentile);

        #endregion Properties

        #region Methods

        public static Simulation Deserialize(Stream stream, params Serializer[] serializers)
        {
            Serializer[] networkSerializers = new Serializer[]
            {
                new Neuron.Serializer(),
                new Synapse.Serializer(),
                new Muscle.Serializer(),
                new Sensor.Serializer(),
                new NeuralLayer.Serializer(),
                new NeuralNetwork.Serializer(),
                new NeuralNetwork.Genome.Serializer(),
                new NeuralNetwork.Dud.Serializer(),
                new NeuralNetwork.SigmonCreator.Serializer(),
                new NeuralNetwork.PerceptronCreator.Serializer(),
                new NeuralNetwork.NodeLinker.Serializer(),
                new NeuralNetwork.InLinker.Serializer(),
                new NeuralNetwork.OutLinker.Serializer(),
            };

            Marshaller marshaller = new Marshaller(serializers.Concat(networkSerializers).ToArray());

            if (stream.ReadInt() is int networkCount)
            {
                Simulation simulation = new Simulation();
                for (int i = 0; i < networkCount; i++)
                {
                    simulation.Add(marshaller.Deserialize<NeuralNetwork>(stream));
                }

                return simulation;
            }
            else
            {
                Warning("Could not read simulation network count!");
                return null;
            }
        }

        public void Serialize(Stream stream, params Serializer[] serializers)
        {
            Serializer[] networkSerializers = new Serializer[]
            {
                new Neuron.Serializer(),
                new Synapse.Serializer(),
                new Muscle.Serializer(),
                new Sensor.Serializer(),
                new NeuralLayer.Serializer(),
                new NeuralNetwork.Serializer(),
                new NeuralNetwork.Genome.Serializer(),
                new NeuralNetwork.Dud.Serializer(),
                new NeuralNetwork.SigmonCreator.Serializer(),
                new NeuralNetwork.PerceptronCreator.Serializer(),
                new NeuralNetwork.NodeLinker.Serializer(),
                new NeuralNetwork.InLinker.Serializer(),
                new NeuralNetwork.OutLinker.Serializer(),
            };

            Marshaller marshaller = new Marshaller(serializers.Concat(networkSerializers).ToArray());

            stream.Write(Count);
            foreach (NeuralNetwork network in this)
            {
                marshaller.Serialize(stream, network);
            }
        }

        public void Genesis()
        {
            for (int i = Count; i < Order; i++)
            {
                Add(new NeuralNetwork(Complexity));
            }
        }

        public void Armageddon()
        {
            if (Fitness == null)
            {
                return;
            }

            foreach (NeuralNetwork network in this.OrderBy(Fitness).Take(CutoffCount).ToList())
            {
                Remove(network);
            }
        }

        public void Pulse()
        {
            foreach (NeuralNetwork network in this)
            {
                network.Pulse();
            }
        }

        #endregion Methods
    }
}