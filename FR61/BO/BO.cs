#define Undo
#define StatusFeedback
#define Desktop

using System;

namespace RiggVar.FR
{
    public enum BOIndexer
    {
        SNR,
        Bib,
        QU,
        DG,
        OT,
        Penalty
    }

    public class CurrentNumbers
    {

        public int race = 0;
        public int tp = 0;
        public int bib = 0;
        public int withTime = 0;
        public int withPenalty = 0;
        public int withTimeOrPenalty = 0;

        public CurrentNumbers()
        {
            Clear();
        }

        public void Clear()
        {
            race = 0;
            tp = 0;
            bib = 0;
            withPenalty = 0;
            withTime = 0;
            withTimeOrPenalty = 0;
        }
    }

    public class TBO : TBaseBO
    {
        public bool UseInputFilter;
        public bool UseOutputFilter;
        public bool UseCompactFormat = true;

        public string ConvertedData;

        private bool FModified;
        private TNodeList FNodeList;
        internal TStrings FSLBackup;
        public int CounterCalc;
        public int MsgCounter;

        public TStammdatenBO StammdatenBO;
        public TStammdatenNode StammdatenNode;

        public TEventBO EventBO;
        public TEventNode EventNode;

        public TRaceBO RaceBO;
        public TRaceNode[] RNode;

        public TCalcEvent CalcEV;
        public TCalcTP CalcTP;

        public TBOParams BOParams;

        public TJavaScoreXML JavaScoreXML;
        public TRaceDataXML RaceDataXML;
        public TEventProps EventProps;
        public TExcelImporter ExcelImporter;
        public TUndoAgent UndoAgent;
        public TMsgTree MsgTree;

#if StatusFeedback
        public TStatusFeedback StatusFeedback;
#endif

#if Undo
        public TUndoManager UndoManager;
        public TConnection UndoConnection;
#endif

