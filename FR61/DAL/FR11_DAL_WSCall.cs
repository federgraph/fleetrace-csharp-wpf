using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using System.Text;

namespace RiggVar.FR
{

    /*        
    <?xml version="1.0" ?>
    <e>
    <answer request="SB.PGS.Output.XML.WF1">
    <![CDATA[
    <?xml version="1.0" ?>
    <Finals>
    <F Gender="W" RoundID="WF1" Pos="1" Heat="1" SNR="0" Bib="1" FLNR="" GTime="" GRank="0" C1="R" DG1="0" QU1="ok" OTime1="" RTime1="" RR1="0" C2="B" DG2="0" QU2="ok" OTime2="" RTime2="" RR2="0" GTime2="" GRank2="0" C3="R" DG3="0" QU3="ok" OTime3="" RTime3="" RR3="0" GTime3="" GRank3="0" />
    <F Gender="W" RoundID="WF1" Pos="2" Heat="1" SNR="0" Bib="2" FLNR="" GTime="" GRank="0" C1="B" DG1="0" QU1="ok" OTime1="" RTime1="" RR1="0" C2="R" DG2="0" QU2="ok" OTime2="" RTime2="" RR2="0" GTime2="" GRank2="0" C3="B" DG3="0" QU3="ok" OTime3="" RTime3="" RR3="0" GTime3="" GRank3="0" />
    </Finals>

    ]]>
    </answer>

    <answer request="SB.PGS.Output.XML.Data.Properties">
    <![CDATA[
    <?xml version="1.0" ?>
    <Properties>
    <P PropName="W.Penalty" PropValue="1.80" />
    <P PropName="M.Penalty" PropValue="1.70" />
    <P PropName="W.QA.STL.Count" PropValue="35" />
    <P PropName="M.QA.STL.Count" PropValue="35" />
    <P PropName="LockPenaltyW" PropValue="True" />
    <P PropName="LockPenaltyM" PropValue="False" />
    <P PropName="SkipWF5" PropValue="False" />
    <P PropName="SkipMF5" PropValue="False" />
    <P PropName="EventModelEnabled" PropValue="False" />
    </Properties>

    ]]>
    </answer>

    </e>    
    */

    public interface ICalcService
    {
        string Calc(int KatID, string EventData);
        string Test(int KatID, string EventData);
        IDictionary GetAnswerObjects();
    }

    public class WSCall
    {
        private static readonly string crlf = "\r\n";
        internal bool isOnline = false;
        internal bool isError = false;
        protected string answer = string.Empty;

        public int KatID = 0;
        public string EventName = string.Empty;
        public string EventData = string.Empty;
        public string Request = string.Empty;
        public string Updates = string.Empty;

        public StringCollection RequestCollection;
        public StringCollection OptionCollection;
        public StringCollection AnswerCollection;

        protected ICalcService cs;

