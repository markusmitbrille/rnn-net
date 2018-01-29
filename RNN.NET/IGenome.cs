using System.Collections.Generic;

namespace Autrage.RNN.NET
{
    public interface IGenome<T> : IEnumerable<IGene<T>>
    {
        IGenome<T> Replicate();
        void Phenotype(T instance);
        void Mutate();
        void Complexify();
        void Simplify();
    }
}
