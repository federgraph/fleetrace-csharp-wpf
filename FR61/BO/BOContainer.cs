using System;
using System.Diagnostics;

namespace RiggVar.FR
{
    /// <summary>
    /// Use this class together with TBOContainer.
    /// The overridden Clear() method will clear out athlete data as well, which is not the default.
    /// The overridden LoadNew(string Data) method will recreate the BO via BOContainer.LoadNew(Data),
    /// inherited LoadNew() just calls Load(), which does not disconnect and delete the old BO.
    /// </summary>
    public class TSDIBO : TBO
    {
        public TMain Main { get { return TMain.Controller; } }

        public TSDIBO(TAdapterParams aParams) : base(aParams)
        {
            BackupDir = TMain.FolderInfo.BackupPath;
//            string s2 = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
//            string s3 = Path.GetDirectoryName(Application.ExecutablePath);
//            BackupDir = Path.Combine(s3, s2);            
        }
        /// <summary>
        /// Disconnects and recreates BO before loading data,
        /// via a call to BOContainer.LoadNew(Data).
        /// </summary>
        /// <param name="Data"></param>
        public override void LoadNew(string Data)
        {
            Main.LoadNew(Data);
        }
        /// <summary>
        /// also clears out Athlete data
        /// </summary>
        public override void Clear()
        {
            this.StammdatenNode.Collection.Clear();
            base.Clear();
        }
    }

    /// <summary>
    /// This BOContainer can host exactly one BO. 
    /// Recreation of BO cannot be done from within a method of TBO.
    /// The BO can be recreated
    /// <list type="">
    /// <item>with new params and old data</item>
    /// <item>from known backup file</item> 
    /// <item>or from data provided.</item>
    /// </list>
    /// The class will begin life with a valid BO containing built-in test-data.
    /// </summary>
    public class TBOContainer : TBOManager, IBORecreator
    {
        private TStringList SL;

        public TBOContainer()
        {
            SL = new TStringList();
            GetDefaultData();
        }

