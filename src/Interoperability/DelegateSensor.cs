using Autrage.LEX.NET.Serialization;
using System;
using System.Reflection;

namespace Autrage.RNN.NET
{
    internal delegate double Fetch();

    [DataContract]
    internal class DelegateSensor : Sensor
    {
        [DataMember]
        private Fetch del;

        public DelegateSensor(Fetch del) => this.del = del ?? throw new ArgumentNullException(nameof(del));

        public DelegateSensor(MethodInfo method) => del = (Fetch)method.CreateDelegate(typeof(Fetch));

        protected override double Fetch() => del();
    }
}