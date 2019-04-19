namespace RiggVar.FR
{
    public class TQProxy
    {

        //in
        public int [] Bib = new int [1];
        public int [] DSQGate = new int [1];
        public int [] Status = new int [1];
        public int [] OTime = new int [1];

        //out
        public int [] ORank = new int [1];
        public int [] Rank = new int [1];
        public int [] PosR = new int [1];
        public int [] PLZ = new int [1];
        public int [] TimeBehind = new int [1]; //behind best at TimePoint
        public int [] TimeBehindPreviousLeader = new int [1]; //behind previous best at TimePoint
        public int [] TimeBehindLeader = new int [1]; //behind previous best at Finish

        public int BestIndex;
        public int BestOTime;

        public virtual void Calc()
        {
        }

        public bool IsOut(int Value)
        {
            return (Value == StatusConst.Status_DSQ) 
                || (Value == StatusConst.Status_DNF) 
                || (Value == StatusConst.Status_DNS);
        }
        public bool IsOK(int Value)
        {
            return (Value == StatusConst.Status_OK) 
                || (Value == StatusConst.Status_DSQPending);
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
                    ORank = new int[value];
                    Rank = new int[value];
                    PosR = new int[value];
                    PLZ = new int[value];
                    TimeBehind = new int[value];
                    TimeBehindPreviousLeader = new int[value];
                    TimeBehindLeader = new int[value];
                }
            }
        }
        public bool HighestBibGoesFirst { get; set; }
    }

    public class TQProxy1 : TQProxy
    {
        private void Calc_ORank()
        {
            for (int j = 0; j < Count; j++)
            {
                ORank[j] = 1;
            }

            for (int j = 0; j < Count; j++)
            {
                int t2 = OTime[j];
                if (t2 <= 0)
                {
                    ORank[j] = 0;
                }
                else
                {
                    for (int l = j + 1; l < Count; l++)
                    {
                        int t1 = OTime[l];
                        if (t1 > 0)
                        {
                            if (t1 < t2)
                            {
                                ORank[j] = ORank[j] + 1;
                            }

                            if (t1 > t2)
                            {
                                ORank[l] = ORank[l] + 1;
                            }
                        }
                    }
                }
            }
        }
        private void Calc_BestIdx()
        {
            BestIndex = 0;
            BestOTime = int.MaxValue;
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
        private void Calc_TimeBehind()
        {
            if (BestOTime == TimeConst.TimeNull)
            {
                for (int i = 0; i < Count; i++)
                {
                    TimeBehind[i] = TimeConst.TimeNull;
                }
            }
            else
            {
                for (int i = 0; i < Count; i++)
                {
                    if (OTime[i] > 0)
                    {
                        TimeBehind[i] =  OTime[i] - BestOTime;
                    }
                    else
                    {
                        TimeBehind[i] = TimeConst.TimeNull;
                    }
                }
            }
        }
        private void EncodeDSQGateAndStatus()
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

                temp = temp - DSQGate[i];
                OTime[i] = temp;
            }
        }
        private void Calc_Rank_PosR_Encoded()
        {
            int t1, t2; //Zeit1 und Zeit2
            int BibMerker; //wegen 'Highest Bib goes first'

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
                    for(int l = j + 1; l < Count; l++)
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
                        int temp = PosR[j];
                        PLZ[temp-1] = j;
                    }
                }
            }
        }
        public override void Calc()
        {
            Calc_ORank();
            Calc_BestIdx();
            Calc_TimeBehind();
            EncodeDSQGateAndStatus();
            Calc_Rank_PosR_Encoded();
        }
    }

}