        public TBO(TAdapterParams aParams) : base(aParams)
        {
            TMain.BO = this;

            TColCaptions.InitDefaultColCaptions();

            cTokenA = "FR";

            if (AdapterParams is TBOParams)
            {
                BOParams = AdapterParams as TBOParams;
            }
            else
            {
                BOParams = new TBOParams();
            }

            DivisionName = BOParams.DivisionName;

#if Desktop
            CalcEV = new TCalcEvent(TMain.IniImage.ScoringProvider);
#else
            CalcEV = new TCalcEvent(TCalcEvent.ScoringProvider_Inline);
#endif

            CalcTP = new TCalcTP(this);

            FNodeList = new TNodeList();

            UndoAgent = new TUndoAgent();
            MsgTree = new TMsgTree(cTokenA, 0);

            //Stammdaten
            StammdatenBO = new TStammdatenBO();
            StammdatenNode = new TStammdatenNode
            {
                ColBO = StammdatenBO,
                NameID = "Stammdaten"
            };
            StammdatenBO.CurrentNode = StammdatenNode;

            //EventNode
            EventBO = new TEventBO();
            EventNode = new TEventNode
            {
                ColBO = EventBO,
                NameID = "E",
                StammdatenRowCollection = StammdatenNode.Collection
            };
            EventBO.CurrentNode = EventNode;
            FNodeList.Add(EventNode);

            //Race
            RaceBO = new TRaceBO();
            RNode = new TRaceNode[BOParams.RaceCount + 1];
            for (int i = 0; i <= BOParams.RaceCount; i++)
            {
                TRaceNode ru = new TRaceNode
                {
                    ColBO = RaceBO,
                    NameID = "W" + i.ToString(),
                    BOParams = BOParams,
                    StammdatenRowCollection = StammdatenNode.Collection,
                    Index = i,
                    Layout = 0
                };
                FNodeList.Add(ru);
                ru.OnCalc = new TNotifyEvent(this.SetModified);
                RNode[i] = ru;
            }
            RaceBO.CurrentNode = RNode[1];

            InitStartlistCount(BOParams.StartlistCount);

            Output = new TOutput();

            TServerIntern ts;
            try
            {
                ts = new TServerIntern(3027, TServerFunction.Input);
                InputServer = new TInputNCP(ts);
                ts = new TServerIntern(3028, TServerFunction.Output);
                OutputServer = new TOutputNCP(ts);
            }
            catch
            {
                InputServer = null;
                OutputServer = null;
            }

            JavaScoreXML = new TJavaScoreXML(this);
            RaceDataXML = new TRaceDataXML(this);
            EventProps = new TEventProps(this);

#if StatusFeedback
            StatusFeedback = new TStatusFeedback(this);
#endif

#if Undo
            UndoManager = new TUndoManager(this);
            UndoConnection = InputServer.Server.Connect("Undo.Input");
#endif

#if Desktop
            Watches = new TLocalWatches();
#endif
            ExcelImporter = new TExcelImporter();
        }
        public string GetHashString()
        {
            return EventNode.Collection.GetHashString();
        }
        public void ClearCommand()
        {
            ClearBtnClick();
#if Undo
            UndoManager.Clear();
#endif
        }
        public int GetSNR(int Index)
        {
            TRaceRowCollectionItem cr = RNode[0].Collection[Index];
            if (cr != null)
            {
                return cr.SNR;
            }
            else
            {
                return -1;
            }
        }
        public int GetBib(int Index)
        {
            TRaceRowCollectionItem cr = RNode[0].Collection[Index];
            return cr != null ? cr.Bib : -1;
        }
        public int GetQU(int RaceIndex, int Index)
        {
            TEventRowCollectionItem cr = EventNode.Collection[Index];
            return cr != null ? cr.Race[RaceIndex].QU : 0;
        }
        public int GetDG(int RaceIndex, int Index)
        {
            TEventRowCollectionItem cr = EventNode.Collection[Index];
            return cr != null ? cr.Race[RaceIndex].DG : 0;
        }
        public int GetOT(int RaceIndex, int Index)
        {
            TEventRowCollectionItem cr = EventNode.Collection[Index];
            return cr != null ? cr.Race[RaceIndex].OTime : 0;
        }
        public void SetSNR(int Index, int Value)
        {
            TEventRowCollectionItem cr = EventNode.Collection[Index];
            if (cr != null)
            {
                cr.SNR = Value;
            }

            for (int i = 0; i <= BOParams.RaceCount; i++)
            {
                TRaceRowCollectionItem wr = RNode[i].Collection[Index];
                if (wr != null)
                {
                    wr.SNR = Value;
                }
            }
        }
        public void SetBib(int Index, int Value)
        {
            TEventRowCollectionItem cr = EventNode.Collection[Index];
            if (cr != null)
            {
                cr.Bib = Value;
            }

            for (int i = 0; i <= BOParams.RaceCount; i++)
            {
                TRaceRowCollectionItem wr = RNode[i].Collection[Index];
                if (wr != null)
                {
                    wr.Bib = Value;
                }
            }
        }
        public void SetQU(int RaceIndex, int Index, int Value)
        {
            TEventRowCollectionItem cr = EventNode.Collection[Index];
            if (cr != null)
            {
                cr.Race[RaceIndex].QU = Value;
                cr.Modified = true;
            }
            TRaceRowCollectionItem wr = RNode[RaceIndex].Collection[Index];
            if (wr != null)
            {
                wr.QU.AsInteger = Value;
                wr.Modified = true;
            }
        }
        public void SetDG(int RaceIndex, int Index, int Value)
        {
            TEventRowCollectionItem cr = EventNode.Collection[Index];
            if (cr != null)
            {
                cr.Race[RaceIndex].DG = Value;
                cr.Modified = true;
            }
            TRaceRowCollectionItem wr = RNode[RaceIndex].Collection[Index];
            if (wr != null)
            {
                wr.DG = Value;
                wr.Modified = true;
            }
        }
        public void SetOT(int RaceIndex, int Index, int Value)
        {
            TEventRowCollectionItem cr = EventNode.Collection[Index];
            if (cr != null)
            {
                cr.Race[RaceIndex].OTime = Value;
                cr.Modified = true;
            }
            TRaceRowCollectionItem wr = RNode[RaceIndex].Collection[Index];
            if (wr != null)
            {
                wr.MRank = Value;
                wr.Modified = true;
            }
        }
        public int this[BOIndexer f, int RaceIndex, int Index]
        {
            get
            {
                switch (f)
                {
                    case BOIndexer.SNR: return GetSNR(Index);
                    case BOIndexer.Bib: return GetBib(Index);
                    case BOIndexer.QU: return GetQU(RaceIndex, Index);
                    case BOIndexer.DG: return GetDG(RaceIndex, Index);
                    case BOIndexer.OT: return GetOT(RaceIndex, Index);
                    default: return 0;
                }
            }
            set
            {
                switch (f)
                {
                    case BOIndexer.SNR: SetSNR(Index, value); break;
                    case BOIndexer.Bib: SetBib(Index, value); break;
                    case BOIndexer.QU: SetQU(RaceIndex, Index, value); break;
                    case BOIndexer.DG: SetDG(RaceIndex, Index, value); break;
                    case BOIndexer.OT: SetOT(RaceIndex, Index, value); break;
                }
            }
        }
        public TPenalty GetPenalty(int RaceIndex, int Index)
        {
            TEventRowCollectionItem cr = EventNode.Collection[Index];
            return cr?.Race[RaceIndex].Penalty;
        }
        public void SetPenalty(int RaceIndex, int Index, TPenalty Value)
        {
            TEventRowCollectionItem cr = EventNode.Collection[Index];
            if (cr != null)
            {
                cr.Race[RaceIndex].Penalty.Assign(Value);
                cr.Modified = true;
            }
            TRaceRowCollectionItem wr = RNode[RaceIndex].Collection[Index];
            if (wr != null)
            {
                wr.QU.Assign(Value);
                wr.Modified = true;
            }
        }
        //Penalty Indexer
        public TPenalty this[int RaceIndex, int Index]
        {
            get => GetPenalty(RaceIndex, Index);
            set => SetPenalty(RaceIndex, Index, value);
        }
        public int Gemeldet => this.EventNode.Collection.Count;
        public int Gesegelt => BOParams.RaceCount;

        public int Gezeitet { get; set; }

