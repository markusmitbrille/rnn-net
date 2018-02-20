using Autrage.LEX.NET;
using Autrage.LEX.NET.Extensions;
using Autrage.LEX.NET.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Autrage.RNN.NET
{
    [DataContract]
    public class Simulation : ICollection<NeuralNetwork>
    {
        [DataMember]
        private ICollection<NeuralNetwork> networks = new List<NeuralNetwork>();

        [DataMember]
        private double cutoffPercentile;

        [DataMember]
        private double replicationPercentile;

        [DataMember]
        public int Order { get; set; }

        [DataMember]
        public int GenomeComplexity { get; set; }

        [DataMember]
        public int MaxChromosomeSize { get; set; }

        [DataMember]
        public int MaxChromosomeSensors { get; set; }

        [DataMember]
        public int MaxChromosomeMuscles { get; set; }

        [DataMember]
        public int MaxChromosomeOrder { get; set; }

        [DataMember]
        public int MaxChromosomeConnectivity { get; set; }

        [DataMember]
        public int MaxChromosomeSensitivity { get; set; }

        [DataMember]
        public int MaxChromosomeProactivity { get; set; }

        [DataMember]
        public double ReplicationFidelity { get; set; } = 1;

        [DataMember]
        public Func<NeuralNetwork, double> Fitness { get; set; }

        public double CutoffPercentile
        {
            get
            {
                return cutoffPercentile;
            }

            set
            {
                cutoffPercentile = value.Clamp01();
            }
        }

        public double ReplicationPercentile
        {
            get
            {
                return replicationPercentile;
            }

            set
            {
                replicationPercentile = value.Clamp01();
            }
        }

        public int CutoffCount => (int)(Count * cutoffPercentile);
        public int ReplicationCount => (int)(Count * replicationPercentile);

        public int Count => networks.Count;

        bool ICollection<NeuralNetwork>.IsReadOnly => networks.IsReadOnly;

        public event EventHandler<NetworkEventArgs> Adding;

        public event EventHandler<NetworkEventArgs> Added;

        public event EventHandler<NetworkEventArgs> Removing;

        public event EventHandler<NetworkEventArgs> Removed;

        public event EventHandler Clearing;

        public event EventHandler Cleared;

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

            foreach (NeuralNetwork network in this.OrderByDescending(Fitness).Take(ReplicationCount).ToArray())
            {
                Add(new NeuralNetwork(network, ReplicationFidelity));
            }
        }

        public void Fornicate()
        {
            if (Fitness == null)
            {
                return;
            }

            List<NeuralNetwork> candidates = this.OrderByDescending(Fitness).Take(ReplicationCount).ToList();
            while (candidates.Count > 2)
            {
                NeuralNetwork mother = candidates[Rnd.Int(candidates.Count)];
                candidates.Remove(mother);
                NeuralNetwork father = candidates[Rnd.Int(candidates.Count)];
                candidates.Remove(father);

                Add(NeuralNetwork.Mate(mother, father, ReplicationFidelity));
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

        public class NetworkEventArgs : EventArgs
        {
            public NeuralNetwork Network { get; }

            public NetworkEventArgs(NeuralNetwork network)
            {
                network.AssertNotNull();

                Network = network;
            }
        }
    }
}