using System.Collections;
using RiggVar.FR;

namespace RiggVar.DM
{

    public class TDBEventREST : IDBEvent
    {
        RESTConnection Connection;
        TStringList SL;
        Hashtable HT;

        public TDBEventREST()
        {
            Connection = new RESTConnection(0);
            SL = Connection.SL;
            HT = Connection.HT;
            string root = GetRoot();
            Connection.SetServerUrl(root);
        }

        private string GetRoot()
        {
            string serverUrl = TMain.IniImage.WebApplicationUrl;
            if (serverUrl != null)
            {
                if (!serverUrl.EndsWith("/"))
                {
                    serverUrl += '/';
                }

                serverUrl += "REST/";
            }
            else
            {
                serverUrl = Connection.GetServerUrl();
            }

            return serverUrl;
        }

        private string SwapLineFeed(string s)
        {
            if (s.Length > 0)
            {
                s = s.Replace("\r\n", "\n");
                s = s.Replace("\n", "\r\n");
            }
            return s;
        }

        private string CheckEventNames(string s)
        {
            if (s == null)
            {
                return "";
            }

            if (s.Trim().Equals(""))
            {
                return "";
            }

            if (s.IndexOf("\r\n") < 1)
            {
                char[] sep;
                sep = new char[1] { '\n' };
                string[] a = s.Split(sep);
                TStrings lines = new TStringList();
                foreach (string n in a)
                {
                    lines.Add(n);
                }

                return lines.Text;
            }

            return s;
        }

        #region IDBEvent2 Member

        public string Load(int KatID, string EventName)
        {
            SL.Clear();
            SL.Add("KatID=" + KatID);
            SL.Add("EventName=" + EventName);
            string s = Connection.Post("LoadEventData");
            SwapLineFeed(s);
            if (s == "error")
            {
                return "";
            }
            else
            {
                return s;
            }
        }

        public void Save(int KatID, string EventName, string Data)
        {
            HT.Clear();
            HT.Add("KatID", Utils.IntToStr(KatID));
            HT.Add("EventName", EventName);
            HT.Add("EventData", Data);
            HT.Add("Password", "abc");
            string s = Connection.PostMultiPart("SaveEventData");
            System.Diagnostics.Debug.WriteLine(s);
        }

        public void Delete(int KatID, string EventName)
        {
            //not implemented
        }

        public string GetEventNames(int KatID)
        {
            string EventFilter = "FR";

            SL.Clear();
            SL.Add("KatID=" + KatID);
            SL.Add("EventFilter=" + EventFilter);
            string result = Connection.Post("LoadEventNames");
            result = CheckEventNames(result);
            return result;
        }

        public void Close()
        {
            //nothing to do
        }

        #endregion

    }

}
