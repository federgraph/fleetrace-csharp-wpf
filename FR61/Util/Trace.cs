using System;

namespace RiggVar.FR
{
    public class FRTrace
    {
        public static bool WriteToConsole = false;

        public FRTrace()
        {
        }
        public static void Trace(string s)
        {
            if (WriteToConsole)
            {
                Console.WriteLine(s);
            }
        }
        public static void Trace(string s, int i)
        {
            Trace(s);
        }
        public static void Trace(string s, string a)
        {
            Trace(s);
        }
    }
}
