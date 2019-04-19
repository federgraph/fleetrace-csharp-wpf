using System.Drawing;
using System;
using System.Diagnostics;
using System.Text;

namespace RiggVar.FR
{

    public class TColGrid<G, B, N, C, I, PC, PI>
        where G : TColGrid<G, B, N, C, I, PC, PI>, new()
        where B : TBaseColBO<G, B, N, C, I, PC, PI>, new ()
        where N : TBaseNode<G, B, N, C, I, PC, PI>, new()
        where C : TBaseRowCollection<G, B, N, C, I, PC, PI>, new()
        where I : TBaseRowCollectionItem<G, B, N, C, I, PC, PI>, new()
        where PC : TBaseColProps<G, B, N, C, I, PC, PI>, new()
        where PI : TBaseColProp<G, B, N, C, I, PC, PI>, new()
    {

        public delegate void TBaseSetTextEvent(I cr, string Value);
        public delegate void TBaseGetTextEvent(I cr, ref string Value);
        public delegate void TBaseGetTextEvent2(I cr, ref string Value, string ColName);
        public delegate void TFinishEditCREvent(I cr);

        public delegate N TGetBaseNodeFunction();

        public delegate void TMyCellClickEvent(object Sender, int ACol, int ARow);
        public delegate void TMyKeyEvent(object Sender, ref int Key, ref int myShift);
        public delegate void TMyCellSelectEvent(object Sender, int ACol, int ARow, ref bool CanSelect);

        public static string crlf = Environment.NewLine;
        public bool IsCollectionGrid;

        public string Name = "";
        public IColGrid Grid = null;

        public TCellProps CellProps = null;
        private readonly PC ColsActiveDefault;
        private PC fColsAvail; //use assign

        protected TTraceProcedure fOnTrace;
        private TColGridColorSchema fColorSchema = TColGridColorSchema.colorRed;
        
        internal Color fDefaultColor;
        internal Color fAlternatingColor;
        internal Color fFocusColor;
        internal Color fEditableColor;
        internal Color fAlternatingEditableColor;
        internal Color fCurrentColor;
        internal Color fTransColor;
        private bool fAutoDelete;
        private bool fMenuMode;
        private int fHeaderRowIndex = 0;
        private int FirstRowIndex = 1;

        public TColGrid()
        {
            ColorSchema = TColGridColorSchema.colorRed;
            CellProps = new TCellProps();
            DisplayOrder = new TDisplayOrderList();

            //fColBODefault = new B(); //usually created in TMain.BO
            //fBaseColBO = fColBODefault; //no longer available

            fColsAvail = new PC
            {
                Grid = (G)this
            };
            //fColsAvail.Init(); //must be done after assignment of ColBO (where TColGrid is configured)

            ColsActiveDefault = new PC
            {
                Grid = (G)this
            };
            ColsActive = ColsActiveDefault;

            AddColumn("col_BaseID");
        }

        [DebuggerStepThrough]
        public int StrToIntDef(string s, int def)
        {
            try
            {
                if (s == string.Empty)
                {
                    return def;
                }

                if (s == null)
                {
                    return def;
                }

                return int.Parse(s);
            }
            catch 
            {
                return def;
            }
        }

        [DebuggerStepThrough]
        public bool Odd(int i)
        {
            return i % 2 == 0 ? false : true;
        }

        [DebuggerStepThrough]
        public N GetBaseNode()
        {
            if (OnGetBaseNode != null)
            {
                return (N) OnGetBaseNode();
            }

            return default(N);
        }

        protected B ColBO { [DebuggerStepThrough]
            get; private set;
        }

        public int HeaderRowIndex
        {
            [DebuggerStepThrough]
            get => fHeaderRowIndex;
            [DebuggerStepThrough]
            set
            {
                fHeaderRowIndex = value;
                FirstRowIndex = value + 1;
            }
        }

        public Color FocusColor => fFocusColor;

        public bool ExcelStyle { get; set; }

        public bool MenuMode
        {
            get => fMenuMode;
            set
            {
                fMenuMode = value;
                if (fMenuMode)
                {
                    Grid.HasFixedRow = false;
                    HeaderRowIndex = -1;
                }
                else
                {
                    Grid.HasFixedRow = true;
                    HeaderRowIndex = 0;
                }
            }
        }

        public TDisplayOrderList DisplayOrder { get; }

        public bool UseHTML { get; set; }

        public int HeatSize { get; set; } = 1;

        public bool AutoInsert { get; set; }

        public bool AutoDelete
        {
            get => AutoInsert;
            set => fAutoDelete = value;
        }

        public bool AutoMark { get; set; }

        public TColGridColorSchema ColorSchema
        {
            get => fColorSchema;
            set
            {
                if (value == TColGridColorSchema.colorMoneyGreen)
                {
                    fColorSchema = TColGridColorSchema.colorMoneyGreen;
                    fDefaultColor = TColGridColors.clCream;
                    fAlternatingColor = TColGridColors.clMoneyGreen;
                    fFocusColor = Color.Yellow;
                    fEditableColor = TColGridColors.clEditable;
                    fAlternatingEditableColor = TColGridColors.clEditable;
                    fCurrentColor = TColGridColors.clHellRot;
                    fTransColor = TColGridColors.clTransRot;
                }
                else if (value == TColGridColorSchema.color256)
                {
                    fColorSchema = TColGridColorSchema.color256;
                    fDefaultColor = Color.White;
                    fAlternatingColor = TColGridColors.clBtnFace;
                    fFocusColor = Color.Teal;
                    fEditableColor = Color.Yellow;
                    fAlternatingEditableColor = Color.Yellow;
                    fCurrentColor = Color.Red;
                    fTransColor = Color.Red;
                }
                else if (value == TColGridColorSchema.colorBlue)
                {
                    fColorSchema = TColGridColorSchema.colorBlue;
                    fDefaultColor = TColGridColors.clNormal;
                    fAlternatingColor = TColGridColors.clAlternate;
                    fFocusColor = Color.Lime;
                    fEditableColor = TColGridColors.clEditable;
                    fAlternatingEditableColor = TColGridColors.clEditable;
                    fCurrentColor = TColGridColors.clHellBlau;
                    fTransColor = TColGridColors.clTransBlau;
                }
                else
                {
                    fColorSchema = TColGridColorSchema.colorRed;
                    fDefaultColor = TColGridColors.clAlternate;
                    fAlternatingColor = TColGridColors.clNormal;
                    fFocusColor = Color.Lime;
                    fEditableColor = TColGridColors.clEditable;
                    fAlternatingEditableColor = TColGridColors.clEditable;
                    fCurrentColor = TColGridColors.clHellRot;
                    fTransColor = TColGridColors.clTransRot;
                }
            }
        }

        public bool AlwaysShowCurrent { get; set; }

        public TMyCellClickEvent OnCellClick { get; set; }

        public TMyCellSelectEvent OnBaseSelectCell { get; set; }

        public TMyKeyEvent OnBaseKeyDown { get; set; }

        public TNotifyEvent OnMarkRow { get; set; }

        public TNotifyEvent OnBaseClearContent { get; set; }

        public TNotifyEvent OnBaseEdit { get; set; }

        public TNotifyEvent OnFinishEdit { get; set; }

        public TFinishEditCREvent OnFinishEditCR { get; set; }

        public TTraceProcedure OnTrace
        {
            get => fOnTrace;
            set => fOnTrace = value;
        }

        public TGetBaseNodeFunction OnGetBaseNode { get; set; }

        public PC ColsAvail
        {
            get => fColsAvail;
            set => fColsAvail.Assign(value);
        }

        public bool CellsBold { get; set; }

        public bool HeaderBold { get; set; } = true;

        public bool ColorPaint
        {
            get
            {
                return ColsActive.SortColIndex == -1;;
            }
            set
            {
                if (value)
                {
                    ColsActive.SortColIndex = -1;
                }
                else
                {
                    ColsActive.SortColIndex = 0;
                }

                UpdateAll();
            }
        }

        public PC ColsActive { get; private set; }

        public void SetColBOReference(B Value)
        {
            ColBO = Value;
        }

        public void SetColsActiveReference(PC Value)
        {
            Grid.CancelEdit();
            if (Value != null)
            {
                ColsActive = Value;
            }
            else
            {
                ColsActive = ColsActiveDefault;
            }

            UpdateAll();
        }

        public I GetRowCollectionItem(int row)
        {
            if (ColsActive.Count == 0)
            {
                return null;
            }

            C cl = GetBaseRowCollection();
            if (cl == null)
            {
                return null;
            }

            int BaseID = -1;
            if (IsCollectionGrid)
            {
                BaseID = Grid.GetBaseID(row - FirstRowIndex);
            }
            else if ((row > HeaderRowIndex) && (row <= cl.Count))
            {
                PI cp = ColsActive.ByName("col_BaseID");
                if (cp != null)
                {
                    BaseID = StrToIntDef(Grid[cp.Index, row], -1);
                }
            }
            return cl.FindBase(BaseID);
        }

        public override string ToString()
        {
            return ToString("");
        }

        public string ToString(string TableCaption)
        {
            StringBuilder SL = new StringBuilder();
            try
            {
                Content(SL, TableCaption);
                return SL.ToString();
            }
            catch
            {
                return "";
            }            
        }

        public void Content(StringBuilder SL, string aCaption)
        {
            //SL.Append("<html><head><title>StringGrid</title></head><body>");
            SL.Append("<table border=\"1\" width=\"100%\" cellspacing=\"0\" cellpadding=\"1\">");
            SL.Append(crlf);
            if (aCaption != "")
            {
                SL.Append("<caption>" + aCaption + "</caption>");
                SL.Append(crlf);
            }
            for (int r = 0; r < Grid.RowCount; r++)
            {
                SL.Append("<tr align=\"left\">");
                for (int c = 0; c < Grid.ColCount; c++)
                {
                    PI cp = ColsActive[c];
                    if (cp == null)
                    {
                        continue;
                    }

                    string s = Grid[c, r];
                    string sColor = CellProps[c, r].HTMLColor;
                    if (s == "")
                    {
                        s = "&nbsp;";
                    }

                    if (r == 0)
                    {
                        if (cp.Alignment == TColAlignment.taRightJustify)
                        {
                            SL.Append("<th align=\"right\">" + s + "</th>");
                        }
                        else
                        {
                            SL.Append("<th>" + s + "</th>");
                        }
                    }
                    else
                    {
                        if (cp.Alignment == TColAlignment.taRightJustify)
                        {
                            SL.Append("<td bgcolor=\"" + sColor + "\" align=\"right\">" + s + "</td>");
                        }
                        else
                        {
                            SL.Append("<td bgcolor=\"" + sColor + "\">" + s + "</td>");
                        }
                    }
                }
                SL.Append("</tr>");
                SL.Append(crlf);
            }
            SL.Append("</table>");
            //SL.Append("</body></html>");
        }

        public void ToggleColorPaint()
        {
            ColsActive.SortColIndex = ColorPaint ? 0 : -1;
            UpdateAll();
        }

        public void MarkRowCollectionItem()
        {
            //VK_F4 was pressed in Grid
            if (ColBO != null)
            {
                I cr = GetRowCollectionItem(Grid.Row);
                if (cr == ColBO.CurrentRow)
                {
                    ColBO.CurrentRow = null;
                }
                else
                {
                    ColBO.CurrentRow = cr;
                }

                ShowData();
                OnMarkRow?.Invoke(this);
            }
        }

        //delete single row
        public void DeleteRowCollectionItem()
        {
            C cl = null;
            if ((ColBO != null) && (ColBO.CurrentNode != null))
            {
                cl = ColBO.CurrentNode.Collection;
            }

            I cr1 = GetRowCollectionItem(Grid.Row);
            if (cr1 != null)
            {
                if (ColBO.CurrentRow == cr1)
                {
                    ColBO.CurrentRow = null;
                }

                cl.DeleteRow(cr1.IndexOfRow);
                for (int i = 0; i < cl.Count; i++)
                {
                    I cr = cl[i];
                    cr.BaseID = i + 1;
                }
                UpdateAll();
                if (cl.FilteredCount() == 0)
                {
                    Grid.Enabled = false;
                }
            }
        }

        public void InsertRowCollectionItem()
        {
            C cl = null;
            if ((ColBO != null) && (ColBO.CurrentNode != null))
            {
                cl = ColBO.CurrentNode.Collection;
            }

            I cr1 = GetRowCollectionItem(Grid.Row);
            if ((cl != null) && (cr1 != null))
            {
                cl.InsertRow(cr1.IndexOfRow);
                for (int i = 0; i < cl.Count; i++)
                {
                    I cr = cl[i];
                    cr.BaseID = i + 1;
                }
                UpdateAll();
                Grid.Enabled = false;
            }
        }

        public void AddRowCollectionItem()
        {
            C cl = null;
            if ((ColBO != null) && (ColBO.CurrentNode != null))
            {
                cl = ColBO.CurrentNode.Collection;
            }

            I cr1 = GetRowCollectionItem(Grid.Row);
            I cr = cl.AddRow();
            cr.BaseID = cl.Count;
            UpdateAll();
            Grid.Enabled = cl.FilteredCount() > 0;
        }

        public void ApplySort(int newSortColIndex)
        {
            ColsActive.SortColIndex = newSortColIndex;
            Grid.CancelEdit();
            Grid.BackupCurrentCell();
            InitDisplayOrder(ColsActive.SortColIndex);
            ShowData();
            Grid.RestoreCurrentCell();
        }

        public void UpdateAll()
        {
            if (Grid == null)
            {
                return;
            }

            Grid.CancelEdit();
            Grid.BackupCurrentCell();
            SetupGrid();
            InitDisplayOrder(ColsActive.SortColIndex);
            Grid.SetupGrid((G)this);
            ShowData();
            Grid.RestoreCurrentCell();
        }

        public void ShowData()
        {
            if (IsCollectionGrid)
            {
                ShowData2_Collection();
            }
            else
            {
                ShowData1();
            }

            if (UseHTML)
            {
                //depends on Strings already updated because it does lookup of BaseID
                InitCellProps();
            }

            Grid.ShowData();
        }

        private void ShowData2_Collection()
        {
            //should never be in edit mode
            Debug.Assert(Grid.IsEditorMode == false,
                "unexpected state: CollectionGrid in edit mode");

            C cl = GetBaseRowCollection();
            Debug.Assert(cl != null, "RowCollection null in Grid");
            if (cl != null)
            {
                //check RowCount, sollte immer stimmen, wenn schon Zeilen vorhanden sind
                if (Grid.RowCount != cl.Count + FirstRowIndex)
                {
                    if (cl.Count > 0)
                    {
                        Debug.Assert(Grid.RowCount == cl.Count + FirstRowIndex,
                            "unexpected state for Grid.RowCount");
                        Grid.RowCount = cl.FilteredCount() + FirstRowIndex;
                    }
                    else
                    {
                        Grid.RowCount = FirstRowIndex + 1;
                    }
                }
            }
        }

        /// <summary>
        /// Update all rows, call IGrid.ShowData().
        /// </summary>
        private void ShowData1()
        {
            if (Grid.IsEditorMode)
            {
                return;
            }

            C cl = GetBaseRowCollection();
            Debug.Assert(cl != null, "RowCollection null in Grid");
            if (cl != null)
            {
                //check RowCount
                if (Grid.RowCount != cl.Count + FirstRowIndex)
                {
                    if (cl.Count > 0)
                    {
                        Grid.RowCount = cl.FilteredCount() + FirstRowIndex;
                    }
                    else
                    {
                        Grid.RowCount = FirstRowIndex + 1;
                    }
                }
                //update all rows
                int r = FirstRowIndex - 1; //will be incremented before use
                I cr;
                for (int j = 0; j < cl.Count; j++)
                {
                    int i = j;
                    if ((DisplayOrder.Count == cl.FilteredCount()) && (j < DisplayOrder.Count))
                    {
                        i = DisplayOrder.GetByIndex(j);
                    }

                    if ((i < 0) || (i > cl.Count-1))
                    {
                        continue;
                    }

                    cr = cl[i];
                    if (cr.IsInFilter())
                    {
                        ++r;
                        if (ColsActive != null)
                        {
                            ColsActive.UpdateRow(Grid, r, cr);
                        }
                    }
                }
            }
        }

        public void SetupGrid()
        {
            C cl = GetBaseRowCollection();

            //init RowCount
            if ((cl != null ) && (cl.Count > 0))
            {
                Grid.RowCount = cl.FilteredCount() + FirstRowIndex;
            }
            else
            {
                Grid.RowCount = FirstRowIndex + 1;
            }

            //clear visible cells
            for (int i = HeaderRowIndex; i < Grid.RowCount + HeaderRowIndex; i++)
            {
                Grid.ClearRow(i);
            }

            //init width of columns, show captions
            ShowHeader();
        }

        public void UpdateCellDecorations()
        {
            Grid.UpdateCellDecorations((G)this);
        }

        public PI AddColumn(string aNameIndex)
        {
            //find column in ColsAvail and add to ColsActive
            PI cp = ColsAvail.ByName(aNameIndex);
            if (cp != null)
            {
                PI result = ColsActive.AddRow();
                result.Assign(cp);
                return result;
            }
            else
            {
                return null;
            }
        }

        public TCellProp InitCellProp(int ACol, int ARow)
        {
            N rd = GetBaseNode();
            if (rd == null)
            {
                return null;
            }

            try
            {
                PI cp = ColsActive[ACol];
                if (cp == null)
                {
                    return null;
                }

                bool IsSorted = ColsActive.SortColIndex != -1;
                return InitCellProp2(rd, cp, IsSorted, ACol, ARow);
            }
            catch
            {
                return null;
            }
        }

        public void CellSelect(PI cp, int ACol, int ARow, ref bool CanSelect)
        {
            OnBaseSelectCell?.Invoke(cp, ACol, ARow, ref CanSelect);
        }

        public void KeyDown(object Sender, ref int Key, ref int myShift)
        {
            OnBaseKeyDown?.Invoke(Sender, ref Key, ref myShift);
        }

        public void MouseDown(object Sender, int ACol, int ARow)
        {
            OnCellClick?.Invoke(this, ACol, ARow);
            //only consider click on header
            if (MenuMode || (ARow != 0))
            {
                return;
            }
            //if (IsCollectionGrid) return; //MouseDown not called from CollectionGrid
            InitDisplayOrder(ACol);
            //draw data rows
            ShowData();
        }

        public void InsertRow()
        {
            if (AutoInsert)
            {
                InsertRowCollectionItem();
            }
        }

        public void AddRow()
        {
            if (AutoInsert)
            {
                AddRowCollectionItem();
            }
        }

        public void DeleteRow()
        {
            if (AutoInsert)
            {
                DeleteRowCollectionItem();
            }
        }

        public void MarkRow()
        {
            if (AutoMark)
            {
                MarkRowCollectionItem();
            }
            else
            {
                OnMarkRow?.Invoke(this);
            }
        }

        public void ClearContent()
        {
            OnBaseClearContent?.Invoke(this);
        }

        public void FinishEdit(int ACol, int ARow, ref string Value)
        {
            PI cp = ColsActive[ACol];
            I cr;
            if ((cp != null) && (cp.OnFinishEdit != null))
            {
                cr = GetRowCollectionItem(ARow);
                if (cr != null)
                {
                    cp.OnFinishEdit(cr, ref Value);
                }
            }
            else if ((cp != null) && (cp.OnFinishEdit2 != null))
            {
                cr = GetRowCollectionItem(ARow);
                if (cr != null)
                {
                    cp.OnFinishEdit2(cr, ref Value, cp.NameID);
                }
            }
            Grid.UpdateInplace(Value);
            OnFinishEdit?.Invoke(this);

            if (OnFinishEditCR != null)
            {
                cr = GetRowCollectionItem(ARow);
                OnFinishEditCR(cr);
            }
        }

        public void BeginEdit()        
        {
            PI cp = ColsActive[Grid.Col];
            if ((cp != null) && cp.ReadOnly && (OnBaseEdit != null))
            {
                OnBaseEdit(this);
            }
        }

        protected void Trace(string s)
        {
            OnTrace?.Invoke(s);
        }

        private TCellProp InitCellProp2(
            N rd,
            PI cp,
            bool IsSorted,
            int ACol, int ARow)
        {

            if (rd == null)
            {
                return null;
            }

            if (cp == null)
            {
                return null;
            }

            try
            {
                TCellProp CellProp = CellProps[ACol, ARow];

                Color TempColor = TColGridColors.clDefault;
                TColGridColorClass ccc = TColGridColorClass.Blank;            

                bool IsNormalRow;
                if ((ARow > HeaderRowIndex) && (AlwaysShowCurrent || (IsSorted == false)))
                {
                    IsNormalRow = Odd((ARow + 1) / HeatSize);
                    //alternating row color
                    if (IsNormalRow)
                    {
                        TempColor = fDefaultColor;
                        ccc = TColGridColorClass.DefaultColor;
                    }
                    else
                    {
                        TempColor = fAlternatingColor;
                        ccc = TColGridColorClass.AlternatingColor;
                    }

                    //editable columns color
                    if (cp.ReadOnly == false)
                    {
                        if (IsNormalRow)
                        {
                            TempColor = fEditableColor;
                            ccc = TColGridColorClass.EditableColor;
                        }
                        else
                        {
                            TempColor = fAlternatingEditableColor;
                            ccc = TColGridColorClass.AlternatingEditableColor;
                        }
                    }

                    //current row color
                    I cr = GetRowCollectionItem(ARow);
                    if (cr != null)
                    {
                        if (ColBO.CurrentRow == cr)
                        {
                            if (cp.ReadOnly)
                            {
                                TempColor = fCurrentColor;
                                ccc = TColGridColorClass.CurrentColor;
                            }
                            else
                            {
                                TempColor = fTransColor;
                                ccc = TColGridColorClass.TransColor;
                            }
                        }
                        Color TempColor2 = TempColor;

                        CellProp.Color = TempColor;
                        cr.UpdateCellProp(cp, CellProp);
                        TempColor = CellProp.Color;

                        if (TempColor != TempColor2)
                        {
                            ccc = TColGridColorClass.CustomColor;
                        }
                    }
                }

                if ((ARow > HeaderRowIndex) && (AlwaysShowCurrent == false) && IsSorted)
                {
                    IsNormalRow = Odd((ARow + FirstRowIndex) / HeatSize);
                    //alternating row color
                    if (IsNormalRow)
                    {
                        TempColor = fDefaultColor;
                        ccc = TColGridColorClass.DefaultColor;
                    }
                    else
                    {
                        TempColor = fAlternatingColor;
                        ccc = TColGridColorClass.AlternatingColor;
                    }

                    //editable column color
                    if (cp.ReadOnly == false)
                    {
                        if (IsNormalRow)
                        {
                            TempColor = fEditableColor;
                            ccc = TColGridColorClass.EditableColor;
                        }
                        else
                        {
                            TempColor = fAlternatingEditableColor;
                            ccc = TColGridColorClass.AlternatingEditableColor;
                        }
                    }
                }

                if (ARow == HeaderRowIndex)
                {
                    TempColor = TColGridColors.clBtnFace;
                    ccc = TColGridColorClass.HeaderColor;
                }

                CellProp.HTMLColor = TColGridColors.HTMLColor(TempColor);
                CellProp.Color = TempColor;
                CellProp.Alignment = cp.Alignment;
                CellProp.ColorClass = ccc;
                return CellProp;
            }
            catch
            {
                return null;
            }
        }

        private void InitCellProps()
        {
            for (int c = 0; c < Grid.ColCount; c++)
            {
                N rd = GetBaseNode();
                PI cp = ColsActive[c];
                bool IsSorted = ColsActive.SortColIndex != -1;
                for (int r = 0; r < Grid.RowCount; r++)
                {
                    InitCellProp2(rd, cp, IsSorted, c, r);
                }
            }
        }

        public void InitDisplayOrder(int col)
        {

            //operate on sortable columns only
            PI cp = ColsActive[col];
            if ((cp == null) || (!cp.Sortable))
            {
                return;
            }

            //remember index of column object in ColsAvail
            ColsActive.SortColIndex = cp.Index;

            //get the Collection
            C cl = GetBaseRowCollection();
            if (cl == null)
            {
                return;
            }

            //update the display order
            DisplayOrder.Clear();
            I cr;
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                if (cr.IsInFilter())
                {
                    string sortkey = "";
                    cp.GetSortKey(cr, ref sortkey);
                    //bei Strings zum Beispiel keine Leadingzeros
                    if (cp.ColType == TColType.colTypeString)
                    {
                        DisplayOrder.Add2(sortkey, i);
                    }
                    else
                    {
                        DisplayOrder.Add2(sortkey.PadLeft(11, '0'), i);
                    }
                }
            }
        }

        public void ShowHeader()
        {
            // ColCount always >= 1, see TCustomGrid.SetColCount
            Grid.ColCount = ColsActive.VisibleCount;
            for (int i = 0; i < ColsActive.Count; i++)        
            {
                PI cp = ColsActive[i];
                if ((cp != null) && cp.Visible)
                {
                    Grid[i] = cp.Width;
                    if (!MenuMode)
                    {
                        Grid[i, 0] = cp.Caption;
                    }
                }
            }
        }

        protected C GetBaseRowCollection()
        {
            N rd = GetBaseNode();
            return rd?.Collection;
        }

    }

}
