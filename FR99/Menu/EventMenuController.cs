using System;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Net;
using System.Xml.Linq;
using RiggVar.FR;
using FRXML;

namespace FR62.Tabs
{
    internal enum TestMode
    {
        Txt,
        Xml,
        Download,
        Live,
        Error
    }

    internal interface IEventMenuUI
    {
        void UpdateWorkspaceCombo(EventCombo sender);
        void InitEventButtons();
        void UpdateMemo();
        void UpdateEventGrid();
        void UpdateEventName(string EventName);
        void UpdateStatus(string Msg, int Progress);
    }

    internal class EventMenuController
    {
        private TransformHelper xt = new FRXML.TransformHelper();
        private TransformerHtml ht = new TransformerHtml();

        public class DownloadUserToken
        {
            public int Index;
            public string Caption;
            public Uri uri;
        }

        internal IEventMenuUI UI;
        public Uri MenuUri = new Uri("https://federgraph.de/EventMenu.xml");

        internal IEventMenu EventMenu;
        internal EventCombo eventCombo;
        internal TestMode testMode;

        internal bool skipDownload;
        internal bool skipImport;

        internal string TestMemoText;
        internal string MemoText;

        private void LoadCaption(TextBlock tb, int i)
        {
            tb.Text = EventMenu.GetCaption(i);
        }

        public void LoadImage(Image img, int i)
        {
            LoadImage(img, EventMenu.GetImageUrl(i));
        }

        private void LoadImage(Image img, string url)    
        {
            img.ImageFailed += new EventHandler<ExceptionRoutedEventArgs>(ImageLoadFailed);
            Uri imageUri = new Uri(url, UriKind.RelativeOrAbsolute);
            ImageSource imgSource = new BitmapImage(imageUri);
            img.SetValue(Image.SourceProperty, imgSource);
        }      
        
        void ImageLoadFailed(object sender, ExceptionRoutedEventArgs e)    
        {
        }

        public void LoadEventData(int i)
        {
            string uriString = EventMenu.GetDataUrl(i);
            if (uriString != string.Empty)
            {
                try
                {
                    UI.UpdateStatus("LoadEventData", 10);
                    WebClient client = Client;                    
                    client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(LoadEventDataCompleted);
                    Uri uri = new Uri(uriString);
                    DownloadUserToken t = new DownloadUserToken
                    {
                        Index = i,
                        Caption = EventMenu.GetCaption(i),
                        uri = uri
                    };
                    client.Encoding = Encoding.UTF8;
                    client.DownloadStringAsync(uri, t);
                    client = null;
                }
                catch (Exception ex)
                {
                    UI.UpdateStatus("Exception in LoadEventData()", 0);
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    MemoText = ex.Message;
                }
            }
        }

        private void LoadEventDataCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                UI.UpdateStatus("Error detected in LoadEventDataCompleted()", 0);
                HandleError(e.Error);
                return;
            }

            DownloadUserToken t = (DownloadUserToken)e.UserState;
            string en;
            if (t.Caption == EventMenu.GetCaption(t.Index))
            {
                en = t.Caption;
            }
            else
            {
                en = t.uri.LocalPath;
            }

