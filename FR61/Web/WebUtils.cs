using System;

namespace RiggVar.FR
{

    public class WebUtils
    {

        public struct BoolStrStruct
        {
            public string this[bool b]
            {
                get
                {
                    switch (b)
                    {
                        case false: return "False";
                        case true: return "True";
                        default: return string.Empty;
                    }
                }
            }
        }

        public static BoolStrStruct BoolStr;

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

        public static string IntToStr(int i)
        {
            return i.ToString();
        }

        public static bool IsTrue(string Value)
        {
            if (Value == null)
            {
                return false;
            }

            bool result = false;
            string s = Value.ToUpper();
            if ((s == "TRUE") || (s == "T"))
            {
                result = true;
            }

            return result;
        }

        public static string StrParamDef(string s, string def)
        {
            if (s == null)
            {
                return def;
            }
            else
            {
                return s.Trim();
            }
        }

        public static string DS => DateTime.Now.ToLongTimeString();

    }

}
