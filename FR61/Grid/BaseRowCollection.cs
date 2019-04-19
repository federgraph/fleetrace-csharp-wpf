using System.Collections.Generic;
using System.Diagnostics;

namespace RiggVar.FR
{
    public class TBaseRowCollection<G, B, N, C, I, PC, PI> : List<I>
        where G : TColGrid<G, B, N, C, I, PC, PI>, new()
        where B : TBaseColBO<G, B, N, C, I, PC, PI>, new()
        where N : TBaseNode<G, B, N, C, I, PC, PI>, new()
        where C : TBaseRowCollection<G, B, N, C, I, PC, PI>, new()
        where I : TBaseRowCollectionItem<G, B, N, C, I, PC, PI>, new()
        where PC : TBaseColProps<G, B, N, C, I, PC, PI>, new()
        where PI : TBaseColProp<G, B, N, C, I, PC, PI>, new()
    {

        public N FBaseNode;

        public TBaseRowCollection() : base()
        {
        }

        private I NewItem()
        {
            I o = new I
            {
                Collection = (C)this
            };
            return o;
        }

        public virtual I AddRow()
        {
            I cr = NewItem();
            Add(cr);
            cr.BaseID = IndexOf(cr) + 1;
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
            RemoveAt(index);
        }

        public int IndexOfRow(I row)
        {
            return IndexOf(row);
        }

        public N BaseNode
        {
            [DebuggerStepThrough]
            get => FBaseNode;
            set => FBaseNode = value;
        }

        new public I this[int index]
        {
            [DebuggerStepThrough]
            get => (index < 0) || (index >= Count) ? null : base[index];
            [DebuggerStepThrough]
            set
            {
                if ((index >= 0) && (index < Count))
                {
                    base[index] = value;
                }
            }
        }

        public virtual void ClearList()
        {
        }

        public virtual void ClearResult()
        {
            foreach (I cr in this)
            {
                cr.ClearResult();
            }
        }

        [DebuggerStepThrough]
        public virtual I FindBase(int BaseID)
        {
            for (int i = 0; i < Count; i++)
            {
                if (BaseID == this[i].BaseID)
                {
                    return this[i];
                }
            }
            return null;
        }

        public virtual int FilteredCount()
        {
            return Count;
        }

    }

}
