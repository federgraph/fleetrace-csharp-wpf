using System;
using System.Text;

namespace RiggVar.FR
{
    //Global.asax:
    //void Application_Start(object sender, EventArgs e) 
    //{
    //    Application["FRBridge"] = new riggvar.fr88.BridgeServiceFR(2000);
    //    Application["SKKBridge"] = new riggvar.fr88.BridgeServiceSKK(10);        
    //}

    //web.config:
    //    <httpHandlers>
    //      <add verb="GET,POST" path="REST/Bridge/*/*.aspx" type="riggvar.fr88.BridgeHandler" />
    //    </httpHandlers>


    /// <summary>
    /// Summary description for BridgeServiceImpl
    /// </summary>
    public abstract class BridgeService : IBridgeService 
    {
        protected DateTime FStartupTime = DateTime.Now;

        /// <summary>
        /// line separator token
        /// </summary>
        private string crlf = "\r\n";

        /// <summary>
        /// stores the backup as a multiline text string
        /// </summary>
        public string FBackup;

        /// <summary>
        /// part of the backup,
        /// used when a backup is uploaded from a switch client
        /// </summary>
        public string FLog;

        /// <summary>
        /// stores the difflog, 
        /// can overflow,
        /// clients cannot synchronize after overflow,
        /// synchronized clients may stay synchronized even after overlow
        /// </summary>
        protected IMsgRing FMsgRing;

        /// <summary>
        /// used to generate new client id,
        /// incremented with each plugin
        /// </summary>
        public int FNextBridgeID;

        /// <summary>
        /// incremented when a new backup is stored
        /// </summary>
        public int FBackupID;

        /// <summary>
        /// identifies the client who stored the backup
        /// </summary>
        public int FBackupSwitchID;

        /// <summary>
        /// current msg id (in the difflog) at the time the backup was stored
        /// </summary>
        public int FBackupMsgID;

        /// <summary>
        /// used to request a client id,
        /// client id is used to prevent retrieval of own messages from the difflog
        /// </summary>
        /// <returns>client id, unique within the application-lifetime of the service</returns>
        public int Plugin()
        {
            FNextBridgeID++;
            return FNextBridgeID;
        }

        /// <summary>
        /// does nothing at the moment
        /// </summary>
        /// <param name="SwitchID">client id</param>
        public void Plugout(int SwitchID)
        {
            //do nothing
        }

        /// <summary>
        /// the primary method to upload and store a backup
        /// </summary>
        /// <param name="SwitchID"></param>
        /// <param name="Backup">the backup</param>
        /// <param name="Log">when uploading from a switch, contains last changes to backup; 
        /// present at the upload-time, considered part of the backup</param>
        /// <returns>id of last msg in the difflog</returns>
        public int SendBackupAndLog(int SwitchID, string Backup, string Log)
        {
            FBackup = Backup;
            FLog = Log;
            FBackupID++;
            FBackupSwitchID = SwitchID;
            FBackupMsgID = FMsgRing.MsgID;
            FMsgRing.Clear();
            return FMsgRing.MsgID;
        }

        /// <summary>
        /// A packet of messages,
        /// use SendMsg() if there is only one msg in the packet.
        /// </summary>
        /// <param name="SwitchID">client id</param>
        /// <param name="DiffLog">multiline text string</param>
        public void SendDiffLog(int SwitchID, string DiffLog)
        {
            //since .net 2.0:
            //string [] c = new string [1] {"\n"};
            //String [] o = DiffLog.Split(c, StringSplitOptions.RemoveEmptyEntries);

            //in .net 1.1:
            char[] c = new char [1] {'\n'};
            string[] o = DiffLog.Split(c);

            string msg;
            foreach (string s in o)
            {
                if (s == string.Empty)
                {
                    continue;
                }

                if (s.EndsWith("\r"))
                {
                    msg = s.Remove(s.Length - 1, 1);
                    SendMsg(SwitchID, msg);
                }
                else
                {
                    SendMsg(SwitchID, s);
                }
            }
        }

