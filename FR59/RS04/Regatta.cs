using System;
using System.Diagnostics;

namespace RiggVar.Scoring
{

    public class TRegatta
    {
        public static bool IsInFinalPhase = false;
        public string Name = "";        
        public TEntryList Entries;
        public TRaceList Races;
        public TScoringManager ScoringManager;

        public TRegatta()
        {            
            Entries = new TEntryList();
            Races = new TRaceList();
            ScoringManager = new TScoringManager();    
        }

        public virtual void ScoreRegatta()
        {
            try
            {
                ScoringManager.ScoreRegatta(this, Races);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
        }

        public int CompareTo(object obj)
        {
            if (!(obj is TRegatta))
            {
                return - 1;
            }

            if (this == obj)
            {
                return 0;
            }

            TRegatta that = (TRegatta) obj;
            
            return Name.CompareTo(that.Name);
        }

        public override string ToString()
        {
            return Name;
        }

        public virtual void AddEntry(TEntry e)
        {
            Entries.Add(e);
        }

    }
}