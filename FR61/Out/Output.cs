using System.IO;
using System.Xml;

namespace RiggVar.FR
{

    public class TOutput : TBaseOutput
    {
        private TBO BO;

        private TOutput01 output01;
        private TOutput02 output02;
        private TOutput03 output03;
        private TOutput04 output04;
        private TOutput05 output05;
        private TOutput06 output06;

        public TOutput() : base()
        {
            BO = TMain.BO;
            //defer creation of output providers until needed
        }

        protected TOutput01 Output01
        {
            get
            {
                if (output01 == null)
                {
                    output01 = new TOutput01();
                }

                return output01;
            }
        }

        protected TOutput02 Output02
        {
            get
            {
                if (output02 == null)
                {
                    output02 = new TOutput02();
                }

                return output02;
            }
        }

        protected TOutput03 Output03
        {
            get
            {
                if (output03 == null)
                {
                    output03 = new TOutput03();
                }

                return output03;
            }
        }

        protected TOutput04 Output04
        {
            get
            {
                if (output04 == null)
                {
                    output04 = new TOutput04();
                }

                return output04;
            }
        }

        protected TOutput05 Output05
        {
            get
            {
                if (output05 == null)
                {
                    output05 = new TOutput05();
                }

                return output05;
            }
        }

        protected TOutput06 Output06
        {
            get
            {
                if (output06 == null)
                {
                    output06 = new TOutput06();
                }

                return output06;
            }
        }

        public override string GetMsg(string sRequest)
        {
            XMLSection = true;
            SL.Clear();
            MsgID++;
                        
            if (sRequest == BO.cTokenOutput + "PageHeaderOn")
            {
                WantPageHeader = true;
                return "PageHeader On";
            }
            else if (sRequest == BO.cTokenOutput + "PageHeaderOff")
            {
                this.WantPageHeader = true;
                return "PageHeader Off";
            }

            int c;
            string temp;
            c = Utils.Pos(BO.cTokenAnonymousOutput, sRequest);
            if (c == 1)
            {
                temp = Utils.Copy(sRequest, BO.cTokenAnonymousOutput.Length + 1, sRequest.Length);
            }
            else
            {
                c = Utils.Pos(BO.cTokenOutput, sRequest);
                if (c == 1)
                {
                    temp = Utils.Copy(sRequest, BO.cTokenOutput.Length + 1, sRequest.Length);
                }
                else
                {
                    return string.Empty;
                }
            }
  
        
            //CSV,HTM,XML
            WantHTMEscape = false;
            OutputType = TOutputType.otCSV;
            if (Utils.Pos("CSV", temp) == 1)
            {
                OutputType = TOutputType.otCSV;
                temp = Utils.Copy(temp, 5, temp.Length);
            }
            else if (Utils.Pos("HTM", temp) == 1)
            {
                OutputType = TOutputType.otHTM;
                temp = Utils.Copy(temp, 5, temp.Length);
            }
            else if (Utils.Pos("XML", temp) == 1)
            {
                OutputType = TOutputType.otXML;
                temp = Utils.Copy(temp, 5, temp.Length);
            }
            else if (Utils.Pos("CSM", temp) == 1)
            {
                OutputType = TOutputType.otCSV;
                WantHTMEscape = true;
                temp = Utils.Copy(temp, 5, temp.Length);
            }
            else if (Utils.Pos("XMM", temp) == 1)
            {
                OutputType = TOutputType.otXML;
                WantHTMEscape = true;
                temp = Utils.Copy(temp, 5, temp.Length);
            }

            if (WantPageHeader)
            {
                PageHeader();
            }

            if (Utils.Copy(temp, 1, 10) == "JavaScore.")
            {
                if (Utils.Copy(temp, 1, 22) == "JavaScore.ScoringNotes")
                {
                    BO.CalcEV.GetScoringNotes(SL);
                }
                else if (Utils.Copy(temp, 1, 13) == "JavaScore.XML")
                {
                    BO.JavaScoreXML.GetXML(SL);
                }
                else if (Utils.Copy(temp, 1, 23) == "JavaScore.ProxyXmlInput")
                {
                    Output04.ProxyXmlInput();
                }
                else if (Utils.Copy(temp, 1, 24) == "JavaScore.ProxyXmlOutput")
                {
                    Output04.ProxyXmlOutput();
                }
            }

            else if (Utils.Copy(temp, 1, 8) == "RiggVar.")
            {
                if (Utils.Copy(temp, 1, 14) == "RiggVar.Params")
                {
                    Output04.Params();
                }
                else if (Utils.Copy(temp, 1, 11) == "RiggVar.TXT")
                {
                    Output04.BackupPreTXT();
                }
                else if (Utils.Copy(temp, 1, 11) == "RiggVar.FR.")
                {
                    Output04.RaceXml(temp);
                }
            }

            else if (Utils.Copy(temp, 1, 7) == "ASPNET.")
            {
                if (Utils.Copy(temp, 1, 14) == "ASPNET.Entries")
                {
                    BO.EventNode.StammdatenRowCollection.GetXML(SL);
                }
                else
                {
                    BO.EventNode.Collection.GetXML(SL);
                }
            }

            else if (Utils.Copy(temp, 1, 7) == "Report.")
            {
                if (Utils.Copy(temp, 1, 19) == "Report.RaceData.SQL")
                {
                    BO.GetRaceDataSQL(SL);
                }
                else if (Utils.Copy(temp, 1, 21) == "Report.SeriesData.SQL")
                {
                    BO.GetSeriesDataSQL(SL);
                }
                else if (Utils.Copy(temp, 1, 15) == "Report.AllRaces")
                {
                    BO.GetReportData(SL);
                }
                else if (Utils.Copy(temp, 1, 13) == "Report.Series")
                {
                    BO.GetReportDataSeries(SL);
                }
                else if (Utils.Copy(temp, 1, 11) == "Report.Race")
                {
                    int p = "Report.Race".Length;
                    string s = temp.Substring(p, temp.Length - p);
                    int r = Utils.StrToIntDef(s, 1);
                    BO.GetReportDataRace(SL, r);
                }
                else if (Utils.Copy(temp, 1, 16) == "Report.IndexData")
                {
                    Output01.IndexReport(SL);
                }
                else if (Utils.Copy(temp, 1, 15) == "Report.CssTable")
                {
                    Output01.CssReport(SL);
                }
                else if (Utils.Copy(temp, 1, 18) == "Report.FinishTable")
                {
                    Output01.FinishTable(SL);
                }
                else if (Utils.Copy(temp, 1, 18) == "Report.PointsTable")
                {
                    Output01.PointsTable(SL);
                }
                else if (Utils.Copy(temp, 1, 19) == "Report.FinishReport")
                {
                    Output01.FinishReport(SL);
                }
                else if (Utils.Copy(temp, 1, 19) == "Report.PointsReport")
                {
                    Output01.PointsReport(SL);
                }
                else if (Utils.Copy(temp, 1, 22) == "Report.TimePointReport")
                {
                    SL.Add("<TimePointReport>not implemented</TimePointReport>");
                    //Output09.TimePointReport(SL);
                }
                else if (Utils.Copy(temp, 1, 22) == "Report.TW_TimePointXML")
                {
                    Output06.TimePointXML(SL, temp.Substring(22 + 1));
                }
                else if (Utils.Copy(temp, 1, 24) == "Report.TW_TimePointTable")
                {
                    Output06.TimePointTable(SL, temp.Substring(24 + 1));
                }

                else if (Utils.Copy(temp, 1, 10) == "Report.XML")
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        StringWriter sw = new StringWriter();
                        XmlTextWriter xw = new XmlTextWriter(sw);
                        xw.Formatting = Formatting.Indented;
                        xw.Indentation = 4;

                        xw.WriteStartElement("root");
                        BO.EventNode.Collection.WriteXml(xw);
                        xw.WriteEndElement();

                        xw.Close();
                        SL.Text = sw.ToString();
                    }
                }
                else if (Utils.Copy(temp, 1, 25) == "Report.PerformanceCounter")
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        StringWriter sw = new StringWriter();
                        XmlTextWriter xw = new XmlTextWriter(sw);
                        xw.Formatting = Formatting.Indented;
                        xw.Indentation = 4;