        public WSCall(ICalcService calcService)
        {
            cs = calcService;

            RequestCollection = new StringCollection();
            OptionCollection = new StringCollection();
            AnswerCollection = new StringCollection();
        }
        public bool IsOnline
        {
            get => isOnline;
            set => isOnline = value;
        }
        public bool IsError => IsOnline;
        public string OfflineMsg()
        {
            if (IsOnline)
            {
                return string.Empty;
            }
            else if (IsError)
            {
                return answer;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<table width=\"100%\" bgcolor=\"#ffcccc\"><tr><td>");
                sb.Append("<FONT face=\"Courier New\" size=\"2\">");
                sb.Append("cannot process request");
                sb.Append("</FONT>");
                sb.Append("</td></tr></table>");
                return sb.ToString();
            }
        }
        protected virtual string Test(string request)
        {
            return cs.Test(KatID, request);
        }
        protected virtual string Calc(string request)
        {
            return cs.Calc(KatID, request);
        }
        protected virtual void SaveAnswer(string answer)
        {
            //virtual
        }
        protected virtual string LoadAnswer()
        {
            //virtual
            return string.Empty;
        }
        public IDictionary AnswerObjects
        {
            get
            {
                return cs.GetAnswerObjects();
            }
        }
        public bool Test()
        {
            string request = "abc\ndef";
            answer = Test(request);
            bool result = true;
            result = result && (answer.IndexOf("1:") > -1);
            result = result && (answer.IndexOf("abc") > -1);
            result = result && (answer.IndexOf("2:") > -1);
            result = result && (answer.IndexOf("def") > -1);
            return result;
        }
        public void Calc()
        {
            AnswerCollection.Clear();

            //single-line request
            if (Request != string.Empty)
            {
                AnswerCollection.Add(string.Empty);
            }
                //no request specified at all
            else if (RequestCollection.Count == 0)
            {
                AnswerCollection.Add("RequestCollection is empty");
                return;
            }

            //Prepare the Call
            string request = GetRequest();
            answer = string.Empty;

            answer = Calc(request);
            if (answer == string.Empty)
            {
                return;
            }

            //Process Answer
            if (Request != string.Empty)
            {
                AnswerCollection[0] = answer;
            }
            else
            {
                try
                {
                    ProcessAnswer(answer);
                }
                catch
                {
                    isError = true;
                }
            }
        }
        private string GetRequest()
        {
            StringBuilder sb = new StringBuilder();
            AnswerCollection.Clear();
            //single-line request
            if (Request != string.Empty)
            {
                sb.Append(Request);
                sb.Append(crlf);
                AnswerCollection.Add(string.Empty);
            }
            //multi-line requests
            foreach (string s in RequestCollection)
            {
                sb.Append(s + crlf);
                AnswerCollection.Add(string.Empty);
            }
            //options
            foreach (string s in OptionCollection)
            {
                sb.Append(s + crlf);
            }
            //event data
            sb.Append(EventData);
            sb.Append(crlf);
            //updates to the eventdata, if any
            if (Updates != string.Empty)
            {
                sb.Append(Updates);
                sb.Append(crlf);
            }
            return sb.ToString();
        }
        private void ProcessAnswer(string answer)
        {
            string request = "";

            try
            {
                XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(answer);
                XmlNode n = doc.DocumentElement;

                if (n.HasChildNodes)
                {
                    XmlNodeList answerNodeList = n.ChildNodes;

                    for (int k = 0; k < answerNodeList.Count; k++)
                    {
                        XmlNode answerNode = answerNodeList.Item(k);

                        if (answerNode.NodeType == XmlNodeType.Element)
                        {
                            XmlAttributeCollection kids = answerNode.Attributes;
                            if (kids.Count > 0)
                            {
                                XmlNode n2 = kids.Item(0);
                                if (n2.Name.Equals("request"))
                                {
                                    request = n2.Value;
                                    request = TranslateRequest(request);
                                }
                                else
                                {
                                    request = "";
                                }
                            }
                            if (answerNode.HasChildNodes)
                            {
                                for (int i = 0; i < RequestCollection.Count; i++)
                                {
                                    if (request == RequestCollection[i])
                                    {
                                        AnswerCollection[i] = answerNode.InnerText.Trim();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
        private string TranslateRequest(string request)
        {
            if (KatID == LookupKatID.SKK)
            {
                return request.Replace("SKK.Kreise.Output", "SKK.Kreise.Request");
            }
            else if (KatID == LookupKatID.Rgg)
            {
                return request.Replace("RiggVar.Rgg.Output", "RiggVar.Rgg.Request");
            }
            else if (KatID >= 400)
            {
                return request.Replace("FR.*.Output", "FR.*.Request");
            }
            else if (KatID >= 300)
            {
                return request.Replace("SB.PGS.Output", "SB.PGS.Request");
            }
            else
            {
                return request;
            }
        }
    }

}
