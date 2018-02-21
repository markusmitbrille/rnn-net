using Autrage.LEX.NET.Extensions;
using Autrage.LEX.NET.Serialization;
using System;
using System.Linq;
using System.Reflection;

namespace Autrage.RNN.NET
{
    [DataContract]
    internal class SensorCreator
    {
        private static Type[] types =
            (from assembly in AppDomain.CurrentDomain.GetAssemblies()
             from type in assembly.GetTypes()
             where type.IsSubclassOf(typeof(Sensor))
             where type.IsDefined(typeof(SensorAttribute))
             select type)
            .ToArray();

        private static MethodInfo[] methods =
            (from assembly in AppDomain.CurrentDomain.GetAssemblies()
             from type in assembly.GetTypes()
             from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
             where method.IsDefined(typeof(SensorAttribute))
             where method.ReturnParameter.ParameterType == typeof(double)
             where method.GetParameters().None()
             select method)
            .ToArray();

        [DataMember]
        private double kind = Rnd.Double();

        [DataMember]
        private int type = Rnd.Int();

        public Sensor Create(NeuralNetwork network)
        {
            Sensor sensor;
            if (kind < 0.5)
            {
                sensor = (Sensor)Activator.CreateInstance(types[type % types.Length], true);
            }
            else
            {
                sensor = new DelegateSensor(methods[type % methods.Length]);
            }

            sensor.Network = network;
            return sensor;
        }
    }
}