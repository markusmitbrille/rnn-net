using Autrage.LEX.NET;
using Autrage.LEX.NET.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Autrage.RNN.NET
{
    [DataContract]
    internal class NeuralNetwork
    {
        #region Fields

        private static IEnumerable<Type> sensorTypes =
            from assembly in AppDomain.CurrentDomain.GetAssemblies()
            from type in assembly.GetTypes()
            where type.IsSubclassOf(typeof(Sensor))
            let attribute = type.GetCustomAttribute<SensorAttribute>()
            where attribute != null
            select type;

        private static IEnumerable<Type> muscleTypes =
            from assembly in AppDomain.CurrentDomain.GetAssemblies()
            from type in assembly.GetTypes()
            where type.IsSubclassOf(typeof(Muscle))
            let attribute = type.GetCustomAttribute<MuscleAttribute>()
            where attribute != null
            select type;

        [DataMember]
        private Genome genome;

        [DataMember]
        private IList<INeuron> nodes = new List<INeuron>();

        [DataMember]
        private IList<IStimuland> stimulands = new List<IStimuland>();

        [DataMember]
        private IList<IStimulator> stimulators = new List<IStimulator>();

        [DataMember]
        private IList<INeuralLayer> layers = new List<INeuralLayer>();

        #endregion Fields

        #region Constructors

        public NeuralNetwork(int complexity)
        {
            InferSensors();
            InferMuscles();

            genome = new Genome(complexity);
            genome.ApplyTo(this);

            InferLayers();
        }

        public NeuralNetwork(NeuralNetwork other)
        {
            InferSensors();
            InferMuscles();

            genome = new Genome(other.genome);
            genome.ApplyTo(this);

            InferLayers();
        }

        public NeuralNetwork(NeuralNetwork other,
            double mutationChance = 0,
            double complexificationChance = 0,
            double simplificationChance = 0,
            int maxMutations = 0,
            int maxComplexifications = 0,
            int maxSimplifications = 0)
        {
            InferSensors();
            InferMuscles();

            genome = new Genome(other.genome,
                mutationChance,
                complexificationChance,
                simplificationChance,
                maxMutations,
                maxComplexifications,
                maxSimplifications);

            genome.ApplyTo(this);

            InferLayers();
        }

        private NeuralNetwork()
        {
        }

        #endregion Constructors

        #region Methods

        public void Pulse()
        {
            foreach (IStimulator stimulator in stimulators)
            {
                stimulator.Activate();
            }

            foreach (INeuralLayer layer in layers)
            {
                layer.Stimulate();
                layer.Activate();
            }

            foreach (IStimuland stimuland in stimulands)
            {
                stimuland.Stimulate();
            }
        }

        private void InferSensors()
        {
            foreach (Type sensorType in sensorTypes)
            {
                if (Activator.CreateInstance(sensorType, true) is Sensor sensor)
                {
                    stimulators.Add(sensor);
                }
            }
        }

        private void InferMuscles()
        {
            foreach (Type muscleType in muscleTypes)
            {
                if (Activator.CreateInstance(muscleType, true) is Muscle muscle)
                {
                    stimulands.Add(muscle);
                }
            }
        }

        private void InferLayers()
        {
            layers.Clear();

            IEnumerable<INeuron> nextLayer = nodes.Where(node => node.Synapses.Any(synapse => stimulators.Contains(synapse.Stimulator)));
            IEnumerable<INeuron> leftoverNodes = nodes.Except(nextLayer);

            while (nextLayer.Count() > 0)
            {
                NeuralLayer layer = new NeuralLayer(nextLayer);
                layers.Add(layer);

                nextLayer = leftoverNodes.Where(node => node.Synapses.Any(synapse => layer.Contains(synapse.Stimulator)));
                leftoverNodes = leftoverNodes.Except(layer);
            }
        }

        #endregion Methods

        #region Classes

        private abstract class Gene
        {
            #region Methods

            public static Gene Next()
            {
                const int GeneTypeCount = 5;
                switch (Singleton<Random>.Instance.Next(GeneTypeCount))
                {
                    case 0:
                        return new PerceptronCreator();

                    case 1:
                        return new SigmonCreator();

                    case 2:
                        return new NodeLinker();

                    case 3:
                        return new InLinker();

                    case 4:
                        return new OutLinker();

                    default:
                        return new Dud();
                }
            }

            public abstract void ApplyTo(NeuralNetwork instance);

            public abstract void Mutate();

            public abstract Gene Replicate();

            #endregion Methods

            #region Classes

            [DataContract]
            private class Dud : Gene
            {
                #region Methods

                public override void ApplyTo(NeuralNetwork instance)
                {
                }

                public override void Mutate()
                {
                }

                public override Gene Replicate() => new Dud();

                #endregion Methods
            }

            [DataContract]
            private class SigmonCreator : Gene
            {
                #region Fields

                [DataMember]
                private double bias = Singleton<Random>.Instance.NextDouble();

                #endregion Fields

                #region Methods

                public override void ApplyTo(NeuralNetwork instance) => instance.nodes.Add(new Sigmon() { Bias = bias });

                public override void Mutate() => bias = Singleton<Random>.Instance.NextDouble();

                public override Gene Replicate() => new SigmonCreator() { bias = bias };

                #endregion Methods
            }

            [DataContract]
            private class PerceptronCreator : Gene
            {
                #region Fields

                [DataMember]
                private double bias = Singleton<Random>.Instance.NextDouble();

                #endregion Fields

                #region Methods

                public override void ApplyTo(NeuralNetwork instance) => instance.nodes.Add(new Perceptron() { Bias = bias });

                public override void Mutate() => bias = Singleton<Random>.Instance.NextDouble();

                public override Gene Replicate() => new PerceptronCreator() { bias = bias };

                #endregion Methods
            }

            [DataContract]
            private class NodeLinker : Gene
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

                public override void ApplyTo(NeuralNetwork instance)
                    => instance.nodes[stimuland % instance.nodes.Count].Synapses.Add(new Synapse(instance.nodes[stimulator % instance.nodes.Count]) { Weight = weight });

                public override void Mutate()
                {
                    stimulator = Singleton<Random>.Instance.Next();
                    stimuland = Singleton<Random>.Instance.Next();
                    weight = Singleton<Random>.Instance.NextDouble();
                }

                public override Gene Replicate() => new NodeLinker() { stimulator = stimulator, stimuland = stimuland, weight = weight };

                #endregion Methods
            }

            [DataContract]
            private class InLinker : Gene
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

                public override void ApplyTo(NeuralNetwork instance)
                    => instance.nodes[stimuland % instance.nodes.Count].Synapses.Add(new Synapse(instance.stimulators[stimulator % instance.stimulators.Count]) { Weight = weight });

                public override void Mutate()
                {
                    stimulator = Singleton<Random>.Instance.Next();
                    stimuland = Singleton<Random>.Instance.Next();
                    weight = Singleton<Random>.Instance.NextDouble();
                }

                public override Gene Replicate() => new InLinker() { stimulator = stimulator, stimuland = stimuland, weight = weight };

                #endregion Methods
            }

            [DataContract]
            private class OutLinker : Gene
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

                public override void ApplyTo(NeuralNetwork instance)
                    => instance.stimulands[stimuland % instance.stimulands.Count].Synapses.Add(new Synapse(instance.nodes[stimulator % instance.nodes.Count]) { Weight = weight });

                public override void Mutate()
                {
                    stimulator = Singleton<Random>.Instance.Next();
                    stimuland = Singleton<Random>.Instance.Next();
                    weight = Singleton<Random>.Instance.NextDouble();
                }

                public override Gene Replicate() => new OutLinker() { stimulator = stimulator, stimuland = stimuland, weight = weight };

                #endregion Methods
            }

            #endregion Classes
        }

        [DataContract]
        private class Genome
        {
            #region Properties

            [DataMember]
            private IList<Gene> Genes { get; } = new List<Gene>();

            #endregion Properties

            #region Constructors

            public Genome(int complexity)
            {
                for (int i = 0; i < complexity; i++)
                {
                    Genes.Add(Gene.Next());
                }
            }

            public Genome(Genome other)
            {
                foreach (Gene gene in other.Genes)
                {
                    Genes.Add(gene.Replicate());
                }
            }

            public Genome(Genome other,
                double mutationChance = 0,
                double complexificationChance = 0,
                double simplificationChance = 0,
                int maxMutations = 0,
                int maxComplexifications = 0,
                int maxSimplifications = 0)
            {
                foreach (Gene gene in other.Genes)
                {
                    Genes.Add(gene.Replicate());
                }

                for (int i = 0; i < maxMutations; i++)
                    if (Singleton<Random>.Instance.NextDouble() < mutationChance)
                        Mutate();
                for (int i = 0; i < maxComplexifications; i++)
                    if (Singleton<Random>.Instance.NextDouble() < complexificationChance)
                        Complexify();
                for (int i = 0; i < maxSimplifications; i++)
                    if (Singleton<Random>.Instance.NextDouble() < simplificationChance)
                        Simplify();
            }

            private Genome()
            {
            }

            #endregion Constructors

            #region Methods

            public void ApplyTo(NeuralNetwork instance)
            {
                foreach (Gene gene in Genes)
                {
                    gene.ApplyTo(instance);
                }
            }

            private void Complexify() => Genes.Add(Gene.Next());

            private void Mutate() => Genes[Singleton<Random>.Instance.Next(Genes.Count)].Mutate();

            private void Simplify() => Genes.Remove(Genes.Last());

            #endregion Methods
        }

        #endregion Classes
    }
}