        /// <summary>
        /// Save eventdata according to configuration settings and delete BO.
        /// </summary>
        public void Destroy()
        {
            try
            {
                if (TMain.IniImage.AutoSave)
                {
                    TMain.DocManager.DocSave(BO);
                }
                else if (TMain.IniImage.NoAutoSave)
                {
                    //do nothing
                }
                else
                {
                    string s = "closing Event...";
                    if (PlatformDiff.ShowMsgBoxYesNo("Save changes?", s))
                    {
                        TMain.DocManager.DocSave(BO);
                    }
                }
                DeleteBO();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in BOContainer.Destroy");
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Init the normal BO instance.
        /// This is not done in the constructor, from TMain, after creation of AdapterBO,
        /// and after pushing the internal server factory on the stack.
        /// </summary>
        public void InitBO()
        {
            CreateNew(SL); //use params in SL
            BO.Load(SL.Text); //load data, ignore params
        }

        /// <summary>
        /// Creates new BO using params in ml, but does not load the data in ml.
        /// </summary>
        /// <param name="ml">message list, text data for the event</param>
        public void CreateNew(TStrings ml)
        {
            DeleteBO();
            TBOParams BOParams = new TBOParams();
            string s, n, v;
            int paramsRead = 0;
            for (int i = 0; i < ml.Count; i++)
            {
                s = ml[i];
                n = ml.Names(i).Trim();
                v = ml.ValueFromIndex(i).Trim();
                if (n == "DP.StartlistCount" || n == "Event.StartlistCount")
                {
                    BOParams.StartlistCount = Utils.StrToIntDef(v, BOParams.StartlistCount);
                    paramsRead++;
                }
                else if (n == "DP.ITCount" || n == "Event.ITCount")
                {
                    BOParams.ITCount = Utils.StrToIntDef(v, BOParams.ITCount);
                    paramsRead++;
                }
                else if (n == "DP.RaceCount" || n == "Event.RaceCount")
                {
                    BOParams.RaceCount = Utils.StrToIntDef(v, BOParams.RaceCount);
                    paramsRead++;
                }
//                else if (n == "EP.DivisionName" || n == "Event.Prop_DivisionName")
//                {
//                    BOParams.DivisionName = v;
//                    paramsRead++;
//                }
                if (paramsRead == 3)
                {
                    break;
                }
            }
            CreateBO(BOParams); //use params in ml
            //do not laod data in ml
        }

        /// <summary>
        /// Create new BO using params in Data, then load data in Data. 
        /// </summary>
        /// <param name="Data">complete text-data for new event</param>
        public void LoadNew(string Data)
        {
            TStrings ml = new TStringList();  
            try
            {
                ml.Text = Data;
            }
            catch
            {
                ml.Text = "";
            }
            CreateNew(ml); //use params in ml
            BO.Load(ml.Text); //load data, ignore params
        }
        /// <summary>
        /// create new BO with new params and old data.
        /// May loose data if new params limit space.
        /// </summary>
        /// <param name="BOParams">new params for event</param>
        public void RecreateBO(TAdapterParams BOParams)
        {
            TStrings ml = new TStringList();
            try
            {
                ml.Text = BO.Save();
            }
            catch
            {
                ml.Text = "";
            }
            TBOParams tempParams = new TBOParams();
            tempParams.Assign(BOParams);
            DeleteBO();
            CreateBO(tempParams); //use new params
            BO.Load(ml.Text); //load old data
        }
        /// <summary>
        /// Create new BO from Backup, params in the backup are evaluated. This is a full restore.
        /// </summary>
        public void RecreateBOFromBackup()
        {  
            string fn = BO.BackupDir + "_Backup.txt";
            if (! TMain.Redirector.DBFileExists(fn))
            {
                return;
            }

            TStringList ml = new TDBStringList();
            ml.LoadFromFile(fn);
            CreateNew(ml); //use params in backup
            BO.Load(ml.Text); //load data in backup
            BO.Calc();
            BO.UndoManager.Clear();
        }

        /// <summary>
        /// Return testdata as string.
        /// </summary>
        /// <returns>a copy of builtin test-data (SL.Text)</returns>
        public string GetTestData()
        {
            return SL.Text;
        }

        /// <summary>
        /// Add testdata to SL, does not clear SL.
        /// </summary>
        public void GetDefaultData()
        {
            //#Params

            SL.Add("DP.StartlistCount = 8");
            SL.Add("DP.ITCount = 0");
            SL.Add("DP.RaceCount = 2");

            //#EventProps

            SL.Add("EP.Name = Test Event");
            //SL.Add("EP.Dates = Event Dates");
            //SL.Add("EP.HostClub = Club Y");
            //SL.Add("EP.PRO =");
            //SL.Add("EP.JuryHead =");
            SL.Add("EP.ScoringSystem = Low Point System");
            SL.Add("EP.Throwouts = 0");
            //SL.Add("EP.ThrowoutScheme = ByNumRaces");
            SL.Add("EP.DivisionName = *");
            SL.Add("EP.InputMode = Strict");
            SL.Add("EP.RaceLayout = Finish");
            SL.Add("EP.NameSchema = ");
            //SL.Add("EP.Uniqua.Enabled = False");
            //SL.Add("EP.Uniqua.Gesegelt = 2");
            //SL.Add("EP.Uniqua.Gemeldet = 8");
            //SL.Add("EP.Uniqua.Gezeitet = 8");

            //#Athletes


            //#Startlist

            SL.Add("FR.*.W1.STL.Pos1.SNR=1001");
            SL.Add("FR.*.W1.STL.Pos2.SNR=1002");
            SL.Add("FR.*.W1.STL.Pos3.SNR=1003");
            SL.Add("FR.*.W1.STL.Pos4.SNR=1004");
            SL.Add("FR.*.W1.STL.Pos5.SNR=1005");
            SL.Add("FR.*.W1.STL.Pos6.SNR=1006");
            SL.Add("FR.*.W1.STL.Pos7.SNR=1007");
            SL.Add("FR.*.W1.STL.Pos8.SNR=1008");

            //#W1

            SL.Add("FR.*.W1.Bib1.Rank=2");
            SL.Add("FR.*.W1.Bib2.Rank=7");
            SL.Add("FR.*.W1.Bib3.Rank=5");
            SL.Add("FR.*.W1.Bib4.Rank=1");
            SL.Add("FR.*.W1.Bib5.Rank=6");
            SL.Add("FR.*.W1.Bib6.Rank=8");
            SL.Add("FR.*.W1.Bib7.Rank=4");
            SL.Add("FR.*.W1.Bib8.Rank=3");

            //#W2

            SL.Add("FR.*.W2.Bib1.Rank=3");
            SL.Add("FR.*.W2.Bib2.Rank=4");
            SL.Add("FR.*.W2.Bib3.Rank=8");
            SL.Add("FR.*.W2.Bib4.Rank=7");
            SL.Add("FR.*.W2.Bib5.Rank=5");
            SL.Add("FR.*.W2.Bib6.Rank=6");
            SL.Add("FR.*.W2.Bib7.Rank=2");
            SL.Add("FR.*.W2.Bib8.Rank=1");

            SL.Add("EP.IM = Strict");

        }

    }

}
