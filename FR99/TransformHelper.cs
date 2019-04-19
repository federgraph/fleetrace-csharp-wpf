using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System;

namespace FRXML
{
    public class TransformHelper
    {
        private StringBuilder sb = null;
        internal string crlf = Environment.NewLine;
        private int RaceCount = 0;
        public string EventName = "";

        public static string TransformEventData(string EventData)
        {
            TransformHelper xt = new TransformHelper();
            XElement xe = XElement.Parse(EventData);
            StringBuilder sb = new StringBuilder();
            xt.TransformEventData(xe, sb);
            return sb.ToString();
        }

        public void TransformEventData(XElement xe, StringBuilder stringBuilder)
        {
            sb = stringBuilder;
            ExtractEventName(xe);
            ExtractParams(xe);
            ExtractProperties(xe);
            ExtractNameList(xe);
            ExtractStartList(xe);
            ExtractFleetList(xe);
            ExtractFinishList(xe);
            for (int r = 1; r <= RaceCount; r++)
            {
                ExtractTimeList(xe, "R" + r);
            }

            ExtractMsgList(xe);
            sb = null;
        }

        private void ExtractEventName(XElement xe)
        {
            //EventName must be in first EP Element
            XElement e;
            XAttribute a;
            e = xe.Element("Properties");
            if (e != null)
            {
                e = e.Elements("EP").FirstOrDefault();
                if (e != null)
                {
                    a = e.Attribute("V");
                    if (a != null)
                    {
                        EventName = a.Value;
                    }
                    else
                    {
                        EventName = "unknown";
                    }
                }
            }
            //EventName = xe.Element("Properties").Element("EP").Attribute("V").Value;
        }

        private void ExtractMsgList(XElement xe)
        {
            if (xe.Element("MsgList") != null)
            {
                sb.Append(crlf);
                sb.Append("#QU Messages");
                sb.Append(crlf);
                sb.Append(crlf);
                IEnumerable<XElement> cl = xe.Descendants("MsgList").Descendants("ML");
                foreach (XElement cr in cl)
                {
                    sb.Append(cr.Value);
                    sb.Append(crlf);
                }
            }
        }

        private void ExtractTimeList(XElement xe, string RaceID)
        {
            IEnumerable<XElement> el = xe.Descendants("TimeList");
            foreach (XElement e in el)
            {
                if (e.Attribute("RaceID").Value == RaceID)
                {
                    sb.Append(crlf);
                    sb.Append("TimeList.Begin.");
                    sb.Append(RaceID);
                    sb.Append(crlf);
                    IEnumerable<XElement> cl = e.Descendants("TL");
                    foreach (XElement cr in cl)
                    {
                        sb.Append(cr.Value);
                        sb.Append(crlf);
                    }
                    sb.Append("TimeList.End");
                    sb.Append(crlf);
                    break;
                }
            }
        }

        private void ExtractFinishList(XElement xe)
        {
            if (xe.Element("FinishList") != null)
            {
                sb.Append(crlf);
                sb.Append("FinishList.Begin");
                sb.Append(crlf);

                IEnumerable<XElement> cl = xe.Element("FinishList").Descendants("FL");
                foreach (XElement cr in cl)
                {
                    sb.Append(cr.Value);
                    sb.Append(crlf);
                }
                sb.Append("FinishList.End");
                sb.Append(crlf);
            }
        }

        private void ExtractFleetList(XElement xe)
        {
            if (xe.Element("FleetList") != null)
            {
                sb.Append(crlf);
                sb.Append("FleetList.Begin");
                sb.Append(crlf);

                IEnumerable<XElement> cl = xe.Element("FleetList").Descendants("FL");
                foreach (XElement cr in cl)
                {
                    sb.Append(cr.Value);
                    sb.Append(crlf);
                }
                sb.Append("FleetList.End");
                sb.Append(crlf);
            }
        }

        private void ExtractParams(XElement xe)
        {
            XElement cr = xe.Element("Properties");
            if (cr != null)
            {
                sb.Append("#Params");
                sb.Append(crlf);
                sb.Append(crlf);

                sb.Append("DP.StartlistCount=");
                sb.Append(cr.Attribute("StartlistCount").Value);
                sb.Append(crlf);

                sb.Append("DP.ITCount=");
                sb.Append(cr.Attribute("ITCount").Value);
                sb.Append(crlf);

                sb.Append("DP.RaceCount=");
                int.TryParse(cr.Attribute("RaceCount").Value, out RaceCount);
                sb.Append(RaceCount);
                sb.Append(crlf);
            }
        }

        private void ExtractProperties(XElement xe)
        {
            XElement ce = xe.Element("Properties");
            if (ce != null)
            {
                sb.Append(crlf);
                sb.Append("#Event Properties");
                sb.Append(crlf);
                sb.Append(crlf);

                //sb.Append("EP.DivisionName=");
                //sb.Append(ce.Attribute("DivisionName").Value);
                //sb.Append(crlf);

                //sb.Append("EP.InputMode=");
                //sb.Append(ce.Attribute("InputMode").Value);
                //sb.Append(crlf);

                IEnumerable<XElement> cl = ce.Descendants("EP");
                foreach (XElement cr in cl)
                {
                    sb.Append("EP.");
                    sb.Append(cr.Attribute("K").Value);
                    sb.Append('=');
                    sb.Append(cr.Attribute("V").Value);
                    sb.Append(crlf);
                }
            }
        }

        private void ExtractStartList(XElement xe)
        {
            if (xe.Element("StartList") != null)
            {
                sb.Append(crlf);
                sb.Append("StartList.Begin");
                sb.Append(crlf);
                sb.Append("Pos;SNR;Bib");
                IEnumerable<XElement> cl = xe.Element("StartList").Descendants("Pos");
                foreach (XElement cr in cl)
                {
                    sb.Append(crlf);
                    sb.Append(cr.Attribute("oID").Value);
                    sb.Append(';');
                    sb.Append(cr.Attribute("SNR").Value);
                    sb.Append(';');
                    sb.Append(cr.Attribute("Bib").Value);
                }
                sb.Append(crlf);
                sb.Append("StartList.End");
                sb.Append(crlf);
            }
        }

        private void ExtractNameList(XElement xe)
        {
            if (xe.Element("Entries") != null)
            {
                sb.Append(crlf);
                sb.Append("NameList.Begin");
                bool isFirstLine = true;
                IEnumerable<XElement> lines = xe.Element("Entries").Descendants("SNR");
                foreach (XElement line in lines)
                {
                    sb.Append(crlf);
                    IEnumerable<XAttribute> cl = line.Attributes();
                    if (isFirstLine)
                    {
                        isFirstLine = false;
                        sb.Append("SNR");
                        for (int i = 1; i < cl.Count(); i++)
                        {
                            sb.Append(";N");
                            sb.Append(i);
                        }
                        sb.Append(crlf);
                    }

                    foreach (XAttribute cr in cl)
                    {
                        if (cr.PreviousAttribute != null)
                        {
                            sb.Append(';');
                        }
                        if (cr.Value.Contains(' '))
                        {
                            sb.Append('"');
                            sb.Append(cr.Value);
                            sb.Append('"');
                        }
                        else
                        {
                            sb.Append(cr.Value);
                        }
                    }
                }
                sb.Append(crlf);
                sb.Append("NameList.End");
                sb.Append(crlf);
            }
        }

    }

}
