using System.Text;

namespace RiggVar.FR
{

    public interface IBridgeService 
    {
        int Plugin();
        void Plugout(int SwitchID);

        int SendBackupAndLog(int SwitchID, string Backup, string Log);
        void SendDiffLog(int SwitchID, string DiffLog);
        void SendMsg(int SwitchID, string msg);
        void SendAnswer(int Target, string Answer);
        void SendContextMsg(int SwitchID, TContextMsg cm);

        string GetBackup();
        string GetNewMessages(int SwitchID, int StartMsgID);

        bool LogValid();

        int GetBackupSwitchID();
        int GetLastBackupID();
        int GetLastMsgID();

        bool CheckForBackup(int SwitchID, int StartBackupID);
        bool CheckForLog(int SwitchID, int StartMsgID);

        string GetServerUrl();
        void SetServerUrl(string Value);

        bool GetHasError();
        bool IsEnabled(SwitchOp Op);
        bool IsSameBridge();
        
        void DoOnIdle();
        void Close();

        void GetStatusReport(StringBuilder sb);
    }
}