        /// <summary>
        /// Used to add a single message to the difflog.
        /// </summary>
        /// <param name="SwitchID">client id</param>
        /// <param name="msg">single line message</param>
        /// <returns>id of last msg in the difflog</returns>
        public void SendMsg(int SwitchID, string msg)
        {
            if (msg == null)
            {
                return;
            }

            if (msg == "")
            {
                return;
            }

            if (msg.EndsWith("\r\n"))
            {
                msg = msg.Remove(msg.Length - 2, 2);
            }
            if (msg.EndsWith("\n"))
            {
                msg = msg.Remove(msg.Length - 1, 1);
            }            
            FMsgRing.AddMsg(SwitchID, msg);
        }

        /// <summary>
        /// Used to retrieve the base application state of the system,
        /// backup + log, the difflog is not included.
        /// The complete application state would be backup + log + difflog,
        /// if no overlow has occured to the difflog yet.
        /// </summary>
        /// <returns>multiline text string representing the state of the application</returns>
        public string GetBackup()
        {
            return FBackup + crlf + FLog;
        }

        /// <summary>
        /// Used to retrieve new messages, sent by other clients,
        /// Use StartMsgID = 0 to retrieve all available messages.
        /// Use SwitchID = 0 to include own messages.
        /// Note that the difflog can overflow.
        /// </summary>
        /// <param name="SwitchID">client id</param>
        /// <param name="StartMsgID">ervery message with greater id is a new message</param>
        /// <returns>list of messages in the form of a multiline text string</returns>
        public string GetNewMessages(int SwitchID, int StartMsgID)
        {
            StringBuilder sb = new StringBuilder();
            FMsgRing.GetDelta(SwitchID, sb, StartMsgID);
            return sb.ToString();
        }

        /// <summary>
        /// Used to identify an overflow of the difflog.
        /// An overflow occure if more than capacity of messages were sent after the last backup.
        /// Note that in a merge situation, where messages may be replaced rather then added, 
        /// the difflog may never overflow. If a message merge is used depends on the application.
        /// </summary>
        /// <param name="StartMsgID"></param>
        /// <returns>false if the difflog has overflowed</returns>
        public bool LogValid()
        {
            return FMsgRing.LogValid();
        }

        /// <summary>
        /// Inquiry 
        /// </summary>
        /// <returns>the id (SwitchID) of the client who stored the backup</returns>
        public int GetBackupSwitchID()
        {
            return FBackupSwitchID;
        }

        /// <summary>
        /// Used to get the id of the current backup.
        /// This is currently the only way to get the id of the backup.
        /// The consequence is an additional server roundtrip.
        /// The decision has been taken to keep things simple.
        /// The answer of GetBackup() does not include the id of the backup.
        /// The alternativs are to return multiple output values in one call,
        /// or encode the different output parts in the string answer of
        /// GetBackup().
        /// </summary>
        /// <returns>BackupID for use in call to CheckForBackup()</returns>
        public int GetLastBackupID()
        {
            return FBackupID;
        }

        /// <summary>
        /// Used to get the id of the last message in the difflog.
        /// </summary>
        /// <returns>last msg id in difflog</returns>
        public int GetLastMsgID()
        {
            return FMsgRing.MsgID;
        }

        /// <summary>
        /// Used to peek for availability of a new backup.
        /// </summary>
        /// <param name="SwitchID">client filter, ignore uploads by self</param>
        /// <param name="StartBackupID">time filter, look for new backups only</param>
        /// <returns>true if new backup is available</returns>
        public bool CheckForBackup(int SwitchID, int StartBackupID)
        {
            if (FBackup == null || FBackup == "")
            {
                return false;
            }

            if (SwitchID == FBackupSwitchID)
            {
                return false;
            }

            return StartBackupID < FBackupID;
        }

