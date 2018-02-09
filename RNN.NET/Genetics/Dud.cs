namespace Autrage.RNN.NET
{
    internal class Dud : Gene
    {
        #region Methods

        public override void ApplyTo(NetworkSkeleton network)
        {
        }

        public override void Mutate()
        {
        }

        public override Gene Replicate() => new Dud();

        #endregion Methods
    }
}