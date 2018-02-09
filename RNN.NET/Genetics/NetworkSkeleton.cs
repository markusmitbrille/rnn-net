using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Autrage.RNN.NET
{
    internal class NetworkSkeleton
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

        #endregion Fields

        #region Properties

        public List<IStimulator> Stimulators { get; }

        public List<IStimuland> Stimulands { get; }

        public List<INeuron> Nodes { get; }

        #endregion Properties

        #region Constructors

        public NetworkSkeleton()
        {
            Stimulators = InstantiateSensors().ToList();
            Stimulands = InstantiateMuscles().ToList();
            Nodes = new List<INeuron>();
        }

        #endregion Constructors

        #region Methods

        internal Phenotype ToPhenotype()
        {
            IList<INeuralLayer> layers = new List<INeuralLayer>();
            layers.Insert(0, new StimulandLayer(Stimulands));

            List<INeuron> leftoverNodes = Nodes.ToList();
            List<INeuron> nextLayer =
                (from node in Nodes
                 from stimuland in Stimulands
                 from synapse in stimuland.Synapses
                 where node == synapse.Stimulator
                 select node)
                .ToList();

            while (nextLayer.Count > 0)
            {
                layers.Insert(0, new StimulatorLayer(nextLayer));
                layers.Insert(0, new StimulandLayer(nextLayer));

                leftoverNodes = leftoverNodes.Except(nextLayer).ToList();
                nextLayer =
                    (from node in Nodes
                     from stimuland in Stimulands
                     from synapse in stimuland.Synapses
                     where node == synapse.Stimulator
                     select node)
                    .ToList();
            }

            layers.Insert(0, new StimulatorLayer(Stimulators));

            return new Phenotype(layers);
        }

        private static IEnumerable<IStimulator> InstantiateSensors()
        {
            foreach (Type sensorType in sensorTypes)
            {
                if (Activator.CreateInstance(sensorType, true) is Sensor sensor)
                {
                    yield return sensor;
                }
            }
        }

        private static IEnumerable<IStimuland> InstantiateMuscles()
        {
            foreach (Type muscleType in muscleTypes)
            {
                if (Activator.CreateInstance(muscleType, true) is Muscle muscle)
                {
                    yield return muscle;
                }
            }
        }

        #endregion Methods
    }
}