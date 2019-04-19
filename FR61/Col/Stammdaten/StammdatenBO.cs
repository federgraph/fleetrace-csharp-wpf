#define Undo

namespace RiggVar.FR
{
    public class TStammdatenBO : TBaseColBO<
        TStammdatenColGrid,
        TStammdatenBO,
        TStammdatenNode,
        TStammdatenRowCollection,
        TStammdatenRowCollectionItem,
        TStammdatenColProps,
        TStammdatenColProp
        >
    {

#if Undo
        public TUndoAgent UndoAgent;
#endif

        public TStammdatenBO()
            : base()
        {
#if Undo
            UndoAgent = TMain.BO.UndoAgent;
#endif
        }

        private int FieldCount => TMain.BO.AdapterParams.FieldCount;

        public override void InitColsActive(TStammdatenColGrid g)
        {
            TStammdatenColProp cp;
            g.ColsActive.Clear();
            g.AddColumn("col_BaseID");

            cp = g.AddColumn("col_SNR");
            cp.OnFinishEdit = new TStammdatenColGrid.TBaseGetTextEvent(EditSNR);
            cp.ReadOnly = false;

            int fc = FieldCount;

            if (fc > 0)
            {
                cp = g.AddColumn("col_FN");
                cp.OnFinishEdit = new TStammdatenColGrid.TBaseGetTextEvent(EditFN);
                cp.ReadOnly = false;
            }

            if (fc > 1)
            {
                cp = g.AddColumn("col_LN");
                cp.OnFinishEdit = new TStammdatenColGrid.TBaseGetTextEvent(EditLN);
                cp.ReadOnly = false;
            }

            if (fc > 2)
            {
                cp = g.AddColumn("col_SN");
                cp.OnFinishEdit = new TStammdatenColGrid.TBaseGetTextEvent(EditSN);
                cp.ReadOnly = false;
            }

            if (fc > 3)
            {
                cp = g.AddColumn("col_NC");
                cp.OnFinishEdit = new TStammdatenColGrid.TBaseGetTextEvent(EditNC);
                cp.ReadOnly = false;
            }

            if (fc > 4)
            {
                cp = g.AddColumn("col_GR");
                cp.OnFinishEdit = new TStammdatenColGrid.TBaseGetTextEvent(EditGR);
                cp.ReadOnly = false;
            }

            if (fc > 5)
            {
                cp = g.AddColumn("col_PB");
                cp.OnFinishEdit = new TStammdatenColGrid.TBaseGetTextEvent(EditPB);
                cp.ReadOnly = false;
            }

            if (fc > TStammdatenRowCollection.FixFieldCount)
            {
                for (int i = TStammdatenRowCollection.FixFieldCount + 1; i <= fc; i++)
                {
                    cp = g.AddColumn("col_N" + i.ToString());
                    cp.OnFinishEdit2 = new TStammdatenColGrid.TBaseGetTextEvent2(EditNameColumn);
                    cp.ReadOnly = false;
                }
            }
        }

        public void EditSNR(TStammdatenRowCollectionItem cr, ref string Value)
        {
            if (cr == null)
            {
                return;
            }

            cr.SNR = Utils.StrToIntDef(Value, cr.SNR);
            Value = Utils.IntToStr(cr.SNR);
            cr.Modified = true;
        }

        public void EditFN(TStammdatenRowCollectionItem cr, ref string Value)
        {
            if (cr == null)
            {
                return;
            }
#if Undo
            string oldValue = cr.FN;
#endif
            cr.FN = Value;
#if Undo
            if (oldValue != Value)
            {
                if (!UndoAgent.UndoLock)
                {
                    cr.Modified = true;
                    UndoAgent.UndoFlag = true;
                    UndoAgent.MsgTree.Division.Athlete(cr.SNR).FN(oldValue);
                    UndoAgent.MsgTree.Division.Athlete(cr.SNR).FN(Value);
                    UndoAgent.UndoLock = false;
                }
            }
#endif
        }

        public void EditLN(TStammdatenRowCollectionItem cr, ref string Value)
        {
            if (cr == null)
            {
                return;
            }
#if Undo
            string oldValue = cr.LN;
#endif
            cr.LN = Value;
#if Undo
            if (oldValue != Value)
            {
                if (!UndoAgent.UndoLock)
                {
                    cr.Modified = true;
                    UndoAgent.UndoFlag = true;
                    UndoAgent.MsgTree.Division.Athlete(cr.SNR).LN(oldValue);
                    UndoAgent.MsgTree.Division.Athlete(cr.SNR).LN(Value);
                    UndoAgent.UndoLock = false;
                }
            }
#endif
        }

        public void EditSN(TStammdatenRowCollectionItem cr, ref string Value)
        {
            if (cr == null)
            {
                return;
            }
#if Undo
            string oldValue = cr.SN;
#endif
            cr.SN = Value;
#if Undo
            if (oldValue != Value)
            {
                if (!UndoAgent.UndoLock)
                {
                    cr.Modified = true;
                    UndoAgent.UndoFlag = true;
                    UndoAgent.MsgTree.Division.Athlete(cr.SNR).SN(oldValue);
                    UndoAgent.MsgTree.Division.Athlete(cr.SNR).SN(Value);
                    UndoAgent.UndoLock = false;
                }
            }
#endif
        }

        public void EditNC(TStammdatenRowCollectionItem cr, ref string Value)
        {
            if (cr == null)
            {
                return;
            }
#if Undo
            string oldValue = cr.NC;
#endif
            cr.NC = Value;
#if Undo
            if (oldValue != Value)
            {
                if (!UndoAgent.UndoLock)
                {
                    cr.Modified = true;
                    UndoAgent.UndoFlag = true;
                    UndoAgent.MsgTree.Division.Athlete(cr.SNR).NC(oldValue);
                    UndoAgent.MsgTree.Division.Athlete(cr.SNR).NC(Value);
                    UndoAgent.UndoLock = false;
                }
            }
#endif
        }

        public void EditGR(TStammdatenRowCollectionItem cr, ref string Value)
        {
            if (cr == null)
            {
                return;
            }
#if Undo
            string oldValue = cr.GR;
#endif
            cr.GR = Value;
#if Undo
            if (oldValue != Value)
            {
                if (!UndoAgent.UndoLock)
                {
                    cr.Modified = true;
                    UndoAgent.UndoFlag = true;
                    UndoAgent.MsgTree.Division.Athlete(cr.SNR).GR(oldValue);
                    UndoAgent.MsgTree.Division.Athlete(cr.SNR).GR(Value);
                    UndoAgent.UndoLock = false;
                }
            }
#endif
        }

        public void EditPB(TStammdatenRowCollectionItem cr, ref string Value)
        {
            if (cr == null)
            {
                return;
            }
#if Undo
            string oldValue = cr.PB;
#endif
            cr.PB = Value;
#if Undo
            if (oldValue != Value)
            {
                if (!UndoAgent.UndoLock)
                {
                    cr.Modified = true;
                    UndoAgent.UndoFlag = true;
                    UndoAgent.MsgTree.Division.Athlete(cr.SNR).PB(oldValue);
                    UndoAgent.MsgTree.Division.Athlete(cr.SNR).PB(Value);
                    UndoAgent.UndoLock = false;
                }
            }
#endif
        }

        public void EditNameColumn(TStammdatenRowCollectionItem cr, ref string Value, string ColName)
        {
            if (cr == null)
            {
                return;
            }

            int i;
            try
            {
                i = int.Parse(ColName.Substring(5)); //'col_N'x
            }
            catch
            {
                i = -1;
            }
            if (i > -1)
            {
                cr[i] = Value;
                cr.Modified = true;
            }
        }

    }

}
