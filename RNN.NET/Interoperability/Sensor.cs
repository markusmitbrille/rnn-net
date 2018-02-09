using Autrage.LEX.NET.Extensions;
using Autrage.LEX.NET.Serialization;
using System;
using System.IO;
using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.RNN.NET
{
    [DataContract]
    public abstract class Sensor : IStimulator
    {
        #region Properties

        [DataMember]
        public NeuralNetwork Network { get; internal set; }

        [DataMember]
        public double State { get; private set; }

        #endregion Properties

        #region Methods

        public void Activate() => State = Fetch();

        protected abstract double Fetch();

        #endregion Methods

        #region Classes

        internal class Serializer : ReferenceTypeSerializer
        {
            #region Methods

            public override bool CanHandle(Type type) => typeof(Sensor).IsAssignableFrom(type);

            protected override bool SerializePayload(Stream stream, object instance)
            {
                Sensor sensor = (Sensor)instance;

                Marshaller.Serialize(stream, sensor.Network);
                stream.Write(sensor.State);

                return true;
            }

            protected override bool DeserializePayload(Stream stream, object instance)
            {
                Sensor sensor = (Sensor)instance;

                sensor.Network = Marshaller.Deserialize<NeuralNetwork>(stream);

                if (stream.ReadDouble() is double state)
                {
                    sensor.State = state;
                }
                else
                {
                    Warning("Could not read sensor state!");
                    return false;
                }

                return true;
            }

            #endregion Methods
        }

        #endregion Classes
    }
}