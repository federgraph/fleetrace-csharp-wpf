namespace RiggVar.FR
{
    public enum TOutputType 
    { 
        otCSV, 
        otHTM, 
        otXML, 
        otCSV_Header, 
        otHTM_Header 
    }

    public class TBaseOutput
    {
        private TTokenParser TokenParser = new TTokenParser();
        protected TStrings SLCommaText = new TStringList();

        public TOutputType OutputType;
        protected int MsgID;
        public bool XMLSection;
        public bool WantHTMEscape;
        public bool WantPageHeader;
        public TStrings SL = new TStringList();

        public TBaseOutput()
        {
        }

        protected void PageHeader()
        {
            switch (OutputType)
            {
                case TOutputType.otCSV:
                    break;
                case TOutputType.otHTM:
                    SL.Add("<HTML>");
                    SL.Add("<HEAD>");
                    SL.Add("<TITLE>FR Output</TITLE>");
                    SL.Add("<style><!-- pre {color=\"maroon\"} --> </style>");
                    SL.Add("</HEAD>");
                    SL.Add("<BODY>");
                    break;
                case TOutputType.otXML:
                    SL.Add("<?xml version=\"1.0\" ?>");
                    break;
            }
        }

        protected void PageFooter()
        {
            switch (OutputType)
            {
                case TOutputType.otCSV:
                    break;
                case TOutputType.otHTM:
                    SL.Add("</BODY>");
                    SL.Add("</HTML>");
                    break;
                case TOutputType.otXML:
                    break;
            }
        }

        protected void SectionHeader(string s)
        {
            switch (OutputType)
            {
                case TOutputType.otCSV:
                    break;
                case TOutputType.otHTM:
                    SL.Add("");
                    SL.Add("#" + s);
                    SL.Add("");
                    break;
                case TOutputType.otXML:
                    break;
            }
        }

        protected virtual void EscapeHTM()
        {
            for (int i = 0; i < SL.Count; i++)
            {
                //don't want to include reference to System.Web here
                //SL[i] = System.Web.HttpUtility.HtmlEncode(SL[i]);
            }
            SL.Insert(0, "<pre>");
            SL.Add("</pre>");
        }

        public virtual string GetMsg(string sRequest)
        {
            return string.Empty;
        }

        public string GetAll(TStrings OutputRequestList)
        {
            string sRequest;
            TStringList SL = new TStringList();
            SL.Add("<?xml version=\"1.0\" ?>");
            SL.Add("<e>");
            for (int i = 0; i < OutputRequestList.Count; i++)
            {
                sRequest = OutputRequestList[i];
                SL.Add("<answer request=\"" + sRequest + "\">");
                SL.Add("<![CDATA[");
                SL.Add(GetMsg(sRequest));
                SL.Add("]]>");
                SL.Add("</answer>");
                SL.Add("");
            }
            SL.Add("</e>");
            return SL.Text;
        }

    }

}
