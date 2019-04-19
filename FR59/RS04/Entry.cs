namespace RiggVar.Scoring
{
    
    public class TEntry
    {
        public static int NextEntryID = 1;
        public int EntryID;
        public int SailID;
                        
        public TEntry()
        {
            EntryID = NextEntryID;
            NextEntryID++;
        }
                                
        public int CompareTo(object obj)
        {
            if (!(obj is TEntry))
            {
                return - 1;
            }

            if (Equals(obj))
            {
                return 0;
            }

            TEntry that = (TEntry) obj;
            
            return this.CompareSailID(that.SailID);            
        }

        public int CompareSailID(int snr)
        {
            if (SailID < snr)
            {
                return -1;
            }
            return SailID == snr ? 0 : 1;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            try
            {
                return EntryID == ((TEntry) obj).EntryID;
            }
            catch
            {
                return false;
            }
        }
        
        public override int GetHashCode()
        {
            return EntryID;
        }
                
        public override string ToString()
        {
            return "E" + SailID.ToString();
        }    

    }
}