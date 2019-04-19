using System.Windows.Controls;

namespace RiggVar.FR
{

    public class TGridModel<G, B, N, C, I, PC, PI> : TGridCells<string>, IColGrid
        where G : TColGrid<G, B, N, C, I, PC, PI>, new()
        where B : TBaseColBO<G, B, N, C, I, PC, PI>, new()
        where N : TBaseNode<G, B, N, C, I, PC, PI>, new()
        where C : TBaseRowCollection<G, B, N, C, I, PC, PI>, new()
        where I : TBaseRowCollectionItem<G, B, N, C, I, PC, PI>, new()
        where PC : TBaseColProps<G, B, N, C, I, PC, PI>, new()
        where PI : TBaseColProp<G, B, N, C, I, PC, PI>, new()
    {
        internal TGridController<G, B, N, C, I, PC, PI> g;

        public TGridModel(Grid cc) : base()
        {
            g = new TGridController<G, B, N, C, I, PC, PI>(cc);
            DefaultValue = "";
        }

        public int ColCount
        {
            get => g.fColCount;
            set => g.fColCount = value;
        }

        public int RowCount
        {
            get => g.fRowCount + 1;
            set => g.RowCount = value - 1;
        }

        public int Col
        {
            get => g.Col;
            set => g.Col = value;
        }

        public int Row
        {
            get => g.Row;
            set => g.Row = value;
        }

        /// <summary>
        /// Get and Set the Width of a Column.
        /// </summary>
        /// <param name="ACol">the column</param>
        /// <returns></returns>
        public int this[int ACol]
        {
            get => 60;
            set
            {
            }
        }

        public bool HasFixedRow
        {
            get => g.fHasFixedRow;
            set
            {
            }
        }

        public bool Enabled
        {
            get => g.fIsEnabled;
            set => g.fIsEnabled = value;
        }

        public bool IsEditorMode
        {
            get => g.fEditorMode;
            set => g.EditorMode = value;
        }

        public object DataSet
        {
            get => null;
            set
            {
            }
        }

        public void ClearRow(int ARow)
        {
            for (int i = 0; i < g.fColCount; i++)
            {
                this[i, ARow] = "";
            }
        }

        public void CancelEdit()
        {
            g.EditorMode = false;
        }

        public void UpdateInplace(string Value)
        {
            //g.CurrentCell.Value = Value; //Stack-Overflow, because it triggers CellValuePushed in EditorMode
        }

        public void SetupGrid(object AColGrid)
        {
            //if (Enabled)
            {
                g.Init((G)AColGrid);
            }
        }

        public void ShowData()
        {
            StatusCache.GridUpdateCounter++;
            g.ShowData();
        }

        public void ShowColor()
        {
            g.UpdateColor();
        }

        public int GetBaseID(int ARow)
        {
            string s = this[0, ARow];
            return int.Parse(s);
        }

        public void UpdateCellDecorations(object AColGrid)
        {
        }

        public void BackupCurrentCell()
        {
        }

        public void RestoreCurrentCell()
        {
        }

    }

}
