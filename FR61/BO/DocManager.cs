using System.Text.RegularExpressions;

namespace RiggVar.FR
{

    public interface IDBEvent
    {
        string Load(int KatID, string EventName);
        void Save(int KatID, string EventName, string Data);
        void Delete(int KatID, string EventName);
        string GetEventNames(int KatID);
        void Close();
    }

    /// <summary>
    /// TMain.Controller has a DocManager
    /// Contains a DB reference of type IDBEvent2.
    /// Caches the EventName and Type.
    /// The controller class behind menuitems Open, Save, SaveAs, New, Download...
    /// Uses two Forms: FormSelectName and FormDocManager
    /// </summary>
    public class TDocManager
    {
        public MemoLogger Logger;

        private IDBEvent DB;
        private TStringList SLDocsAvail;
        public bool SaveEnabled;
        public bool SaveAsEnabled;
        public bool DeleteEnabled;

        public TDocManager(string CompData)
        {
            Logger = new MemoLogger();
            SLDocsAvail = new TStringList();
            EventName = TIniImage.DefaultEventName;
            EventType = TIniImage.DefaultEventType;
            InitDBInterface(CompData);
        }

        public string EventName { get; set; }
        public int EventType { get; set; }

        public string DBInterface { get; private set; }

        public void InitDBInterface(string CompData)
        {
            SaveEnabled = false;
            SaveAsEnabled = false;
            DeleteEnabled = false;
            if (CompData == "MDB")
            {
                DB = DBEventFactory.GetDBEvent("MDB");
                DBInterface = "MDB";
                SaveEnabled = true;
                SaveAsEnabled = true;
                DeleteEnabled = true;
            }
            else if (CompData == "WEB")
            {
                DB = DBEventFactory.GetDBEvent("WEB");
                DBInterface = "WEB";
            }
            else if (CompData == "REST")
            {
                DB = DBEventFactory.GetDBEvent("REST");
                DBInterface = "REST";
                SaveEnabled = true;
            }
            else
            {
                DB = DBEventFactory.GetDBEvent("TXT");
                DBInterface = "TXT";
                SaveEnabled = true;
                SaveAsEnabled = true;
                DeleteEnabled = true;
            }
            TMain.IniImage.DBInterface = DBInterface;
        }

        private bool IsValidIdent(string s)
        {
            string ValidIdentifierRegex = @"[A-Z][a-zA-Z0-9]*";
            return Regex.IsMatch(s, ValidIdentifierRegex);
        }

        protected string MakeValidName(string NewName)
        {
            string result = NewName;

            if (NewName == null || NewName == "")
            {
                result = "Event1";
            }
            else if ((result != "") && (! IsValidIdent(result)))
            {
                //first char must be AlphaChar, erstes Zeichen mu� ein Buchstabe sein        
                if (Regex.IsMatch(result, "0-9"))
                {
                    result = "_" + result;
                }

                //replace some characters, einige Zeichen ersezten
                result = Regex.Replace(result, " ", "_");
                result = Regex.Replace(result, "�", "ue", RegexOptions.IgnoreCase);
                result = Regex.Replace(result, "�", "ae", RegexOptions.IgnoreCase);
                result = Regex.Replace(result, "�", "oe", RegexOptions.IgnoreCase);
                result = Regex.Replace(result, "�", "sz");

                if (!IsValidIdent(result))
                {
                    //remove other invalid characters, andere ung�ltige Zeichen entfernen
                    string s = "";
                    for (int i = 1; i < result.Length; i++)
                    {
                        if (IsValidIdent(s + result[i]))
                        {
                            s = s + result[i];
                        }
                    }

                    result = s;
                }
            }

            return result;
        }

        public void EditDBEvent()
        {
            if (TMain.Controller.ChooseDB())
            {
                if (DBInterface != TMain.IniImage.DBInterface)
                {
                    InitDBInterface(TMain.IniImage.DBInterface);
                }
            }
        }

        public void FillEventNameList(TStrings SL)
        {
            SL.Text = DB.GetEventNames(EventType);
        }

        public bool DocNew(TBO BO)
        {            
            SLDocsAvail.Text = DB.GetEventNames(EventType);
            string s = TMain.Controller.GetNewDocName();
            if (SLDocsAvail.IndexOf(s) != -1)
            {
                PlatformDiff.ShowMsgBox("NewDocName must have unique name");
                return false;
            }
            else if (s != "")
            {
                EventName = s;
                BO.Clear();
            }
            return true;
        }

        public string DocDownload()
        {
            SLDocsAvail.Text = DB.GetEventNames(EventType);
            string en = TMain.Controller.ChooseDocAvail(SLDocsAvail);
            return DocDownloadByName(en);
        }

        public string DocDownloadByName(string en)
        {
            Logger.AppendLine("DocManager.DocDownloadByName()");
            if (en != "")
            {
                EventName = en;
                return DB.Load(EventType, en);
                //BO.LoadNew(); //this is now done by caller
                // + further care must be taken (recreating views) by caller
            }
            return "";
        }

        public void DocOpen(TBO BO)
        {
            Logger.AppendLine("DocManager.DocOpen()");
            string s = DocDownload();
            if (s != "")
            {
                BO.LoadNew(DB.Load(EventType, EventName)); //this still done here
                // + further care must be taken (recreating views) by caller
            }
        }

        public void DocSave(TBO BO)
        {
            DB.Save(EventType, EventName, BO.Save());
        }

        public void DocSaveAs(TBO BO)
        {
            string s = TMain.Controller.ChooseNewEventName();
            if (s != "")
            {
                EventName = s;
                DocSave(BO);
            }
        }

        public void DocDelete()
        {
            SLDocsAvail.Text = DB.GetEventNames(EventType);
            //Get Name of Document to be deleted
            string s = TMain.Controller.ChooseDocAvail(SLDocsAvail);
            if (s != "")
            {
                DB.Delete(EventType, s);
            }
        }
        
        public void RawDelete(string en)
        {
            DB.Delete(EventType, en);
        }

    }

}
