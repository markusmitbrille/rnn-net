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
        #region Fields

        private static Type[] types =
            (from assembly in AppDomain.CurrentDomain.GetAssemblies()
             from type in assembly.GetTypes()
             where type.IsSubclassOf(typeof(Sensor))
             let attribute = type.GetCustomAttribute<SensorAttribute>()
             where attribute != null
             select type)
            .ToArray();

        [DataMember]
        private int type = Rnd.Int();

        #endregion Fields

        #region Methods

        public Sensor Create(NeuralNetwork network)
        {
            Sensor sensor = (Sensor)Activator.CreateInstance(types[type % types.Length], true);
            sensor.Network = network;
            return sensor;
        }

        #endregion Methods
    }
}