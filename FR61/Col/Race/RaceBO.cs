using System;

namespace RiggVar.FR
{
    public class TRaceBO : TBaseColBO<
        TRaceColGrid,
        TRaceBO,
        TRaceNode,
        TRaceRowCollection,
        TRaceRowCollectionItem,
        TRaceColProps,
        TRaceColProp
        >
    {
        public TRaceBO()
            : base()
        {
        }

        private string ValidateOTime(TPTime t, string Value)
        {
            if (true) //not Locked
            {
                if (Value == "-1")
                {
                    t.Clear();
                }
                else if (Value == "n")
                {
                    //t.Parse(TimeToStr(Now, FFormatSettings))
                    t.Parse(DateTime.Now.ToString("HH:mm:ss.ff"));
                }
                else
                {
                    t.Parse(Value);
                }
            }
            return t.ToString();
        }

        public TNotifyEvent OnChange { get; set; }

        public override void InitColsActive(TRaceColGrid g)
        {
            if (CurrentNode != null)
            {
                InitColsActiveLayout(g, CurrentNode.Layout);
            }
        }

        public override void InitColsActiveLayout(TRaceColGrid g, int aLayout)
        {
            g.ColsActive.Clear();
            g.AddColumn("col_BaseID");

            TRaceColProp cp;

            g.AddColumn("col_SNR");

            g.AddColumn("col_Bib");
            g.AddColumn("col_NC");
            g.AddColumn("col_MRank");

            cp = g.AddColumn("col_QU");
            cp.OnFinishEdit = new TRaceColGrid.TBaseGetTextEvent(EditQU);
            cp.ReadOnly = false;

            cp = g.AddColumn("col_DG");
            cp.OnFinishEdit = new TRaceColGrid.TBaseGetTextEvent(EditDG);
            cp.ReadOnly = false;

            cp = g.AddColumn("col_ST");
            cp.OnFinishEdit = new TRaceColGrid.TBaseGetTextEvent(EditST);
            cp.ReadOnly = false;

            if ((aLayout >= 0) && (aLayout <= TMain.BO.BOParams.ITCount))
            {
                string s = "col_IT" + aLayout.ToString();
                cp = g.AddColumn(s);
                if (cp != null)
                {
                    cp.OnFinishEdit2 = new TRaceColGrid.TBaseGetTextEvent2(EditIT);
                    cp.ReadOnly = false;
                }
                g.AddColumn(s + "B");
                g.AddColumn(s + "BFT");
                g.AddColumn(s + "BPL");
                g.AddColumn(s + "Rank");
                g.AddColumn(s + "PosR");
                g.AddColumn(s + "PLZ");
            }

            cp = g.AddColumn("col_FT");
            cp.OnFinishEdit = new TRaceColGrid.TBaseGetTextEvent(EditFT);
            cp.ReadOnly = false;

            g.AddColumn("col_ORank");
            g.AddColumn("col_Rank");
            g.AddColumn("col_PosR");
            g.AddColumn("col_PLZ");
        }

        private void Changed()
        {
            OnChange?.Invoke(this);
        }

        public void EditSNR(TRaceRowCollectionItem cr, ref string Value)
        {
            cr.SNR = Utils.StrToIntDef(Value, cr.SNR);
            Value = Utils.IntToStr(cr.SNR);
            //horizontal kopieren
            TMain.BO[BOIndexer.SNR, 0, cr.IndexOfRow] = cr.SNR;
        }

        public void EditBib(TRaceRowCollectionItem cr, ref string Value)
        {
            cr.Bib = Utils.StrToIntDef(Value, cr.Bib);
            Value = Utils.IntToStr(cr.Bib);
            //horizontal kopieren
            TMain.BO[BOIndexer.Bib, 0, cr.IndexOfRow] = cr.Bib;
        }

        public void EditQU(TRaceRowCollectionItem cr, ref string Value)
        {
            cr.QU.Parse(Value);

            Value = cr.QU.ToString();
            cr.Modified = true;
            //Penalty-Indexer:
            TMain.BO[cr.Ru.Index, cr.IndexOfRow] = cr.QU;
        }

        public void EditDG(TRaceRowCollectionItem cr, ref string Value)
        {
            cr.DG = Utils.StrToIntDef(Value, cr.DG);
            Value = Utils.IntToStr(cr.DG);
            cr.Modified = true;
            TMain.BO[BOIndexer.DG, cr.Ru.Index, cr.IndexOfRow] = cr.DG;
        }

        public void EditOTime(TRaceRowCollectionItem cr, ref string Value)
        {
            cr.MRank = Utils.StrToIntDef(Value, cr.MRank);
            Value = Utils.IntToStr(cr.MRank);
            //cr.Modified = true;
            TMain.BO[BOIndexer.OT, cr.Ru.Index, cr.IndexOfRow] = cr.MRank;
        }

        public void EditMRank(TRaceRowCollectionItem cr, ref string Value)
        {
            TRaceRowCollection cl = cr.Ru.Collection;

            int oldRank = cr.MRank;
            int newRank = Utils.StrToIntDef(Value, cr.MRank);
            int maxRank = 0;

            TRaceRowCollectionItem cr1;
            for (int i = 0; i < cl.Count; i++)
            {
                cr1 = cl[i];
                if (cr == cr1)
                {
                    continue;
                }
                else if (cr1.MRank > 0)
                {
                    maxRank++;
                }
            }

            //limit new value
            if (newRank < 0)
            {
                newRank = 0;
            }

            if (newRank > maxRank + 1)
            {
                newRank = maxRank + 1;
            }

            if (newRank > cl.Count)
            {
                newRank = cl.Count;
            }

            if (oldRank == newRank)
            {
                Value = Utils.IntToStr(cr.MRank);
            }
            else
            {
                for (int i = 0; i < cl.Count; i++)
                {
                    cr1 = cl[i];
                    if (cr1 == cr)
                    {
                        continue;
                    }

                    int temp = cr1.MRank;
                    //remove
                    if ((oldRank > 0) && (oldRank < temp))
                    {
                        cr1.MRank = temp - 1;
                    }
                    //insert
                    if ((newRank > 0) && (newRank <= cr1.MRank))
                    {
                        cr1.MRank = cr1.MRank + 1;
                    }
                }
                cr.MRank = newRank;
                Value = Utils.IntToStr(cr.MRank);
                Changed();
                cr.Modified = true;
            }
        }

        public void EditST(TRaceRowCollectionItem cr, ref string Value)
        {
            Value = ValidateOTime(cr.ST, Value);
            cr.Modified = true;
        }

        public void EditIT(TRaceRowCollectionItem cr, ref string Value, string ColName)
        {
            int i = Utils.StrToIntDef(Utils.Copy(ColName, 7, ColName.Length), -1);
            TTimePoint t = cr[i]; //cr.IT[i];
            if (t != null)
            {
                Value = ValidateOTime(t.OTime, Value);
                cr.Modified = true;
                TMain.BO.CalcTP.UpdateDynamicBehind(this, cr, i);
            }
        }

        public void EditFT(TRaceRowCollectionItem cr, ref string Value)
        {
            Value = ValidateOTime(cr.FT.OTime, Value);
            cr.Modified = true;
            TMain.BO.CalcTP.UpdateDynamicBehind(this, cr, 0); //channel_FT
        }

    }

}