        private void SaveLine(object sender, string s)
        {
            FSLBackup.Add(s);
        }

        private void ClearList(string rd)
        {
            FNodeList.ClearList(rd);
        }

        private void ClearResult(string rd)
        {
            FNodeList.ClearResult(rd);
        }

        private void SetModified(object sender)
        {
            FModified = true;
        }

        public override bool Calc()
        {
            CalcNodes();
            bool result = FModified;
            if (FModified)
            {
                CalcEvent();
            }
            return result;
        }

        private void CalcEvent()
        {
            CalcNodes();
            CounterCalc++;
            FModified = false;
        }

        private void CalcNodes()
        {
            FNodeList.CalcNodes();
        }

        private void InitStartlistCount(int newCount)
        {
            for (int i = 0; i <= BOParams.RaceCount; i++)
            {
                RNode[i].Init(newCount);
            }
            EventNode.Init(newCount);
        }

        public bool UpdateStartlistCount(string roName, int newCount)
        {
            bool result = false;
            TRaceRowCollection cl = RNode[0].Collection;
            TEventRowCollectionItem cr;
            TRaceRowCollectionItem wr;
            if ((cl.Count < newCount) && (newCount <= BOParams.MaxStartlistCount))
            {
                while (cl.Count < newCount)
                {
                    for (int i = 0; i <= BOParams.RaceCount; i++)
                    {
                        wr = RNode[i].Collection.AddRow();
                        wr.SNR = 1001 + wr.BaseID;
                        wr.Bib = wr.BaseID + 1;
                    }
                    cr = EventNode.Collection.AddRow();
                    cr.SNR = 1001 + cr.BaseID;
                    cr.Bib = cr.BaseID + 1;
                }
                result = true;
            }
            if ((cl.Count > newCount) && (newCount >= BOParams.MinStartlistCount))
            {
                while (cl.Count > newCount)
                {
                    int c = cl.Count;
                    for (int i = 0; i <= BOParams.RaceCount; i++)
                    {
                        RNode[i].Collection.DeleteRow(c - 1);
                    }

                    EventNode.Collection.DeleteRow(c - 1);
                }
                result = true;
            }
            BOParams.StartlistCount = cl.Count;
            return result;
        }

        public bool UpdateAthlete(int SNR, string Cmd, string Value)
        {
            TStammdatenRowCollectionItem cr;

            cr = StammdatenNode.Collection.FindKey(SNR);
            if (cr == null)
            {
                cr = StammdatenNode.Collection.AddRow();
                cr.BaseID = StammdatenNode.Collection.Count;
                cr.SNR = SNR;
            }

            TStammdatenBO bo = StammdatenBO;

            if (Utils.Pos("Prop_", Cmd) > 0)
            {
                string Key = Utils.Copy(Cmd, 6, Cmd.Length);
                cr.Props[Key] = Value;
            }
            else if (Cmd == FieldNames.FN || Cmd == "FN")
            {
                bo.EditFN(cr, ref Value);
            }
            else if (Cmd == FieldNames.LN || Cmd == "LN")
            {
                bo.EditLN(cr, ref Value);
            }
            else if (Cmd == FieldNames.SN || Cmd == "SN")
            {
                bo.EditSN(cr, ref Value);
            }
            else if (Cmd == FieldNames.NC || Cmd == "NC")
            {
                bo.EditNC(cr, ref Value);
            }
            else if (Cmd == FieldNames.GR || Cmd == "GR")
            {
                bo.EditGR(cr, ref Value);
            }
            else if (Cmd == FieldNames.PB || Cmd == "PB")
            {
                bo.EditPB(cr, ref Value);
            }
            else if (Cmd.StartsWith("N"))
            {
                bo.EditNameColumn(cr, ref Value, "col_" + Cmd);
            }
            return true;
        }
        public TRaceNode FindNode(string roName)
        {
            if (Utils.Copy(roName, 1, 1) != "W")
            {
                return null;
            }

            string s = Utils.Copy(roName, 2, roName.Length);
            int i = Utils.StrToIntDef(s, -1);
            if ((i < 1) || (i > BOParams.RaceCount))
            {
                return null;
            }

            return RNode[i];
        }
        public string Save()
        {
            string result = string.Empty;
            FSLBackup = new TDBStringList();
            try
            {
                BackupToSL(null);
                result = FSLBackup.Text;
            }
            finally
            {
                FSLBackup = null;
            }
            return result;
        }
        public void Load(string Data)
        {
            FLoading = true;

            Clear();

            TStringList m = new TStringList();
            TBOMsg msg = new TBOMsg();

            try
            {
                ExcelImporter.RunImportFilter(Data, m);
                ConvertedData = m.Text;

                for (int i = 0; i < m.Count; i++)
                {
                    string s = m[i];
                    msg.Prot = s;
                    if (!msg.DispatchProt())
                    {
                        FRTrace.Trace("MessageError: " + s);
                    }
                }
                InitEventNode();
#if Undo
                UndoManager.Clear();
                UndoManager.UpdateBase(Data);
#endif
            }
            finally
            {
                FLoading = false;
            }
        }
        /// <summary>
        /// Virtual method, delegates to Load(Data).
        /// Overridden in TSDIBO, so that BO is disconnected and recreated, with new params.
        /// </summary>
        /// <param name="Data">new event data</param>
        public virtual void LoadNew(string Data)
        {
            Load(Data);
        }

