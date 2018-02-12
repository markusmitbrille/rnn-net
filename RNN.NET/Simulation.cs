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
        private ICollection<NeuralNetwork> networks = new List<NeuralNetwork>();

        private double cutoffPercentile;
        private double proliferationPercentile;

        public int Order { get; set; }
        public int GenomeComplexity { get; set; }

        public int MaxChromosomeSize { get; set; }
        public int MaxChromosomeSensors { get; set; }
        public int MaxChromosomeMuscles { get; set; }
        public int MaxChromosomeOrder { get; set; }
        public int MaxChromosomeConnectivity { get; set; }
        public int MaxChromosomeSensitivity { get; set; }
        public int MaxChromosomeProactivity { get; set; }

        public double Fidelity { get; set; } = 1;

        public Func<NeuralNetwork, double> Fitness { get; set; }

        public double CutoffPercentile
        {
            get => cutoffPercentile;
            set => cutoffPercentile = value.Clamp01();
        }

        public double ProliferationPercentile
        {
            get => proliferationPercentile;
            set => proliferationPercentile = value.Clamp01();
        }

        public int CutoffCount => (int)(Count * cutoffPercentile);
        public int ProliferationCount => (int)(Count * proliferationPercentile);

        public int Count => networks.Count;

        bool ICollection<NeuralNetwork>.IsReadOnly => networks.IsReadOnly;

        public event EventHandler<NetworkEventArgs> Adding;

        public event EventHandler<NetworkEventArgs> Added;

        public event EventHandler<NetworkEventArgs> Removing;

        public event EventHandler<NetworkEventArgs> Removed;

        public event EventHandler Clearing;

        public event EventHandler Cleared;

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

        public void Populate()
        {
            for (int i = Count; i < Order; i++)
            {
                Add(new NeuralNetwork(GenomeComplexity, MaxChromosomeSize, MaxChromosomeSensors, MaxChromosomeMuscles, MaxChromosomeOrder, MaxChromosomeConnectivity, MaxChromosomeSensitivity, MaxChromosomeProactivity));
            }
        }

        public void Proliferate()
        {
            if (Fitness == null)
            {
                return;
            }

            foreach (NeuralNetwork network in this.OrderByDescending(Fitness).Take(ProliferationCount).ToArray())
            {
                Add(new NeuralNetwork(network, Fidelity));
            }
        }

        public void Cutoff()
        {
            if (Fitness == null)
            {
                return;
            }

            foreach (NeuralNetwork network in this.OrderBy(Fitness).Take(CutoffCount).ToArray())
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
                new Phenotype.Serializer(),
                new PrimitiveSerializer(),
                new EnumSerializer(),
                new ValueTypeSerializer(),
                new ListSerializer(),
                new DictionarySerializer(),
                new GenericCollectionSerializer(),
                new ContractSerializer(),
            };
        }

        public class NetworkEventArgs : EventArgs
        {
            public NeuralNetwork Network { get; }

            public NetworkEventArgs(NeuralNetwork network)
            {
                Network = network ?? throw new ArgumentNullException(nameof(network));
            }
        }
    }
}