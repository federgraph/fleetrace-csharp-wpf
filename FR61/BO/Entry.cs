namespace RiggVar.FR
{
    public delegate int TNewIDFunction();

    public class TBaseEntry : TBOPersistent
    {
        public static char SpaceChar = ' ';
        public static char EmptyMark = '-';
        protected string sep = ";";

        public int SO;
        public string sOutput;

        public TBaseEntry()
        {
        }
        private bool Need_CSV_Escape(string aTagValue)
        {
            return (aTagValue.IndexOf(" ") > -1) || (sep == "," && aTagValue.IndexOf(sep) > -1);
        }
        private void SLADD_CSV(string aTagName, string aTagValue)
        {
            if (aTagValue == string.Empty)
            {
                sOutput = sOutput + aTagValue + sep;
            }
            else if (Need_CSV_Escape(aTagValue))
            {
                sOutput = sOutput + '"' + aTagValue + '"' + sep;
            }
            else
            {
                sOutput = sOutput + aTagValue + sep;
            }
        }
        private void SLADD_CSV_LAST(string aTagName, string aTagValue)
        {
            if (aTagValue == string.Empty)
            {
                sOutput = sOutput + aTagValue;
            }
            else if (Need_CSV_Escape(aTagValue))
            {
                sOutput = sOutput + '"' + aTagValue + '"';
            }
            else
            {
                sOutput = sOutput + aTagValue;
            }
        }
        private void SLADD_CSV_Header_LAST(string aTagName, string aTagValue)
        {
            sOutput = sOutput + aTagName;
        }
        private void SLADD_CSV_Header(string aTagName, string aTagValue)
        {
            sOutput = sOutput + aTagName + sep;
        }
        private void SLADD_HTM(string aTagName, string aTagValue)
        {
            if (aTagValue == string.Empty)
            {
                aTagValue = "&nbsp";
            }

            sOutput = sOutput + "<td>" + aTagValue + "</td>";
        }
        private void SLADD_HTM_Header(string aTagName, string aTagValue)
        {
            if (aTagValue == string.Empty)
            {
                aTagValue = "&nbsp";
            }

            sOutput = sOutput + "<th>" + aTagName + "</th>";
        }
        private void SLADD_XML(string aTagName, string aTagValue)
        {
            sOutput = sOutput + aTagName + "=\"" + aTagValue + "\" ";
        }
        protected void SLADD(string aTagName, string aTagValue)
        {
            switch (OutputType)
            {
                case TOutputType.otCSV: SLADD_CSV(aTagName, aTagValue);    break;
                case TOutputType.otHTM: SLADD_HTM(aTagName, aTagValue); break;
                case TOutputType.otXML: SLADD_XML(aTagName, aTagValue); break;
                case TOutputType.otCSV_Header: SLADD_CSV_Header(aTagName, aTagValue); break;
                case TOutputType.otHTM_Header: SLADD_HTM_Header(aTagName, aTagValue); break;
                default: break;
            }
        }
        protected void SLADDLAST(string aTagName, string aTagValue)
        {
            switch (OutputType)
            {
                case TOutputType.otCSV: SLADD_CSV_LAST(aTagName, aTagValue); break;
                case TOutputType.otHTM: SLADD_HTM(aTagName, aTagValue); break;
                case TOutputType.otXML: SLADD_XML(aTagName, aTagValue); break;
                case TOutputType.otCSV_Header: SLADD_CSV_Header_LAST(aTagName, aTagValue); break;
                case TOutputType.otHTM_Header: SLADD_HTM_Header(aTagName, aTagValue); break;
                default: break;
            }
        }
        protected virtual void GetOutput()
        {
            //overrides should make calls to SLADD here
        }
        public void GetFooter(TStrings SL, TOutputType ot, string aName, bool XMLSection)
        {
            switch (ot)
            {
                case TOutputType.otCSV: 
                    break;
                case TOutputType.otHTM: 
                    SL.Add("</table>"); 
                    break;
                case TOutputType.otXML: 
                    if (XMLSection)
                    {
                        SL.Add("</" + aName + ">");
                    }

                    break;
                default: 
                    break;
            }
        }
        public void GetHeader(TStrings SL, TOutputType ot, string aName, bool XMLSection)
        {
            switch (ot)
            {
                case TOutputType.otCSV: 
                    SL.Add(GetCSV_Header());
                    break;
                case TOutputType.otHTM:
                    SL.Add("<table border=\"1\" cellspacing=\"0\" cellpadding=\"1\">");
                    SL.Add("<caption>" + aName + "</caption>");
                    SL.Add(GetHTM_Header());
                    break;
                case TOutputType.otXML:
                    if (XMLSection)
                    {
                        SL.Add("<" + aName + ">");
                    }

                    break;
            }
        }
        public virtual string GetCommaText(TStrings SL)
        {
            return string.Empty;
        }
        public virtual void SetCommaText(TStrings SL)
        {
        }
        public virtual string GetCSV()
        {
            OutputType = TOutputType.otCSV;
            sOutput = "";
            GetOutput();
            return sOutput;
        }
        public virtual string GetHTM()
        {
            OutputType = TOutputType.otHTM;
            sOutput = "<tr>";
            GetOutput();
            sOutput = sOutput + "</tr>";
            return sOutput;
        }
        public virtual string GetXML(string aTagName)
        {
            OutputType = TOutputType.otXML;
            sOutput = "";
            GetOutput();
            return "<" + aTagName + " " + sOutput + "/>";
        }
        public virtual string GetCSV_Header()
        {
            return string.Empty;
        }
        public virtual string GetHTM_Header()
        {
            OutputType = TOutputType.otHTM_Header;
            sOutput = "<tr align=\"left\">";
            GetOutput();
            sOutput = sOutput + "</tr>";
            return sOutput;
        }
        public TOutputType OutputType { get; set; }
        public TNewIDFunction NewID { get; set; }
    }


}
