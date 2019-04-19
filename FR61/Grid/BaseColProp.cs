using System.Diagnostics;

namespace RiggVar.FR
{
    public class TBaseColProp<G, B, N, C, I, PC, PI>
        where G : TColGrid<G, B, N, C, I, PC, PI>, new()
        where B : TBaseColBO<G, B, N, C, I, PC, PI>, new()
        where N : TBaseNode<G, B, N, C, I, PC, PI>, new()
        where C : TBaseRowCollection<G, B, N, C, I, PC, PI>, new()
        where I : TBaseRowCollectionItem<G, B, N, C, I, PC, PI>, new()
        where PC : TBaseColProps<G, B, N, C, I, PC, PI>, new()
        where PI : TBaseColProp<G, B, N, C, I, PC, PI>, new()
    {
        private static int SID = -1;
        private string fNameID;
        public int NumID; //for better performance of method GetTextDefault

        public TBaseColProp()
        {
            SID++;
            ID = SID;

            NameID = ""; //--> SetNameID
            Visible = true;
            Alignment = TColAlignment.taRightJustify;
            Width = 35;
            ReadOnly = true;
        }

        public TColumnType ColumnType { get; set; }

        public string BindingPath { get; set; }

        public int Width { get; set; }

        public bool Visible { get; set; }

        public bool Sortable { get; set; }

        public string NameID
        {
            [DebuggerStepThrough]
            get => fNameID;
            [DebuggerStepThrough]
            set
            {
                PC o = Collection;
                if ((value == "") || ((value != fNameID) && o.IsDuplicateNameID(value)))
                {
                    fNameID = "col_" + ID.ToString();
                    if ((Caption == "") || (Caption != fNameID))
                    {
                        Caption = value;
                    }
                }
                else
                {
                    if (Caption == fNameID)
                    {
                        Caption = value;
                    }

                    fNameID = value;
                }
            }
        }

        public string Caption { get; set; }

        public TColAlignment Alignment { get; set; }

        public bool ReadOnly { get; set; }

        public TColType ColType { get; set; }

        public bool Descending { get; set; }

        public TColGrid<G, B, N, C, I, PC, PI>.TBaseGetTextEvent OnGetSortKey { get; set; }

        public TColGrid<G, B, N, C, I, PC, PI>.TBaseGetTextEvent2 OnGetSortKey2 { get; set; }

        public TColGrid<G, B, N, C, I, PC, PI>.TBaseGetTextEvent OnGetText { get; set; }

        public TColGrid<G, B, N, C, I, PC, PI>.TBaseSetTextEvent OnSetText { get; set; }

        public TColGrid<G, B, N, C, I, PC, PI>.TBaseGetTextEvent OnFinishEdit { get; set; }

        public TColGrid<G, B, N, C, I, PC, PI>.TBaseGetTextEvent2 OnFinishEdit2 { get; set; }

        public virtual bool IsGroupCol()
        {
            return false;
        }

        public virtual void InitColsAvail()
        {
            //virtual
        }

        [DebuggerStepThrough]
        protected virtual void GetTextDefault(I cr, ref string Value)
        {
            Value = "";
            if (cr == null)
            {
                return;
            }

            if (NumID == 0) //(NameID == "col_BaseID")
            {
                Value = cr.BaseID.ToString();
            }
        }

        public string GetText(I cr)
        {
            string result = "";
            if (OnGetText != null)
            {
                OnGetText(cr, ref result);
            }
            else
            {
                GetTextDefault(cr, ref result);
            }

            return result;
        }

        [DebuggerStepThrough]
        public void GetSortKey(I cr, ref string SortKey)
        {
            SortKey = GetText(cr);

            //0 or blank must be sorted towards the bottom in some columns
            if ((ColType == TColType.colTypeRank) && ((SortKey == "0") || (SortKey == "")))
            {
                SortKey = (999 + cr.BaseID).ToString();
            }
            else if ((ColType == TColType.colTypeString) && (SortKey == ""))
            {
                SortKey = "ZZZ" + cr.BaseID.ToString().PadLeft(3, '0');
            }

            if (OnGetSortKey != null)
            {
                OnGetSortKey(cr, ref SortKey);
            }
            else
            {
                OnGetSortKey2?.Invoke(cr, ref SortKey, NameID);
            }
        }

        public void Assign(PI cp)
        {
            if (cp != null)
            {
                NameID = cp.NameID;
                Caption = cp.Caption;
                Width = cp.Width;
                Alignment = cp.Alignment;
                Visible = cp.Visible;
                Sortable = cp.Sortable;
                ColType = cp.ColType;
                NumID = cp.NumID;
                BindingPath = cp.BindingPath;
                ColumnType = cp.ColumnType;

                OnGetSortKey = cp.OnGetSortKey;
                OnGetSortKey2 = cp.OnGetSortKey2;
                OnGetText = cp.OnGetText;
                OnSetText = cp.OnSetText;
                ReadOnly = cp.ReadOnly;
                OnFinishEdit = cp.OnFinishEdit;
                OnFinishEdit2 = cp.OnFinishEdit2;
            }
        }

        public int ID { [DebuggerStepThrough]
            get; }

        public PC Collection { [DebuggerStepThrough]
            get; set; }

        public int Index
        {
            [DebuggerStepThrough]
            get => Collection.IndexOf((PI)this);
        }

        public void Delete()
        {
            Collection.DeleteRow(Index);
        }

    }

}
