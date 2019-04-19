using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Net;
using System.Xml.Linq;
using System.Text;

using RiggVar.FR;
using FR62;
using FR62.Tabs;

namespace FR73.Tabs
{

    public partial class MenuTab : UserControl, IEventMenuUI
    {
        Style BtnStyle;
        Style TxtStyle;
        internal bool WantImages = false;

        EventMenuController EMC;
        SelectionChangedEventHandler comboChangedHandler;
        TWorkspaceListBase WorkspaceList;

        int CurrentEventIndex = -1;

        TEventMenuConnection FileCon;
        TEventMenuConnection HttpCon;
        TEventMenuConnection AppCon;

        public MenuTab()
        {
            FileCon = new TEventMenuConnection();
            HttpCon = FileCon;
            AppCon = FileCon;

            InitializeComponent();
            DebugMode = false;

            EMC = new EventMenuController
            {
                UI = this
            };

            WorkspaceList = new WorkspaceList();

            InitUrlCombo();
            InitCommands();
        }

        private void InitCommands()
        {
            DownloadBtn.Click += new RoutedEventHandler(DownloadBtn_Click);
            TestBtn.Click += new RoutedEventHandler(LoadTestDataIntoMemoBtn_Click);
            TransformBtn.Click += new RoutedEventHandler(TransformBtn_Click);
            ConvertBtn.Click += new RoutedEventHandler(ConvertBtn_Click);
            LoadBtn.Click += new RoutedEventHandler(LoadDataFromMemoBtn_Click);
            DebugBtn.Click += new RoutedEventHandler(DebugBtn_Click);
            UrlBtn.Click += new RoutedEventHandler(UrlBtn_Click);
            WriteBtn.Click += new RoutedEventHandler(WriteBtn_Click);

            GetMenuBtn.Click += new RoutedEventHandler(GetMenuBtn_Click);
            cbSkipDownload.Click += new RoutedEventHandler(CbSkipDownload_Click);
            cbSkipImport.Click += new RoutedEventHandler(CbSkipImport_Click);

            comboChangedHandler = new SelectionChangedEventHandler(Combo_SelectionChanged);
        }

        private void InitUrlCombo()
        {
            WorkspaceList.Init();
            UrlCombo.Items.Clear();
            WorkspaceList.Load(UrlCombo.Items);
            if (UrlCombo.Items.Count > 0)
            {
                UrlCombo.SelectedIndex = 0;
            }
        }

        TFormFR62 Page
        {
            get { return Application.Current.MainWindow as TFormFR62; }
        }

        TextBox Memo
        {
            get { return Page.TabReport.Memo; }
        }

        TextBox TestMemo
        {
            get { return Page.TabReport.TestMemo; }
        }

        private void InitMenuLocation()
        {
            EMC.MenuUri = new Uri(UrlCombo.Text, UriKind.Absolute);
        }

        private void InitStyle()
        {
            BtnStyle = EventBtnPanel.Resources["BtnStyle"] as Style;
            TxtStyle = EventBtnPanel.Resources["TxtStyle"] as Style;
        }

        public void InitEventButtons()
        {
            EventBtnPanel.Children.Clear();
            InitStyle();

            Button B;
            Image I;
            Grid G;
            TextBlock T;

            for (int i = 1; i <= EMC.EventMenu.Count; i++)
            {
                B = new Button
                {
                    Style = BtnStyle,
                    Tag = i
                };
                B.Click += new RoutedEventHandler(Btn_Click);

                G = new Grid();

                if (WantImages && EMC.EventMenu.GetImageUrl(i) != null)
                {
                    I = new Image();
                    EMC.LoadImage(I, i);
                    G.Children.Add(I);
                }

                T = new TextBlock
                {
                    Style = TxtStyle
                };
                LoadCaption(T, i);
                G.Children.Add(T);

                B.Content = G;
                EventBtnPanel.Children.Add(B);
            }
        }

        private void LoadCaption(TextBlock tb, int i)
        {
            tb.Text = EMC.EventMenu.GetCaption(i);
        }

        void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EMC.eventCombo.MenuIndex = WorkspaceCombo.SelectedIndex;
            InitEventButtons();
        }

        private void CbSkipDownload_Click(object sender, RoutedEventArgs e)
        {
            if (cbSkipDownload.IsChecked == true)
            {
                cbSkipImport.IsChecked = true;
            }
        }

        private void CbSkipImport_Click(object sender, RoutedEventArgs e)
        {
            if (cbSkipImport.IsChecked == false)
            {
                cbSkipDownload.IsChecked = false;
            }
        }