        /// <summary>
        /// Used to peek for new messages in the difflog.
        /// </summary>
        /// <param name="SwitchID">used to exlude own</param>
        /// <param name="StartMsgID">used to define what means new</param>
        /// <returns>true if new messages found</returns>
        public bool CheckForLog(int SwitchID, int StartMsgID)
        {
            return FMsgRing.GetDiffCount(SwitchID, StartMsgID) > 0;
        }

        /// <summary>
        /// Used to lookup current msg id (in MsgRing).
        /// </summary>
        public int MsgID
        {
            get
            {
                return FMsgRing.MsgID;
            }
        }

        /// <summary>
        /// Clear backup, log, and difflog.
        /// Also clears out the identity of the client who stored the backup.
        /// </summary>
        public void Clear()
        {
            FBackup = "";
            FLog = "";
            FMsgRing.Clear();
            FBackupSwitchID = 0;
        }

        /// <summary>
        /// used only for testing
        /// </summary>
        public void InitTestState()
        {
            FMsgRing.Clear();
            
            FBackup = "Backup" + crlf + "Line1" + crlf;
            FLog = "Log" + crlf + "Line1" + crlf;
            FMsgRing.AddMsg(1, "Test.ID=1");
            FMsgRing.AddMsg(1, "Test.ID=2");
            FMsgRing.AddMsg(2, "Test.ID=3");
            FMsgRing.AddMsg(2, "Test.ID=4");
            FMsgRing.AddMsg(1, "Test.ID=5");
            FMsgRing.AddMsg(1, "Test.ID=6");
            FMsgRing.AddMsg(2, "Test.ID=7");
            FMsgRing.AddMsg(2, "Test.ID=8");

            FNextBridgeID = 4;
            FBackupID = 2;
            FBackupSwitchID = 3;
            FBackupMsgID = 15;
        }

        /// <summary>
        /// includes the content of the integer, only for debugging
        /// </summary>
        /// <returns>a multiline text string for display in a pre-tag</returns>
        public string GetIDReport()
        {
            StringBuilder sb = new StringBuilder();

            GetStatusReport(sb);

            return sb.ToString();
        }

        public virtual void GetStatusReport(StringBuilder sb)
        {
            sb.Append("BridgeService:" + crlf);

            sb.Append("StartupTime = ");
            sb.Append(FStartupTime.ToString());
            sb.Append(crlf);

            sb.Append("NextBridgeID = ");
            sb.Append(FNextBridgeID);
            sb.Append(crlf);

            sb.Append("BackupID = ");
            sb.Append(FBackupID);
            sb.Append(crlf);

            sb.Append("BackupSwitchID = ");
            sb.Append(FBackupSwitchID);
            sb.Append(crlf);

            sb.Append("BackupMsgID = ");
            sb.Append(FBackupMsgID);
            sb.Append(crlf);

            sb.Append("MsgID = ");
            sb.Append(MsgID);
            sb.Append(crlf); 
        }

        public virtual string GetServerUrl() 
        { 
            return ""; 
        }
        public virtual void SetServerUrl(string Value)
        { 
        }

        public virtual bool GetHasError() 
        { 
            return false; 
        }

        public bool IsSameBridge()
        {
            return false;
        }

        public virtual bool IsEnabled(SwitchOp Op)
        { 
            return false; 
        }
        
        public virtual void DoOnIdle()
        { 
        }

        public virtual void Close() 
        { 
        }

        public virtual void SendAnswer(int Target, string Answer)
        {
        }

        public virtual void SendContextMsg(int SwitchID, TContextMsg cm)
        {
            SendMsg(SwitchID, cm.msg);
        }

    }

    public class BridgeServiceFR : BridgeService
    {
        public BridgeServiceFR(int Capacity)
        {
            FMsgRing = new MsgRing(Capacity);
        } 
    }

    public class BridgeServiceSKK : BridgeService
    {
        public BridgeServiceSKK(int Capacity)
        {
            FMsgRing = new MergeMsgRing(Capacity);
        }
    }

}
