using System.Windows;
using System.Windows.Controls;

using RiggVar.FR;

namespace FR73.Tabs
{
    public partial class ReportTab : UserControl
    {
        public ReportTab()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(OnLoaded);
            InitCommands();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void InitCommands()
        {
            SourceBtn.Click += new RoutedEventHandler(ToolBar_Click);
            TxtBtn.Click += new RoutedEventHandler(ToolBar_Click);
            XmlBtn.Click += new RoutedEventHandler(ToolBar_Click);
            HtmlBtn.Click += new RoutedEventHandler(ToolBar_Click);
            JsonBtn.Click += new RoutedEventHandler(ToolBar_Click);
            HashBtn.Click += new RoutedEventHandler(ToolBar_Click);
        }

        private void ToolBar_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource == SourceBtn)
            {
                Memo.Text = TMain.BO.ConvertedData;
            }
            else if (e.OriginalSource == TxtBtn)
            {
                Memo.Text = TMain.BO.ToString();
            }
            else if (e.OriginalSource == XmlBtn)
            {
                Memo.Text = TMain.BO.ToXML();
            }
            else if (e.OriginalSource == JsonBtn)
            {
                Memo.Text = TMain.BO.ToJson();
            }
            else if (e.OriginalSource == HtmlBtn)
            {
                if (TMain.GuiManager.CacheMotor != null)
                {
                    Memo.Text = TMain.GuiManager.CacheMotor.FinishReport;
                }
                else
                {
                    Memo.Text = TMain.BO.Output.GetMsg("FR.*.Output.Report.FinishReport");
                }
            }
            else if (e.OriginalSource == HashBtn)
            {
                Memo.Text = ResultHash.MemoString;
            }
        }

    }

}
