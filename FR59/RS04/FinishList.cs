using System.Collections.Generic;

namespace RiggVar.Scoring
{

    public class TFinishList : List<TFinish>
    {
        public TFinishList() : base()
        {
        }

        /// <summary>
        /// returns a finish if found, otherwise null
        /// </summary>
        public virtual TFinish FindEntry(TEntry e)
        {
            if (Count == 0)
            {
                return null;
            }

            foreach (TFinish f in this)
            {
                if ((f.Entry != null) && f.Entry.Equals(e))
                {
                    return f;
                }
            }
            return null;
        }

    }
}