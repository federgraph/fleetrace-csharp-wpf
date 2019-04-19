using System.Collections.Generic;

namespace RiggVar.Scoring
{

    public class TEntryList : List<TEntry>
    {
        public TEntryList() : base()
        {
        }

        public virtual TEntryList DuplicateIDs
        {
            get
            {
                SortSailId();
                TEntryList dupList = new TEntryList();
                
                TEntry laste = null;
                int lastid = -1;
                foreach (TEntry thise in this)
                {
                    int thisid = thise.SailID;
                    
                    if (laste != null)
                    {
                        if (lastid == thisid)
                        {
                            if (!dupList.Contains(laste))
                            {
                                dupList.Add(laste);
                            }

                            dupList.Add(thise);
                        }
                    }
                    laste = thise;
                    lastid = thisid;
                }
                
                return dupList;
            }
            
        }        
                            
        public virtual TEntryList FindId(int snr)
        {
            TEntryList list = new TEntryList();
            foreach (TEntry e in this)
            {
                if (e.SailID == snr)
                {
                    list.Add(e);
                }
            }
            return list;
        }

        public TEntryList CloneEntries()
        {
            TEntryList result = new TEntryList();
            foreach (TEntry e in this)
            {
                result.Add(e);
            }
            return result;
        }

        public virtual void SortSailId()
        {
            Sort(new CompareSailID());
        }        

        public class CompareSailID : IComparer<TEntry>
        {
            public virtual int Compare(TEntry left, TEntry right)
            {
                if (left == null && right == null)
                {
                    return 0;
                }

                if (left == null)
                {
                    return - 1;
                }

                if (right == null)
                {
                    return 1;
                }

                return left.CompareSailID(right.SailID);
            }
        }

    }
}