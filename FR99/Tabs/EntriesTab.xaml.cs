using System;
using System.Windows;
using System.Windows.Controls;
using RiggVar.FR;

namespace FR73.Tabs
{
    public partial class EntriesTab : UserControl
    {
        internal bool waiting;

        private int GridUpdateCounter;
        internal TGridUpdate GridUpdate;
        private TStammdatenColGrid ColGrid;
        private TStammdatenGridModel GridModel;
        private TStammdatenBO ColBO;
        private TStammdatenNode Node;
        private IColGrid Model;

        public EntriesTab()
        {
            InitializeComponent();
            InitCommands();
            GridUpdate = new TGridUpdate();
            InitGrid();
        }

        private void InitCommands()
        {
            AddBtn.Click += new RoutedEventHandler(ToolBar_Click);
            ClearBtn.Click += new RoutedEventHandler(ToolBar_Click);
            UpdateBtn.Click += new RoutedEventHandler(ToolBar_Click);
        }

        private void ToolBar_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource == AddBtn)
            {
                TStammdatenRowCollectionItem cr = TMain.BO.StammdatenNode.Collection.AddRow();
                cr.SNR = cr.BaseID + 1000;
                DoUpdateGrid();
            }
            else if (e.OriginalSource == ClearBtn)
            {
                TStammdatenRowCollection cl = TMain.BO.StammdatenNode.Collection;
                cl.Clear();
                DoUpdateGrid();
            }
            else if (e.OriginalSource == UpdateBtn)
            {
                DoUpdateGrid();
            }
        }

        private TStammdatenNode GetBaseNode()
        {
            return Node;
        }

        private void CellSelected(object Sender, int ACol, int ARow, ref bool CanSelect)
        {
            GridUpdate.DelayUpdate();
        }

        private void EditFinished(object sender)
        {
            //note: cannot directly call DoUpdateGrid from here
            GridUpdate.ScheduleFullUpdate();
        }

        internal void InitGrid()
        {
            if (ColGrid != null)
            {
                return;
            }

            ColBO = TMain.BO.StammdatenBO;
            Node = TMain.BO.StammdatenNode;
            ColBO.CurrentNode = Node;

            GridModel = new TStammdatenGridModel(StammdatenView);
            Model = GridModel;
            ColGrid = new TStammdatenColGrid
            {
                Grid = Model,
                Name = "EntriesGrid",
                OnGetBaseNode = new TStammdatenColGrid.TGetBaseNodeFunction(GetBaseNode)
            };
            ColGrid.SetColBOReference(ColBO);
            ColGrid.ColsAvail.Init();
            ColBO.InitColsActive(ColGrid);

            ColGrid.AlwaysShowCurrent = true;
            ColGrid.UseHTML = true;
            ColGrid.AutoMark = true;
            ColGrid.ColorPaint = true;

            ColGrid.OnBaseSelectCell = new TStammdatenColGrid.TMyCellSelectEvent(CellSelected);
            ColGrid.OnFinishEdit = new TNotifyEvent(EditFinished);

            GridUpdate.OnUpdate = new EventHandler(UpdateView);

            ColGrid.Grid.Row = 1;
            ColGrid.Grid.Col = 0;
        }

        internal void DisposeGrid()
        {
            GridUpdate.Enabled = false;
            StammdatenView.Children.Clear();
            GridModel = null;
            ColGrid = null;
            Node = null;
            ColBO = null;
        }

        internal void DoUpdateGrid()
        {
            GridUpdateCounter++;
            ColGrid.UpdateAll();
        }

        private void UpdateView(object sender, EventArgs target)
        {
            if (waiting)
            {
                return;
            }

            waiting = true;
            try
            {
                DoUpdateGrid();
            }
            finally
            {
                waiting = false;
            }
        }

    }

}