        public virtual void Clear()
        {
            ClearBtnClick();
        }

        //---start load/save methods

        /// <summary>
        /// BackupToSL() and SaveToFile(FileName)
        /// </summary>
        /// <param name="aFileName"></param>
        public void Backup(string aFileName)
        {
            FSLBackup = new TDBStringList();
            try
            {
                BackupToSL(null);
                FSLBackup.SaveToFile(aFileName);
            }
            finally
            {
                FSLBackup = null;
            }
        }
        /// <summary>
        /// difference to Load: no clear() and data is read from file.
        /// </summary>
        /// <param name="aFileName">file name for LoadFromfile</param>
        public void Restore(string aFileName)
        {
            //Unterschied zu Load: 1. kein Clear(), 2. Data from File

            //Clear();

            TStringList m = new TDBStringList();
            TBOMsg msg = new TBOMsg();

            FLoading = true;
            try
            {
                m.LoadFromFile(aFileName);
                for (int i = 0; i < m.Count; i++)
                {
                    string s = m[i];
                    msg.Prot = s;
                    if (!msg.DispatchProt())
                    {
                        FRTrace.Trace("MessageError: " + s);
                    }
                }
                InitEventNode();
            }
            finally
            {
                FLoading = false;
            }
        }

        public void BackupAthletes()
        {
            int savedSchemaCode = FieldNames.SchemaCode;
            if (this.EventProps.NormalizedOutput)
            {
                FieldNames.SchemaCode = 2;
            }

            TStammdatenRowCollection cl = StammdatenNode.Collection;
            TStammdatenRowCollectionItem cr;
            TProp prop = new TProp();

            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                if (cr.FN != "")
                {
                    MsgTree.Division.Athlete(cr.SNR).FN(cr.FN);
                }

                if (cr.LN != "")
                {
                    MsgTree.Division.Athlete(cr.SNR).LN(cr.LN);
                }

                if (cr.SN != "")
                {
                    MsgTree.Division.Athlete(cr.SNR).SN(cr.SN);
                }

                if (cr.NC != "")
                {
                    MsgTree.Division.Athlete(cr.SNR).NC(cr.NC);
                }

                if (cr.GR != "")
                {
                    MsgTree.Division.Athlete(cr.SNR).GR(cr.GR);
                }

                if (cr.PB != "")
                {
                    MsgTree.Division.Athlete(cr.SNR).PB(cr.PB);
                }

                if (cl.FieldCount > TStammdatenRowCollection.FixFieldCount)
                {
                    for (int j = TStammdatenRowCollection.FixFieldCount + 1; j <= cl.FieldCount; j++)
                    {
                        MsgTree.Division.Athlete(cr.SNR).FieldN(j, cr[j]);
                    }
                }
                else
                {
                    for (int j = 0; j < cr.Props.Count; j++)
                    {
                        cr.Props.GetProp(j, ref prop);
                        MsgTree.Division.Athlete(cr.SNR).Prop(prop.Key, prop.Value);
                    }
                }
                if (FSLBackup != null)
                {
                    FSLBackup.Add("");
                }
            }
            FieldNames.SchemaCode = savedSchemaCode;
        }

        public override string ToString()
        {
            return ToTXT();
        }

        public string ToTXT()
        {
            TStrings SL = new TStringList();
            try
            {
                BackupToSL(SL);
                return SL.Text;
            }
            catch
            {
                return "";
            }
        }

        public string ToXML()
        {
            TStrings SL = new TStringList();
            try
            {
                BackupToXML(SL);
                return SL.Text;
            }
            catch
            {
                return "";
            }
        }

        public string ToJson()
        {
            TStrings SL = new TStringList();
            try
            {
                BackupToJson(SL);
                return SL.Text;
            }
            catch
            {
                return "";
            }
        }

        public void BackupToJson(TStrings SL)
        {
            JsonInfo ji;
            UpdateRaceNodes();
            if (SL != null)
            {
                FSLBackup = SL;
            }

            ji = new JsonInfo();
            try
            {
                ji.WriteJson(FSLBackup);
            }
            finally
            {
                if (SL != null)
                {
                    FSLBackup = null;
                }
                //ji.Free;
            }
        }

        public void BackupToXML(TStrings SL)
        {
            TFRXMLWriter XmlWriter;
            UpdateRaceNodes();
            if (SL != null)
            {
                FSLBackup = SL;
            }

            XmlWriter = new TFRXMLWriter();
            try
            {
                XmlWriter.Delimiter = ';';
                XmlWriter.WriteXml(FSLBackup);
            }
            finally
            {
                if (SL != null)
                {
                    FSLBackup = null;
                }
                //XmlWriter.Free;
            }
        }

        public override void BackupToSL(TStrings SL)
        {
            BackupToSL(SL, UseCompactFormat);
        }