        private void DownloadBtn_Click(object sender, EventArgs e)
        {
            TestMemo.Clear();
            EMC.testMode = TestMode.Download;
            InitMenuLocation();
            PrepareEMC();
            EMC.LoadMenuData();
            ShowMemo();
        }

        private void GetMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            CurrentEventIndex = -1;
            TestMemo.Clear();
            Memo.Clear();
            EMC.testMode = TestMode.Live;
            InitMenuLocation();
            PrepareEMC();
            EMC.LoadMenu();
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            CurrentEventIndex = -1;
            TestMemo.Clear();
            Memo.Clear();
            if (e.OriginalSource is Button)
            {
                int b;
                Button B = (Button)e.OriginalSource;
                int.TryParse(B.Tag.ToString(), out b);

                IEventMenu em = EMC.eventCombo.CurrentMenu;
                TestMemo.AppendText("Tag = " + b + Environment.NewLine);
                TestMemo.AppendText("Caption = " + em.GetCaption(b) + Environment.NewLine);
                TestMemo.AppendText("DataUrl = " + em.GetDataUrl(b) + Environment.NewLine);

                if (cbSkipDownload.IsChecked == true || cbSkipImport.IsChecked == true)
                {
                    Page.PageControl.SelectedItem = Page.tsReport;
                }

                if (cbSkipDownload.IsChecked == false)
                {
                    if (em.IsMock())
                    {
                        Memo.Text = "download skipped (Error/Timeout before or url not selected)";
                    }
                    else
                    {
                        PrepareEMC();
                        EMC.LoadEventData(b); //asynchron
                    }
                }
                CurrentEventIndex = b;
            }
        }

        private void LoadTestDataIntoMemoBtn_Click(object sender, RoutedEventArgs e)
        {
            Memo.Text = TMain.BOManager.GetTestData();
        }

        private void TransformBtn_Click(object sender, RoutedEventArgs e)
        {
            string ed = Memo.Text;
            PrepareEMC();
            EMC.Transform(ed);
            ShowMemo();
        }

        private void ConvertBtn_Click(object sender, RoutedEventArgs e)
        {
            string ed = Memo.Text;
            PrepareEMC();
            EMC.Convert(ed);
            ShowMemo();
        }

        private void LoadDataFromMemoBtn_Click(object sender, RoutedEventArgs e)
        {
            string ed = Memo.Text;
            PrepareEMC();
            EMC.LoadDataFromMemo(ed);
        }

        private void DebugBtn_Click(object sender, RoutedEventArgs e)
        {
            DebugMode = !DebugMode;
        }

        internal bool DebugMode
        {
            get => TestBtnPanel.Visibility == Visibility.Visible;
            set
            {
                if (value)
                {
                    TestBtnPanel.Visibility = Visibility.Visible;
                    DebugBtn.Content = "Less";
                    cbSkipImport.IsChecked = true;
                    Page.PageControl.SelectedItem = Page.tsReport;
                }
                else
                {
                    TestBtnPanel.Visibility = Visibility.Collapsed;
                    DebugBtn.Content = "More";
                    cbSkipDownload.IsChecked = false;
                    cbSkipImport.IsChecked = false;
                    Page.PageControl.SelectedItem = Page.tsEvent;
                }
            }
        }
        private void PrepareEMC()
        {
            EMC.MemoText = "";
            EMC.TestMemoText = "";
            EMC.skipImport = cbSkipImport.IsChecked == true;
            EMC.skipDownload = cbSkipDownload.IsChecked == true;
        }

        private void ShowMemo()
        {
            TestMemo.Text = EMC.TestMemoText;
            Memo.Text = EMC.MemoText;
            if (EMC.skipDownload || EMC.skipImport)
            {
                Page.PageControl.SelectedItem = Page.tsReport;
            }
        }

        void IEventMenuUI.UpdateWorkspaceCombo(EventCombo cl)
        {
            ComboBox c = WorkspaceCombo;
            if (comboChangedHandler.GetInvocationList().GetLength(0) > 0)
            {
                c.SelectionChanged -= comboChangedHandler;
            }

            c.Items.Clear();
            foreach (EventMenu02 cr in cl.MenuCollection)
            {
                c.Items.Add(cr.ComboCaption);
            }
            if (c.Items.Count > 0)
            {
                c.SelectedIndex = 0;
            }

            c.SelectionChanged += new SelectionChangedEventHandler(comboChangedHandler);
        }

        void IEventMenuUI.UpdateMemo()
        {
            Memo.Text = EMC.MemoText;
        }

        void IEventMenuUI.UpdateEventGrid()
        {
            Page.TabEvent.DoUpdateGrid();
        }

        void IEventMenuUI.UpdateEventName(string EventName)
        {
            EventNameLabel.Text = EventName; //xt.EventName;
        }

        void IEventMenuUI.UpdateStatus(string Msg, int Progress)
        {
            //StatusLabel.Text = string.Format("{0} ({1})", Msg, Progress);
        }

        private void UrlBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowMemo();
            TestMemo.Clear();
            if (EMC.EventMenu != null)
            {
                StringBuilder sb = new StringBuilder();
                UrlBtn1(sb);
                TestMemo.Text = sb.ToString();
            }
            else
            {
                TestMemo.Text = "EMC.EventMenu == null";
            }

        }

        private void UrlBtn1(StringBuilder sb)
        {
            string fs1, fs2, fs3, fs4;

            int UrlComboItemIndex;
            string UrlComboText;
            string SelectedUrlString;

            int CurrentBtnIndex;
            string CurrentBtnCaption;
            string CurrentBtnUrl;

            /*
            Selected ComboEntry (WorkspaceMenuUrl): 14 - RN05
              http://www.fleetrace.org/DemoIndex.xml
            Current Button (EventDataUrl) = 2 - FleetTest
              http://www.fleetrace.org/Demo/Test/NameTest.xml
            */

            fs1 = "Selected ComboEntry (EventMenu): {0} ({1})";
            fs2 = "  {0}";
            fs3 = "Current Button (EventData): {0} ({1})";
            fs4 = "  {0}";

            UrlComboItemIndex = UrlCombo.SelectedIndex;
            UrlComboText = UrlCombo.Text;
            SelectedUrlString = SelectedUrl;

            CurrentBtnIndex = CurrentEventIndex;
            CurrentBtnCaption = EMC.EventMenu.GetCaption(CurrentEventIndex);
            CurrentBtnUrl = EMC.EventMenu.GetDataUrl(CurrentEventIndex);

            fs1 = string.Format(fs1, UrlComboItemIndex, UrlComboText);
            fs2 = string.Format(fs2, SelectedUrlString);
            fs3 = string.Format(fs3, CurrentBtnIndex, CurrentBtnCaption);
            fs4 = string.Format(fs4, CurrentBtnUrl);

            sb.AppendLine(fs1);
            sb.AppendLine(fs2);
            sb.AppendLine(fs3);
            sb.AppendLine(fs4);
        }

        private string SelectedUrl
        {
            get
            {
                int i = UrlCombo.SelectedIndex;
                return WorkspaceList.GetUrl(i);
            }
        }

        private void WriteBtn_Click(object sender, RoutedEventArgs e)
        {
            int i = CurrentEventIndex;
            if (i > -1)
            {
                string u = EMC.EventMenu.GetDataUrl(i);

                TestMemo.Text = "DataUrl = " + u;

                string s;

                //s = TMain.BO.ToTXT();
                //s = TMain.BO.ToXML();
                //s = TMain.BO.ToString();

                if (TMain.GuiManager.CacheMotor != null)
                {
                    s = TMain.GuiManager.CacheMotor.FinishReport;
                }
                else
                {
                    s = TMain.BO.Output.GetMsg("FR.*.Output.Report.FinishReport");
                }

                Memo.Text = s;

                Upload(u, s);
            }
            else
            {
                TestMemo.Text = "CurrentEventIndex = -1, cannot post data.";
                Memo.Text = "";
            }
        }

        private void Upload(string url, string ed)
        {
            TEventMenuConnection c = GetCon(url);
            if (c != null)
            {
                c.Url = url;
                c.Post(ed);
            }
        }

        private TEventMenuConnection GetCon(string url)
        {
            switch (GetScheme(url))
            {
                case UrlScheme.Http:
                    return HttpCon;
                case UrlScheme.File: return FileCon;
                case UrlScheme.App: return AppCon;
                default: return FileCon;
            }
        }

        private UrlScheme GetScheme(string url)
        {
            if (IsHttpScheme(url))
            {
                return UrlScheme.Http;
            }

            if (IsAppScheme(url))
            {
                return UrlScheme.App;
            }

            return UrlScheme.File;
        }

        private bool IsHttpScheme(string url)
        {
            return url.Contains("http");
        }

        private bool IsAppScheme(string url)
        {
            return url.Contains("App");
        }

    }
}
