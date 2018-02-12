using Autrage.LEX.NET.Extensions;
using Autrage.LEX.NET.Serialization;
using System.Collections.Generic;

namespace Autrage.RNN.NET
{
    [DataContract]
    internal class Chromosome
    {
        #region Fields

        [DataMember]
        private List<NeuronCreator> neuronCreators;

        [DataMember]
        private List<SensorCreator> sensorCreators;

        [DataMember]
        private List<MuscleCreator> muscleCreators;

        [DataMember]
        private List<Linker> intraLinkers;

        [DataMember]
        private List<Linker> interLinkers;

        [DataMember]
        private List<Linker> sensorConnectors;

        [DataMember]
        private List<Linker> muscleConnectors;

        #endregion Fields

        #region Constructors

        public Chromosome(int size, int sensors, int muscles, int order, int connectivity, int sensitivity, int proactivity)
        {
            neuronCreators = new List<NeuronCreator>(size);
            for (int i = 0; i < size; i++)
                neuronCreators.Add(new NeuronCreator());

            sensorCreators = new List<SensorCreator>(sensors);
            for (int i = 0; i < sensors; i++)
                sensorCreators.Add(new SensorCreator());

            muscleCreators = new List<MuscleCreator>(muscles);
            for (int i = 0; i < muscles; i++)
                muscleCreators.Add(new MuscleCreator());

            intraLinkers = new List<Linker>(order);
            for (int i = 0; i < order; i++)
                intraLinkers.Add(new Linker());

            interLinkers = new List<Linker>(connectivity);
            for (int i = 0; i < connectivity; i++)
                interLinkers.Add(new Linker());

            sensorConnectors = new List<Linker>(sensitivity);
            for (int i = 0; i < sensitivity; i++)
                sensorConnectors.Add(new Linker());

            muscleConnectors = new List<Linker>(proactivity);
            for (int i = 0; i < proactivity; i++)
                muscleConnectors.Add(new Linker());
        }

        public Chromosome(Chromosome other, double fidelity = 1)
        {
            neuronCreators = ReplicateList(other.neuronCreators, fidelity);
            sensorCreators = ReplicateList(other.sensorCreators, fidelity);
            muscleCreators = ReplicateList(other.muscleCreators, fidelity);
            intraLinkers = ReplicateList(other.intraLinkers, fidelity);
            interLinkers = ReplicateList(other.interLinkers, fidelity);
            sensorConnectors = ReplicateList(other.sensorConnectors, fidelity);
            muscleConnectors = ReplicateList(other.muscleConnectors, fidelity);
        }

        private Chromosome()
        {
        }

        #endregion Constructors

        #region Methods

        public void Phenotype(NeuralNetwork network, NetworkSkeleton skeleton)
        {
            Neuron[] neurons = new Neuron[neuronCreators.Count];
            for (int i = 0; i < neuronCreators.Count; i++)
            {
                neurons[i] = neuronCreators[i].Create();
            }

            Sensor[] sensors = new Sensor[sensorCreators.Count];
            for (int i = 0; i < sensorCreators.Count; i++)
            {
                sensors[i] = sensorCreators[i].Create(network);
            }

            Muscle[] muscles = new Muscle[muscleCreators.Count];
            for (int i = 0; i < muscleCreators.Count; i++)
            {
                muscles[i] = muscleCreators[i].Create(network);
            }

            foreach (Linker intraLinker in intraLinkers)
            {
                intraLinker.Link(neurons, neurons);
            }

            foreach (Linker interLinker in interLinkers)
            {
                interLinker.Link(neurons, skeleton.Neurons.ToArray());
            }

            foreach (Linker sensorConnector in sensorConnectors)
            {
                sensorConnector.Link(sensors, neurons);
            }

            foreach (Linker muscleConnector in muscleConnectors)
            {
                muscleConnector.Link(neurons, muscles);
            }

            skeleton.Neurons.AddRange(neurons);
            skeleton.Stimulators.AddRange(sensors);
            skeleton.Stimulands.AddRange(muscles);
        }

        private static List<T> ReplicateList<T>(List<T> replicand, double fidelity) where T : new()
        {
            List<T> replica = new List<T>();
            foreach (T item in replicand)
            {
                if (Rnd.Chance(fidelity))
                {
                    replica.Add(item);
                }
                else
                {
                    replica.Add(new T());
                }
            }

            return replica;
        }

        #endregion Methods
    }
}