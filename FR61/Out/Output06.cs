namespace RiggVar.FR
{
    public class TOutput06
    {

        public void TimePointXML(TStrings ASL, string QueryString)
        {
            TTokenParser TokenParser;
            int Race;
            int IT;
            int Bib;
            TRaceNode rn;
            TRaceRowCollectionItem cr;
            TTimePoint tp;
            string t;

            //Rx.ITx.Bibx
            TokenParser = new TTokenParser
            {
                sRest = QueryString
            };
            Race = TokenParser.NextTokenX("R");
            IT = TokenParser.NextTokenX("IT");
            Bib = TokenParser.NextTokenX("Bib");
            t = "no time";
            if (TMain.BO != null && Race > 0 && Race < TMain.BO.BOParams.RaceCount)
            {
                rn = TMain.BO.RNode[Race];
                cr = rn.FindBib(Bib);
                if (cr != null)
                {
                    if (IT >= 0 && IT <= TMain.BO.BOParams.ITCount)
                    {
                        tp = cr[IT];
                        t = tp.BPL.ToString();
                    }
                }
            }
            ASL.Add(string.Format("<TW><b>{0}</b><t>{1}</t></TW>", Bib, t));
        }

        public void TimePointTable(TStrings ASL, string QueryString)
        {
            TTokenParser TokenParser;
            int Race;
            TRaceNode rn;
            TRaceRowCollection cl;
            TRaceRowCollectionItem cr;
            TRaceRowCollectionItem cr2;

            //QueryString == Rx
            TokenParser = new TTokenParser
            {
                sRest = QueryString
            };
            Race = TokenParser.NextTokenX("R");

            if (TMain.BO != null && Race > 0 && Race <= TMain.BO.BOParams.RaceCount)
            {
                rn = TMain.BO.RNode[Race];
                cl = rn.Collection;

                ASL.Add("<table border=\"1\" cellpadding=\"1\" cellspacing=\"1\">");

                //Header row
                ASL.Add("<tr>");
                ASL.Add("<th>Pos</th>");
                for (int j = 1; j <= TMain.BO.BOParams.ITCount; j++)
                {
                    AddHeaderCell(ASL, j);
                }
                AddHeaderCell(ASL, 0);
                ASL.Add("</tr>");

                //Body rows
                for (int i = 0; i < cl.Count; i++)
                {
                    cr = cl[i];
                    ASL.Add("<tr>");
                    ASL.Add(string.Format("<td>{0}</td>", i + 1));
                    for (int j = 1; j < cr.ITCount; j++) //cr.ITCount returns the Length of dynamic Array
                    {
                        cr2 = cl[cr[j].PLZ];
                        AddCell(ASL, cr2, j);
                    }
                    cr2 = cl[cr[0].PLZ];
                    AddCell(ASL, cr2, 0);
                    ASL.Add("</tr>");
                }

                ASL.Add("</table>");
            }
            else
            {
                ASL.Add("<p>TimePointReport: invalid race param</p>");
            }
        }

        public void AddCell(TStrings ASL, TRaceRowCollectionItem cr, int j)
        {
            TTimePoint tp;
            string t;

            if (cr != null)
            {
                tp = cr[j];
                if (tp != null)
                {
                    t = tp.Behind.ToString();
                    ASL.Add(string.Format("<td>Bib {0}<br/>{1}</td>", cr.Bib, t));
                }
                else
                {
                    ASL.Add("<td>-<br/>tp</td>");
                }
            }
            else
            {
                ASL.Add("<td>-<br/>-</td>");
            }
        }

        private void AddHeaderCell(TStrings ASL, int j)
        {
            string s;

            if (j == 0)
            {
                s = "FT";
            }
            else
            {
                s = "IT" + j.ToString();
            }

            ASL.Add(string.Format("<th>{0}</th>", s));
        }

    }

}
