using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Input;

using RiggVar.FR;
using FR73.Tabs;

namespace FR62
{

    public partial class TFormFR62 : Window, IGuiInterface
    {
        internal MenuTab TabMenu;
        EntriesTab TabEntries;
        internal TimingTab TabTiming;
        internal EventTab TabEvent;
        internal ReportTab TabReport;

        DispatcherTimer dt;
        protected int OnIdleCounter = 0;

        THandleMsgEvent FSynchronizedHandleBackup;

        public TFormFR62()
        {
            InitializeComponent();
        }

        #region Loading and Closing
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Width = 800;

            Program.Init();
            TMain.MainForm = this;
            TMain.GuiManager.GuiInterface = this;
            TMain.BO.Calc();

            LoadTabs();

            TabReport.Memo.Text = Controller.GetTestData();

            //Bridge
            InitBridge();
            FSynchronizedHandleBackup = new THandleMsgEvent(TMain.GuiManager.SynchronizedHandleBackup);

            PageControl.SelectedIndex = 1;

            TMain.IdleAction = new Action(IdleAction);
            InitIdleTimer();

            tsMenu.Visibility = Visibility.Collapsed;
            tsTiming.Visibility = Visibility.Collapsed;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            DisposeViews();
        }

        private void Window_Closed(object sender, EventArgs e)
        {

            TMain.IniImage.SaveConfiguration();
            TMain.PeerManager.Close();
            TMain.BO.StatusFeedback.Dispose();
        }
        #endregion

        private void LoadTabs()
        {
            TabEntries = new EntriesTab();
            tsEntries.Content = TabEntries;

            TabTiming = new TimingTab();
            tsTiming.Children.Add(TabTiming);

            TabEvent = new EventTab();
            tsEvent.Content = TabEvent;

            TabReport = new ReportTab();
            tsReport.Content = TabReport;

            TabMenu = new MenuTab();
            tsMenu.Children.Add(TabMenu);
        }

        #region Properties
        protected TMain Controller
        {
            get
            {
                return TMain.Controller;
            }
        }
        protected TCacheMotor CacheMotor
        {
            get
            {
                return TMain.GuiManager.CacheMotor;
            }
        }

        protected bool IsEventModified
        {
            get
            {
                return TMain.BO.EventNode.Modified;
            }
        }

        private bool IsRaceModified
        {
            get
            {
                return false;
            }
        }

        internal TextBox Memo
        {
            get
            {
                return TabReport.Memo;
            }
        }

        #endregion

        #region Idle Processing
        private void IdleAction()
        {
            this.ApplicationIdleEvent(null, null);
        }

