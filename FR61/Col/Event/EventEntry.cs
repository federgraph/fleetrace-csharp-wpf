namespace RiggVar.FR
{

    public class TEventEntry : TBaseEntry
    {
        public int Bib;
       
        public int SNR;
        public string DN;
        public string NC;
        
        public TEventRaceEntry[] Race;
        public TEventRaceEntry GRace;
        
        public int Cup;
        
        public TEventEntry(int aRaceCount)
            : base()
        {
            Race = new TEventRaceEntry[aRaceCount + 1];
            for (int i = 0; i < RCount; i++)
            {
                Race[i] = new TEventRaceEntry(null);
            }

            GRace = Race[0];
        }

        public int RCount => Race.Length;

    }

}
