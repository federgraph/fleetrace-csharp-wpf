using System.Collections.Generic;
using System.Diagnostics;

namespace RiggVar.FR
{
    public class TCollectionItem<C, I>
        where C : TCollection<C, I>
        where I : TCollectionItem<C, I>, new()
    {
        private static int SID = -1;
        public C Collection;

        public TCollectionItem()
        {
            SID++;
            ID = SID;
        }

        public int ID { [DebuggerStepThrough]
            get; }

        public int Index
        {
            get
            {
                Debug.Assert(Collection != null);
                return Collection.IndexOfRow((I)this);
            }
        }

        /// <summary>
        /// dispose of additional resources added in subclass
        /// </summary>
        protected virtual void Dispose()
        {
        }

        public void Delete()
        {
            Dispose();
            if (Collection != null)
            {
                Collection.DeleteRow(Index);
            }
        }

    }

    public class TCollection<C, I> : List<I>
        where C : TCollection<C, I>
        where I : TCollectionItem<C, I>, new()
    {

        public TCollection() : base()
        { 
        }

        private I NewItem()
        {
            I o = new I();
            o.Collection = (C) this;
            return o;
        }

        public virtual I AddRow()
        {
            I cr = NewItem();
            Add(cr);
            return cr;
        }

        public I InsertRow(int index)
        {
            I cr = NewItem();
            Insert(index, cr);
            return cr;
        }

        public void DeleteRow(int index)
        {
            if (index > -1)
            {
                RemoveAt(index);
            }
            //else
            //Debug.WriteLine("DeleteRow index -1"); //happens when collection is finalized
        }

        public int IndexOfRow(I row)
        {
            if (row == null)
            {
                return -1; //can happen when collection is finalized
            }

            return IndexOf(row);
        }

        public new I this[int Index]
        {
            [DebuggerStepThrough]
            get => (Index < 0) || (Index >= Count) ? null : (I)base[Index];
            [DebuggerStepThrough]
            set
            {
                if ((Index < 0) || (Index >= Count))
                {
                    base[Index] = value;
                }
            }
        }

    }

}
