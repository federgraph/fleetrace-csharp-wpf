using System;

using FR60.WorkspaceFiles;

namespace RiggVar.FR
{
    class TWorkspaceWeb : IDBWorkspace
    {
        private WorkspaceFiles wf;

        private WorkspaceFiles WF
        {
            get
            { 
                if (wf == null)
                {
                    wf = new WorkspaceFiles();
                    wf.Url = TMain.WorkspaceInfo.WorkspaceUrl;
                }
                return wf;
            }
        }
        
        public string CheckSLString(string s)
        {
            TStringList SL = new TStringList();
            string[] stringArray = s.Split('\n');
            foreach(string t in stringArray)
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
                s = s.Replace("\r\n", "\n");
                s = s.Replace("\n", "\r\n");
            }
            return s;
        }

        private void ShowError(Exception ex, string mn)
        {
            if (TMain.IsWinGUI)
            {
                //todo: filter by exception type for login error
                //if (ex.ClassName == "EDOMParseError")
                if (mn == "DBGetEventNames")
                {
                    TMain.FormAdapter.ShowError("please login to the web-application first...");
                }
            }

            TMain.Logger.Log(ex.Message + " when calling TWorkspaceWeb." + mn);
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
            string result = WF.DBGetEventNames(WorkspaceID, ExtensionFilter);
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
    }
}
