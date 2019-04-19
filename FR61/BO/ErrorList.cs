using System.Collections.Generic;

namespace RiggVar.FR
{
    public class TOTimeErrorList
    {
        private List<TEventRowCollectionItem> FL;
        private TStringList FErrorList;
        private TStringList FSortedSL;
        private TBO BO;
        
        public TOTimeErrorList(TBO abo)
        {
            FL = new List<TEventRowCollectionItem>();
            BO = abo;
            FErrorList = new TStringList();
            FSortedSL = new TStringList();
            //FSortedSL.Sorted = true; //IndexOf does not work yet
        }

        private bool CheckOTime(TEventNode ev)
        {
            if (ev.UseFleets)
            {
                bool hasError = false;
                TEventRowCollection cl = ev.Collection;
                for (int r = 1; r < cl.RCount; r++)
                {
                    int fc = cl.FleetCount(r);
                    for (int f = 0; f <= fc; f++)
                    {
                        ev.Collection.FillFleetList(FL, r, f);
                        if (FL.Count > 0)
                        {
                            if (CheckOTimeForFleet(FL, r))
                            {
                                hasError = true;
                            }
                        }
                    }
                }
                FL.Clear();
                return hasError;
            }
            else
            {
                return CheckOTimeForAll(ev.Collection);
            }
        }

        private bool CheckOTimeForFleet(List<TEventRowCollectionItem> cl, int r)
        {
            TEventRowCollectionItem cr;
            TEventRaceEntry re;

            int oldErrorCount = FErrorList.Count;
            int EntryCount = cl.Count;
            int[] a = new int[EntryCount + 1];

            //clear array slots
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = 0;
            }

            for (int i = 0; i < cl.Count; i++)
            {
                cr = (TEventRowCollectionItem) cl[i];
                re = cr.Race[r];
                int temp = re.OTime;
                if (temp < 0)
                {
                    re.FinishErrors.Include((int)TFinishError.error_OutOfRange_OTime_Min);
                    AddFinishError(r, cr, TFinishError.error_OutOfRange_OTime_Min); //below lower limit
                }
                else if (temp > EntryCount)
                {
                    re.FinishErrors.Include((int)TFinishError.error_OutOfRange_OTime_Max);
                    AddFinishError(r, cr, TFinishError.error_OutOfRange_OTime_Max); //beyond upper limit
                }
                else if ((temp > 0) && (a[temp] == 1))
                {
                    re.FinishErrors.Include((int)TFinishError.error_Duplicate_OTime);
                    AddFinishError(r, cr, TFinishError.error_Duplicate_OTime); //has duplicates
                }
                else
                {
                    a[temp] = 1;
                }
            }

            bool HasNull = false;
            for (int position = 1; position <= EntryCount; position++)
            {
                if ((a[position] == 1) && HasNull)
                {
                    AddContiguousError(cl, r, position);
                    HasNull = false;
                }
                if (a[position] == 0)
                {
                    HasNull = true;
                }
            }

            return (FErrorList.Count > oldErrorCount);
        }

