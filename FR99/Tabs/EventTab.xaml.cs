using System;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using RiggVar.FR;

namespace FR73.Tabs
{
    public partial class EventTab : UserControl
    {

        internal bool waiting;

        private int GridUpdateCounter;
        internal TGridUpdate GridUpdate;
        private TEventColGrid ColGrid;
        private TEventGridModel GridModel;
        private TEventBO ColBO;
        private TEventNode Node;
        private IColGrid Model;

        public EventTab()
        {
            InitializeComponent();
            InitCommands();
            GridUpdate = new TGridUpdate();
            InitGrid();
        }

        private void InitCommands()
        {
            PointsBtn.Click += new RoutedEventHandler(ToolBar_Click);
            FinishBtn.Click += new RoutedEventHandler(ToolBar_Click);
            StrictBtn.Click += new RoutedEventHandler(ToolBar_Click);
            DollarBtn.Click += new RoutedEventHandler(ToolBar_Click);
            ThrowoutMinusBtn.Click += new RoutedEventHandler(ToolBar_Click);
            ThrowoutPlusBtn.Click += new RoutedEventHandler(ToolBar_Click);
            UpdateBtn.Click += new RoutedEventHandler(ToolBar_Click);
            UndoBtn.Click += new RoutedEventHandler(ToolBar_Click);
            RedoBtn.Click += new RoutedEventHandler(ToolBar_Click);
            ColorBtn.Click += new RoutedEventHandler(ToolBar_Click);
            SelectRaceBtn.Click += new RoutedEventHandler(ToolBar_Click);
        }

        private void ToolBar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (e.OriginalSource == this.PointsBtn)
                {
                    TMain.BO.EventNode.ShowPoints = TEventNode.Layout_Points;
                    InvalidateView();
                }
                else if (e.OriginalSource == this.FinishBtn)
                {
                    TMain.BO.EventNode.ShowPoints = TEventNode.Layout_Finish;
                    InvalidateView();
                }
                else if (e.OriginalSource == this.StrictBtn)
                {
                    TMain.BO.EventBO.RelaxedInputMode = !TMain.BO.EventBO.RelaxedInputMode;
                    UpdateStrictRelaxed();
                }
                else if (e.OriginalSource == this.DollarBtn)
                {
                    SwapRaceEnabled();
                }
                else if (e.OriginalSource == this.ThrowoutMinusBtn)
                {
                    NumberOfThrowoutsChanged(TMain.BO.EventProps.Throwouts - 1);
                }
                else if (e.OriginalSource == this.ThrowoutPlusBtn)
                {
                    NumberOfThrowoutsChanged(TMain.BO.EventProps.Throwouts + 1);
                }
                else if (e.OriginalSource == this.UpdateBtn)
                {
                    TMain.BO.EventNode.ErrorList.CheckAll(TMain.BO.EventNode);
                    DoUpdateGrid();
                }
                else if (e.OriginalSource == this.UndoBtn)
                {
                    TMain.GuiManager.AcUndoExecute();
                }
                else if (e.OriginalSource == this.RedoBtn)
                {
                    TMain.GuiManager.AcRedoExecute();
                }
                else if (e.OriginalSource == this.ColorBtn)
                {
                    TMain.GuiManager.AcColorCycleExecute();
                }
                else if (e.OriginalSource == this.SelectRaceBtn)
                {
                    TMain.GuiManager.Race = GetSelectedRaceIndex();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                //Memo.Text = ex.Message;
            }
        }

        private TEventNode GetBaseNode()
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

            ColBO = TMain.BO.EventBO;
            Node = TMain.BO.EventNode;
            ColBO.CurrentNode = Node;

            UpdateRelaxedBtnText();
            UpdateFinishBtnText();
            UpdateColorBtnText();

            GridModel = new TEventGridModel(EventView);
            Model = GridModel;

            ColGrid = new TEventColGrid
            {
                Grid = Model,
                Name = "EventGrid",
                OnGetBaseNode = new TEventColGrid.TGetBaseNodeFunction(GetBaseNode)
            };
            ColGrid.SetColBOReference(ColBO);
            ColGrid.ColsAvail.Init();
            ColBO.InitColsActiveLayout(ColGrid, 0);

            ColGrid.AlwaysShowCurrent = true;
            ColGrid.UseHTML = true;
            ColGrid.AutoMark = true;

