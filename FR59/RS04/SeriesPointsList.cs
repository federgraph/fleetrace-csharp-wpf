using System.Collections.Generic;
using System.Text;

namespace RiggVar.Scoring
{

    public class TSeriesPointsList : List<TSeriesPoints>
    {

        public TSeriesPointsList() : base()
        {
        }
                                
        /// <summary> 
        /// returns first (and hopefully only) value in list for entry
        /// </summary>
        public virtual TSeriesPoints FindPoints(TEntry e)
        {
            foreach (TSeriesPoints p in this)
            {
                if (p.EqualsWithNull(p.Entry, e))
                {
                    return p;
                }
            }
            return null;
        }
        
        public virtual TSeriesPointsList FindAllPoints(TEntry e)
        {
            TSeriesPointsList list = new TSeriesPointsList();
            foreach (TSeriesPoints p in this)
            {
                if ((p.Entry != null) && p.Entry.Equals(e))
                {
                    list.Add(p);
                }
            }
            return list;
        }

        /// <summary>
        /// generates a string of elements
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("splist=(");
            int i = 0;
            foreach (object o in this)
            {
                i++;
                sb.Append(o.ToString());
                if (i < Count)
                {
                    sb.Append(',');
                }
            }
            sb.Append(')');
            return sb.ToString();
        }

        /// <summary>
        /// clears old list, adds a new seriespoint for each entry to self
        /// </summary>
        /// <param name="entries">list of entries</param>
        /// <returns>copy of list</returns>        
        public virtual TSeriesPointsList InitPoints(TEntryList entries)
        {
            Clear();
            TSeriesPointsList list = new TSeriesPointsList();
            foreach (TEntry e in entries)
            {
                TSeriesPoints sp = new TSeriesPoints(e);
                Add(sp);
                list.Add(sp);
            }
            return list;
        }

        public virtual void SortPosition()
        {
            Sort(new ComparatorPosition());
        }
        
        public class ComparatorPosition: IComparer<TSeriesPoints>
        {
            public virtual int Compare(TSeriesPoints left, TSeriesPoints right)
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

                int ileft = left.Position;
                int iright = right.Position;
                
                if (ileft < iright)
                {
                    return - 1;
                }

                if (ileft > iright)
                {
                    return 1;
                }

                return 0;
            }
        }
        
        public virtual void SortPoints()
        {
            Sort(new ComparatorPoints());
        }
        
        public class ComparatorPoints: IComparer<TSeriesPoints>
        {
            public virtual int Compare(TSeriesPoints left, TSeriesPoints right)
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

                //need to compare by EntryID also, because Sort is not a stable sort,
                //order of elements is not preserved if elements are equal
                return left.CompareTo(right);
            }
        }
    }
}