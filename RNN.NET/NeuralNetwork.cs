using Autrage.LEX.NET;
using Autrage.LEX.NET.Extensions;
using Autrage.LEX.NET.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.RNN.NET
{
    [DataContract]
    public sealed class NeuralNetwork
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

        internal class Serializer : ReferenceTypeSerializer
        {
            #region Methods

            public override bool CanHandle(Type type) => typeof(NeuralNetwork).IsAssignableFrom(type);

            protected override bool SerializePayload(Stream stream, object instance)
            {
                NeuralNetwork network = (NeuralNetwork)instance;

                Marshaller.Serialize(stream, network.genome);

                stream.Write(network.nodes.Count);
                foreach (INeuron node in network.nodes)
                {
                    Marshaller.Serialize(stream, node);
                }

                stream.Write(network.stimulands.Count);
                foreach (IStimuland stimuland in network.stimulands)
                {
                    Marshaller.Serialize(stream, stimuland);
                }

                stream.Write(network.stimulators.Count);
                foreach (IStimulator stimulator in network.stimulators)
                {
                    Marshaller.Serialize(stream, stimulator);
                }

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

                if (stream.ReadInt() is int nodeCount)
                {
                    DeserializeNodes(stream, network, nodeCount);
                }
                else
                {
                    Warning("Could not read network node count!");
                    return false;
                }

                if (stream.ReadInt() is int stimulandCount)
                {
                    DeserializeStimulands(stream, network, stimulandCount);
                }
                else
                {
                    Warning("Could not read network stimuland count!");
                    return false;
                }

                if (stream.ReadInt() is int stimulatorCount)
                {
                    DeserializeStimulators(stream, network, stimulatorCount);
                }
                else
                {
                    Warning("Could not read network stimulator count!");
                    return false;
                }

                if (stream.ReadInt() is int layerCount)
                {
                    DeserializeLayers(stream, network, layerCount);
                }
                else
                {
                    Warning("Could not read network layer count!");
                    return false;
                }

                return true;
            }

            private void DeserializeNodes(Stream stream, NeuralNetwork network, int nodeCount)
            {
                network.nodes = new List<INeuron>(nodeCount);
                for (int i = 0; i < nodeCount; i++)
                {
                    network.nodes.Add(Marshaller.Deserialize<INeuron>(stream));
                }
            }

            private void DeserializeStimulands(Stream stream, NeuralNetwork network, int stimulandCount)
            {
                network.stimulands = new List<IStimuland>(stimulandCount);
                for (int i = 0; i < stimulandCount; i++)
                {
                    network.stimulands.Add(Marshaller.Deserialize<IStimuland>(stream));
                }
            }

            private void DeserializeStimulators(Stream stream, NeuralNetwork network, int stimulatorCount)
            {
                network.stimulators = new List<IStimulator>(stimulatorCount);
                for (int i = 0; i < stimulatorCount; i++)
                {
                    network.stimulators.Add(Marshaller.Deserialize<IStimulator>(stream));
                }
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

        [DataContract]
        internal class Genome
        {
            #region Fields

            [DataMember]
            private IList<Gene> genes = new List<Gene>();

            #endregion Fields

            #region Constructors

            public Genome(int complexity)
            {
                for (int i = 0; i < complexity; i++)
                {
                    genes.Add(Gene.Next());
                }
            }

            public Genome(Genome other)
            {
                foreach (Gene gene in other.genes)
                {
                    genes.Add(gene.Replicate());
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
                foreach (Gene gene in other.genes)
                {
                    genes.Add(gene.Replicate());
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
                foreach (Gene gene in genes)
                {
                    gene.ApplyTo(instance);
                }
            }

            private void Complexify() => genes.Add(Gene.Next());

            private void Mutate() => genes[Singleton<Random>.Instance.Next(genes.Count)].Mutate();

            private void Simplify() => genes.Remove(genes.Last());

            #endregion Methods

            #region Classes

            internal class Serializer : ReferenceTypeSerializer
            {
                #region Methods

                public override bool CanHandle(Type type) => type == typeof(Genome);

                protected override bool SerializePayload(Stream stream, object instance)
                {
                    Genome genome = (Genome)instance;

                    stream.Write(genome.genes.Count);
                    foreach (Gene gene in genome.genes)
                    {
                        Marshaller.Serialize(stream, gene);
                    }

                    return true;
                }

                protected override bool DeserializePayload(Stream stream, object instance)
                {
                    Genome genome = (Genome)instance;

                    if (stream.ReadInt() is int geneCount)
                    {
                        genome.genes = new List<Gene>(geneCount);
                        for (int i = 0; i < geneCount; i++)
                        {
                            genome.genes.Add(Marshaller.Deserialize<Gene>(stream));
                        }
                    }
                    else
                    {
                        Warning("Could not read genome gene count!");
                        return false;
                    }

                    return true;
                }

                #endregion Methods
            }

            #endregion Classes
        }

        internal abstract class Gene
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
        }

        [DataContract]
        internal class Dud : Gene
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

            #region Classes

            internal class Serializer : ReferenceTypeSerializer
            {
                #region Methods

                public override bool CanHandle(Type type) => type == typeof(Dud);

                protected override bool SerializePayload(Stream stream, object instance) => true;

                protected override bool DeserializePayload(Stream stream, object instance) => true;

                #endregion Methods
            }

            #endregion Classes
        }

        [DataContract]
        internal class SigmonCreator : Gene
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

            #region Classes

            internal class Serializer : ReferenceTypeSerializer
            {
                #region Methods

                public override bool CanHandle(Type type) => type == typeof(SigmonCreator);

                protected override bool SerializePayload(Stream stream, object instance)
                {
                    SigmonCreator gene = (SigmonCreator)instance;

                    stream.Write(gene.bias);

                    return true;
                }

                protected override bool DeserializePayload(Stream stream, object instance)
                {
                    SigmonCreator gene = (SigmonCreator)instance;

                    if (stream.ReadDouble() is double bias)
                    {
                        gene.bias = bias;
                    }
                    else
                    {
                        Warning("Could not read sigmon creator bias!");
                        return false;
                    }

                    return true;
                }

                #endregion Methods
            }

            #endregion Classes
        }

        [DataContract]
        internal class PerceptronCreator : Gene
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

            #region Classes

            internal class Serializer : ReferenceTypeSerializer
            {
                #region Methods

                public override bool CanHandle(Type type) => type == typeof(PerceptronCreator);

                protected override bool SerializePayload(Stream stream, object instance)
                {
                    PerceptronCreator gene = (PerceptronCreator)instance;

                    stream.Write(gene.bias);

                    return true;
                }

                protected override bool DeserializePayload(Stream stream, object instance)
                {
                    PerceptronCreator gene = (PerceptronCreator)instance;

                    if (stream.ReadDouble() is double bias)
                    {
                        gene.bias = bias;
                    }
                    else
                    {
                        Warning("Could not read perceptron creator bias!");
                        return false;
                    }

                    return true;
                }

                #endregion Methods
            }

            #endregion Classes
        }

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

            public override void ApplyTo(NeuralNetwork instance)
            {
                if (!instance.nodes.Any())
                {
                    return;
                }

                instance.nodes[stimuland % instance.nodes.Count].Synapses.Add(new Synapse(instance.nodes[stimulator % instance.nodes.Count]) { Weight = weight });
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

                public override bool CanHandle(Type type) => type == typeof(NodeLinker);

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

        [DataContract]
        internal class InLinker : Gene
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
            {
                if (!instance.stimulators.Any())
                {
                    return;
                }
                if (!instance.nodes.Any())
                {
                    return;
                }

                instance.nodes[stimuland % instance.nodes.Count].Synapses.Add(new Synapse(instance.stimulators[stimulator % instance.stimulators.Count]) { Weight = weight });
            }

            public override void Mutate()
            {
                stimulator = Singleton<Random>.Instance.Next();
                stimuland = Singleton<Random>.Instance.Next();
                weight = Singleton<Random>.Instance.NextDouble();
            }

            public override Gene Replicate() => new InLinker() { stimulator = stimulator, stimuland = stimuland, weight = weight };

            #endregion Methods

            #region Classes

            internal class Serializer : ReferenceTypeSerializer
            {
                #region Methods

                public override bool CanHandle(Type type) => type == typeof(NodeLinker);

                protected override bool SerializePayload(Stream stream, object instance)
                {
                    InLinker gene = (InLinker)instance;

                    stream.Write(gene.stimuland);
                    stream.Write(gene.stimulator);
                    stream.Write(gene.weight);

                    return true;
                }

                protected override bool DeserializePayload(Stream stream, object instance)
                {
                    InLinker gene = (InLinker)instance;

                    if (stream.ReadInt() is int stimuland)
                    {
                        gene.stimuland = stimuland;
                    }
                    else
                    {
                        Warning("Could not read in-linker stimuland!");
                        return false;
                    }

                    if (stream.ReadInt() is int stimulator)
                    {
                        gene.stimulator = stimulator;
                    }
                    else
                    {
                        Warning("Could not read in-linker stimulator!");
                        return false;
                    }

                    if (stream.ReadDouble() is double weight)
                    {
                        gene.weight = weight;
                    }
                    else
                    {
                        Warning("Could not read in-linker weight!");
                        return false;
                    }

                    return true;
                }

                #endregion Methods
            }

            #endregion Classes
        }

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

            public override void ApplyTo(NeuralNetwork instance)
            {
                if (!instance.stimulands.Any())
                {
                    return;
                }
                if (!instance.nodes.Any())
                {
                    return;
                }

                instance.stimulands[stimuland % instance.stimulands.Count].Synapses.Add(new Synapse(instance.nodes[stimulator % instance.nodes.Count]) { Weight = weight });
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

                public override bool CanHandle(Type type) => type == typeof(NodeLinker);

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

        #endregion Classes
    }
}