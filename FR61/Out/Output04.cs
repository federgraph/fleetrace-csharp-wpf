using System.Xml;
using System.IO;

namespace RiggVar.FR
{
    /// <summary>
    /// WebMotor Reports
    /// </summary>
    public class TOutput04 : TOutput00
    {

        public TOutput04() : base()
        {
        }

        public void ProxyXmlInput()
        {
            WantPageHeader = false;
            SL.Clear();
            TCalcEventProxy11 o = new TCalcEventProxy11();
            TextWriter tw = new System.IO.StringWriter();
            XmlTextWriter xw = new XmlTextWriter(tw)
            {
                Formatting = Formatting.Indented
            };
            o.GetProxyXmlInput(BO.EventNode, xw);
            SL.Text = tw.ToString();
        }

        public void ProxyXmlOutput()
        {
            TCalcEventProxy11 o;
            WantPageHeader = false;
            SL.Clear();
            TextWriter tw = new System.IO.StringWriter();
            XmlTextWriter xw = new XmlTextWriter(tw)
            {
                Formatting = Formatting.Indented
            };
            if (BO.CalcEV.Proxy is TCalcEventProxy11)
            {
                //may be TCalcEventProxy04
                o = (TCalcEventProxy11)BO.CalcEV.Proxy;
                o.GetProxyXmlOutput(BO.EventNode, xw); //virtual method
            }
            else
            {
                o = new TCalcEventProxy11();
                o.GetProxyXmlOutput(BO.EventNode, xw);
            }
            SL.Text = tw.ToString();

            //SL.Text = "<answer>not implemented</answer>";
        }

        public void BackupPreTXT()
        {
            SL.Clear();
            SL.Add("<pre>");
            BO.BackupToSL(SL);
            SL.Add("</pre>");
        }

        public void Params()
        {
            SL.Clear();
            WantPageHeader = false;
            SL.Add(string.Format("RaceCount={0}", BO.BOParams.RaceCount));
            SL.Add(string.Format("ITCount={0}", BO.BOParams.ITCount));
        }

        public void RaceXml(string Selector)
        {
            TRaceRowCollection cl;
            TRaceRowCollectionItem cr;
            int Race;
            int IT;
            TTimePoint tp;

            SL.Clear();
            try
            {
                TTokenParser TokenParser = new TTokenParser
                {
                    sRest = Selector
                };
                TokenParser.NextToken(); //RiggVar.
                TokenParser.NextToken(); //FR.
                Race = TokenParser.NextTokenX("Race");
                IT = TokenParser.NextTokenX("IT");

                if (Race < 1)
                {
                    Race = 1;
                }

                if (IT < 0)
                {
                    IT = 0;
                }

                if (Race > BO.BOParams.RaceCount)
                {
                    Race = BO.BOParams.RaceCount;
                }

                if (IT > BO.BOParams.ITCount)
                {
                    IT = BO.BOParams.ITCount;
                }

                cl = BO.RNode[Race].Collection;

                SL.Add(string.Format(@"<RaceXml Race=""{0}"" IT=""{1}"">", Race, IT));
                for (int i = 0; i < cl.Count; i++)
                {
                    cr = cl[i];
                    tp = cr[IT];
                    SL.Add(string.Format(@"<Entry Bib=""{0}"" SNR=""{1}"" PosR=""{2}"" OTime=""{3}"" Behind=""{4}""/>",
                        cr.Bib,
                        cr.SNR,
                        tp.PosR,
                        tp.OTime.ToString(),
                        tp.Behind.ToString()
                        ));
                }
                SL.Add("</RaceXml>");
            }
            catch
            {
                SL.Clear();
                SL.Add("<RaceXml>" + Selector + "</RaceXml>");
            }
        }

    }

}
