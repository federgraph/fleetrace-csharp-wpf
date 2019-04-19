namespace RiggVar.FR
{
    /// <summary>
    /// Minimal implementation of IColGrid
    /// </summary>
    public class TSimpleCellGrid<G, B, N, C, I, PC, PI> : TGridCells<string>, IColGrid
        where G : TColGrid<G, B, N, C, I, PC, PI>, new()
        where B : TBaseColBO<G, B, N, C, I, PC, PI>, new()
        where N : TBaseNode<G, B, N, C, I, PC, PI>, new()
        where C : TBaseRowCollection<G, B, N, C, I, PC, PI>, new()
        where I : TBaseRowCollectionItem<G, B, N, C, I, PC, PI>, new()
        where PC : TBaseColProps<G, B, N, C, I, PC, PI>, new()
        where PI : TBaseColProp<G, B, N, C, I, PC, PI>, new()
    {
        public TSimpleCellGrid() : base()
        {
            DefaultValue = "";
        }

        public int ColCount { get; set; }

        public int RowCount { get; set; }

        public int Col { get; set; }

        public int Row { get; set; }

        public int this[int ACol] //ColWidth
        {
            get => 35; //return fColWidth[ACol];
            set
            {
                //fColWidth[ACol] = value;
            }
        }

        public bool HasFixedRow { get; set; }

        public bool Enabled { get; set; }

        public bool IsEditorMode { get; set; }

        public void ClearRow(int ARow)
        {
            for (int i = 0; i < ColCount; i++)
            {
                this[i, ARow] = "";
            }
        }

        public void CancelEdit()
        {
            IsEditorMode = false;
        }

        public void UpdateInplace(string Value)
        {
            this[Col, Row] = Value;
        }

        public virtual void SetupGrid(object colGrid)
        {
        }

        public virtual void ShowData()
        {
        }

        public virtual object DataSet
        {
            get => null;
            set
            {
            }
        }

        public virtual int GetBaseID(int ARow)
        {
            return -1;
        }

        public void UpdateCellDecorations(object AColGrid)
        {
        }

        public virtual void BackupCurrentCell()
        {
        }

        public virtual void RestoreCurrentCell()
        {
        }

    }

}
