using System;

namespace RiggVar.FR
{
    class TWorkspaceFiles : IDBWorkspace     
    {
        #region IDBWorkspace Members

        public void SetWorkspaceID(int WorkspaceID)
        {
            //throw new NotImplementedException();
        }

        public int GetWorkspaceID()
        {
            //throw new NotImplementedException();
            return 1;
        }

        public bool DBFileExists(string fn)
        {
            //throw new NotImplementedException();
            return false;
        }

        public bool DBDirectoryExists(string dn)
        {
            //throw new NotImplementedException();
            return false;
        }

        public string DBGetEventNames(string ExtensionFilter)
        {
            //throw new NotImplementedException();
            return "";
        }

        public void DBLoadFromFile(string fn, TStrings SL)
        {
            //throw new NotImplementedException();
        }

        public void DBSaveToFile(string fn, TStrings SL)
        {
            //throw new NotImplementedException();
        }

        public bool DBDeleteFile(string fn)
        {
            //throw new NotImplementedException();
            return false;
        }

        public bool DBDeleteWorkspace()
        {
            //throw new NotImplementedException();
            return false;
        }

        public void ExportFile(string WorkspaceDir)
        {
            //throw new NotImplementedException();
        }

        public void ExportFiles(string WorkspaceDir)
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
}
