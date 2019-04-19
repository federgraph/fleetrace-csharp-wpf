using System;
using System.Text;

namespace RiggVar.FR
{
    public class TAsynchronBridge : IBridgeService
    {
        protected int FSwitchID;
        protected int FMsgID;
        protected int FBackupID;
        protected int FBackupSwitchID;
        protected bool FLogValid;
        protected bool FCheckForBackup;
        protected bool FCheckForLog;

        protected TStringList SL;
        protected int MsgCounterIn;
        protected int MsgCounterOut;
        protected string MsgText;

        public TBridgeController BridgeController;
        protected TBaseIniImage IniImage;

        protected MemoLogger Logger => TMain.PeerManager.Logger;

        public TAsynchronBridge(TBaseIniImage aIniImage) 
        {
            IniImage = aIniImage;
            SL = new TStringList();
        }

        protected int PopInt()
        {
            int result = -1;
            if (SL.Count > 0)
            {
                result = Utils.StrToIntDef(SL[0], -1);
                SL.Delete(0);
            }
            return result;
        }
        protected string PopStr()
        {
            string result = "";
            if (SL.Count > 0)
            {
                result = SL[0];
                SL.Delete(0);
            }
            return result;
        }

        protected bool PopBool()
        {
            bool result = false;
            if (SL.Count > 0)
            {
                result = Utils.IsTrue(SL[0]);
                SL.Delete(0);
            }
            return result;
        }

        protected virtual void Send(string s)
        {
            //virtual
        }

        public virtual int Plugin() 
        {
            Logger.AppendLine("AsynchronBridge.Plugin()");
            SL.Clear();
            SL.Add("Plugin");
            SL.Add("");
            Send(SL.Text);
            return FSwitchID;
        }

        public virtual void Plugout(int SwitchID) 
        {
            Logger.AppendLine(string.Format("AsynchronBridge.Pluout(), SwitchID = {0}", SwitchID));
            SL.Clear();
            SL.Add("Plugout");
            SL.Add("");
            SL.Add(SwitchID.ToString());
            Send(SL.Text);
        }

        public virtual int SendBackupAndLog(int SwitchID, string Backup, string Log) 
        {
            Logger.AppendLine("AsynchronBridge.SendBackupAndLog()");
            //Log is always '' (in case of ResultServer)
            SL.Text = Backup;
            SL.Insert(0, SwitchID.ToString());
            SL.Insert(0, "SendBackup");
            Send(SL.Text);
            return FBackupID;
        }

        public virtual void SendDiffLog(int SwitchID, string DiffLog) 
        {
            Logger.AppendLine("AsynchronBridge.SendDiffLog()");
            SL.Text = DiffLog;
            SL.Insert(0, SwitchID.ToString());
            SL.Insert(0, "SendDiffLog");
            Send(SL.Text);
        }

        public virtual void SendMsg(int SwitchID, string msg) 
        {
            SL.Clear();
            SL.Add("SendMsg");
            SL.Add(SwitchID.ToString());
            SL.Add(msg);
            Send(SL.Text);
        }

        public virtual void SendContextMsg(int SwitchID, TContextMsg cm) 
        {
            //overridden by TServerBridge
            SendMsg(SwitchID, cm.msg);
        }

        public virtual string GetBackup() 
        {
            Logger.AppendLine("AsynchronBridge.GetBackup()");
            Send("GetBackup");
            return ""; //asynchron, notify later when backup has arrived
        }

        public virtual string GetNewMessages(int SwitchID, int StartMsgID) 
        {
            Logger.AppendLine("AsynchronBridge.GetNewMessages()");
            SL.Clear();
            SL.Add("GetNewMessages");
            SL.Add(SwitchID.ToString());
            SL.Add(StartMsgID.ToString());
            Send(SL.Text);
            return "";
        }

        public virtual bool LogValid() 
        {
            Logger.AppendLine("AsynchronBridge.LogValid()");
            SL.Clear();
            SL.Add("LogValid");
            SL.Add("");
            Send(SL.Text);
            return FLogValid;
        }

        public virtual int GetBackupSwitchID() 
        {
            Logger.AppendLine("AsynchronBridge.GetBackupSwitchID()");
            SL.Clear();
            SL.Add("GetBackupSwitchID");
            SL.Add("");
            Send(SL.Text);
            return FBackupSwitchID;
        }

        public virtual int GetLastBackupID() 
        {
            Logger.AppendLine("AsynchronBridge.GetLastBackupID()");
            SL.Clear();
            SL.Add("GetLastBackupID");
            SL.Add("");
            Send(SL.Text);
            return FBackupID;
        }

