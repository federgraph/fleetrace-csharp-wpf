namespace RiggVar.FR
{
    public class LookupKatID
    {
        public const int Readme_en = 1;
        public const int Readme_de = 2;

        public const int SBPGS = 300;
        public const int PGSF = 301;
        public const int PGSQ = 302;
        public const int PGSE = 303;
        public const int PGSC = 304;
        public const int PGSV = 305;

        public const int FR = 400;
        public const int Rgg = 500;
        public const int SKK = 600;

        public const int Adapter = 700;

        public static string AsString(int KatID)
        {
            switch (KatID)
            {
                case Readme_en: return "Readme_en";
                case Readme_de: return "Readme_de";

                case PGSF: return "PGSF";
                case PGSQ: return "PGSQ";
                case PGSE: return "PGSE";
                case PGSC: return "PGSC";
                case PGSV: return "PGSV";

                case FR: return "FR";
                case Rgg: return "Rgg";
                case SKK: return "SKK";

                case Adapter: return "Adapter";

                default: break;
            }
            return "";
        }

        public static int AsInteger(string KatName)
        {
            switch (KatName)
            {
                case "Readme_en": return 1;
                case "Readme_de": return 2;

                case "PGSF": return 301;
                case "PGSQ": return 302;
                case "PGSE": return 303;
                case "PGSC": return 304;
                case "PGSV": return 305;

                case "FR": return 400;
                case "FleetRace": return 400;
                case "Rgg": return 500;
                case "SKK": return 600;

                case "Adapter": return 700;

                default: break;
            }
            return 0;
        }

    }
}