            LoadEvent(e.Result, en);
        }

        private void LoadEvent(string EventData, string EventName)
        {
            try
            {
                UI.UpdateStatus("completed", 100);
                if (skipImport)
                {
                    MemoText = MoveToProcessingInstruction(EventData);
                    UI.UpdateMemo();
                }
                else
                {
                    string ed;
                    string en;
                    if (IsHtmlData(EventData))
                    {
                        ed = RemovePreamble(EventData);
                        ed = MoveToHtmlElement(ed);
                        ed = TransformHtmlEventData(ed);
                        en = ht.EventName;
                    }
                    else if (IsXmlData(EventData))
                    {
                        ed = RemovePreamble(EventData);
                        ed = MoveToProcessingInstruction(ed);
                        ed = RemoveProcessingInstruction(ed);
                        ed = TransformXmlEventData(ed);
                        en = xt.EventName;
                    }
                    else
                    {
                        ed = Utils.SwapLineFeed(EventData);
                        en = EventName;
                    }
                    MemoText = ed;
                    TMain.DocManager.EventName = en;
                    ImportData(ed);
                    UI.UpdateEventName(en);
                    UI.UpdateEventGrid();
                }
            }
            catch (Exception ex)
            {
                UI.UpdateStatus("Exception in LoadFromFileCompleted", 0);
                UI.UpdateEventName("");
                HandleError(ex);
            }
        }

        private void HandleError(Exception ex)
        {
            if (ex.InnerException != null)
            {
                TestMemoText = ex.InnerException.Message;
            }
            else
            {
                TestMemoText = ex.Message;
            }
        }

        public void LoadMenu()
        {
            eventCombo = new EventCombo();
            EventMenu = eventCombo;
            LoadMenuData();
        }

        public void LoadMenuData()
        {
            try
            {
                WebClient client = Client;
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(LoadMenuDataCompleted);
                client.Encoding = Encoding.UTF8;
                client.DownloadStringAsync(MenuUri);
                client = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void LoadMenuDataCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                HandleError(e.Error);
                return;
            }
            try
            {
                switch (testMode)
                {
                    case TestMode.Download:                        
                        MemoText = MoveToProcessingInstruction(e.Result);
                        if (eventCombo.LoadingCompleted)
                        {
                            UI.UpdateMemo();
                        }
                        break;

                    case TestMode.Live:
                        string em = e.Result;
                        em = RemovePreamble(em);
                        em = RemoveProcessingInstruction(em);
                        EventMenu.Load(em);
                        if (eventCombo.LoadingCompleted)
                        {
                            UI.UpdateWorkspaceCombo(eventCombo);
                            UI.InitEventButtons();
                        }
                        break;
                }

            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        //public event EventHandler DownloadCompleted;
        public void ImportData(string EventData)
        {
            //if (DownloadCompleted != null)
            //{
            //    DownloadCompleted(EventData, new EventArgs());
            //    DownloadCompleted = null;
            //}

            if (EventData != "")
            {
                TMain.GuiManager.SwapEvent(EventData);
            }
        }

        private WebClient Client
        {
            get
            {
                return new WebClient();
            }
        }

        private bool IsEventDataXml(string s)
        {
            return s.Contains("<FR>") && s.Contains("</FR>");
        }

        private bool IsHtmlData(string s)
        {
            if (s.Contains("<html"))
            {
                return true;
            }

            return false;
        }

        private bool IsXmlData(string s)
        {
            if (s.StartsWith("<?"))
            {
                return true;
            }

            if (s.StartsWith("<"))
            {
                return true;
            }

            if (s.Contains("<?"))
            {
                return true;
            }

            return false;
        }

        private bool HasPreamble(string s)
        {
            byte[] p = Encoding.UTF8.GetPreamble();
            string t = Encoding.Default.GetString(p);
            if (s.StartsWith(t))
            {
                return true;
            }

            return false;
        }

        private string RemovePreamble(string s)
        {
            byte[] p;
            string t;

            p = Encoding.UTF8.GetPreamble();
            t = Encoding.Default.GetString(p);
            if (s.StartsWith(t))
            {
                s = s.Substring(t.Length);
            }

            return s;
        }

        private string MoveToHtmlElement(string s)
        {
            //remove everything before opening <html> Tag
            string t = "<html";
            int i = s.IndexOf(t);
            if (i > 0)
            {
                s = s.Substring(i);
            }
            return s;
        }

        private string MoveToProcessingInstruction(string s)
        {
            //remove everything before first processing instruction
            string t = "<?";
            int i = s.IndexOf(t);
            if (i > 0)
            {
                s = s.Substring(i);
            }
            return s;
        }

        private string RemoveProcessingInstruction(string s)
        {
            //remove processing instruction
            string t = "?>";
            int i = s.IndexOf(t);
            if (i > 0)
            {
                s = s.Substring(i + t.Length);

                //skip whitespace until first opening tag
                t = "<";
                i = s.IndexOf(t);
                if (i > 0)
                {
                    s = s.Substring(i);
                }
            }
            return s;
        }

        private string TransformHtmlEventData(string EventData)
        {

            XElement xe;

            //xe = XElement.Parse(EventData);
            
            xe = XElement.Parse(@"<!DOCTYPE documentElement[
                        <!ENTITY nbsp ""&#160;"">
                        <!ENTITY ndash ""&#8211;"">
                        <!ENTITY mdash ""&#8212;"">
                        ]>" + EventData);

            TStrings SL = new TStringList();
            ht.TransformEventData(xe, SL);
            return SL.Text;
        }

        private string TransformXmlEventData(string EventData)
        {
            XElement xe = XElement.Parse(EventData);
            StringBuilder sb = new StringBuilder();
            xt.TransformEventData(xe, sb);
            return sb.ToString();
        }

        private void Download(Uri uri)
        {
            testMode = TestMode.Download;
            LoadMenuData();
        }

        private void GetMenu(Uri uri)
        {
            testMode = TestMode.Live;
            LoadMenu();
        }

        private void GetEventData(int b)
        {
            IEventMenu em = eventCombo.CurrentMenu;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Tag = " + b);
            sb.AppendLine("Caption = " + em.GetCaption(b));
            sb.AppendLine("DataUrl = " + em.GetDataUrl(b));
            TestMemoText = sb.ToString();

            if (!skipDownload)
            {
                LoadEventData(b);
            }
        }

        public void Transform(string ed)
        {
            if (IsXmlData(ed) == false || IsEventDataXml(ed) == false)
            {
                TestMemoText = DateTime.Now.ToString() + " Transform only applicable to EventData.xml";
                return;
            }

            if (IsXmlData(ed))
            {
                ed = RemovePreamble(ed);
                ed = RemoveProcessingInstruction(ed);
                ed = TransformXmlEventData(ed);
            }
            else
            {
                ed = Utils.SwapLineFeed(ed); 
            }
            MemoText = ed;
            TestMemoText = DateTime.Now.ToString() + " Transform ok";
        }

        public void Convert(string ed)
        {
            if (IsHtmlData(ed))
            {
                ed = RemovePreamble(ed);
                ed = MoveToHtmlElement(ed);
                ed = TransformHtmlEventData(ed);
            }
            else
            {
                TestMemoText = DateTime.Now.ToString() + " Conversion only applicable to EventData.html";
                return;
            }
            MemoText = ed;
            TestMemoText = DateTime.Now.ToString() + " Transform ok";
        }

        public void LoadDataFromMemo(string ed)
        {
            try
            {
                if (IsHtmlData(ed))
                {
                    ed = RemovePreamble(ed);
                    ed = TransformHtmlEventData(ed);
                }
                else if (IsXmlData(ed))
                {
                    ed = RemovePreamble(ed);
                    ed = RemoveProcessingInstruction(ed);
                    ed = TransformXmlEventData(ed);
                }
                else
                {
                    ed = Utils.SwapLineFeed(ed);
                }
                ImportData(ed);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

    }

}
