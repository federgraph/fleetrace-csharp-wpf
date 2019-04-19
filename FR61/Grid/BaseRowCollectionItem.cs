using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace RiggVar.FR
{
    /// <summary>
    /// row object, BaseID is primary key
    /// </summary>
    public class TBaseRowCollectionItem<G, B, N, C, I, PC, PI> : IEditableObject
        where G : TColGrid<G, B, N, C, I, PC, PI>, new()
        where B : TBaseColBO<G, B, N, C, I, PC, PI>, new()
        where N : TBaseNode<G, B, N, C, I, PC, PI>, new()
        where C : TBaseRowCollection<G, B, N, C, I, PC, PI>, new()
        where I : TBaseRowCollectionItem<G, B, N, C, I, PC, PI>, new()
        where PC : TBaseColProps<G, B, N, C, I, PC, PI>, new()
        where PI : TBaseColProp<G, B, N, C, I, PC, PI>, new()
    {
        private static int SID = -1;
        protected I fBackupData;

        public TBaseRowCollectionItem()
        {
            SID++;
            ID = SID;
        }

        public int BaseID { get; set; }

        public string Col_BaseID
        {
            [DebuggerStepThrough]
            get => BaseID.ToString();
        }

        [DebuggerStepThrough]
        public bool IsInFilter()
        {
            return true;
        }

        public virtual void ClearList()
        {
        }

        public virtual void ClearResult()
        {
        }

        public virtual void UpdateCellProp(PI cp, TCellProp cellProp)
        {
            cellProp.Color = ColumnToColorDef(cp, cellProp.Color); 
        }

        /// <summary>
        /// retrieves custom color for a table cell
        /// </summary>
        /// <param name="aColName">name of column</param>
        /// <param name="aColor">default color</param>
        /// <returns>the color of the cell</returns>
        public virtual Color ColumnToColorDef(PI cp, Color aColor)
        {
            return aColor;
        }


        public virtual void Assign(I source)
        {
            //virtual
        }

        public int ID { [DebuggerStepThrough]
            get; }

        public C Collection { [DebuggerStepThrough]
            get; set; }

        /// <summary>
        /// Get the Index of this row in the collection.
        /// </summary>
        public int IndexOfRow
        {
            [DebuggerStepThrough]
            get => Collection.IndexOfRow((I)this);
        }

        /// <summary>
        /// Ask the collection to delete this row.
        /// </summary>
        public void Delete()
        {
            Collection.DeleteRow(IndexOfRow);
        }

        /// <summary>
        /// Set the <code>Modified</code> flag on the Node.
        /// </summary>
        public bool Modified
        {
            set
            {
                //ru.Modified = value;
                if (Collection != null && Collection.BaseNode != null)
                {
                    Collection.BaseNode.Modified = value;
                }
            }
        }

        /// <summary>
        /// shortcut to Collection
        /// </summary>
        public C Cl
        {
            get
            {
                return Collection;
            }
        }

        /// <summary>
        /// shortcut to Node
        /// </summary>
        public N Ru => Collection.BaseNode;

        /// <summary>
        /// not used, no override needed
        /// </summary>
        /// <returns>null</returns>
        protected virtual I BackupData()
        {
            return null;
        }

        /// <summary>
        /// not used, no override needed
        /// </summary>
        protected virtual void RestoreData()
        {
            if (fBackupData != null)
            {
                Assign(fBackupData);
            }
        }

        protected int GetIndex()
        {
            return Ru.Collection.IndexOf((I)this);
        }

        #region IEditableObject Member

        [DebuggerStepThrough]
        void IEditableObject.BeginEdit()
        {
                Debug.WriteLine("  BaseRowCollectionItem.BeginEdit() - ID={0}", GetIndex());
            }

        [DebuggerStepThrough]
        void IEditableObject.CancelEdit()
        {
            Debug.WriteLine("  BaseRowCollectionItem.CancelEdit() - ID={0}", GetIndex());
        }

        [DebuggerStepThrough]
        void IEditableObject.EndEdit()
        {
            Debug.WriteLine("  BaseRowCollectionItem.EndEdit() - ID={0}",  GetIndex());
        }

        #endregion

    }
}
