using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

namespace RiggVar.FR
{
    public class TGridController<G, B, N, C, I, PC, PI>
        where G : TColGrid<G, B, N, C, I, PC, PI>, new()
        where B : TBaseColBO<G, B, N, C, I, PC, PI>, new()
        where N : TBaseNode<G, B, N, C, I, PC, PI>, new()
        where C : TBaseRowCollection<G, B, N, C, I, PC, PI>, new()
        where I : TBaseRowCollectionItem<G, B, N, C, I, PC, PI>, new()
        where PC : TBaseColProps<G, B, N, C, I, PC, PI>, new()
        where PI : TBaseColProp<G, B, N, C, I, PC, PI>, new()
    {
        private Grid hg;
        private G ColGrid;
        internal DataGridView dg;

        internal int fRowCount = 2;
        internal int fColCount = 1;
        internal int fRow = 1;
        internal int fCol = 0;
        internal bool fEditorMode = false;
        internal bool fHasFixedRow = true;
        internal bool fIsEnabled = false;
        internal string fBackupValue = "";

        public TGridController()
        {
        }

        public TGridController(Grid host) : this()
        {
            hg = host;
        }

        internal void InitProps()
        {
            dg.VirtualMode = true;
            dg.AutoGenerateColumns = false;
            dg.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dg.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            dg.StandardTab = false;
            dg.RowHeadersVisible = false;
            dg.AllowUserToAddRows = false;
            dg.AllowUserToDeleteRows = false;
            dg.AllowUserToOrderColumns = false;
            dg.AllowUserToResizeRows = false;
            dg.AutoSize = false;
            dg.CausesValidation = false;
            dg.MultiSelect = false;
            dg.ReadOnly = false;

            dg.CellValueNeeded += new DataGridViewCellValueEventHandler(CellValueNeeded);
            dg.CellValuePushed += new DataGridViewCellValueEventHandler(CellValuePushed);
            dg.CellBeginEdit += new DataGridViewCellCancelEventHandler(CellBeginEdit);
            dg.CellEndEdit += new DataGridViewCellEventHandler(CellEndEdit);
            dg.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(Dg_ColumnHeaderMouseClick);
            dg.KeyUp += new KeyEventHandler(Dg_KeyUp);
        }
        
        private void CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            EditorMode = true;
        }

        private void CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            EditorMode = false;
        }

        private void CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            int r = e.RowIndex;
            int c = e.ColumnIndex;
            e.Value = ColGrid.Grid[c, r + 1];
        }

        private void CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (EditorMode)
            {
                if (!(e.Value is string v))
                {
                    v = string.Empty;
                }

                int r = e.RowIndex;
                int c = e.ColumnIndex;

                ColGrid.FinishEdit(c, r + 1, ref v);

                ColGrid.Grid[c, r + 1] = v;
                e.Value = v;
            }
        }

        private void Dg_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            ColGrid.ColsActive.SortColIndex = e.ColumnIndex;
            ColGrid.UpdateAll();
            dg.Invalidate();
        }

        private void Dg_KeyUp(object sender, KeyEventArgs e)
        {
            int c = dg.CurrentCell.ColumnIndex;
            int r = dg.CurrentCell.RowIndex;
            if (e.KeyCode == Keys.F3)
            {
                if (c > -1 && r > -1)
                {
                    ColGrid.ColsActive.SortColIndex = c;
                    ColGrid.UpdateAll();
                    dg.Invalidate();
                }
            }
            if (e.KeyCode == Keys.F4)
            {
                if (c > -1 && r > -1)
                {
                    fCol = c;
                    fRow = r;
                    ColGrid.MarkRow();
                    dg.Invalidate();
                }
            }
        }

        public void ShowData()
        {
            UpdateText();
            UpdateColor();
            dg.Invalidate();
        }

        public void UpdateColor()
        {
            DataGridViewCell cell;
            for (int r = 0; r < dg.RowCount; r++)
            {
                for (int c = 0; c < dg.ColumnCount; c++)
                {
                    cell = dg[c, r];
                    cell.Style.BackColor = ColGrid.CellProps[c, r + 1].Color;
                }
            }
        }

        public void UpdateText()
        {
            DataGridViewCell cell;
            for (int r = 0; r < dg.RowCount; r++)
            {
                for (int c = 0; c < dg.ColumnCount; c++)
                {
                    cell = dg[c, r];
                    cell.Value = ColGrid.Grid[c, r + 1];
                }
            }
        }

        internal DataGridViewCell CurrentCell
        {
            get
            {
                return dg.CurrentCell;
            }
        }

        internal DataGridViewCell this[int c, int r]
        {
            get
            {
                if (dg != null)
                {
                    if (r < dg.RowCount)
                    {
                        DataGridViewCell tb = dg[c, r];
                        if (tb is DataGridViewCell)
                        {
                            return tb;
                        }
                    }
                }
                return null;
            }
        }

        internal bool EditorMode
        {
            get => fEditorMode;
            set => fEditorMode = value;
        }

        public void Init(G AColGrid)
        {
            if (AColGrid == null)
            {
                return;
            }
            if (hg == null)
            {
                throw new ArgumentException("hosting control must not be null");
            }
            if (dg != null)
            {
                return;
            }

            ColGrid = AColGrid;

            PC ca = ColGrid.ColsActive;
            Debug.Assert(ca.Count == ColGrid.Grid.ColCount);

            fRowCount = ColGrid.Grid.RowCount - 1;
            fColCount = ColGrid.Grid.ColCount;

            dg = new DataGridView();
            InitProps();

            DataGridViewTextBoxColumn cd;
            for (int c = 0; c < fColCount; c++)
            {
                PI ci = ca[c];
                cd = new DataGridViewTextBoxColumn
                {
                    Name = ci.NameID,
                    HeaderText = ci.Caption,
                    Width = ci.Width
                };
                if (ci.ReadOnly)
                {
                    cd.ReadOnly = true;
                }

                if (ci.ColType != TColType.colTypeString)
                {
                    cd.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                dg.Columns.Add(cd);
            }

            //Debug.Assert(fRowCount == colGrid.GetBaseNode().Collection.Count);
            dg.RowCount = fRowCount;
            
            dg.Dock = DockStyle.Fill;
            WindowsFormsHost wfh = new WindowsFormsHost();            
            hg.Children.Add(wfh);
            wfh.Child = dg;
        }

        public int RowCount
        {
            get => fRowCount;
            set
            {
                fRowCount = value;
                if (dg != null)
                {
                    dg.RowCount = fRowCount;
                }
            }
        }

        public int Row
        {
            get
            {
                if (dg != null)
                {
                    return dg.CurrentCell.RowIndex + 1;
                }
                else
                {
                    return fRow;
                }
            }
            set => fRow = value;
        }

        public int Col
        {
            get
            {
                if (dg != null)
                {
                    return dg.CurrentCell.ColumnIndex;
                }
                else
                {
                    return fCol;
                }
            }
            set => fCol = value;
        }

    }

}
