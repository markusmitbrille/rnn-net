using Autrage.LEX.NET;
using Autrage.LEX.NET.Extensions;
using Autrage.LEX.NET.Serialization;

namespace Autrage.RNN.NET
{
    [DataContract]
    internal class Linker
    {
        [DataMember]
        private int stimuland = Rnd.Int();

        [DataMember]
        private int stimulator = Rnd.Int();

        [DataMember]
        private double weight = Rnd.Double();

        public void Link(IStimulator[] stimulators, IStimuland[] stimulands)
        {
            stimulators.AssertNotNull();
            stimulands.AssertNotNull();
            if (stimulators.Length == 0) return;
            if (stimulands.Length == 0) return;

            Synapse.Link(stimulators[stimulator % stimulators.Length], stimulands[stimuland % stimulands.Length], weight);
        }
    }
}