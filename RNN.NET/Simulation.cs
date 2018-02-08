using Autrage.LEX.NET.Extensions;
using Autrage.LEX.NET.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.RNN.NET
{
    public class Simulation : ICollection<NeuralNetwork>
    {
        #region Fields

        public Func<NeuralNetwork, double> Fitness;
        private ICollection<NeuralNetwork> networks = new List<NeuralNetwork>();
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

        public int Count => networks.Count;

        bool ICollection<NeuralNetwork>.IsReadOnly => networks.IsReadOnly;

        #endregion Properties

        #region Events

        public event EventHandler<NetworkEventArgs> Adding;

        public event EventHandler<NetworkEventArgs> Added;

        public event EventHandler<NetworkEventArgs> Removing;

        public event EventHandler<NetworkEventArgs> Removed;

        public event EventHandler Clearing;

        public event EventHandler Cleared;

        #endregion Events

        #region Methods

        public static Simulation Deserialize(Stream stream, params Serializer[] serializers)
        {
            Marshaller marshaller = new Marshaller(serializers.Concat(GetDefaultSerializers()).ToArray());

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
            Marshaller marshaller = new Marshaller(serializers.Concat(GetDefaultSerializers()).ToArray());

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

        public void Add(NeuralNetwork item)
        {
            if (item == null) return;

            Adding?.Invoke(this, new NetworkEventArgs(item));
            networks.Add(item);
            Added?.Invoke(this, new NetworkEventArgs(item));
        }

        public bool Remove(NeuralNetwork item)
        {
            if (!Contains(item)) return false;

            Removing?.Invoke(this, new NetworkEventArgs(item));
            networks.Remove(item);
            Removed?.Invoke(this, new NetworkEventArgs(item));

            return true;
        }

        public void Clear()
        {
            Clearing?.Invoke(this, EventArgs.Empty);
            networks.Clear();
            Cleared?.Invoke(this, EventArgs.Empty);
        }

        public bool Contains(NeuralNetwork item) => networks.Contains(item);

        public void CopyTo(NeuralNetwork[] array, int arrayIndex) => networks.CopyTo(array, arrayIndex);

        IEnumerator<NeuralNetwork> IEnumerable<NeuralNetwork>.GetEnumerator() => networks.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => networks.GetEnumerator();

        private static Serializer[] GetDefaultSerializers()
        {
            return new Serializer[]
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
                new PrimitiveSerializer(),
                new EnumSerializer(),
                new ValueTypeSerializer(),
                new ListSerializer(),
                new DictionarySerializer(),
                new GenericCollectionSerializer(),
                new ContractSerializer(),
            };
        }

        #endregion Methods

        #region Classes

        public class NetworkEventArgs : EventArgs
        {
            #region Properties

            public NeuralNetwork Network { get; }

            #endregion Properties

            #region Constructors

            public NetworkEventArgs(NeuralNetwork network)
            {
                Network = network ?? throw new ArgumentNullException(nameof(network));
            }

            #endregion Constructors
        }

        #endregion Classes
    }
}