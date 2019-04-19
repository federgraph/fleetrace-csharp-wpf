//#define Indy

using System;
using System.IO;
using System.Xml;
using RiggVar.Scoring;

namespace RiggVar.FR
{
    /// <summary>
    /// ScoreRegatta by passing xml to native javascore via TCP/IP
    /// </summary>
    public class TCalcEventProxy04 : TCalcEventProxy11
    {
        public string Host;
        public int Port;
        public int Timeout;

        public TCalcEventProxy04() : base()
        {
            Host = TMain.IniImage.CalcHost;
            Port = TMain.IniImage.CalcPort;
            Timeout = 1000;
        }
        private    void ScoreRegattaXml(TFRProxy p)
        {
            string xml;

            //build Xml in UTF-8
            MemoryStream ms = new MemoryStream();
            ms.WriteByte(2);
            XmlTextWriter tw = new XmlTextWriter(ms, System.Text.Encoding.UTF8)
            {
                Formatting = Formatting.Indented
            };
            p.WriteXml(tw);
            tw.Flush();
            ms.WriteByte(3);

            //xml-string wird im Fehlerfall wieder zur�ckgegeben
            ms.Seek(0, SeekOrigin.Begin);
            TextReader tr = new StreamReader(ms);
            xml = tr.ReadToEnd();

            byte [] b = ms.ToArray();

            string xml2 = ScoreRegattaRemote(xml, b);

            if (xml2.StartsWith(((char)2).ToString()))
            {
                //kein remote scoring durchgef�hrt
                //dennoch sinnvoll zum debuggen von proxy read write
                p.ReadXml(Trim2and3(xml)); //wurde nicht
            }
            else
            {
                p.ReadXml(xml2);
            }
        }
//        private    void ScoreRegattaXml1(TFRProxy p)
//        {
//            string xml;
//            StringWriter sw = new StringWriter();
//            XmlTextWriter tws = new XmlTextWriter(sw);
//            p.WriteXml(tws);
//            xml = sw.ToString(); //<-- UTF-16, erforderlich: UTF-8
//
//            //string xml2 = ScoreRegattaRemote1((char)2 + xml + (char)3));
//            string xml2 = ScoreRegattaRemote1(xml);
//
//            p.ReadXml(xml2);
//        }

        private string Trim2and3(string s)
        {
            if (
                (s != "") && 
                (s.Length > 3) && 
                (s[0] == (char)2) && 
                (s[s.Length-1] == (char)3)
                )
            {
                return Utils.Copy(s, 2, s.Length-2);
            }
            return s;
        }

        protected virtual string ScoreRegattaRemote(string xml, byte [] b)
        {
            string result = xml;
#if Indy
            if (Host == "") return result;
            if (Port <= 0) return result;
            if (Timeout <= 10) 
                Timeout = 1000;
            try
            {
                if (xml.Length != b.Length)
                    System.Diagnostics.Debug.WriteLine(b.Length);
                Indy.Sockets.TCPClient TestClient = new Indy.Sockets.TCPClient();
                try
                {
                    TestClient.ConnectTimeout = Timeout;
                    TestClient.Connect(Host, Port);
                    if (TestClient.Connected())
                    {
                        TestClient.IOHandler.Write(b);
                        string temp = ((char)3).ToString();
                        string s = TestClient.IOHandler.ReadLn(temp, 6000, 64000) + (char)3;
                        result = Trim2and3(s);
                        TestClient.Disconnect();
                    }
                }
                finally
                {
                    TestClient.Dispose();
                }
            }
            catch (Exception ex)
            {
                TCalcEvent._ScoringResult = -1;
                TCalcEvent._ScoringExceptionLocation = "TCalcEventProxy4.ScoreRegattaRemote";
                TCalcEvent._ScoringExceptionMessage = ex.Message;
            }    
#endif
            return result;
        }
        protected virtual string ScoreRegattaRemote1(string xml)
        {
            string result = xml;
#if Indy
            if (Host == "") return result;
            if (Port <= 0) return result;
            if (Timeout <= 10) 
                Timeout = 1000;
            try
            {
                Indy.Sockets.TCPClient TestClient = new Indy.Sockets.TCPClient();
                try
                {
                    TestClient.ConnectTimeout = Timeout;
                    TestClient.Connect(Host, Port);
                    if (TestClient.Connected())
                    {
                        TestClient.IOHandler.Write(((char)2 + xml + (char)3));
                        string temp = ((char)3).ToString();
                        string s = TestClient.IOHandler.ReadLn(temp, 6000, 64000) + (char)3;
                        if (
                            (s != "") && 
                            (s.Length > 3) && 
                            (s[0] == (char)2) && 
                            (s[s.Length-1] == (char)3)
                            )
                        {
                            result = Utils.Copy(s, 2, s.Length-2);
                        }
                        TestClient.Disconnect();
                    }
                }
                finally
                {
                    TestClient.Dispose();
                }
            }
            catch (Exception ex)
            {
                TCalcEvent._ScoringResult = -1;
                TCalcEvent._ScoringExceptionLocation = "TCalcEventProxy4.ScoreRegattaRemote";
                TCalcEvent._ScoringExceptionMessage = ex.Message;
            }    
#endif
            return result;
        }
        public override void Calc(TEventNode aqn)
        {
            try
            {
                eventNode = aqn;
                EventProps = TMain.BO.EventProps;

                if (eventNode.Collection.Count == 0)
                {
                    return;
                }

                proxyNode = new TFRProxy();
                try
                {
                    LoadProxy();
                    ScoreRegattaXml(proxyNode);
                    if (WithTest)
                    {
                        WithTest = false;
                        CheckResult(proxyNode);
                    }
                    UnLoadProxy();
                }
                finally
                {                    
                    //p.Free;
                }
                proxyNode = null;
            }
            catch (Exception ex)
            {

                TCalcEvent._ScoringResult = -1;
                TCalcEvent._ScoringExceptionLocation = "TCalcEventProxy4.Calc";
                TCalcEvent._ScoringExceptionMessage = ex.Message;
            }
        }

    }
}
