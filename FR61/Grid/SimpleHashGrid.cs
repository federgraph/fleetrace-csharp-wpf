using System.Collections;
using System.Diagnostics;

namespace RiggVar.FR
{
    /// <summary>
    /// Minimal implementation of IColGrid
    /// </summary>
    public class TSimpleHashGrid : Hashtable, IColGrid
    {

        #region IColGrid Member

        public string this[int ACol, int ARow]
        {
            [DebuggerStepThrough]
            get
            {
                string s = ACol.ToString() + "_" + ARow.ToString();
                object o = base[s];
                if (o is string)
                {
                    return o as string;
                }
                else
                {
                    return "";
                }
            }
            [DebuggerStepThrough]
            set
            {
                string s = ACol.ToString() + "_" + ARow.ToString();
                if (ContainsKey(s))
                {
                    this[s] = value;
                }
                else
                {
                    Add(s, value);
                }
            }
        }
        public int ColCount { get; set; }
        public int RowCount { get; set; }
        public int Col { get; set; }
        public int Row { get; set; }
        public int this[int ACol] //ColWidth
        {
            get
            {
                return 35;
                //return fColWidth[ACol];
            }
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
            get
            {
                return null;
            }
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

        #endregion
    }
}
