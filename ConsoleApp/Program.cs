using Autrage.LEX.NET;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp
{
    internal class Program
    {
        #region Methods

        private static void Main(string[] args)
        {
            List<A> l = new List<A>() { new B() { mem = "derp", childmem = 2 } };

            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.Serialize(stream, l);
                stream.Position = 0;
                List<A> res = Serializer.Deserialize<List<A>>(stream);

                foreach (var item in res)
                {
                    Console.WriteLine($"mem={item.mem}, type={item.GetType()}");
                }
            }

            Console.ReadLine();
        }

        #endregion Methods
    }

    [ProtoContract]
    class A
    {
        [ProtoMember(1)]
        public string mem;
    }

    [ProtoContract]
    class B : A
    {
        [ProtoMember(1)]
        public int childmem;
    }
}