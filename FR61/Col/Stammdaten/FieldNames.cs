namespace RiggVar.FR
{

    public class FieldNames
    {
        //NameSchema_N0
        public const string N0_FN = "FN";
        public const string N0_LN = "LN";
        public const string N0_SN = "SN";
        public const string N0_NC = "NC";
        public const string N0_GR = "GR";
        public const string N0_PB = "PB";

        //NameSchema_N1
        public const string N1_FN = "FirstName";
        public const string N1_LN = "LastName";
        public const string N1_SN = "ShortName";
        public const string N1_NC = "NOC";
        public const string N1_GR = "Gender";
        public const string N1_PB = "PersonalBest";

        //NameSchema_N2
        public const string N2_FN = "N1";
        public const string N2_LN = "N2";
        public const string N2_SN = "N3";
        public const string N2_NC = "N4";
        public const string N2_GR = "N5";
        public const string N2_PB = "N6";

        //actual mapping
        public static string FN = N0_FN;
        public static string LN = N0_LN;
        public static string SN = N0_SN;
        public static string NC = N0_NC;
        public static string GR = N0_GR;
        public static string PB = N0_PB;

        public static int nameSchema = 0;

        static FieldNames()
        {
            SchemaCode = 0;
        }

        public static int SchemaCode
        {
            get => nameSchema;
            set
            {
                switch (value)
                {
                    case 0:
                        nameSchema = 0;
                        FN = N0_FN;
                        LN = N0_LN;
                        SN = N0_SN;
                        NC = N0_NC;
                        GR = N0_GR;
                        PB = N0_PB;
                        break;
                    case 1:
                        nameSchema = 1;
                        FN = N1_FN;
                        LN = N1_LN;
                        SN = N1_SN;
                        NC = N1_NC;
                        GR = N1_GR;
                        PB = N1_PB;
                        break;
                    case 2:
                        nameSchema = 2;
                        FN = N2_FN;
                        LN = N2_LN;
                        SN = N2_SN;
                        NC = N2_NC;
                        GR = N2_GR;
                        PB = N2_PB;
                        break;
                    default:
                        break;
                }
            }
        }

        public static string GetStandardFieldCaption(int index, int NameSchema)
        {
            string result = "";

            if (index == 0)
            {
                result = "SNR";
            }

            switch (NameSchema)
            {
                case 0:
                    switch (index)
                    {
                        case 1: result = N0_FN; break;
                        case 2: result = N0_LN; break;
                        case 3: result = N0_SN; break;
                        case 4: result = N0_NC; break;
                        case 5: result = N0_GR; break;
                        case 6: result = N0_PB; break;
                        default:
                            break;
                    }
                    break;

                case 1:
                    switch (index)
                    {
                        case 1: result = N1_FN; break;
                        case 2: result = N1_LN; break;
                        case 3: result = N1_SN; break;
                        case 4: result = N1_NC; break;
                        case 5: result = N1_GR; break;
                        case 6: result = N1_PB; break;
                        default:
                            break;
                    }
                    break;

                case 2:
                    switch (index)
                    {
                        case 1: result = N2_FN; break;
                        case 2: result = N2_LN; break;
                        case 3: result = N2_SN; break;
                        case 4: result = N2_NC; break;
                        case 5: result = N2_GR; break;
                        case 6: result = N2_PB; break;
                        default:
                            break;
                    }
                    break;
            }

            if (result == "")
            {
                result = "N" + index.ToString();
            }

            return result;
        }

    }

}
