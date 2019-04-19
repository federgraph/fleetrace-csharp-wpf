using System.IO;

namespace RiggVar.FR
{
    public struct TWorkspaceFile
    {
        public int WorkspaceID;
        public string Key;
        public string Value;

        public string Path;
        public string Name;
        public string Ext;

        public void Prepare()
        {
            //TODO: test out TWorkspaceFile.Prepare()
            FileInfo fi = new FileInfo(Key);
            
            Ext = fi.Extension;
            Path = fi.DirectoryName;            
            Name = fi.Name;
            Name = Name.Remove(Name.LastIndexOf('.'));
            //Ext := ExtractFileExt(Key);
            //Path := ExtractFilePath(Key);
            //Name := ExtractFileName(Key);
            //Name := ChangeFileExt(Name, "");
        }

        public void Clear()
        {
            WorkspaceID = 1;
            Key = "Trace\\Test.txt";
            Value = "abc";
            Prepare();
        }
    }

    public interface IDBWorkspace
    {
        void SetWorkspaceID(int WorkspaceID);
        int GetWorkspaceID();

        bool DBFileExists(string fn);
        bool DBDirectoryExists(string dn);
        string DBGetEventNames(string ExtensionFilter);
        void DBLoadFromFile(string fn, TStrings SL);
        void DBSaveToFile(string fn, TStrings SL);
        bool DBDeleteFile(string fn);
        bool DBDeleteWorkspace();

        //TDataSet GetDataSet();
        void ExportFile(string WorkspaceDir);
        void ExportFiles(string WorkspaceDir);
    }
}
