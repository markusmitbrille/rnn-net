using Autrage.LEX.NET;
using Autrage.LEX.NET.Serialization;
using System.Reflection;

namespace Autrage.RNN.NET
{
    internal delegate double Fetch();

    [DataContract]
    internal class DelegateSensor : Sensor
    {
        [DataMember]
        private Fetch del;

        public DelegateSensor(Fetch del)
        {
            del.AssertNotNull();

            this.del = del;
        }

        public DelegateSensor(MethodInfo method)
        {
            method.AssertNotNull();

            del = (Fetch)method.CreateDelegate(typeof(Fetch));
        }

        protected override double Fetch() => del();
    }
}