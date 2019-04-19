using System.IO;

namespace RiggVar.FR
{
    public interface IWSDB
      {
        bool DBFileExists(string fn);
        bool DBDirectoryExists(string dn);
        bool DBCreateDir(string dn);
        bool DBDeleteFile(string fn);
        void DBLoadFromFile(string fn, TStrings SL);
        void DBSaveToFile(string fn, TStrings SL);
      }

    public class TRedirector : IWSDB
    {
        public TRedirector()
        {
        }

        public bool UseDB => TMain.UseDB;


        private IDBWorkspace DBWorkspace => TMain.WorkspaceManager.DBWorkspace;

        public bool DBDirectoryExists(string dn)
        {
            return UseDB ? true : Directory.Exists(dn);
        }

        public bool DBFileExists(string fn)
        {
            return UseDB ? DBWorkspace.DBFileExists(fn) : File.Exists(fn);
        }

        public bool DBCreateDir(string dn)
        {
            if (UseDB)
            {
                return true;
            }
            else
            {
                return PlatformDiff.CreateDirectory(dn);
            }
        }

        public bool DBDeleteFile(string fn)
        {
            if (UseDB)
            {
                return DBWorkspace.DBDeleteFile(fn);
            }
            else
            {
                File.Delete(fn);
                return true;
            }
        }

        public void DBSaveToFile(string fn, TStrings SL)
        {
            if (UseDB)
            {
                DBWorkspace.DBSaveToFile(fn, SL);
            }
            else
            {
                SL.SaveToFile(fn);
            }
        }

        public void DBLoadFromFile(string fn, TStrings SL)
        {
            if (UseDB)
            {
                DBWorkspace.DBLoadFromFile(fn, SL);
            }
            else
            {
                SL.LoadFromFile(fn);
            }
        }

    }

    public class TDBStringList : TStringList
    {
        private TRedirector Redirector => TMain.Redirector;

        public override void LoadFromFile(string AFileName)
        {
            if (Redirector.UseDB)
            {
                Redirector.DBLoadFromFile(AFileName, this);
            }
            else
            {
                base.LoadFromFile(AFileName);
            }
        }

        public override void SaveToFile(string AFileName)
        {
            if (Redirector.UseDB)
            {
                Redirector.DBSaveToFile(AFileName, this);
            }
            else
            {
                base.SaveToFile(AFileName);
            }
        }
    }

    public class TDBStringListCreator : TDBStringListFactory
    {

        public override TStringList CreateInstance()
        {
            return new TDBStringList();
        }

    }

}