        /// <summary>
        /// Worker method. Fills SL with the backup.
        /// </summary>
        /// <param name="SL">StringList, internal SL used if null</param>
        public override void BackupToSL(TStrings SL, bool CompactFormat)
        {
            TInputAction InputAction;
            TDivision g;
            TRun r;

            TRaceNode qn;
            TRaceRowCollection qc;
            TRaceRowCollectionItem qr;

            UpdateRaceNodes();

            if (!Object.Equals(SL, null))
            {
                FSLBackup = SL;
            }

            InputAction = new TInputAction
            {
                OnSend = new TInputAction.ActionEvent(SaveLine)
            };
            TInputActionManager.DynamicActionRef = InputAction;
            try
            {
                FSLBackup.Add("#Params");
                FSLBackup.Add("");
                FSLBackup.Add("DP.StartlistCount = " + BOParams.StartlistCount.ToString());
                FSLBackup.Add("DP.ITCount = " + BOParams.ITCount.ToString());
                FSLBackup.Add("DP.RaceCount = " + BOParams.RaceCount.ToString());

                //EventProps
                FSLBackup.Add("");
                FSLBackup.Add("#Event Properties");
                FSLBackup.Add("");

                EventProps.SaveProps(FSLBackup);

                TExcelExporter o = new TExcelExporter
                {
                    Delimiter = ';'
                };

                //CaptionList
                if (TColCaptions.ColCaptionBag.IsPersistent && TColCaptions.ColCaptionBag.Count > 0)
                {
                    FSLBackup.Add("");
                    o.AddSection(TExcelImporter.TableID_CaptionList, this, FSLBackup);
                }

                if (CompactFormat)
                {
                    try
                    {
                        //NameList
                        FSLBackup.Add("");
                        o.AddSection(TExcelImporter.TableID_NameList, this, FSLBackup);

                        //StartList
                        FSLBackup.Add("");
                        o.AddSection(TExcelImporter.TableID_StartList, this, FSLBackup);

                        //FleetList
                        if (EventNode.UseFleets)
                        {
                            FSLBackup.Add("");
                            o.AddSection(TExcelImporter.TableID_FleetList, this, FSLBackup);
                        }

                        //FinishList
                        FSLBackup.Add("");
                        o.AddSection(TExcelImporter.TableID_FinishList, this, FSLBackup);

                        //TimeList(s)
                        if (BOParams.ITCount > 0 || EventProps.IsTimed)
                        {
                            FSLBackup.Add("");
                            o.AddSection(TExcelImporter.TableID_TimeList, this, FSLBackup);
                        }
                    }
                    catch
                    {
                    }
                }
                else
                {
                    //Athletes
                    FSLBackup.Add("");
                    FSLBackup.Add("#Athletes");
                    FSLBackup.Add("");

                    BackupAthletes();

                    //Startlist
                    FSLBackup.Add("");
                    FSLBackup.Add("#Startlist");
                    FSLBackup.Add("");

                    qn = RNode[1];
                    g = MsgTree.Division;
                    qc = qn.Collection;
                    for (int i = 0; i < qc.Count; i++)
                    {
                        qr = qc[i];
                        if ((qr.Bib > 0) && (qr.Bib != qr.BaseID))
                        {
                            g.Race1.Startlist.Pos(qr.BaseID).Bib(qr.Bib.ToString());
                        }

                        if (qr.SNR > 0)
                        {
                            g.Race1.Startlist.Pos(qr.BaseID).SNR(qr.SNR.ToString());
                        }
                    }
                }

                //Results
                for (int n = 1; n <= BOParams.RaceCount; n++)
                {
                    FSLBackup.Add("");
                    FSLBackup.Add("#" + cTokenRace + n.ToString());
                    FSLBackup.Add("");

                    qn = RNode[n];
                    g = MsgTree.Division;
                    qc = qn.Collection;
                    if (n == 1)
                    {
                        r = g.Race1;
                    }
                    else if ((n > 1) && (n <= BOParams.RaceCount))
                    {
                        r = g.Race(n);
                    }
                    else
                    {
                        r = null;
                    }

                    if (r == null)
                    {
                        continue;
                    }

                    if (!qn.IsRacing)
                    {
                        r.IsRacing(Utils.BoolStr[false]);
                    }

                    for (int i = 0; i < qc.Count; i++)
                    {
                        qr = qc[i]; //Indexer Items
                        if ((i == 0) && qr.ST.TimePresent)
                        {
                            r.Bib(qr.Bib).ST(qr.ST.AsString);
                        }

                        if (!CompactFormat)
                        {

                            for (int t = 1; t <= BOParams.ITCount; t++)
                            {
                                if (qr[t].OTime.TimePresent) //Indexer IT
                                {
                                    r.Bib(qr.Bib).IT(t, qr[t].OTime.AsString);
                                }
                            }
                            if (qr.FT.OTime.TimePresent)
                            {
                                r.Bib(qr.Bib).FT(qr.FT.OTime.AsString);
                            }

                            if (qr.MRank > 0)
                            {
                                r.Bib(qr.Bib).Rank(qr.MRank.ToString());
                            }

                            if (EventNode.UseFleets)
                            {
                                TEventRaceEntry ere = EventNode.Collection[i].Race[n];
                                int f = ere.Fleet;
                                if (f > 0)
                                {
                                    r.Bib(qr.Bib).FM(f.ToString());
                                }
                            }

                        }

                        if (EventNode.UseFleets)
                        {
                            TEventRaceEntry ere = EventNode.Collection[i].Race[n];
                            if (!ere.IsRacing)
                            {
                                r.Bib(qr.Bib).RV("x");
                            }
                        }

                        if (qr.QU.AsInteger != 0)
                        {
                            r.Bib(qr.Bib).QU(qr.QU.ToString());
                        }

                        if (qr.DG > 0)
                        {
                            r.Bib(qr.Bib).DG(qr.DG.ToString());
                        }
                    }
                }

                FSLBackup.Add("");
                FSLBackup.Add("EP.IM = " + TEventProps.InputModeStrings[EventProps.InputMode]);

                //Errors
                EventNode.ErrorList.CheckAll(EventNode);
                if (EventNode.ErrorList.HasErrors())
                {
                    FSLBackup.Add("");
                    FSLBackup.Add("#Errors");
                    FSLBackup.Add("");
                    EventNode.ErrorList.GetMsg(FSLBackup);
                }
            }
            finally
            {
                if (!Object.Equals(SL, null))
                {
                    FSLBackup = null;
                }

                TInputActionManager.DynamicActionRef = null;
            }
        }
        public override void Backup()
        {
            BackupBtnClick();
        }
        public override void Restore()
        {
            RestoreBtnClick();
        }
        public void BackupBtnClick()
        {
            string fn = BackupDir + "_Backup.txt";
            Backup(fn);
        }
        public void RestoreBtnClick()
        {
            Clear();
            string fn = BackupDir + "_Backup.txt";
            Restore(fn);
        }

