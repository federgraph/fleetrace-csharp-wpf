using System;

namespace RiggVar.Scoring
{
    
    /// <summary> 
    /// covering class for scoring related exceptions
    /// </summary>
    public class TScoringException : Exception
    {
        private TEntry fEntry;
        private TRace fRace;

        public virtual TEntry Entry => fEntry;
        public virtual TRace Race => fRace;

        public TScoringException(string msg, TRace race, TEntry entry) : base(msg)
        {
            fEntry = entry;
            fRace = race;
        }
                
        public override string ToString()
        {
            if (fEntry == null || fRace == null)
            {
                return base.ToString();
            }
            else
            {
                return base.ToString() + ", entry=" + fEntry.SailID + ", race" + fRace.NameID;
            }
        }
    }
}