using Autrage.LEX.NET.Serialization;
using System;
using System.Reflection;

namespace Autrage.RNN.NET
{
    internal delegate void Move(double stimulus);

    [DataContract]
    internal class DelegateMuscle : Muscle
    {
        [DataMember]
        private Move del;

        public DelegateMuscle(Move del) => this.del = del ?? throw new ArgumentNullException(nameof(del));

        public DelegateMuscle(MethodInfo method) => del = (Move)method.CreateDelegate(typeof(Move));

        protected override void Move(double stimulus) => del(stimulus);
    }
}