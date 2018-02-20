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
        private static Type[] types =
            (from assembly in AppDomain.CurrentDomain.GetAssemblies()
             from type in assembly.GetTypes()
             where type.IsSubclassOf(typeof(Muscle))
             where type.IsDefined(typeof(MuscleAttribute))
             select type)
            .ToArray();

        private static MethodInfo[] methods =
            (from assembly in AppDomain.CurrentDomain.GetAssemblies()
             from type in assembly.GetTypes()
             from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
             where method.IsDefined(typeof(MuscleAttribute))
             where method.ReturnParameter.ParameterType == typeof(void)
             let parameters = method.GetParameters()
             where parameters.Count() == 1
             where parameters[0].ParameterType == typeof(double)
             select method)
            .ToArray();

        [DataMember]
        private double kind = Rnd.Double();

        [DataMember]
        private int type = Rnd.Int();

        public Muscle Create(NeuralNetwork network)
        {
            Muscle muscle;
            if (kind < 0.5)
            {
                muscle = (Muscle)Activator.CreateInstance(types[type % types.Length], true);
            }
            else
            {
                muscle = new DelegateMuscle(methods[type % methods.Length]);
            }

            muscle.Network = network;
            return muscle;
        }
    }
}