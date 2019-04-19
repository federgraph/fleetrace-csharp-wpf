using System;
using RiggVar.FR;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace FR73.Tabs
{
    public partial class TimingTab : UserControl
    {
        private TConnection TimingConnection;
        private Random random = new Random();

        private int Option = 0;
        private ViewModelBib vm = new ViewModelBib();
        private ViewModelEventParams vmep = new ViewModelEventParams();
        private string MessageHeader => "FR." + TMain.BO.BOParams.DivisionName + ".W";

        public TimingTab()
        {
            InitializeComponent();
            InitCommands();
            InitTiming();

            TimingLog.IsReadOnly = true;
            ic.ItemsSource = vm.Items;
            BibBtn.DataContext = vmep;
        }

        private void InitCommands()
        {
            RaceDownBtn.Click += new RoutedEventHandler(ToolBar_Click);
            RaceBtn.Click += new RoutedEventHandler(ToolBar_Click);
            RaceUpBtn.Click += new RoutedEventHandler(ToolBar_Click);
            ClearBtn.Click += new RoutedEventHandler(ToolBar_Click);
            RandomBtn.Click += new RoutedEventHandler(ToolBar_Click);
            AgeBtn.Click += new RoutedEventHandler(ToolBar_Click);
            SendBtn.Click += new RoutedEventHandler(ToolBar_Click);
            BibBtn.Click += new RoutedEventHandler(ToolBar_Click);
        }

        private void ToolBar_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource == RaceDownBtn)
            {
                TMain.GuiManager.Race--;
                RaceChanged(TMain.GuiManager.Race);
            }
            else if (e.OriginalSource == RaceBtn)
            {
                InitCurrent();
            }
            if (e.OriginalSource == RaceUpBtn)
            {
                TMain.GuiManager.Race++;
                RaceChanged(TMain.GuiManager.Race);
            }
            else if (e.OriginalSource == ClearBtn)
            {
                vm.ClearAll();
                TMain.BO.EventNode.ClearRace(Race);
                TMain.GuiManager.GuiInterface.HandleInform(TGuiAction.ScheduleEventUpdate);
            }
            else if (e.OriginalSource == RandomBtn)
            {
                Debug.Assert(TMain.BO.BOParams.StartlistCount == vm.Items.Count, "ViewModelBib.Items.Count does not match");
                int tag = random.Next(TMain.BO.BOParams.StartlistCount);
                Debug.Assert(tag < vm.Items.Count, "ViewModelBib.Items.Count does not match");
                ViewModelBibItem bi = vm.Items[tag];
                DoBibAction(bi);
            }
            else if (e.OriginalSource == AgeBtn)
            {
                vm.ClearAll();
            }
            else if (e.OriginalSource == SendBtn)
            {
                SendMsg(TimingMemo.Text);
            }
            else if (e.OriginalSource == BibBtn)
            {
                ViewModelBibItem bi = vm.FindBibItem(vmep.CurrentBib);
                if (bi != null)
                {
                    DoBibAction(bi);
                }
            }
        }

        internal void InitTiming()
        {
            TimingConnection = TMain.BO.InputServer.Server.Connect("Timing.Panel");

            InitCurrent();
            UpdateRace();
        }

        internal void DisposeTiming()
        {
            TimingConnection = null;
            TimingMemo.Text = "";
            TimingLog.Text = "";
        }

        private void Send(string s)
        {
            if (AutoSend.IsChecked == true)
            {
                SendMsg(s);
            }
            else
            {
                TimingMemo.Text = s;
            }
        }

        private void SendMsg(string s)
        {
            if (TimingConnection != null)
            {
                TimingLog.Text = "";
                TimingConnection.HandleMsg(s);
                TimingLog.Text = s;
            }
            else
            {
                TimingLog.Text = "";
            }

            TimingMemo.Text = s;
        }

        private int Race
        {
            get
            {
                return TMain.GuiManager.Race;
            }
        }

        internal void UpdateRace()
        {
            RaceBtn.Content = "R" + Race.ToString();
        }

        private void Option_Checked(object sender, RoutedEventArgs e)
        {

            if (sender is RadioButton rb)
            {
                string tagName = rb.Tag.ToString();
                switch (tagName)
                {
                    case "finish":
                        Option = 0;
                        break;
                    case "dns":
                        Option = 1;
                        break;
                    case "dnf":
                        Option = 2;
                        break;
                    case "dsq":
                        Option = 3;
                        break;
                    case "ok":
                        Option = 4;
                        break;
                    case "erase":
                        Option = 5;
                        break;
                }
            }
        }


        private string GetTimeString(int digits = 2)
        {
            DateTime d = DateTime.Now;

            int hh = d.Hour;
            int mm = d.Minute;
            int ss = d.Second;
            int t = d.Millisecond;

            string shh = "" + hh;
            string smm = mm < 10 ? "0" + mm : mm.ToString();
            string sss = ss < 10 ? "0" + ss : ss.ToString();
            string sms = "" + t;
            if (t < 10) { sms = "00" + t; }
            else if (t < 100) sms = "0" + t;

            switch (digits)
            {
                case 1: sms = sms.Substring(0, 1); break;
                case 2: sms = sms.Substring(0, 2); break;
            }

            string tm = shh + ':' + smm + ':' + sss + '.' + sms;
            return tm;
        }

        private string GenMsg(int bib)
        {
            TimingParams tm = new TimingParams()
            {
                Race = Race,
                TP = 0,
                Bib = bib
            };

            string time = GetTimeString(2);

            int mt = 0;
            string qu = "";

            switch (Option)
            {
                case 1: mt = 1; qu = "dns"; break;
                case 2: mt = 2; qu = "dnf"; break;
                case 3: mt = 3; qu = "dsq"; break;
                case 4: mt = 4; qu = "ok"; break;
            }

            bool erase = Option == 5;

            string te, tr, t;

            string mh = MessageHeader;
            if (mt > 0)
            {
                t = mh + tm.Race + ".Bib" + tm.Bib + ".QU" + " = " + qu;
            }
            else if (erase)
            {
                te = mh + tm.Race + ".Bib" + tm.Bib + ".RV=0";
                tr = mh + tm.Race + ".Bib" + tm.Bib + ".IT" + tm.TP + " = -1";
                t = te;
            }
            else
            {
                te = mh + tm.Race + ".Bib" + tm.Bib + ".RV=500";
                tr = mh + tm.Race + ".Bib" + tm.Bib + ".IT" + tm.TP + " = " + time;
                t = te;
            }

            return t;
        }

        private void RaceChanged(int r)
        {
            vmep.CurrentRace = r;
            vmep.NotifyRaceChanged();

            UpdateFabs();
            UpdateRace();
        }

        private void UpdateFabs()
        {
            int r = Race;
            TBOParams bop = TMain.BO.BOParams;

            if (r < 1 || r > bop.RaceCount)
            {
                return;
            }

            int slc = TMain.BO.BOParams.StartlistCount;
            if (vm.Items.Count != slc)
            {
                vm.InitRowCount(slc);
            }

            ViewModelBibItem b;
            TEventRaceEntry ere;
            TEventRowCollection cl = TMain.BO.EventNode.Collection;
            foreach(TEventRowCollectionItem cr in cl)
            {
                b = vm.Items[cr.BaseID-1];
                ere = cr.Race[r];
                b.Bib = cr.Bib;
                b.Pos = ere.OTime;
                if (ere.Penalty.IsOut)
                {
                    b.Pos = 999;
                }
            }

            vm.InvalidateAllItems();
        }

        private void DoBibAction(ViewModelBibItem bi)
        {
            if (bi != null)
            {
                string s = GenMsg(bi.Bib);
                Send(s);
                bi.Pos = 500;
                bi.Invalidate();
                FinishRadio.IsChecked = true;
                vmep.UpdateBib(bi.Bib);
            }
        }

        private void SendTestMsg()
        {
            string s = TMain.BO.cTokenA + '.' + TMain.BO.BOParams.DivisionName + '.' + TMain.BO.cTokenRace + Race.ToString() + ".Bib1.XX = Test";
            SendMsg(s);
        }

        private void ResetCurrent()
        {
            vmep.CurrentRace = 1;
            vmep.CurrentTP = 0;
            vmep.CurrentBib = 1;
        }

        private void AssignCurrent(CurrentNumbers value)
        {
            vmep.CurrentRace = value.race;
            vmep.CurrentTP = value.tp;
            vmep.CurrentBib = value.bib;
            CheckCurrent();
            TMain.GuiManager.Race = vmep.CurrentRace;
            UpdateFabs();
        }

        private void CheckCurrent()
        {
            TBOParams p = TMain.BOManager.BO.BOParams;
            if (vmep.CurrentRace > p.RaceCount)
            {
                vmep.CurrentRace = p.RaceCount;
            }
            if (vmep.CurrentTP > p.ITCount)
            {
                vmep.CurrentTP = p.ITCount;
            }
        }

        private void InitCurrent()
        {
            //this.processQueue();

            CurrentNumbers re = new CurrentNumbers();
            re = TMain.BO.FindCurrentInEvent(re);
            AssignCurrent(re);
        }

        private void InitCurrentDefault()
        {
            TBO bo = TMain.BOManager.BO;

            int r = 1;

            int tp = 1;
            if (bo.EventProps.IsTimed == false || bo.BOParams.ITCount == 0)
                tp = 0;

            vmep.CurrentRace = r;
            vmep.CurrentTP = tp;
        }

        private void FindCurrentB()
        {
            TBO bo = TMain.BOManager.BO;
            CurrentNumbers cn = new CurrentNumbers();
            cn = bo.FindCurrentInEvent(cn);
            AssignCurrent(cn);
        }

        private void FindCurrentE()
        {
            TBO bo = TMain.BOManager.BO;
            CurrentNumbers result = new CurrentNumbers();
            result = bo.FindCurrentInEvent(result);
            AssignCurrent(result);
        }

        //private void Gv_ItemClick(object sender, ItemClickEventArgs e)
        //{
        //    if (e.ClickedItem is ViewModelBibItem bi)
        //    {
        //        DoBib(bi);
        //    }
        //}

        private void DoBib(ViewModelBibItem bi)
        {
            if (bi != null)
            {
                if (bi.Used)
                {
                    vmep.UpdateBib(bi.Bib);
                }
                else
                {
                    DoBibAction(bi);
                }
            }
        }

        private void Ellipse_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Ellipse el)
            {
                int t = (int) el.Tag;
                ViewModelBibItem bi = vm.Items[t];
                DoBib(bi);
            }
        }

    }

}
