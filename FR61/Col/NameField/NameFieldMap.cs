namespace RiggVar.FR
{

    class NameFieldMap
    {
        private readonly int[] fieldIndex = new int[JS_NameFieldMax];

        //Singleton Instance
        private static readonly NameFieldMap instance;
        static NameFieldMap()
        {
            instance = new NameFieldMap();
        }

        public static NameFieldMap Instance()
        {
            return instance;
        }

        public const int JS_SkipperF = 1;
        public const int JS_SkipperL = 2;
        public const int JS_SkipperID = 3;
        public const int JS_Club = 4;
        public const int JS_CBYRA = 5;
        public const int JS_USSA = 6;
        public const int JS_Crew1F = 7;
        public const int JS_Crew1L = 8;
        public const int JS_Crew1ID = 9;
        public const int JS_Crew2F = 10;
        public const int JS_Crew2L = 11;
        public const int JS_Crew2ID = 12;

        public const int JS_NameFieldMax = 12;

        public string FieldName(int FieldID)
        {
            switch (FieldID)
            {
                case JS_SkipperF: return "Skipper First";
                case JS_SkipperL: return "Skipper Last";
                case JS_SkipperID: return "Skipper ID";

                case JS_Crew1F: return "Crew 1 First";
                case JS_Crew1L: return "Crew 1 Last";
                case JS_Crew1ID: return "Crew 1 ID";

                case JS_Crew2F: return "Crew 2 First";
                case JS_Crew2L: return "Crew 2 Last";
                case JS_Crew2ID: return "Crew 2 ID";

                case JS_Club: return "Club";
                case JS_CBYRA: return "CBYRA";
                case JS_USSA: return "USSA";
                default: return "";
            }
        }

        public string FieldValue(int FieldID, TStammdatenRowCollectionItem cr)
        {
            if (FieldID > 0 && FieldID <= JS_NameFieldMax && cr != null)
            {
                int m = fieldIndex[FieldID - 1];
                return cr[m];
            }
            return "";
        }

        public int this[int FieldID]
        {
            get => FieldID > 0 && FieldID <= JS_NameFieldMax ? fieldIndex[FieldID - 1] : 0;
            set
            {
                if (FieldID > 0 && FieldID <= JS_NameFieldMax)
                {
                    fieldIndex[FieldID - 1] = value;
                }
            }
        }

    }

}
