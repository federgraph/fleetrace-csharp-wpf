using System;
using RiggVar.FR;
using FR60.FRService;

namespace RiggVar.DM
{

    public class TDBEventWEB : IDBEvent
    {
        private FRService DBEvent = new FRService();

        public TDBEventWEB()
            : base()
        {
            this.SetWebApplicationUrl(TMain.IniImage.WebApplicationUrl);
        }
        /// <summary>
        /// Set the path to the WebApplication that hosts the WebService
        /// </summary>
        /// <param name="s">WebApplicationUrl, including trailing slash
        /// </param>
        public void SetWebApplicationUrl(string s)
        {
            if (s.EndsWith("/"))
            {
                DBEvent.Url = s + "FR42_FRService.asmx";
            }
            else
            {
                DBEvent.Url = s + "/FR42_FRService.asmx";
            }
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

        #region IDBEvent2 Member

        public string Load(int KatID, string EventName)
        {
            try
            {
                string s = DBEvent.LoadEventData(KatID, EventName);
                return SwapLineFeed(s);
            }
            catch
            {
                return "";
            }
        }

        public void Save(int KatID, string EventName, string Data)
        {
            try
            {
                string Password = "abc";
                DBEvent.SaveEventData(KatID, EventName, Data, Password);
            }
            catch
            {
            }
        }

        public void Delete(int KatID, string EventName)
        {
            //not implemented
        }

        public string GetEventNames(int KatID)
        {
            try
            {
                string EventFilter = "";
                string s = DBEvent.LoadEventNames(KatID, EventFilter);
                char[] sep = new char[2] { '\r', '\n' };
                string[] a = s.Split(sep);
                TStrings SL = new TStringList();
                foreach (string n in a)
                {
                    SL.Add(n);
                }

                return SL.Text;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return "";
            }
        }

        public void Close()
        {
            //not implemented
        }

        #endregion

    }

}