        public virtual int GetLastMsgID() 
        {
            Logger.AppendLine("AsynchronBridge.GetLastMsgID()");
            SL.Clear();
            SL.Add("GetLastMsgID");
            SL.Add("");
            Send(SL.Text);
            return FMsgID;
        }

        public virtual bool CheckForBackup(int SwitchID, int StartBackupID) 
        {
            Logger.AppendLine("AsynchronBridge.CheckForBackup()");
            SL.Add("CheckForBackup");
            SL.Add(SwitchID.ToString());
            SL.Add(StartBackupID.ToString());
            Send(SL.Text);
            return FCheckForBackup;
        }

        public virtual bool CheckForLog(int SwitchID, int StartMsgID) 
        {
            return FCheckForLog;
        }

        public string GetServerUrl() 
        {
            return "";
        }

        public void SetServerUrl(string Value) 
        {
        }

        public virtual bool GetHasError() 
        {
            return false;
        }

        public virtual bool IsEnabled(SwitchOp Op)
        {
            return false;
        }

        public virtual bool IsSameBridge()
        {
            //PeerController.AllowRecreate may need to detect configuration changes

            //check instance properties against configuration for changed parameters
            //return true to suppress recreation of bridge with same params
            //which might fail, e.g. because of 'port already open'
            return false;
        }

        public virtual void Close()
        {
        }

        public void SetMsg(object sender, string s) 
        {
            SetOutputMsg(sender, s);
        }

        public virtual void DoOnIdle()
        {
            //virtual: TClientBridge will process MsgQueue
        }

        protected void SetOutputMsg(object Sender, string msg)
        {
            MsgCounterIn++;
            MsgText = Utils.SwapLineFeed(msg);
            if (MsgText != "")
            {
                SL.Text = MsgText;
                string s = PopStr();
                if (s != "")
                {
                    DispatchMsg(s);
                }
            }
        }

        protected virtual void DispatchMsg(string s)
        {
            if (s == "Plugin")
            {
                int switchID = PopInt();
                BridgeController.DoOnPlugin(switchID);
            }

            else if (s == "SendBackup") //SendBackupAndLog
            {
                int backupID = PopInt();
                BridgeController.DoOnGetLastBackupID(backupID);
            }

            else if (s == "GetBackup")
            {
                if (SL.Count > 0)
                {
                    BridgeController.DoOnBackup(SL.Text);
                }
            }

            else if (s == "GetNewMessages")
            {
                BridgeController.DoOnNewMessages(SL.Text);
            }

            else if (s == "LogValid")
            {
                FLogValid = PopBool();
            }

            else if (s == "GetLastBackupID")
            {
                int backupID = PopInt();
                BridgeController.DoOnGetLastBackupID(backupID);
            }

            else if (s == "GetLastMsgID")
            {
                int msgID = PopInt();
                BridgeController.DoOnGetLastMsgID(msgID);
            }

            else if (s == "GetLastBackupSwitchID")
            {
                FBackupSwitchID = PopInt();
                //BridgeController.
            }

            else if (s == "CheckForBackup")
            {
                FCheckForBackup = PopBool();
                //BridgeController
            }

            else if (s == "CheckForLog")
            {
                FCheckForLog = PopBool();
                //BridgeController
            }
        }        

        public virtual void SendAnswer(int Target, string Answer)
        {
            //virtual
        }

        public virtual void GetStatusReport(StringBuilder sb)
        {
            string crlf = Environment.NewLine;

            sb.Append("-- TAsynchronBridge");
            sb.Append(crlf);

            sb.Append("SwitchID = ");
            sb.Append(FSwitchID);
            sb.Append(crlf);

            sb.Append("MsgID = ");
            sb.Append(FMsgID);
            sb.Append(crlf);

            sb.Append("BackupID = ");
            sb.Append(FBackupID);
            sb.Append(crlf);

            sb.Append("BackupSwitchID = ");
            sb.Append(FBackupSwitchID);
            sb.Append(crlf);

            sb.Append("LogValid = ");
            sb.Append(FLogValid);
            sb.Append(crlf);

            sb.Append("CheckForBackup = ");
            sb.Append(FCheckForBackup);
            sb.Append(crlf);

            sb.Append("CheckForLog = ");
            sb.Append(FCheckForLog);
            sb.Append(crlf);

            sb.Append("MsgCounterIn = ");
            sb.Append(MsgCounterIn);
            sb.Append(crlf);

            sb.Append("MsgCounterOut = ");
            sb.Append(MsgCounterOut);
            sb.Append(crlf);

            sb.Append("MsgText = ");
            sb.Append(MsgText);
            sb.Append(crlf);
        }

    }
}
