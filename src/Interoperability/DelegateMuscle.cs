using Autrage.LEX.NET;
using Autrage.LEX.NET.Serialization;
using System.Reflection;

namespace Autrage.RNN.NET
{
    internal delegate void Move(double stimulus);

    [DataContract]
    internal class DelegateMuscle : Muscle
    {
        [DataMember]
        private Move del;

        public DelegateMuscle(Move del)
        {
            del.AssertNotNull();

            this.del = del;
        }

        public DelegateMuscle(MethodInfo method)
        {
            method.AssertNotNull();

            del = (Move)method.CreateDelegate(typeof(Move));
        }

        protected override void Move(double stimulus) => del(stimulus);
    }
}