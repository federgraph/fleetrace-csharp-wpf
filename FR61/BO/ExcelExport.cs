namespace RiggVar.FR
{
    public class TExcelExporter
    {
        protected TStringList SL;
        private TStringList SLToken;
        public char Delimiter;

        public TExcelExporter()
        {
            SL = new TStringList();
            SLToken = new TStringList();
            Delimiter = ';';
        }

        public void FillTable(int TableID, TBO BO)
        {
            switch (TableID)
            {
                case TExcelImporter.TableID_NameList: GetNameList(BO); break;
                case TExcelImporter.TableID_StartList: GetStartList(BO); break;
                case TExcelImporter.TableID_FleetList: GetFleetList(BO); break;
                case TExcelImporter.TableID_FinishList: GetFinishList(BO); break;
                case TExcelImporter.TableID_ResultList: GetResultList(BO); break;
                case TExcelImporter.TableID_CaptionList: GetCaptionList(); break;
            }
        }

        private void CopyLines(TStrings Memo)
        {
            for (int i = 0; i < SL.Count; i++)
            {
                Memo.Add(SL[i]);
            }
        }

        public void AddTimingSection(TBO BO, TStrings Memo, int r)
        {
            Memo.Add(TExcelImporter.TimeListStartToken + ".R" + r.ToString());
            GetTimeList(r, BO);
            CopyLines(Memo);
            Memo.Add(TExcelImporter.TimeListEndToken);
        }

        public void AddRaceFinishSection(TBO BO, TStrings Memo, int r)
        {
            //partial finish list with only one race and only Bib (without SNR)
            Memo.Add(TExcelImporter.FinishListStartToken);
            GetRaceFinishList(BO, r);
            CopyLines(Memo);
            Memo.Add(TExcelImporter.FinishListEndToken);
        }

        public void AddSection(int TableID, TBO BO, TStrings Memo)
        {
            switch (TableID)
            {
                case TExcelImporter.TableID_NameList:
                    Memo.Add(TExcelImporter.NameListStartToken);
                    GetNameList(BO);
                    CopyLines(Memo);
                    Memo.Add(TExcelImporter.NameListEndToken);
                    break;

                case TExcelImporter.TableID_StartList:
                    Memo.Add(TExcelImporter.StartListStartToken);
                    GetStartList(BO);
                    CopyLines(Memo);
                    Memo.Add(TExcelImporter.StartListEndToken);
                    break;

                case TExcelImporter.TableID_FleetList:
                    Memo.Add(TExcelImporter.FleetListStartToken);
                    GetFleetList(BO);
                    CopyLines(Memo);
                    Memo.Add(TExcelImporter.FleetListEndToken);
                    break;

                case TExcelImporter.TableID_FinishList:
                    Memo.Add(TExcelImporter.FinishListStartToken);
                    GetFinishList(BO);
                    CopyLines(Memo);
                    Memo.Add(TExcelImporter.FinishListEndToken);
                    break;

                case TExcelImporter.TableID_ResultList:
                    Memo.Add(TExcelImporter.ResultListStartToken);
                    GetResultList(BO);
                    CopyLines(Memo);
                    Memo.Add(TExcelImporter.ResultListEndToken);
                    break;

                case TExcelImporter.TableID_TimeList:
                    for (int r = 1; r <= BO.BOParams.RaceCount; r++)
                    {
                        if (r > 0)
                        {
                            SL.Add("");
                        }

                        Memo.Add(TExcelImporter.TimeListStartToken + ".R" + r.ToString());
                        GetTimeList(r, BO);
                        CopyLines(Memo);
                        Memo.Add(TExcelImporter.TimeListEndToken);
                        Memo.Add("");
                    }
                    break;

                case TExcelImporter.TableID_CaptionList:
                    Memo.Add(TExcelImporter.CaptionListStartToken);
                    GetCaptionList();
                    CopyLines(Memo);
                    Memo.Add(TExcelImporter.CaptionListEndToken);
                    break;

            }
        }

        public void AddLines(int TableID, TBO BO, TStrings Memo)
        {
            FillTable(TableID, BO);
            for (int i = 0; i < SL.Count; i++)
            {
                Memo.Add(SL[i]);
            }
        }

        public string GetString(int TableID, TBO BO)
        {
            FillTable(TableID, BO);
            return SL.Text;
        }


        public void GetNameList(TBO BO)
        {
            string s;
            TStammdatenRowCollection cl;
            TStammdatenRowCollectionItem cr;

            SL.Clear();
            SLToken.Delimiter = Delimiter;
            cl = BO.EventNode.StammdatenRowCollection;
            if (cl.Count < 1)
            {
                return;
            }

            cr = cl[0];

            //HeaderLine
            SLToken.Clear();
            SLToken.Add("SNR");
            for (int j = 1; j <= cr.FieldCount; j++)
            {
                if (cr.GetFieldUsed(j))
                {
                    SLToken.Add("N" + j.ToString());
                }
            }
            s = SLToken.DelimitedText;
            SL.Add(s);

            //DataLines
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                SLToken.Clear();
                SLToken.Add(cr.SNR.ToString());
                for (int j = 1; j <= cr.FieldCount; j++)
                {
                    if (!cr.GetFieldUsed(j))
                    {
                        continue;
                    }

                    SLToken.Add(cr[j]);
                }
                s = SLToken.DelimitedText;
                SL.Add(s);
            }
        }
        public void GetStartList(TBO BO)
        {
            SL.Clear();
            SLToken.Delimiter = Delimiter;
            TEventRowCollection cl = BO.EventNode.Collection;
            if (cl.Count < 1)
            {
                return;
            }

            //HeaderLine
            SLToken.Clear();
            SLToken.Add("Pos");
            SLToken.Add("SNR");
            SLToken.Add("Bib");
            string s = SLToken.DelimitedText;
            SL.Add(s);

            //DataLines
            for (int i = 0; i < cl.Count; i++)
            {
                TEventRowCollectionItem cr = cl[i];
                SLToken.Clear();
                SLToken.Add(cr.BaseID.ToString());
                SLToken.Add(cr.SNR.ToString());
                SLToken.Add(cr.Bib.ToString());
                s = SLToken.DelimitedText;
                SL.Add(s);
            }
        }
        public void GetRaceFinishList(TBO BO, int r)
        {
            string s;
            TEventRowCollection cl;
            TEventRowCollectionItem cr;
            TEventRaceEntry ere;

            SL.Clear();
            SLToken.Delimiter = Delimiter;
            cl = BO.EventNode.Collection;
            if (cl.Count< 1)
            {
                return;
            }

            // HeaderLine
            this.SLToken.Clear();
            //this.SLToken.Add("SNR");
            this.SLToken.Add("Bib");
            cr = cl[0];
            this.SLToken.Add("R" + r.ToString());
            s = this.SLToken.DelimitedText;
            this.SL.Add(s);

            // DataLines
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                SLToken.Clear();
                //SLToken.Add(cr.SNR.ToString());
                SLToken.Add(cr.Bib.ToString());
                ere = cr.Race[r];
                SLToken.Add(ere.OTime.ToString());
                s = SLToken.DelimitedText;
                SL.Add(s);
            }
        }
        public void GetFinishList(TBO BO)
        {
            string s;
            TEventRowCollection cl;
            TEventRowCollectionItem cr;
            TEventRaceEntry ere;

            SL.Clear();
            SLToken.Delimiter = Delimiter;
            cl = BO.EventNode.Collection;
            if (cl.Count < 1)
            {
                return;
            }

            //HeaderLine
            SLToken.Clear();
            SLToken.Add("SNR");
            SLToken.Add("Bib");
            cr = cl[0];
            for (int r = 1; r < cr.RCount; r++)
            {
                SLToken.Add("R" + r.ToString());
            }

            s = SLToken.DelimitedText;
            SL.Add(s);

            //DataLines
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                SLToken.Clear();
                SLToken.Add(cr.SNR.ToString());
                SLToken.Add(cr.Bib.ToString());
                for (int r = 1; r < cr.RCount; r++)
                {
                    ere = cr.Race[r];
                    SLToken.Add(ere.OTime.ToString());
                }
                s = SLToken.DelimitedText;
                SL.Add(s);
            }
        }
        public void GetFleetList(TBO BO)
        {
            string s;
            TEventRowCollection cl;
            TEventRowCollectionItem cr;
            TEventRaceEntry ere;

            SL.Clear();
            SLToken.Delimiter = Delimiter;
            cl = BO.EventNode.Collection;
            if (cl.Count < 1)
            {
                return;
            }

            //HeaderLine
            SLToken.Clear();
            SLToken.Add("SNR");
            SLToken.Add("Bib");
            cr = cl[0];
            for (int r = 1; r < cr.RCount; r++)
            {
                SLToken.Add("R" + r.ToString());
            }

            s = SLToken.DelimitedText;
            SL.Add(s);

            //DataLines
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                SLToken.Clear();
                SLToken.Add(cr.SNR.ToString());
                SLToken.Add(cr.Bib.ToString());
                for (int r = 1; r < cr.RCount; r++)
                {
                    ere = cr.Race[r];
                    SLToken.Add(ere.Fleet.ToString());
                }
                s = SLToken.DelimitedText;
                SL.Add(s);
            }
        }
        public void GetResultList(TBO BO)
        {
            string s;
            TEventRowCollection cl;
            TEventRowCollectionItem cr;
            TEventRaceEntry ere;

            SL.Clear();
            SLToken.Delimiter = Delimiter;
            cl = BO.EventNode.Collection;
            if (cl.Count < 1)
            {
                return;
            }

            //HeaderLine
            SLToken.Clear();
            SLToken.Add("SNR");
            SLToken.Add("Bib");
            SLToken.Add("N1");
            SLToken.Add("N2");
            SLToken.Add("N3");
            SLToken.Add("N4");
            SLToken.Add("N5");
            SLToken.Add("N6");
            cr = cl[0];
            for (int r = 1; r < cr.RCount; r++)
            {
                SLToken.Add("R" + r.ToString());
            }

            s = SLToken.DelimitedText;
            SL.Add(s);

            //DataLines
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                SLToken.Clear();
                SLToken.Add(cr.SNR.ToString());
                SLToken.Add(cr.Bib.ToString());
                SLToken.Add(cr.FN);
                SLToken.Add(cr.LN);
                SLToken.Add(cr.SN);
                SLToken.Add(cr.NC);
                SLToken.Add(cr.GR);
                SLToken.Add(cr.PB);
                for (int r = 1; r < cr.RCount; r++)
                {
                    ere = cr.Race[r];
                    SLToken.Add(ere.OTime.ToString());
                }
                s = SLToken.DelimitedText;
                SL.Add(s);
            }
        }

        public void GetTimeList(int r, TBO BO)
        {
            string s;
            TRaceRowCollection cl;
            TRaceRowCollectionItem cr;
            TTimePoint tp;

            SL.Clear();
            SLToken.Delimiter = Delimiter;
            cl = BO.RNode[r].Collection;
            if (cl.Count < 1)
            {
                return;
            }

            //HeaderLine
            SLToken.Clear();
            SLToken.Add("SNR");
            SLToken.Add("Bib");
            cr = cl[0];
            for (int i = 1; i < cr.ITCount; i++)
            {
                SLToken.Add("IT" + i.ToString());
            }

            SLToken.Add("FT");
            s = SLToken.DelimitedText;
            SL.Add(s);

            //DataLines
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                SLToken.Clear();
                SLToken.Add(cr.SNR.ToString());
                SLToken.Add(cr.Bib.ToString());
                for (int t = 1; t < cr.ITCount; t++)
                {
                    tp = cr[t];
                    SLToken.Add(tp.OTime.AsString);
                }
                tp = cr[0];
                SLToken.Add(tp.OTime.AsString);
                s = SLToken.DelimitedText;
                SL.Add(s);
            }

        }

        public void GetCaptionList()
        {
            SL.Clear();
            SLToken.Clear();
            SLToken.Delimiter = Delimiter;
            SL.Text = TColCaptions.ColCaptionBag.Text;
        }

    }
}
