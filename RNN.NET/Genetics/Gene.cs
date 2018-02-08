using Autrage.LEX.NET;
using System;

namespace Autrage.RNN.NET
{
    internal abstract class Gene
    {
        #region Methods

        public static Gene Next()
        {
            const int GeneTypeCount = 5;
            switch (Singleton<Random>.Instance.Next(GeneTypeCount))
            {
                case 0:
                    return new PerceptronCreator();

                case 1:
                    return new SigmonCreator();

                case 2:
                    return new NodeLinker();

                case 3:
                    return new InLinker();

                case 4:
                    return new OutLinker();

                default:
                    return new Dud();
            }
        }

        public abstract void ApplyTo(NetworkSkeleton network);

        public abstract void Mutate();

        public abstract Gene Replicate();

        #endregion Methods
    }
}