using System;
using System.Collections;
using System.Text;

namespace RiggVar.FR
{
    public class TRESTBridge : IBridgeService
    {
        protected const string DefaultControllerUrl = "http://localhost/FR88/";
        private RESTConnection Connection;
        private TStringList SL;
        private Hashtable HT;

        public TRESTBridge(int eventType)
        {
            Connection = new RESTConnection(eventType);
            SL = Connection.SL;
            HT = Connection.HT;
            Connection.SetServerUrl(DefaultControllerUrl);
        }

        public int Plugin()
        {
            string s = Connection.Get("Plugin");
            int i = Utils.StrToIntDef(s, -1);
            return i;
        }

        public void Plugout(int SwitchID)
        {
            SL.Clear();
            SL.Add("SwitchID=" + SwitchID.ToString());
            Connection.Post("Plugout");
        }

        public int SendBackupAndLog(int SwitchID, string Backup, string Log)
        {
            HT.Clear();
            HT.Add("SwitchID", Utils.IntToStr(SwitchID));
            HT.Add("Backup", Backup);
            HT.Add("Log", Log); //Log is always "" (in case of ResultServer)
            string s = Connection.PostMultiPart("SendBackupAndLog");
            int i = Utils.StrToIntDef(s, -1);
            return i;
        }

        public void SendDiffLog(int SwitchID, string DiffLog)
        {
            HT.Clear();
            HT.Add("SwitchID", Utils.IntToStr(SwitchID));
            HT.Add("DiffLog", DiffLog);
            Connection.PostMultiPart("SendDiffLog");
        }

        public void SendMsg(int SwitchID, string msg)
        {
            //Post nicht verwendet wegen Problem mit msg=M1X=150

            //        SL.Clear();
            //        SL.Add("SwitchID=" + Utils.IntToStr(SwitchID));
            //        SL.Add("msg=" + msg);
            //        Post("SendMsg");

            HT.Clear();
            HT.Add("SwitchID", SwitchID.ToString());
            HT.Add("msg", msg);
            Connection.PostMultiPart("SendMsg");
        }

        public void SendContextMsg(int SwitchID, TContextMsg cm)
        {
            SendMsg(SwitchID, cm.msg);
        }

        public void SendAnswer(int Target, string Answer)
        {
            // TODO:  Implementierung von RESTBridge.SendAnswer hinzuf�gen
        }

        public string GetBackup()
        {
            string s = Connection.Get("GetBackup");
            if (s.Equals("e"))
            {
                return "";
            }
            else
            {
                return s;
            }
        }

        public string GetNewMessages(int SwitchID, int StartMsgID)
        {
            SL.Clear();
            SL.Add("SwitchID=" + SwitchID.ToString());
            SL.Add("StartMsgID=" + StartMsgID.ToString());
            string s = Connection.Post("GetNewMessages");
            if (s.Equals("e"))
            {
                return "";
            }
            else
            {
                return s;
            }
        }

        public bool LogValid()
        {
            SL.Clear();
            string s = Connection.Get("LogValid");
            return Utils.IsTrue(s);
        }

        public int GetBackupSwitchID()
        {
            string s = Connection.Get("GetBackupSwitchID");
            int i = Utils.StrToIntDef(s, -1);
            return i;
        }

        public int GetLastBackupID()
        {
            string s = Connection.Get("GetLastBackupID");
            int i = Utils.StrToIntDef(s, -1);
            return i;
        }

        public int GetLastMsgID()
        {
            string s = Connection.Get("GetLastMsgID");
            int i = Utils.StrToIntDef(s, -1);
            return i;
        }

        public bool CheckForBackup(int SwitchID, int StartBackupID)
        {
            SL.Clear();
            SL.Add("SwitchID=" + SwitchID.ToString());
            SL.Add("StartBackupID=" + StartBackupID.ToString());
            string s = Connection.Post("CheckForBackup");
            return Utils.IsTrue(s);
        }

        public bool CheckForLog(int SwitchID, int StartMsgID)
        {
            SL.Clear();
            SL.Add("SwitchID=" + SwitchID.ToString());
            SL.Add("StartMsgID=" + StartMsgID.ToString());
            string s = Connection.Post("CheckForLog");
            return Utils.IsTrue(s);
        }

        public string GetServerUrl()
        {
            return Connection.GetServerUrl();
        }

        public void SetServerUrl(string value)
        {
            Connection.SetServerUrl(value);
        }

        public bool GetHasError()
        {
            // not implemented
            return false;
        }

        public bool IsSameBridge()
        {
            return false;
        }

        public bool IsEnabled(SwitchOp Op)
        {
            switch (Op)
            {
                case SwitchOp.Plugin:
                    return true;
                case SwitchOp.Plugout:
                    return true;
                case SwitchOp.Synchronize:
                    return false;
                case SwitchOp.Upload:
                    return true;
                case SwitchOp.Download:
                    return true;
                default:
                    return false;
            }
        }

        public void DoOnIdle()
        {
            // not implemented
        }

        public void Close()
        {
            // not implemented
        }

        public virtual void GetStatusReport(StringBuilder sb)
        {
            sb.Append("-- TRESTBridge" + Environment.NewLine);
            //sb.AppendLine("ServerUrl = " + getServerUrl());
        }

    }
}
