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

        private ICollection<NeuralNetwork> networks = new List<NeuralNetwork>();

        private double cutoffPercentile;
        private double proliferationPercentile;

        #endregion Fields

        #region Properties

        public int Order { get; set; }
        public int Complexity { get; set; }

        public double MutationChance { get; set; }
        public double ComplexificationChance { get; set; }
        public double SimplificationChance { get; set; }
        public int MaxMutations { get; set; }
        public int MaxComplexifications { get; set; }
        public int MaxSimplifications { get; set; }

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

        public void Populate()
        {
            for (int i = Count; i < Order; i++)
            {
                Add(NeuralNetwork.Create(Complexity));
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
                Add(network.Replicate(MutationChance, ComplexificationChance, SimplificationChance, MaxMutations, MaxComplexifications, MaxSimplifications));
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
                new Neuron.Serializer(),
                new Synapse.Serializer(),
                new Muscle.Serializer(),
                new Sensor.Serializer(),
                new Dud.Serializer(),
                new SigmonCreator.Serializer(),
                new PerceptronCreator.Serializer(),
                new NodeLinker.Serializer(),
                new InLinker.Serializer(),
                new OutLinker.Serializer(),
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