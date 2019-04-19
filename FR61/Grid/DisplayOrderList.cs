using System;
using System.Collections.Generic;
using System.Linq;

namespace RiggVar.FR
{

    public class TDisplayOrderEntry : IComparable<TDisplayOrderEntry>, IEqualityComparer<TDisplayOrderEntry>
    {
        public int N { get; set; }
        public string K { get; set; }
        public int V { get; set; }

        public int CompareTo(TDisplayOrderEntry other)
        {
            int result = K.CompareTo(other.K);
            if (result == 0)
            {
                result = N.CompareTo(other.N);
            }

            return result;
        }

        public bool Equals(TDisplayOrderEntry x, TDisplayOrderEntry y)
        {
            return x.N == y.N;
        }

        public int GetHashCode(TDisplayOrderEntry obj)
        {
            return N;
        }
    }

    public class TDisplayOrderList 
    {
        private static int fID;
        private bool isSorted;
        private List<TDisplayOrderEntry> dol;
        private int[] doa;

        public TDisplayOrderList()
        {
            dol = new List<TDisplayOrderEntry>();
        }

        public void Add2(string aKey, int aValue)
        {
            try
            {
                fID++;
                TDisplayOrderEntry doe = new TDisplayOrderEntry
                {
                    N = fID,
                    K = aKey,
                    V = aValue
                };
                dol.Add(doe);
            }
            catch
            {
            }
        }

        public int GetByIndex(int j)
        {
            if (!isSorted)
            {
                DoSort();
            }

            if (j >= 0 && j < doa.Length)
            {
                return doa[j];
            }

            return -1;
        }

        public void Clear()
        {
            isSorted = false;
            dol.Clear();
            doa = null;
            fID = 0;
        }

        private void DoSort()
        {            
            dol.Sort();
            var a = from doe in dol select doe.V;
            doa = a.ToArray();
            isSorted = true;
        }

        public int Count => dol.Count;

    }

}