            ColGrid.OnBaseSelectCell = new TEventColGrid.TMyCellSelectEvent(CellSelected);
            ColGrid.OnFinishEdit = new TNotifyEvent(EditFinished);
            GridUpdate.OnDraw = new EventHandler(DrawView);
            GridUpdate.OnUpdate = new EventHandler(UpdateView);
            GridUpdate.Enabled = true;

            ColBO.RelaxedInputMode = false;
            ColGrid.ColorPaint = true; //causes full update
        }

        internal void DisposeGrid()
        {
            GridUpdate.Enabled = false;
            EventView.Children.Clear();
            ColGrid = null;
            GridModel = null;
            Node = null;
            ColBO = null;
        }

        internal void DoUpdateGrid()
        {
            GridUpdateCounter++;
            GridUpdate.NeedFullUpdate = false;
            TMain.BO.OnIdle();
            ColGrid.UpdateAll();
        }

        private void InvalidateView()
        {
            if (ColGrid != null)
            {
                ColGrid.ShowData();
                UpdateFinishBtnText();
                UpdateRelaxedBtnText();
                UpdateUndoBtnText();
            }
        }

        private void DrawView(object sender, EventArgs target)
        {
            if (waiting)
            {
                return;
            }

            waiting = true;
            try
            {
                InvalidateView();
            }
            finally
            {
                waiting = false;
            }
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

        private bool NumberOfThrowoutsChanged(int value)
        {
            if (TMain.BO == null)
            {
                return false;
            }
            else if (value >= TMain.BO.BOParams.RaceCount)
            {
                return false;
            }
            else if (value < 0)
            {
                return false;
            }
            else if (TMain.BO.EventProps.Throwouts != value)
            {
                TMain.BO.EventProps.Throwouts = value;
                Node.Modified = true;
                DoUpdateGrid();
            }
            return true;
        }

        private void UpdateRelaxedBtnText()
        {
            if (TMain.BO.EventBO.RelaxedInputMode)
            {
                StrictBtn.Content = "Relaxed";
            }
            else
            {
                StrictBtn.Content = "Strict";
            }
        }

        private void UpdateFinishBtnText()
        {
            if (TMain.BO.EventNode.ShowPoints == 0)
            {
                FinishBtn.Content = "Finish";
                PointsBtn.Content = "*Points";
            }
            else
            {
                FinishBtn.Content = "*Finish";
                PointsBtn.Content = "Points";
            }
        }

        private void UpdateUndoBtnText()
        {
        }

        internal int GetSelectedRaceIndex()
        {
            int c = Model.Col;
            int r = Model.Row;
            string sColCaption = Model[c, 0];
            string sPrefix = "R";
            int RaceIndex = -1;
            if (Utils.Copy(sColCaption, 1, sPrefix.Length) == sPrefix)
            {
                string sRaceIndex = Utils.Copy(sColCaption, sPrefix.Length + 1, sColCaption.Length);
                RaceIndex = Utils.StrToIntDef(sRaceIndex, -1);
            }
            return RaceIndex;
        }

        private void SwapRaceEnabled()
        {
            int r = GetSelectedRaceIndex();
            if (r != -1)
            {
                TEventRowCollectionItem cr = Node.Collection[Model.Row - 1];
                if (cr != null)
                {
                    string sValue = "$";
                    TMain.BO.EventBO.EditRaceValue(cr, ref sValue, "colR_" + Utils.IntToStr(r));
                    GridUpdate.UpdateNow();
                }
            }
        }

        internal void UpdateStrictRelaxed()
        {
            if (TMain.BO.EventBO.RelaxedInputMode)
            {
                StrictBtn.Content = "Relaxed";
            }
            else
            {
                StrictBtn.Content = "Strict";
            }
        }

        internal void UpdateColorBtnText()
        {
            switch (TMain.BO.EventNode.ColorMode)
            {
                case TColorMode.ColorMode_None: ColorBtn.Content = "Color N"; break;
                case TColorMode.ColorMode_Error: ColorBtn.Content = "Color E"; break;
                case TColorMode.ColorMode_Fleet: ColorBtn.Content = "Color F"; break;
            }
        }

        internal void UpdateColorMode()
        {
            UpdateColorBtnText();
            ColGrid.UpdateAll();
        }

    }

}
