using Autrage.LEX.NET.Extensions;
using Autrage.LEX.NET.Serialization;
using System;
using System.Linq;
using System.Reflection;

namespace Autrage.RNN.NET
{
    [DataContract]
    internal class MuscleCreator
    {
        #region Fields

        private static Type[] types =
            (from assembly in AppDomain.CurrentDomain.GetAssemblies()
             from type in assembly.GetTypes()
             where type.IsSubclassOf(typeof(Muscle))
             let attribute = type.GetCustomAttribute<MuscleAttribute>()
             where attribute != null
             select type)
            .ToArray();

        [DataMember]
        private int type = Rnd.Int();

        #endregion Fields

        #region Methods

        public Muscle Create(NeuralNetwork network)
        {
            Muscle muscle = (Muscle)Activator.CreateInstance(types[type % types.Length], true);
            muscle.Network = network;
            return muscle;
        }

        #endregion Methods
    }
}