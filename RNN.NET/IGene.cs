namespace Autrage.RNN.NET
{
    public interface IGene<T>
    {
        void Phenotype(T instance);
        IGene<T> Replicate();
        void Mutate();
    }
}
