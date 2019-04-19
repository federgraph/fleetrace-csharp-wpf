using System;
using System.Diagnostics;
using System.IO;
using RiggVar.FR;

namespace RiggVar.DM
{

    public class TDBEventTXT : IDBEvent
    {
        private TStringList SL;
        private string GetPrefix(int KatID)
        {
            switch (KatID)
            {
                case LookupKatID.FR: return "FR_";
                case LookupKatID.SKK: return "SKK_";
                case LookupKatID.SBPGS: return "PGS_";
                case LookupKatID.Adapter: return "Adapter_";
                default: return Utils.IntToStr(KatID) + "_";
            }
        }
        private string GetSuffix(int KatID)
        {
            switch (KatID)
            {
                case LookupKatID.FR: return "_FRData";
                case LookupKatID.SKK: return "";
                case LookupKatID.SBPGS: return "";
                case LookupKatID.Adapter: return "";
                default: return "";
            }
        }
        private string GetFileName(int KatID, string EventName)
        {
            if (TMain.UseDB)
            {
                return Dir + EventName + ".txt";
            }
            else
            {
                return Dir + GetPrefix(KatID) + EventName + GetSuffix(KatID) + ".txt";
            }
        }
        private string Dir
        {
            get { return TMain.FolderInfo.DataPath; }
        }
        public TDBEventTXT()
        {
            SL = new TDBStringList();
        }

        #region IDBEvent2 Member

        public string Load(int KatID, string EventName)
        {
            string fn = GetFileName(KatID, EventName);
            if (TMain.Redirector.DBFileExists(fn))
            {
                SL.LoadFromFile(fn);
                return SL.Text;
            }
            return "";
        }

        public void Save(int KatID, string EventName, string Data)
        {
            string fn = GetFileName(KatID, EventName);
            SL.Text = Data;
            SL.SaveToFile(fn);
        }

        public void Delete(int KatID, string EventName)
        {
            string fn = GetFileName(KatID, EventName);
            if (TMain.Redirector.DBFileExists(fn))
            {
                TMain.Redirector.DBDeleteFile(fn);
            }
        }

        public string GetEventNames(int KatID)
        {
            try
            {
                if (TMain.UseDB)
                {
                    return GetEventNamesDB(KatID); //DataBase
                }
                else
                {
                    return GetEventNamesFS(KatID); //FileSystem
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "";
            }
        }

        private string GetEventNamesDB(int KatID)
        {
            return TMain.WorkspaceManager.DBWorkspace.DBGetEventNames(".txt");
        }

        private string GetEventNamesFS(int KatID)
        {
            SL.Clear();
            string prefix = GetPrefix(KatID);
            string suffix = GetSuffix(KatID);
            int prefixLength = prefix.Length;
            int suffixLength = suffix.Length;
            DirectoryInfo di = new DirectoryInfo(Dir);
            string s;
            foreach (FileInfo fi in di.GetFiles(prefix + "*" + suffix + ".txt"))
            {
                s = Path.GetFileNameWithoutExtension(fi.Name);
                s = s.Substring(prefixLength, s.Length - prefixLength - suffixLength);
                SL.Add(s);
            }
            return SL.Text;
        }

        public void Close()
        {
            //nothing to be done here
        }

        #endregion
    }

}
