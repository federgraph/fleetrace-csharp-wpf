using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace RiggVar.FR
{
    public class TBaseColProps<G, B, N, C, I, PC, PI> : List<PI>
        where G : TColGrid<G, B, N, C, I, PC, PI>, new()
        where B : TBaseColBO<G, B, N, C, I, PC, PI>, new()
        where N : TBaseNode<G, B, N, C, I, PC, PI>, new()
        where C : TBaseRowCollection<G, B, N, C, I, PC, PI>, new()
        where I : TBaseRowCollectionItem<G, B, N, C, I, PC, PI>, new()
        where PC : TBaseColProps<G, B, N, C, I, PC, PI>, new()
        where PI : TBaseColProp<G, B, N, C, I, PC, PI>, new()
    {
        public bool UseCustomColCaptions;
        private int FSortColIndex;

        public TBaseColProps() : base()
        {
        }

        private PI NewItem()
        {
            PI cp = new PI
            {
                Collection = (PC)this
            };
            return cp;
        }

        public virtual PI AddRow()
        {
            PI cr = NewItem();
            Add(cr);
            return cr;
        }

        public PI InsertRow(int index)
        {
            PI cr = NewItem();
            Insert(index, cr);
            return cr;
        }

        public void DeleteRow(int index)
        {
            RemoveAt(index);
        }

        public int IndexOfRow(PI row)
        {
            return IndexOf(row);
        }

        /// <summary>
        /// Adds first ColProp for column col_BaseID 
        /// and calls virtual method InitColsAvail() on ColProp
        /// where more ColProps can be added.
        /// </summary>
        public void Init()
        {
            Clear();

            //always add BaseID column as primary key
            PI cp = AddRow();
            if (cp != null)
            {
                cp.NameID = "col_BaseID";
                cp.Caption = "ID";
                cp.Width = 25;
                cp.Sortable = true;
                cp.InitColsAvail(); //virtual
                cp.NumID = 0; //default
                cp.BindingPath = "ID";

                if (UseCustomColCaptions)
                {
                    InitCustomCaptions();
                }
            }
            else
            {
                Console.WriteLine("construction of TBaseColProp for BaseID failed.");
            }
        }

        public int SortColIndex
        {
            [DebuggerStepThrough]
            get
            {
                if ((FSortColIndex >= 0)
                    && (FSortColIndex < Count)
                    && (this[FSortColIndex] != null)
                    && ((this[FSortColIndex]).Sortable))
                {
                    return FSortColIndex;
                }
                else
                {
                    FSortColIndex = -1;
                    return FSortColIndex;
                }
            }
            [DebuggerStepThrough]
            set
            {
                if ((value >= 0) && (value < Count) && (this[value]).Sortable)
                {
                    FSortColIndex = value;
                }
                else
                {
                    FSortColIndex = -1;
                }
            }
        }

        [DebuggerStepThrough]
        public void Assign(PC source)
        {
            Clear();
            for (int i = 0; i < source.Count; i++)
            {
                AddRow().Assign(source[i]);
            }

            return;
        }

        [DebuggerStepThrough]
        public bool IsDuplicateNameID(string s)
        {
            foreach (PI cp in this)
            {
                if (cp.NameID == s)
                {
                    return true;
                }
            }
            return false;
        }

        public int VisibleCount
        {
            [DebuggerStepThrough]
            get
            {
                int result = 0;
                for (int i = 0; i < Count; i++)
                {
                    if ((this[i]).Visible)
                    {
                        result++;
                    }
                }

                return result;
            }
        }

        [DebuggerStepThrough]
        public void UpdateRow(IColGrid StringGrid, int ARow, I cr)
        {
            int i = 0;
            foreach (PI cp in this)
            {
                StringGrid[i, ARow] = cp.GetText(cr);
                i++;
            }
        }

        [DebuggerStepThrough]
        public PI ByName(string NameIndex)
        {
            //Enumerator wird jedesmal erzeugt, kann dies vermieden werden?
            foreach (PI cp in this)
            {
                if (cp.NameID == NameIndex)
                {
                    return cp;
                }
            }
            return null;
        }

        new public PI this[int index]
        {
            [DebuggerStepThrough]
            get
            {
                if ((index < 0) || (index >= Count))
                {
                    return null;
                }
                else
                {
                    return (PI)base[index];
                }
            }
            [DebuggerStepThrough]
            set
            {
                if ((index >= 0) && (index < Count))
                {
                    base[index] = value;
                }
            }
        }

        public string GridName
        {
            get
            {
                G cg = Grid;
                return cg != null ? cg.Name : "";
            }
        }

        public void InitCustomCaptions()
        {
            foreach (PI cp in this)
            {
                cp.Caption = GetCaptionOverride(cp);
            }
        }

        public string GetCaptionOverride(PI cp)
        {
            N bn;
            string key;
            string result = "";

            //first try, Grid specific search
            string gn = GridName;
            if (gn != "")
            {
                key = gn + "_" + cp.NameID;
                result = TColCaptions.ColCaptionBag.GetCaption(key);
            }

            //second try, Table specific search
            if (result == "")
            {
                if (Grid != null)
                {
                    bn = Grid.GetBaseNode();
                    if (bn != null)
                    {
                        if (bn.NameID != "")
                        {
                            key = bn.NameID + "_" + cp.NameID;
                            result = TColCaptions.ColCaptionBag.GetCaption(key);
                        }
                    }
                }
            }

            //third try, cross table, column name based
            if (result == "")
            {
                result = TColCaptions.ColCaptionBag.GetCaption(cp.NameID);
            }

            //else use default
            if (result == "")
            {
                result = cp.Caption;
            }

            return result;
        }

        public G Grid {
            [DebuggerStepThrough]
            get;
            [DebuggerStepThrough]
            set; }

    }

}
