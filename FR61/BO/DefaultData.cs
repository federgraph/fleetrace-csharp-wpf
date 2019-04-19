using System.Text;

namespace RiggVar.FR
{
    public class DefaultData
    {
        private StringBuilder sb = new StringBuilder();
        private readonly string crlf = "\r\n";

        public DefaultData()
        {
        }

        private void A(string s)
        {
            sb.Append(s);
            sb.Append(crlf);
        }

        public string FR()
        {
            sb = new StringBuilder();
            A("FR.420.Request.XML.ASPNET.Default");
            A("");
            A("#Params");
            A("");
            A("DP.StartlistCount = 8");
            A("DP.ITCount = 2");
            A("DP.RaceCount = 2");
            A("");
            A("#Event Properties");
            A("");
            A("EP.Name = Test-Regatta");
            A("EP.Dates = Event Dates");
            A("EP.HostClub = Segelverein Y");
            A("EP.PRO");
            A("EP.JuryHead");
            A("EP.ScoringSystem = Low Point System");
            A("EP.Throwouts = 1");
            A("EP.ThrowoutScheme = ByNumRaces");
            A("EP.DivisionName = 420");
            A("EP.InputMode = Strict");
            A("EP.Uniqua_Enabled = True");
            A("EP.Uniqua_Gesegelt = 2");
            A("EP.Uniqua_Gemeldet = 8");
            A("EP.Uniqua_Gezeitet = 8");
            A("EP.Uniqua_Faktor = 1.10");
            A("");
            A("#Athletes");
            A("");
            A("FR.420.SNR1000.SN=Boot Nr.8");
            A("FR.420.SNR1001.SN=Boot Nr.7");
            A("FR.420.SNR1002.SN=Boot Nr.6");
            A("FR.420.SNR1003.SN=Boot Nr.5");
            A("FR.420.SNR1004.SN=Boot Nr.4");
            A("FR.420.SNR1005.SN=Boot Nr.3");
            A("FR.420.SNR1006.SN=Boot Nr.2");
            A("FR.420.SNR1007.SN=Boot Nr.1");
            A("");
            A("FR.420.SNR1000.LN=Boot Nr.8");
            A("FR.420.SNR1001.LN=Boot Nr.7");
            A("FR.420.SNR1002.LN=Boot Nr.6");
            A("FR.420.SNR1003.LN=Boot Nr.5");
            A("FR.420.SNR1004.LN=Boot Nr.4");
            A("FR.420.SNR1005.LN=Boot Nr.3");
            A("FR.420.SNR1006.LN=Boot Nr.2");
            A("FR.420.SNR1007.LN=Boot Nr.1");
            A("");
            A("FR.420.SNR1000.NOC=GER");
            A("FR.420.SNR1001.NOC=GER");
            A("FR.420.SNR1002.NOC=USA");
            A("FR.420.SNR1003.NOC=FRA");
            A("FR.420.SNR1004.NOC=RUS");
            A("FR.420.SNR1005.NOC=GER");
            A("FR.420.SNR1006.NOC=GER");
            A("FR.420.SNR1007.NOC=SWE");
            A("");
            A("#Startlist");
            A("");
            A("#W1");
            A("");
            A("FR.420.W1.Bib1.Rank=2");
            A("FR.420.W1.Bib2.Rank=7");
            A("FR.420.W1.Bib3.Rank=5");
            A("FR.420.W1.Bib4.Rank=1");
            A("FR.420.W1.Bib5.Rank=6");
            A("FR.420.W1.Bib6.Rank=8");
            A("FR.420.W1.Bib7.Rank=4");
            A("FR.420.W1.Bib8.Rank=3");
            A("");
            A("#W2");
            A("");
            A("FR.420.W2.Bib1.Rank=4");
            A("FR.420.W2.Bib2.Rank=5");
            A("FR.420.W2.Bib3.Rank=1");
            A("FR.420.W2.Bib4.Rank=8");
            A("FR.420.W2.Bib5.Rank=6");
            A("FR.420.W2.Bib6.Rank=7");
            A("FR.420.W2.Bib7.Rank=3");
            A("FR.420.W2.Bib8.Rank=2");
            return sb.ToString();
        }

        public string SKK()
        {
            sb = new StringBuilder();
            A("DP.StartlistCount = 2");
            A("SKK.Kreise.W1.STL.Count=2");
            A("");
            A("SKK.Kreise.W1.Bib1.R1=50");
            A("SKK.Kreise.W1.Bib1.R2=50");
            A("SKK.Kreise.W1.Bib1.M1X=150");
            A("SKK.Kreise.W1.Bib1.M1Y=100");
            A("SKK.Kreise.W1.Bib1.M2X=150");
            A("SKK.Kreise.W1.Bib1.M2Y=200");
            A("");
            A("SKK.Kreise.W1.Bib2.R1=41");
            A("SKK.Kreise.W1.Bib2.R2=100");
            A("SKK.Kreise.W1.Bib2.M1Y=120");
            return sb.ToString();
        }
    }
}
