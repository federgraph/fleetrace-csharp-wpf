using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Security;

using RiggVar.FR;
using FR60.RemoteWorkspace;

namespace RiggVar.DM
{

    public class TRemoteWorkspace : IDBEvent
    {
        private RemoteWorkspaceClient DBEvent;

        public TRemoteWorkspace()
        {
            DBEvent = new FR60.RemoteWorkspace.RemoteWorkspaceClient();
            UserNamePasswordClientCredential un = DBEvent.ClientCredentials.UserName;
            if (un != null)
            {
                un.UserName = "Gustav";
                un.Password = "gsMberg1";
            }

///System.ServiceModel.Security.MessageSecurityException:
///  Ein nicht gesicherter oder fehlerhaft gesicherter Fehler wurde vom anderen Teilnehmer empfangen. 
///  Den Fehlercode und Details finden Sie unter der inneren FaultException.
///InnerException: System.ServiceModel.FaultException
///  Mindestens ein Sicherheitstoken in der Nachricht konnte nicht überprüft werden.

        }

        TWorkspaceInfo WorkspaceInfo
        {
            get
            {
                return TMain.WorkspaceInfo;
            }
        }

        private string SwapLineFeed(string s)
        {
            if (s.Length > 0)
            {
                s = s.Replace("\r\n", "\n"); //original Windows
                s = s.Replace("\r", "\n"); //original Mac
                s = s.Replace("\n\r", "\n"); //invalid
                s = s.Replace("\n", Environment.NewLine);
            }
            return s;
        }

        #region IDBEvent2 Member

        public string Load(int KatID, string EventName)
        {
            try
            {
                string s = DBEvent.DBLoadFromFile(WorkspaceInfo.WorkspaceID, ItemKey(EventName));
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
                //not implemented
                //DBEvent.DBSaveToFile(WorkspaceInfo.WorkspaceID, ItemKey(EventName), Data);
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
                string EventFilter = ".txt";
                string[] result = DBEvent.DBGetEventNames(WorkspaceInfo.WorkspaceID, EventFilter);
                TStrings SL = new TStringList();
                foreach (string s in result)
                {
                    SL.Add(s);
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
            DBEvent.Close();
        }

        private string ItemKey(string fn)
        {
            return @"\DBEvent\" + fn + ".txt";
        }

        #endregion

    }

}
