using System;
using System.Globalization;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace RiggVar.FR
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

        public static bool Odd(int i)
        {
            return (Math.Abs(i % 2) == 1);
        }

        public static bool Even(int i)
        {
            return (i % 2 == 0);
        }

        public static double StrToFloatDef(string s, double def)
        {
            try
            {
                return double.Parse(s.Replace(',', '.'), NumberFormatInfo.InvariantInfo);
            }
            catch 
            {
                return def;
            }
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
                    if (!int.TryParse(s, out i))
                    {
                        i = def;
                    }
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

        public static string CopyRest(string s, int startpos)
        {
            return Copy(s, startpos, s.Length);
        }

        public static string Copy(string s, int startpos, int len)
        {
            if (len > s.Length - startpos)
            {
                len = s.Length - startpos + 1;
            }

            try
            {
                if ((startpos < 1) || (startpos > s.Length))
                {
                    return string.Empty;
                }
                else
                {
                    return s.Substring(startpos-1, len);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                return string.Empty;
            }
        }

        public static int Pos(string subs, string s)
        {
            return s.IndexOf(subs) + 1;
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

        public static string Cut(string delim, string s, ref string token)
        {
            // Trennt einen String beim ersten Auftreten von delim
            // parameter delim : das Trennzeichen, erstes Auftreten = Trennposition
            // parameter s : input
            // parameter token : output, vorn abgeschnitten
            // returns rest of inputstring
            string rest;
            int posi = Pos(delim, s);
            if (posi > 0)
            {
                token = Copy(s, 1, posi-1).Trim();
                rest = Copy(s, posi+1, s.Length).Trim();
            }
            else
            {
                token = s;
                rest = "";
            }
            return rest;
        }

        public static string SwapLineFeed(string s)
        {
            if (s.Length > 0)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(s);
                sb = sb.Replace("\r\n", "\n");
                sb = sb.Replace("\r", "\n");
                sb = sb.Replace("\n\r", "\n");
                sb = sb.Replace("\n", Environment.NewLine);
                return sb.ToString();
            }
            return s;
        }

        public static int EnumInt(object o)
        {
            if (o is Enum)
            {
                return Convert.ToInt32(o);
            }
            else
            {
                return -1;
            }
        }

        public static IEnumerable<object> GetEnumValues(Type enumType)
        {
            foreach (FieldInfo fi in enumType.GetFields().Where(f => f.IsLiteral))
            {
                yield return fi.GetValue(null);
            }
         }

    }

}
