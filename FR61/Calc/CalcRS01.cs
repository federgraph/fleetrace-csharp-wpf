namespace RiggVar.FR
{

    public class TCalcEventProxy01 : TCalcEventProxy
    {        
        //in
        private int [] Bib = new int [1];
        private int [] DSQGate = new int [1];
        private int [] Status = new int [1];
        private int [] OTime = new int [1];

        //out
        private int [] Rank = new int [1];
        private int [] PosR = new int [1];
        private int [] PLZ = new int [1];
        private int [] BTime = new int [1]; //behind best at TimePoint
        //
        private int BestIndex;
        private int BestOTime;

        protected void Calc1()
        {
            //Calc_BestIdx;
            //Calc_BTime;
            EncodeDSQGateAndStatus();
            Calc_Rank_PosR_Encoded();
        }
        protected void Calc_BestIdx()
        {
            BestIndex = 0;
            BestOTime = int.MaxValue; //MaxInt;
            for (int i = 0; i < Count; i++)
            {
                int t = OTime[i];
                if ((t > 0) && (t < BestOTime) && IsOK(Status[i]))
                {
                    BestIndex = i;
                    BestOTime = OTime[i];
                }
            }
        }
        protected void Calc_BTime()
        {
            if (BestOTime == TimeConst.TimeNull)
            {
                for (int i = 0; i < Count; i++)
                {
                    BTime[i] = TimeConst.TimeNull;
                }
            }
            else
            {
                for (int i = 0; i < Count; i++)
                {
                    if (OTime[i] > 0)
                    {
                        BTime[i] =  OTime[i] - BestOTime;
                    }
                    else
                    {
                        BTime[i] = TimeConst.TimeNull;
                    }
                }
            }
        }
        protected void EncodeDSQGateAndStatus()
        {
            for (int i = 0; i < Count; i++)
            { 
                int temp = OTime[i];
                if (Status[i] == StatusConst.Status_DNF)
                {
                    temp = int.MaxValue - 300;
                }
                else if (Status[i] == StatusConst.Status_DSQ)
                {
                    temp = int.MaxValue - 200;
                }
                else if (Status[i] == StatusConst.Status_DNS)
                {
                    temp = int.MaxValue - 100;
                }
                //temp = temp - DSQGate[i];
                OTime[i] = temp;
            }
        }
        protected void Calc_Rank_PosR_Encoded()
        {  
            int t1, t2; //Zeit1 und Zeit2
            int BibMerker; //wegen 'Highest Bib goes first'
            int temp;

            //reset
            for (int j = 0; j < Count; j++)
            {
                Rank[j] = 1;
                PosR[j] = 1;
                PLZ[j] = -1;
            }

            //new calculation
            for (int j = 0; j < Count; j++)
            {
                t2 = OTime[j];
                BibMerker = Bib[j];
                //TimePresent = False
                if (t2 <= 0)
                {
                    Rank[j] = 0;
                    PosR[j] = 0;
                }
                    //TimePresent
                else
                {
                    for (int l = j + 1; l < Count; l++)
                    {
                        t1 = OTime[l];
                        if (t1 > 0)
                        {
                            if (t1 < t2)
                            {
                                //increment Rank and PosR for j
                                Rank[j] = Rank[j] + 1;
                                PosR[j] = PosR[j] + 1;
                            }

                            if (t1 > t2)
                            {
                                //increment Rank and PosR for l
                                Rank[l] = Rank[l] + 1;
                                PosR[l] = PosR[l] + 1;
                            }

                            if (t1 == t2)
                            {
                                //do not increment Rank if Times are equal
                                //increment PosR for one of the riders, j or l
                                if (HighestBibGoesFirst)
                                {
                                    if (BibMerker > Bib[l])
                                    {
                                        PosR[l] = PosR[l] + 1;
                                    }
                                    else
                                    {
                                        PosR[j] = PosR[j] + 1;
                                    }
                                }
                                else
                                {
                                    if (BibMerker < Bib[l])
                                    {
                                        PosR[l] = PosR[l] + 1;
                                    }
                                    else
                                    {
                                        PosR[j] = PosR[j] + 1;
                                    }
                                }
                            }
                        }
                    }
                    if (PosR[j] > 0)
                    {
                        temp = PosR[j];
                        PLZ[temp-1] = j;
                    }
                }  
            }
        }
        protected bool IsOut(int Value)
        {
            return ((Value == StatusConst.Status_DSQ) 
                || (Value == StatusConst.Status_DNF) 
                || (Value == StatusConst.Status_DNS));
        }
        protected bool IsOK(int Value)
        {
            return ((Value == StatusConst.Status_OK) 
                || (Value == StatusConst.Status_DSQPending));
        }
        public override void Calc(TEventNode qn)
        {
            int RaceCount;
            //
            TEventRowCollection cl;
            TEventRowCollectionItem cr;
            int GPoints;

            if (qn.Collection.Count < 1)
            {
                return;
            }

            RaceCount = qn.Collection[0].RCount;
            for (int i = 1; i < RaceCount; i++)
            {
                LoadProxy(qn, i);
                Calc1();
                UnLoadProxy(qn, i);
            }

            //Points
            cl = qn.Collection;
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                GPoints = 0;
                for (int j = 1; j < cr.RCount; j++)
                {
                    //RacePoints
                    cr.Race[j].CTime1 = cr.Race[j].Rank;

                    cr.Race[j].Drop = false;
                    if (cr.Race[j].QU != 0)
                    {
                        cr.Race[j].CTime1 = 400;
                        cr.Race[j].Drop = true;
                    }      
                    //      case cr.Race[j].QU of
                    //        Status_DNF: cr.Race[j].CTime := 100;
                    //        Status_DSQ: cr.Race[j].CTime := 200;
                    //        Status_DNS: cr.Race[j].CTime := 300;
                    //      end;      

                    //EventPoints
                    GPoints = GPoints + cr.Race[j].CTime1;
                }
                cr.GRace.CTime1 = GPoints;
            }

            LoadProxy(qn, 0); //channel_FT
            Calc1();
            UnLoadProxy(qn, 0); //channel_FT
        }
        public void LoadProxy(TEventNode qn, int channel)
        {
            TEventRowCollection cl = qn.Collection;
            Count = cl.Count;
            for (int i = 0; i < cl.Count; i++)
            {
                TEventRowCollectionItem cr = cl[i];
                Bib[i] = cr.Bib;
                DSQGate[i] = cr.Race[channel].DG;
                Status[i] = cr.Race[channel].QU;
                if (channel == 0) //channel_FT
                {
                    OTime[i] = cr.Race[channel].CTime1;
                }
                else
                {
                    OTime[i] = cr.Race[channel].OTime;
                }
            }
        }
        public void UnLoadProxy(TEventNode qn, int channel)
        {
            TEventRowCollection cl = qn.Collection;
            if (Count != cl.Count)
            {
                return;
            }

            for (int i = 0; i < cl.Count; i++)
            {
                TEventRowCollectionItem cr = cl[i];
                //cr.Race[channel].BTime = BTime[i];
                //cr.ru.BestTime[channel] = BestOTime;
                //cr.ru.BestIndex[channel] = BestIndex;
                cr.Race[channel].Rank = Rank[i];
                cr.Race[channel].PosR = PosR[i];
                cr.Race[channel].PLZ = PLZ[i];
            }
        }
        public int Count
        {
            get => Rank.Length;
            set
            {
                if ((value != Rank.Length) && (value >= 0))
                {
                    Bib = new int[value];
                    DSQGate = new int[value];
                    Status = new int[value];
                    OTime = new int[value];
                    BTime = new int[value];
                    Rank = new int[value];
                    PosR = new int[value];
                    PLZ = new int[value];
                }
            }
        }
    }

}