                        xw.WriteStartElement("root");
                        xw.WriteEndElement();

                        xw.Close();
                        SL.Text = sw.ToString();
                    }
                }
                else
                {
                    BO.GetReportData(SL);
                }
            }

            else if (Utils.Copy(temp, 1, 4) == "Web.")
            {
                if (temp.Contains("Event"))
                {
                Output02.ReportID = 0;
                Output02.GetMsg(SL, Utils.Copy(temp, 5, temp.Length));
            }
                else if (temp.Contains("Race"))
                {
                    Output03.ReportID = 0;
                    Output03.GetMsg(SL, Utils.Copy(temp, 5, temp.Length));
                }
            }

            else
            {
                if (Utils.Copy(temp, 1, 5) == "Data.")
                {
                    temp = Utils.Copy(temp, 6, temp.Length);
                }

                if (temp == "JSXML")
                {
                    SL.Text = BO.JavaScoreXML.ToString();
                    WantPageHeader = false;
                    WantHTMEscape = false;
                }
                else if (temp.StartsWith("H"))
                {
                    Output05.Welcome();
                }
                else if (temp.StartsWith("A"))
                {
                    Output05.Athletes();
                }
                else if (temp.StartsWith("B")) //Backup
                {
                    Output05.Backup(BackupFormat.Default);
                }
                else if (temp.StartsWith("D")) //Data
                {
                    Output05.Backup(BackupFormat.Compact);
                }
                else if (temp.StartsWith("M")) //MsgList
                {
                    Output05.Backup(BackupFormat.MsgList);
                }
                else if (temp.StartsWith("E"))
                {
                    Output05.EventResult(BO.EventNode, true, true);
                }
                else if (temp.StartsWith("P"))
                {
                    Output05.Properties();
                }
            }

            if (WantPageHeader)
            {
                PageFooter();
            }

            if (WantHTMEscape)
            {
                EscapeHTM();
            }

            BO.MsgCounter++;

            return SL.Text;
        }

    }

}
