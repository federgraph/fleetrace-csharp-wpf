using System;
using System.Text;

namespace RiggVar.FR
{

    /// <summary>
    /// (U)pdate (U)ser (I)nterface Action constants
    /// </summary>
    public class UUIAction
    {
        public const int Port = 0;
        public const int SwitchID = 1;
        public const int MsgID = 2;
        public const int BackupID = 3;
        public const int BackupMsgID = 4;
        public const int BackupSwitchID = 5;
        public const int MsgCounterIn = 6;
        public const int Msg = 7;
        public const int Clear = 8;
        public const int TopicCount = 9;
        //  public const int ClientCounter = 10;
        //  public const int TopicBroadcast = 11;
        public const int TopicRequest = 12;
    }

    public enum SwitchOp
    {
        Plugin,
        Plugout,
        Synchronize,
        Upload,
        Download
    }

    public class TPeerController
    {
        public bool IsBridgeServerPluggedIn;
        public bool PlugTouched;
        public THandleMsgEvent OnBackup;

        public TPeerController(TBaseIniImage aIniImage) 
        {
        }

        public virtual void GetStatusReport(StringBuilder sb)
        {
            sb.Append("ClassType: TPeerController (abstract)" + Environment.NewLine);
        }

        public virtual bool IsEnabled(SwitchOp Op)
        {
            return false;
        }
        public virtual bool AllowRecreate => true;
        public virtual bool IsMaster => false;

        public virtual void Connect() {}
        public virtual void Disconnect() {}

        public virtual bool Connected => false;
        public virtual void Close() {}
        
        public virtual void DoOnIdle() {}
        public virtual void DoOnBackup(string s)         
        {
            if (s != "") 
            {
                OnBackup?.Invoke(this, s);
            }
        }

        public virtual void EditProps() {}

        public virtual void Plugin() {}
        public virtual void Plugout() {}
        public virtual void Synchronize() {}
        public virtual void Upload(string s) {}
        public virtual string Download() 
        {
            return "";
        }
    }
}