        private void AddContiguousError(TEventRowCollection cl, int r, int position)
        {
            TEventRowCollectionItem cr;
            TEventRaceEntry re;
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                re = cr.Race[r];
                if (re.OTime == position)
                {
                    re.FinishErrors.Include((int)TFinishError.error_Contiguous_OTime);
                    AddFinishError(r, cr, TFinishError.error_Contiguous_OTime);
                }
            }
        }
        private void AddContiguousError(List<TEventRowCollectionItem> cl, int r, int position)
        {
            TEventRowCollectionItem cr;
            TEventRaceEntry re;
            for (int i = 0; i < cl.Count; i++)
            {
                cr = (TEventRowCollectionItem) cl[i];
                re = cr.Race[r];
                if (re.OTime == position)
                {
                    re.FinishErrors.Include((int)TFinishError.error_Contiguous_OTime);
                    AddFinishError(r, cr, TFinishError.error_Contiguous_OTime);
                }
            }
        }

        private bool CheckOTimeForAll(TEventRowCollection cl)
        {
            TEventRowCollectionItem cr;
            TEventRaceEntry re;

            int oldErrorCount = FErrorList.Count;
            int EntryCount = cl.Count;
            int [] a = new int [EntryCount + 1]; //SetLength(a, EntryCount + 1);
            for (int r = 1; r < cl.RCount; r++)
            {
                //clear array slots
                for (int i = 0; i < a.Length; i++)
                {
                    a[i] = 0;
                }
                for (int i = 0; i < cl.Count; i++)
                {
                    cr = cl[i];
                    re = cr.Race[r];
                    int temp = re.OTime;
                    if (temp < 0)
                    {
                        re.FinishErrors.Include((int)TFinishError.error_OutOfRange_OTime_Min);
                        AddFinishError(r, cr, TFinishError.error_OutOfRange_OTime_Min); //below lower limit
                    }
                    else if (temp > EntryCount)
                    {
                        re.FinishErrors.Include((int)TFinishError.error_OutOfRange_OTime_Max);
                        AddFinishError(r, cr, TFinishError.error_OutOfRange_OTime_Max); //beyond upper limit
                    }
                    else if ((temp > 0) && (a[temp] == 1))
                    {
                        re.FinishErrors.Include((int)TFinishError.error_Duplicate_OTime);
                        AddFinishError(r, cr, TFinishError.error_Duplicate_OTime); //has duplicates
                    }
                    else
                    {
                        a[temp] = 1;
                    }
                }
                bool HasNull = false;
                for (int position = 1; position <= EntryCount; position++)
                {
                    if ((a[position] == 1) && HasNull)
                    {
                        AddContiguousError(cl, r, position);
                        HasNull = false;
                    }
                    if (a[position] == 0)
                    {
                        HasNull = true;
                    }
                }
            }
            return (FErrorList.Count > oldErrorCount);
        }
        private bool CheckBib(TEventNode ev)
        {
            //Bib must be unique, should be > 0
            bool result = true;
            FSortedSL.Clear();

            TEventRowCollection cl = ev.Collection;
            for (int i = 0; i < cl.Count; i++)
            {
                TEventRowCollectionItem cr = cl[i];
                string s = cr.Bib.ToString();
                //check for duplicates
                int foundIndex = FSortedSL.IndexOf(s);
                if (foundIndex > -1)
                {
                    cr.EntryErrors.Include((int)TEntryError.error_Duplicate_Bib);
                    AddEntryError(cr, TEntryError.error_Duplicate_Bib);
                    result = false;
                }
                else
                {
                    FSortedSL.Add(s);
                }
            }
            return result;
        }
        private void ClearFlags(TEventNode ev)
        {
            TEventRowCollection cl = ev.Collection;
            for (int i = 0; i < cl.Count; i++)
            {
                TEventRowCollectionItem cr = cl[i];
                cr.EntryErrors.Clear();
                for (int r = 1; r < cl.RCount; r++)
                {
                    cr.Race[r].FinishErrors.Clear();
                }
            }
        }
        private bool CheckSNR(TEventNode ev)
        {
            //SNR must be unique, must be > 0
            TEventRowCollection cl = ev.Collection;
            FSortedSL.Clear();
            for (int i = 0; i < cl.Count; i++)
            {
                TEventRowCollectionItem cr = cl[i];
                string s = cr.SNR.ToString();
                //check for duplicates
                if (FSortedSL.IndexOf(s) > -1)
                {
                    cr.EntryErrors.Include((int)TEntryError.error_Duplicate_SNR);
                    AddEntryError(cr, TEntryError.error_Duplicate_SNR);
                    return false;
                }
                else
                {
                    FSortedSL.Add(s);
                }
            }
            return true;
        }
        protected void AddEntryError(TEventRowCollectionItem cr, TEntryError e)
        {
            string s = "Error." + BO.cTokenSport;
            if (cr != null)
            {
                s = s + ".ID" + cr.BaseID.ToString();
            }

            s = s + " = " + EntryErrorStrings(e);
            FErrorList.Add(s);
        }
        protected void AddFinishError(int r, TEventRowCollectionItem cr, TFinishError e)
        {
            string s = "Error." + BO.cTokenSport + BO.cTokenRace + r.ToString();
            if (cr != null)
            {
                s = s + ".ID" + cr.BaseID.ToString();
            }

            s = s + " = " + FinishErrorStrings(e);
            FErrorList.Add(s);
        }
        public virtual bool IsPreconditionForStrictInputMode(TEventNode ev)
        {
            TEventRowCollection cl = ev.Collection;
            if (ev.UseFleets)
            {
                if (cl.Count < 2)
                {
                    return true;
                }

                for (int r = 1; r < cl.RCount; r++)
                {
                    int fc = cl.FleetCount(r);
                    for (int f = 0; f <= fc; f++)
                    {
                        cl.FillFleetList(FL, r, f);
                        if (FL.Count > 0)
                        {
                            if (! IsPreconditionForFleet(FL, r))
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            else
            {
                return IsPreconditionForAll(cl);
            }
        }
        private bool IsPreconditionForFleet(List<TEventRowCollectionItem> cl, int r)
        {
            int[] a; //: array of Integer;
            TEventRowCollectionItem cr;
            int EntryCount;
            int temp;
            bool HasNull;

            //exit at the first encouter of an error, only boolean result is important
            if (cl.Count < 2)
            {
                return false;
            }

            EntryCount = cl.Count;
            a = new int[EntryCount + 1]; //SetLength(a, EntryCount + 1);

            //clear array slots
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = 0;
            }
            for (int i = 0; i < cl.Count; i++)
            {
                cr = (TEventRowCollectionItem) cl[i];
                temp = cr.Race[r].OTime;
                if (temp < 0)
                {
                    return false; //below lower limit
                }

                if (temp > EntryCount)
                {
                    return false; //beyond upper limit
                }

                if ((temp > 0) && (a[temp] == 1))
                {
                    return false; //has duplicates
                }

                a[temp] = 1;
            }
            HasNull = false;
            for (int i = 1; i < cl.Count; i++)
            {
                if ((a[i] == 1) && HasNull)
                {
                    return false; //not contiguous
                }

                if (a[i] == 0)
                {
                    HasNull = true;
                }
            }
            return true;
        }
        private bool IsPreconditionForAll(TEventRowCollection cl)
        {
            int [] a; //: array of Integer;
            TEventRowCollectionItem cr;
            int EntryCount;
            int temp;
            bool HasNull;

            //exit at the first encouter of an error, only boolean result is important
            if (cl.Count < 2)
            {
                return false;
            }

            EntryCount = cl.Count;            
            a = new int [EntryCount + 1]; //SetLength(a, EntryCount + 1);
            for (int r = 1; r < cl.RCount; r++)
            {
                //clear array slots
                for (int i = 0; i < a.Length; i++)
                {
                    a[i] = 0;
                }
                for (int i = 0; i < cl.Count; i++)
                {
                    cr = cl[i]; //cl.Items[i];
                    temp = cr.Race[r].OTime;
                    if (temp < 0)
                    {
                        return false; //below lower limit
                    }

                    if (temp > EntryCount)
                    {
                        return false; //beyond upper limit
                    }

                    if ((temp > 0) && (a[temp] == 1))
                    {
                        return false; //has duplicates
                    }

                    a[temp] = 1;
                }
                HasNull = false;
                for (int i = 1; i < cl.Count; i++)
                {
                    if ((a[i] == 1) && HasNull)
                    {
                        return false; //not contiguous
                    }

                    if (a[i] == 0)
                    {
                        HasNull = true;
                    }
                }
            }
            return true;
        }
        public virtual bool CheckAll(TEventNode ev)
        {
            FErrorList.Clear();
            ClearFlags(ev);
              CheckOTime(ev);
            CheckBib(ev);
            CheckSNR(ev);
            return FErrorList.Count > 0;
        }
        public virtual void GetMsg(TStrings Memo)
        {
            for (int i = 0; i < FErrorList.Count; i++)
            {
                Memo.Add(FErrorList[i]);
            }
        }
        public virtual bool HasErrors()
        {
            return FErrorList.Count > 0;
        }
        public string EntryErrorStrings(TEntryError e)
        {
            switch (e)
            {
                case TEntryError.error_Duplicate_Bib: return "duplicate SNR";
                case TEntryError.error_Duplicate_SNR: return "duplicate Bib";
                case TEntryError.error_OutOfRange_Bib: return "Bib out of range";
                case TEntryError.error_OutOfRange_SNR: return "Bib out of range";
            }
            return string.Empty;
        }
        public string FinishErrorStrings(TFinishError e)
        {
            switch (e)
            {
                case TFinishError.error_Duplicate_OTime: return "duplicate OTime";
                case TFinishError.error_OutOfRange_OTime_Max: return "OTime beyond EntryCount";
                case TFinishError.error_OutOfRange_OTime_Min: return "OTime below zero";
                case TFinishError.error_Contiguous_OTime: return "OTime gap";
            }
            return string.Empty;
        }
    }
}
