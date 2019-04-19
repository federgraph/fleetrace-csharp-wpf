using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FR60.RemoteWorkspace;
using System.ServiceModel.Security;

namespace RiggVar.FR
{
    class TRemoteWorkspace : IDBWorkspace, IDisposable
    {
        RemoteWorkspaceClient wf;
        bool closed;

        private MemoLogger Logger => TMain.DocManager.Logger;

        private RemoteWorkspaceClient WF
        {
            get
            {
                if (wf == null || closed == true)
                {
                    //see Dispose() method
                    wf = new RemoteWorkspaceClient();
                    UserNamePasswordClientCredential un = wf.ClientCredentials.UserName;
                    if (un != null)
                    {
                        un.UserName = "User";
                        un.Password = "Password";
                    }
                }
                return wf;
            }
        }

        public string CheckSLString(string[] stringArray)
        {
            TStringList SL = new TStringList();
            foreach (string t in stringArray)
            {
                if (t.Equals(string.Empty))
                {
                    continue;
                }

                SL.Add(t);
            }
            return SL.Text;
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

        private void ShowError(Exception ex, string mn)
        {
            Logger.AppendLine("RemoteWorkspace.ShowError(), logging exception...");
            Logger.AppendLine(string.Format("Exception was caught in method {0}...", mn));

            if (TMain.IsWinGUI)
            {
                //todo: filter by exception type for login error
                //if (ex.ClassName == "EDOMParseError")
                if (mn == "DBGetEventNames")
                {
                    TMain.FormAdapter.ShowError("Login problem at RemoteWorkspace WCF Service?");
                    Logger.AppendLine("Looks like login problem at RemoteWorkspace WCF Service...");
                }
            }

            Logger.AppendLine(ex.Message);
            Logger.AppendEmptyLine();
        }


        #region IDBWorkspace Members

        public void SetWorkspaceID(int WorkspaceID)
        {
            this.WorkspaceID = WorkspaceID;
        }

        public int GetWorkspaceID()
        {
            return WorkspaceID;
        }

        public int WorkspaceID { get; set; } = 1;
        public bool DBFileExists(string fn)
        {
            return WF.DBFileExists(WorkspaceID, fn);
        }

        public bool DBDirectoryExists(string dn)
        {
            return true;
            //return WF.DBDirectoryExists(WorkspaceID, dn);
        }

        public string DBGetEventNames(string ExtensionFilter)
        {
            try
            {
                string[] result = WF.DBGetEventNames(WorkspaceID, ExtensionFilter);
                return CheckSLString(result);
            }
            catch (Exception ex)
            {
                ShowError(ex, "DBGetEventNames");
                return "";
            }
        }

        public void DBLoadFromFile(string fn, TStrings SL)
        {
            string result = WF.DBLoadFromFile(WorkspaceID, fn);
            result = SwapLineFeed(result);
            SL.Text = result;
        }

        public void DBSaveToFile(string fn, TStrings SL)
        {
            WF.DBSaveToFile(WorkspaceID, fn, SL.Text);
        }

        public bool DBDeleteFile(string fn)
        {
            return WF.DBDeleteFile(WorkspaceID, fn);
        }

        public bool DBDeleteWorkspace()
        {
            try
            {
                return WF.DBDeleteWorkspace(WorkspaceID);
            }
            catch
            {
                return false;
            }
        }

        public void ExportFile(string WorkspaceDir)
        {
            throw new NotImplementedException();
        }

        public void ExportFiles(string WorkspaceDir)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            if (wf != null)
            {
                wf.Close();
                closed = true;
            }
        }

        #endregion
    }

}