        private void InitIdleTimer()
        {
            dt = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 1)
            };
            dt.Tick += new EventHandler(IdleTimerTick);
            dt.Start();
        }

        private void IdleTimerTick(object sender, EventArgs e)
        {
            IdleAction();
        }

        public bool IsBuissy
        {
            get
            {
                return TabEvent.waiting && TabEntries.waiting;
            }
        }

        private void ApplicationIdleEvent(object sender, RoutedEventArgs e)
        {
            OnIdleCounter++;

            DoOnIdle_LED();
            DoOnIdle_FullUpdate(); //if scheduled

            if (DoOnIdle_CheckWaiting())
            {
                Cursor oldCursor = this.Cursor;
                this.Cursor = Cursors.Wait;
                try
                {
                    DoOnIdle_BO();
                    DoOnIdle_TabRace(); //if selected
                    DoOnIdle_TabEvent();
                    DoOnIdle_TabCache();
                }
                finally
                {
                    this.Cursor = oldCursor;
                }
            }

            DoOnIdle_Cache();
            DoOnIdle_Peer();
        }

        private void DoOnIdle_LED()
        {
        }

        private void DoOnIdle_FullUpdate()
        {
            try
            {
                if (TabEvent.GridUpdate.NeedFullUpdate)
                {
                    TabEvent.DoUpdateGrid();
                }
            }
            catch (Exception)
            {
            }
        }

        private bool DoOnIdle_CheckWaiting()
        {
            if (IsBuissy)
            {
                return false;
            }

            if (!(IsRaceModified || IsEventModified))
            {
                return false;
            }

            OnIdleCounter++;
            return true;
        }

        private void DoOnIdle_BO()
        {
            TMain.BO.OnIdle();
        }

        private void DoOnIdle_TabRace()
        {
        }

        private void DoOnIdle_TabEvent()
        {
            if (PageControl.SelectedItem == tsEvent)
            {
                if (IsEventModified)
                {
                    TabEvent.GridUpdate.DoOnIdle();
                }
            }
        }

        private void DoOnIdle_TabCache()
        {
        }

        private void DoOnIdle_Cache()
        {
        }

        private void DoOnIdle_Peer()
        {
            TMain.PeerController.DoOnIdle();
        }

        private void HandleError(Exception ex)
        {
            Memo.Text = ex.ToString();
        }
        #endregion

        #region BO/View Interaction
        public void InitViews()
        {
            TMain.GuiManager.InitCache();
            TMain.GuiManager.InitPeer();

            TabEntries.InitGrid();
            TabTiming.InitTiming();
            TabEvent.InitGrid();
        }

        public void DisposeViews()
        {
            TabEntries.GridUpdate.NeedFullUpdate = false;
            TabEvent.GridUpdate.NeedFullUpdate = false;

            TabEntries.GridUpdate.OnUpdate = null;
            TabEvent.GridUpdate.OnUpdate = null;

            TabEntries.DisposeGrid();
            TabTiming.DisposeTiming();
            TabEvent.DisposeGrid();

            TMain.GuiManager.DisposePeer();
            TMain.GuiManager.DisposeCache();
        }

        private void SwapEvent(string EventData)
        {
            TMain.GuiManager.SwapEvent(EventData);
        }

        public void HandleInform(TGuiAction action)
        {
            switch (action)
            {
                case TGuiAction.RaceChanged:
                    if (TabTiming != null)
                    {
                        TabTiming.UpdateRace();
                    }
                    break;
                case TGuiAction.CaptionChanged:
                    UpdateCaption();
                    break;
                case TGuiAction.ScheduleEventUpdate:
                    if (TabEvent != null)
                    {
                        TabEvent.GridUpdate.ScheduleFullUpdate();
                    }
                    break;
                case TGuiAction.ScheduleEventDraw:
                    TabEvent.GridUpdate.InvalidateView();
                    break;
                case TGuiAction.acUndo:
                case TGuiAction.acRedo:
                    TabEvent.DoUpdateGrid();
                    break;
                case TGuiAction.acStrict:
                case TGuiAction.acRelaxed:
                    TabEvent.UpdateStrictRelaxed();
                    break;
                case TGuiAction.acColor:
                    TabEvent.UpdateColorMode();
                    break;
                case TGuiAction.InitBridge:
                    InitBridge();
                    break;
                default:
                    DefaultHandleInform();
                    break;
            }
        }

        private void DefaultHandleInform()
        {
            TabEvent.GridUpdate.UpdateNow();
            TabEntries.GridUpdate.UpdateNow();
        }

        public void UpdateCaption()
        {
            this.Title = TMain.AppName + " - " + TMain.DocManager.EventName;
        }

        private void CopyRank()
        {
        }
        #endregion

        #region Bridge methods
        private void InitBridge()
        {   
            //called via GuiInterface
            TMain.PeerController.Connect();
            TMain.PeerController.OnBackup = new THandleMsgEvent(HandleBackup);
        }
        public void HandleBackup(object sender, string EventData)
        {
            this.Dispatcher.Invoke(FSynchronizedHandleBackup, new object[] { null, EventData });
        }
        #endregion

        private void TestDataBtn_Click(object sender, RoutedEventArgs e)
        {
            SwapEvent(Controller.GetTestData());
        }

        private void EventMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            if (tsMenu.IsVisible)
            {
                tsMenu.Visibility = Visibility.Collapsed;
            }
            else
            {
                tsMenu.Visibility = Visibility.Visible;
            }
        }

        private void TimingBtn_Click(object sender, RoutedEventArgs e)
        {
            if (tsTiming.IsVisible)
            {
                tsTiming.Visibility = Visibility.Collapsed;
            }
            else
            {
                tsTiming.Visibility = Visibility.Visible;
            }
        }

        private void InfoBtn_Click(object sender, RoutedEventArgs e)
        {
            PageControl.SelectedIndex = PageControl.Items.IndexOf(tsReport);
            InfoMemo m = new InfoMemo();
            m.Init();
            Memo.Text = m.Fill();
        }

        public void InitCacheGui()
        {
        }

        public void DisposeCacheGui()
        {
        }
    }

}
