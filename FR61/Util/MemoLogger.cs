using System.Text;

namespace RiggVar.FR
{
    public class MemoLogger
    {
        public int Count;
        private StringBuilder sb;
        public bool HasUpdate;
        public int MaxCount = 300;

        public MemoLogger()
        {
            sb = new StringBuilder();
        }

        public string Text
        {
            get => sb.ToString();
            set
            {
                Clear();
                sb.Append(value);
            }
        }

        public void Clear()
        {
            sb.Remove(0, sb.Length);
        }

        public void AppendLine(string s)
        {
            Count++;
            if (Count > MaxCount)
            {
                Clear();
            }

            sb.AppendLine(s);
            HasUpdate = true;
        }

        public void AppendEmptyLine()
        {
            AppendLine("");
        }

        public void Notify()
        {
        }

    }
}