        //---end load/save methods

        public void ClearBtnClick()
        {
            ClearResult(string.Empty);
            ClearList(string.Empty);
            UpdateEventNode();
        }

        public override void OnIdle()
        {
            Calc();
            //pass any input message on to output clients
            if (InputServer != null)
            {
                InputServer.ProcessQueue();
            }
        }

        public void CreateTimestampedBackupBtnClick()
        {
            DateTime dt = DateTime.Now;
            string dts = string.Format("{0}{1}{2}_{3}{4}{5}",
                new object[] { dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second });
            string fn = BackupDir + "_Backup" + "_" + dts + ".txt";
            Backup(fn);
        }

        public void CopyFromRaceNode(TRaceNode ru, bool MRank)
        {
            int RaceIndex = ru.Index;
            TEventRowCollection wl = EventNode.Collection;
            TRaceRowCollection cl = ru.Collection;
            TEventRowCollectionItem wr;
            TRaceRowCollectionItem cr;
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                wr = wl[i];
                //wr.Race[RaceIndex].QU = cr.QU.AsInteger;
                wr.Race[RaceIndex].Penalty.Assign(cr.QU);
                wr.Race[RaceIndex].DG = cr.DG;
                if (MRank)
                {
                    wr.Race[RaceIndex].OTime = cr.MRank;
                }
                else
                {
                    wr.Race[RaceIndex].OTime = cr.FT.ORank;
                }
            }
            wl.BaseNode.Modified = true;
        }
        public void CopyToRaceNode(TRaceNode ru)
        {
            int RaceIndex = ru.Index;
            TEventRowCollection wl = EventNode.Collection;
            TRaceRowCollection cl = ru.Collection;
            TEventRowCollectionItem wr;
            TRaceRowCollectionItem cr;
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                wr = wl[i];
                cr.QU.Assign(wr.Race[RaceIndex].Penalty);
                cr.DG = wr.Race[RaceIndex].DG;
                cr.MRank = wr.Race[RaceIndex].OTime;
            }
        }
        public void CopyOTimes(int RaceIndex)
        {
            TEventRowCollection wl = EventNode.Collection;
            TRaceRowCollection cl = RNode[RaceIndex].Collection;
            TEventRowCollectionItem wr;
            TRaceRowCollectionItem cr;
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                wr = wl[i];
                cr.MRank = wr.Race[RaceIndex].OTime;
            }
        }

        public void InitEventNode()
        {
            for (int i = 1; i <= BOParams.RaceCount; i++)
            {
                CopyFromRaceNode(RNode[i], true);
            }
        }

        public void UpdateEventNode()
        {
            for (int i = 1; i <= BOParams.RaceCount; i++)
            {
                CopyFromRaceNode(RNode[i], false);
            }
        }

        public void UpdateRaceNodes()
        {
            for (int i = 1; i <= BOParams.RaceCount; i++)
            {
                CopyToRaceNode(RNode[i]);
            }
        }

        public void RebuildEventNode()
        {
            TEventRowCollection wl = EventNode.Collection;
            wl.Clear();
            TRaceRowCollection cl = RNode[0].Collection;
            TEventRowCollectionItem wr;
            TRaceRowCollectionItem cr;
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                wr = wl.AddRow();
                wr.BaseID = i + 1;
                wr.SNR = cr.SNR;
                wr.Bib = cr.Bib;
            }
            UpdateEventNode();
        }

        public override TBaseMsg NewMsg()
        {
            return TMain.MsgFactory.CreateMsg();
        }

        public void GetReportData(TStrings SL)
        {
            for (int j = 1; j <= EventNode.RaceCount; j++)
            {
                //GetReportDataRace(SL, j);
            }
        }

        /// <summary>
        /// Fill Report Data StringList with Information for Race j.
        /// </summary>
        /// <param name="SL">StringList</param>
        /// <param name="j">Race</param>
        public void GetReportDataRace(TStrings SL, int j)
        {
            if ((j < 1) || (j > EventNode.RaceCount))
            {
                return;
            }

            TRaceNode rn;
            TRaceRowCollection rl;
            TRaceRowCollectionItem rr;
            TEventRowCollectionItem cr;
            string s;

            rn = RNode[j];
            if (rn.IsRacing)
            {
                rl = rn.Collection;
                int LastPos = rl.Count;
                SL.Add("StartlistCount=" + rl.Count.ToString());
                for (int i = 0; i < rl.Count; i++)
                {
                    rr = rl[i];
                    cr = EventNode.Collection[i];

                    int Line = rr[0].ORank;
                    if (Line == 0)
                    {
                        Line = LastPos;
                        LastPos = LastPos - 1;
                    }

                    string temp = BOParams.DivisionName + ".W" + j.ToString() + ".L" + Line.ToString();

                    SL.Add(temp + ".NOC=" + rr.NC);
                    SL.Add(temp + ".Helm=" + rr.FN + ' ' + rr.LN);

                    if (rr.QU.ToString() != "")
                    {
                        SL.Add(temp + ".QU=" + rr.QU.ToString());
                    }

                    SL.Add(temp + ".Points=" + cr.Race[j].Points);

                    s = cr.Race[j].OTime.ToString();
                    if (s == "0")
                    {
                        s = "";
                    }

                    SL.Add(temp + ".PosR=" + s);

                    for (int k = 0; k < rr.ITCount; k++)
                    {
                        string tempIT = temp + ".IT" + k.ToString();

                        s = rr[k].ORank.ToString();
                        if (s == "0")
                        {
                            s = "-";
                        }

                        SL.Add(tempIT + ".PosR=" + s);

                        s = rr[k].Behind.ToString();
                        if (s.EndsWith(".00"))
                        {
                            s = s.Substring(0, s.Length - 3);
                        }

                        if (s == "")
                        {
                            s = " ";
                        }

                        SL.Add(tempIT + ".Behind=" + s);
                    }
                }
            }
        }
        public void GetReportDataSeries(TStrings SL)
        {
            SL.Add(BOParams.DivisionName + ".StartlistCount=" + BOParams.StartlistCount.ToString());
            SL.Add(BOParams.DivisionName + ".RaceCount=" + BOParams.RaceCount.ToString());

            for (int i = 0; i <= EventNode.Collection.Count - 1; i++)
            {
                TEventRowCollectionItem cr = EventNode.Collection[i];
                string s = BOParams.DivisionName + ".L" + cr.GPosR.ToString();

                SL.Add(s + ".Pos=" + cr.GPosR.ToString());
                SL.Add(s + ".Bib=" + cr.Bib.ToString());
                SL.Add(s + ".NOC=" + cr.NC);
                SL.Add(s + ".Helm=" + cr.FN + " " + cr.LN);
                for (int r = 1; r <= EventNode.RaceCount; r++)
                {
                    SL.Add(s + ".W" + r.ToString() + "=" + cr[r]); //cr.Race[r].RaceValue;
                }
                SL.Add(s + ".Netto=" + cr.GPoints);
                SL.Add(s + ".Rank=" + cr.GRank.ToString());
            }
        }
        public void GetSeriesDataSQL(TStrings SL)
        {
            TRaceNode rn;
            TRaceRowCollection rl;
            TEventRowCollectionItem cr;
            string s;

            for (int j = 1; j <= EventNode.RaceCount; j++)
            {
                rn = RNode[j];
                if (rn.IsRacing)
                {
                    rl = rn.Collection;
                    for (int i = 0; i < rl.Count; i++)
                    {
                        cr = EventNode.Collection[i];

                        string sValue = cr[j]; //cr.RaceValue[j];        
                        //sValue = cr.Race[j].Points + "<br/>" + IntToStr(cr.Race[j].OTime);
                        //if (rr.QU.ToString != "")
                        //  sValue = sValue + "<br/>" + rr.QU.ToString();        
                        s = string.Format("Update SeriesResults set W{0} = '{1}' where DivisionID = {2} and Bib = {3}",
                            j, sValue, BOParams.DivisionID, cr.Bib);
                        SL.Add(s);

                        if (j == EventNode.RaceCount)
                        {
                            s = string.Format("Update SeriesResults set Netto = {0:G} where DivisionID = {1} and Bib = {2}",
                                cr.GPoints, BOParams.DivisionID, cr.Bib);
                            SL.Add(s);

                            s = string.Format("Update SeriesResults set Rank = {0} where DivisionID = {1} and Bib = {2}",
                                cr.GRank, BOParams.DivisionID, cr.Bib);
                            SL.Add(s);

                            s = string.Format("Update SeriesResults set PosR = {0} where DivisionID = {1} and Bib = {2}",
                                cr.GPosR, BOParams.DivisionID, cr.Bib);
                            SL.Add(s);
                        }
                    }
                }
            }
        }

        public void GetRaceDataSQL(TStrings SL)
        {
            TRaceNode rn;
            TRaceRowCollection rl;
            TRaceRowCollectionItem rr;
            TEventRowCollectionItem cr;

            for (int j = 1; j <= EventNode.RaceCount; j++)
            {
                rn = RNode[j];
                if (rn.IsRacing)
                {
                    rl = rn.Collection;
                    int LastPos = rl.Count;
                    for (int i = 0; i < rl.Count; i++)
                    {
                        rr = rl[i];
                        cr = EventNode.Collection[i];

                        int Line = rr[0].PosR;
                        if (Line == 0)
                        {
                            Line = LastPos;
                            LastPos = LastPos - 1;
                        }

                        string s = string.Format("Update RaceResults set Val = '{0}' where DivisionID = {1} and Bib = {2} and Race = {3} and MarkID = 0",
                            Line, BOParams.DivisionID, cr.Bib, j);
                        SL.Add(s);

                        int MarkID;
                        for (int k = 0; k < rr.ITCount; k++)
                        {
                            if (k == 0)
                            {
                                MarkID = rr.ITCount;
                            }
                            else
                            {
                                MarkID = k;
                            }

                            string s1 = rr[k].ORank.ToString();
                            if (s1 == "0")
                            {
                                s1 = "-";
                            }

                            string s2 = rr[k].Behind.ToString();
                            if (s2.EndsWith(".00"))
                            {
                                s2 = s2.Substring(0, s2.Length - 3);
                            }

                            if (s2 == "")
                            {
                                s2 = " ";
                            }

                            string sValue = s1 + "<br/>" + s2;
                            s = string.Format("Update RaceResults set Val = '{0}' where DivisionID = {1} and Bib = {2} and Race = {3} and MarkID = {4}",
                                sValue, BOParams.DivisionID, cr.Bib, j, MarkID);
                            SL.Add(s);
                        }
                    }
                }
            }

        }

        public void BackupPenalties(TStrings SL, int n)
        {
            TInputAction InputAction;
            TDivision g;
            TRun r;

            TRaceNode qn;
            TRaceRowCollection qc;
            TRaceRowCollectionItem qr;

            UpdateRaceNodes();

            if (SL != null)
            {
                FSLBackup = SL;
            }

            InputAction = new TInputAction
            {
                OnSend = SaveLine
            };
            TInputActionManager.DynamicActionRef = InputAction;
            try
            {
                qn = RNode[n];
                g = MsgTree.Division;
                qc = qn.Collection;
                if (n == 1)
                    r = g.Race1;
                else if ((n > 1) && (n <= this.BOParams.RaceCount))
                    r = g.Race(n);
                else
                    r = null;
                if (r != null)
                {
                    if (!qn.IsRacing)
                        r.IsRacing(Utils.BoolStr[false]);
                    for (int i = 0; i < qc.Count; i++)
                    {
                        qr = qc[i];
                        if ((i == 0) && qr.ST.TimePresent)
                            r.Bib(qr.Bib).ST(qr.ST.AsString);

                        if (EventNode.UseFleets)
                        {
                            TEventRaceEntry ere = EventNode.Collection[i].Race[n];
                            if (!ere.IsRacing)
                                r.Bib(qr.Bib).RV("x");
                        }

                        if (qr.QU.AsInteger != 0)
                            r.Bib(qr.Bib).QU(qr.QU.ToString());
                        if (qr.DG > 0)
                            r.Bib(qr.Bib).DG(qr.DG.ToString());
                    }
                }
            }
            finally
            {
                if (SL != null)
                    FSLBackup = null;
                TInputActionManager.DynamicActionRef = null;
            }
        }

        public CurrentNumbers FindCurrentInEvent(CurrentNumbers result)
        {
            int rc = EventNode.RaceCount;
            int tc = BOParams.ITCount;

            TEventRowCollection cl;
            TEventRowCollectionItem cr;
            TEventRaceEntry tp;
            for (int r = rc; r > 0; r--)
            {
                for (int t = tc; t >= 0; t--)
                {
                    cl = EventNode.Collection;
                    result.Clear();
                    for (int i = 0; i < cl.Count; i++)
                    {
                        cr = cl[i];
                        tp = cr.Race[r];
                        if (tp.IsRacing) {
                            if (tp.OTime > 0 || tp.Penalty.IsOut)
                            {
                                result.race = r;
                                result.bib = cr.Bib;
                                if (tp.OTime > 0)
                                {
                                    result.withTime++;
                                }
                                if (tp.Penalty.IsOut)
                                {
                                    result.withPenalty++;
                                }
                                result.withTimeOrPenalty++;
                            }
                        }
                    }
                    if (result.withTimeOrPenalty > 0)
                    {
                        if (result.withTimeOrPenalty == cl.Count)
                        {
                            if (r < rc)
                            {
                                result.race++;
                            }
                        }
                        return result;
                    }
                }
            }
            result.race = 1;
            return result;
        }
    }

}
