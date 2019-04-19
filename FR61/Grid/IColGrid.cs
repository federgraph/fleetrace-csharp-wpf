namespace RiggVar.FR
{
    public interface IColGrid
    {
        string this[int ACol, int ARow] //Cells
        {
            get;
            set;
        }
        int ColCount
        {
            get;
            set;
        }
        int RowCount
        {
            get;
            set;
        }
        int Col
        {
            get;
            set;
        }
        int Row
        {
            get;
            set;
        }
        int this[int ACol] //ColWidth
        {
            get;
            set;
        }
        bool HasFixedRow
        {
            get;
            set;
        }
        bool Enabled
        {
            get;
            set;
        }
        bool IsEditorMode
        {
            get;
            set;
        }
        object DataSet
        {
            get;
            set;
        }
        void ClearRow(int ARow);
        void CancelEdit();
        void UpdateInplace(string Value);
        void SetupGrid(object AColGrid);
        void ShowData();
        int GetBaseID(int ARow);
        void UpdateCellDecorations(object AColGrid);
        void BackupCurrentCell();
        void RestoreCurrentCell();
    }
}
