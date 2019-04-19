using System;
using System.Collections.Generic;

namespace RiggVar.Scoring
{

    public struct BoolStrStruct
    {
        public string this [bool b]
        {
            get 
            {
                switch (b)
                {
                    case false: return "False";    
                    case true: return "True";
                }
                return string.Empty;
            }
        }
    }

    public class Utils
    {
        public static BoolStrStruct BoolStr;

        public Utils()
        {
        }    

        public static bool IsTrue(string Value)
        {
            bool result = false;
            string s = Value.ToUpper();
            if ((s == "TRUE") || (s == "T"))
            {
                result = true;
            }

            return result;
        }

        public static int StrToIntDef(string s, int def)
        {
            int i;
            try
            {
                if (s == null)
                {
                    i = def;
                }
                else if (s == string.Empty)
                {
                    i = def;
                }
                else
                {
                    i = int.Parse(s);
                }
            }
            catch 
            {
                i = def;
            }
            return i;
        }
    }

}
