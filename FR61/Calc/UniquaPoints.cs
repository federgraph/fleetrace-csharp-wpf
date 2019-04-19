namespace RiggVar.FR
{
    public class TUniquaPoints
    {
        public static void Calc(TEventNode qn)
        {
            TEventProps u;
            TEventRowCollection cl;
            TEventRowCollectionItem cr;
            TEventRowCollectionItem cr1;

            double f; // Faktor
            int s; // Zahl der mindestens einmal gezeiteten Boote
            int z; // Anzahl der Wettfahrten
            //int m; // Multiplikator
            double PL; // Punktzahl des fiktiven Letzten
            double P1; // Punktzahl des Gesamtersten
            double PX; // Punktzahl des Bootes
            //double RA; // RanglistenPunkte aus dieser Regatta (kann m mal eingehen)
            int Platz;
            //double QR; // Punkte aus dieser Regatta f�r WMA/EMA

            try
            {
                u = TMain.BO.EventProps;

                //Ranglistenpunkte
                f = u.Faktor; //Ranglistenfaktor der Regatta
                s = u.Gezeitet; //Zahl der mindestens einmal gezeiteten Boote
                if (s == 0)
                {
                    return;
                }

                z = u.Gesegelt; //Anzahl der Wettfahrten
                //m = 1; //Multiplikator

                //Punktzahl des fiktiven letzten
                if (u.ScoringSystem == TScoringSystem.LowPoint)
                {
                    PL = s * z; // Low Point System
                }
                else
                {
                    PL = (s + 6) * z; //Bonus System
                }

                cl = qn.Collection;

                //Punktzahl des ersten
                cr1 = cl[cl[0].PLZ];
                if (cr1 != null)
                {
                    P1 = cr1.GRace.CPoints; //schnelle Variante �ber Platzziffer
                }
                else
                {
                    P1 = 0;
                    for (int i = 0; i < cl.Count; i++)
                    {
                        cr = cl[i];
                        if (cr.GRace.CPoints < P1)
                        {
                            P1 = cr.GRace.CPoints;
                        }
                    }
                }

                for (int i = 0; i < cl.Count; i++)
                {
                    cr = cl[i];

                    //Ranglistenpunkte
                    PX = cr.GRace.CPoints; //Punktzahl des Bootes
                    if (PL - P1 != 0)
                    {
                        cr.RA = f * 100 * (PL - PX) / (PL - P1);
                    }
                    else
                    {
                        cr.RA = 0;
                    }
                    //begrenzen
                    if (cr.RA < 0)
                    {
                        cr.RA = 0;
                    }

                    //Punkte f�r WMA/EMA
                    Platz = cr.GRace.Rank;
                    if (s != 0)
                    {
                        cr.QR = 100 * (s + 1 - Platz) / s;
                    }
                    else
                    {
                        cr.QR = 0;
                    }
                    //begrenzen
                    if (cr.QR < 0)
                    {
                        cr.QR = 0;
                    }
                }
            }
            catch
            {
            }
        }
    }
}
