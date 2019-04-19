using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Diagnostics;

namespace RiggVar.FR
{

    public class TStringListError : Exception {
        public TStringListError(string AMessage) : base(AMessage) {
        }
    }
    
    [Flags]
    enum TStringsDefined {
        Delimiter = 0x1,
        QuoteChar = 0x2,
        NameValueSeparator = 0x4
    }
    
    public class TStringsEnumerator : object, IEnumerator {
        private int FIndex = -1;
        private TStrings FStrings;
        
        internal TStringsEnumerator(TStrings AStrings) {
            FStrings = AStrings;
        }

        public object Current => FStrings[FIndex];

        public bool MoveNext() {
            if (FIndex < FStrings.Count)
            {
                ++FIndex;
            }
            return FIndex < FStrings.Count;
        }
        
        public void Reset() {
            FIndex = -1;
        }
    }

    public abstract class TStrings : object, ICloneable, IEnumerable, ICollection {
        private TStringsDefined FDefined;
        private char FDelimiter = ',';
        private char FQuoteChar = '"';
        private char FNameValueSeparator = '=';
        internal int FUpdateCount = 0;
        
        private string GetCommaText() {
            TStringsDefined oldDefined = FDefined;
            char oldDelim = FDelimiter;
            char oldQuote = FQuoteChar;
            FDelimiter = ',';
            try {
                return GetDelimitedText();
            } finally {
                FDelimiter = oldDelim;
                FQuoteChar = oldQuote;
                FDefined = oldDefined;
            }
        }
        
        static private string AnsiQuotedStr(string AValue, string AQuote) {
            if (AValue.StartsWith(AQuote)) {
                if (AValue.EndsWith(AQuote))
                {
                    return AValue;
                }
                else
                {
                    return AValue + AQuote;
                }
            } else {
                if (AValue.EndsWith(AQuote))
                {
                    return AQuote + AValue;
                }
                else
                {
                    return AQuote + AValue + AQuote;
                }
            }
        }
        
        private string GetDelimitedText() {
            int count = GetCount();
            string result;
            if (count  == 0)
            {
                return "";
            }
            else if (count == 1 && Get(0) == string.Empty)
            {
                result = new string(QuoteChar, 1);
            }
            else {
                result = "";
                for (int i = 0; i < Count; ++i) {
                    string s = Get(i);
                    if (s == null)
                    {
                        s = string.Empty;
                    }

                    int p = 0;
                    while (p < s.Length && s[p] > ' ' && s[p] != QuoteChar && s[p] != Delimiter)
                    {
                        ++p;
                    }

                    if (p < s.Length && s[p] != '\0')
                    {
                        s = AnsiQuotedStr(s, "" + QuoteChar);
                    }

                    result = result + s + Delimiter;
                }
            }
            return result.Substring(0, result.Length - 1);
        }
        
        private string GetName(int Index) {
            return ExtractName(Get(Index));
        }
        
        private string GetValue(string AName) {
            int i = IndexOfName(AName);
            if (i >= 0)
            {
                return Get(i).Substring(AName.Length + 1);
            }
            else
            {
                return string.Empty;
            }
        }
        
        private void ReadData(TextReader AReader) {
            BeginUpdate();
            try {
                Clear();
                string s;
                while ((s = AReader.ReadLine()) != null)
                {
                    Add(s);
                }
            } finally {
                EndUpdate();
            }
        }
        
        private void SetCommaText(string AValue) {
            FDelimiter = ',';
            SetDelimitedText(AValue);
        }
        
        private static string AnsiExtractQuotedStr(string AValue, char AQuote) {
            int i = AValue.IndexOf(AQuote);
            return i != -1 ? AValue.Substring(0, i) : AValue;
        }
        
        private void SetDelimitedText(string AValue)
        {
            BeginUpdate();
            try
            {
                string s;
                char c;
                bool skip;

                Clear();
                int p = 0;

                //skip over leading spaces (whitespace)
                while (p < AValue.Length)
                {
                    c = AValue[p];
                    skip = (c > '\0' && c <= ' ');
                    //test = (char.IsControl(c) || char.IsWhiteSpace);
                    if (!skip)
                    {
                        break;
                    }

                    p++;
                }

                while (p < AValue.Length)
                {
                    //consume current token
                    c = AValue[p];
                    if (c == QuoteChar && p + 1 < AValue.Length)
                    {
                        //special case: get quoted token
                        s = AnsiExtractQuotedStr(AValue.Substring(p + 1), QuoteChar);
                        p = p + s.Length + 2;
                    }
                    else
                    {
                        //normal case: get unquoted token
                        int p1 = p;
                        while (p < AValue.Length && AValue[p] != Delimiter) //do not break at Space
                        {
                            ++p;
                        }

                        s = AValue.Substring(p1, p - p1);
                    }
                    Add(s);

                    //skip over 'additional' whitespace, quote chars
                    while (p < AValue.Length)
                    {
                        c = AValue[p];
                        skip = c == QuoteChar || (c > '\0' && c <= ' ');
                        if (!skip)
                        {
                            break;
                        }

                        ++p;
                    }

                    //expected: delimiter found at current position
                    if (p < AValue.Length && AValue[p] == Delimiter)
                    {
                        //add one empty line if delimiter is the last char in string
                        if (p + 1 >= AValue.Length)
                        {
                            Add(string.Empty);
                        }

                        //increment p at least once (step past delimiter)
                        p++;

                        //skip over whitespace until start of next 'token'
                        while (p < AValue.Length)
                        {
                            skip = (c > '\0' && c <= ' ');
                            //test = (char.IsControl(c) || char.IsWhiteSpace);
                            if (!skip)
                            {
                                break;
                            }

                            p++;
                        }
                    }
                }
            }
            finally
            {
                EndUpdate();
            }
        }

        private void SetValue(string AName, string AValue) {
            int i = IndexOfName(AName);
            if (AValue != string.Empty) {
                if (i < 0)
                {
                    i = Add(string.Empty);
                }

                Put(i, AName + NameValueSeparator + AValue);
            } else 
                if (i >= 0)
            {
                Delete(i);
            }
        }
        
        private void WriteData(TextWriter AWriter) {
            for (int i = 0; i < Count; ++i)
            {
                AWriter.WriteLine(Get(i));
            }
        }
        
        private char GetDelimiter() {
            if ((FDefined & TStringsDefined.Delimiter) == 0)
            {
                Delimiter = ',';
            }

            return FDelimiter;
        }
        
        private void SetDelimiter(char AValue) {
            if (FDelimiter != AValue || (FDefined & TStringsDefined.Delimiter) == 0) {
                FDefined |= TStringsDefined.Delimiter;
                FDelimiter = AValue;
            }
        }
        
        private char GetQuoteChar() {
            if ((FDefined & TStringsDefined.QuoteChar) == 0)
            {
                QuoteChar = '"';
            }

            return FQuoteChar;
        }
        
        private void SetQuoteChar(char AValue) {
            if (FQuoteChar != AValue || (FDefined & TStringsDefined.QuoteChar) == 0) {
                FDefined |= TStringsDefined.QuoteChar;
                FQuoteChar = AValue;
            }
        }
        
        private char GetNameValueSeparator() {
            if ((FDefined & TStringsDefined.NameValueSeparator) == 0)
            {
                NameValueSeparator = '=';
            }

            return FNameValueSeparator;
        }
        
        private void SetNameValueSeparator(char AValue) {
            if (FQuoteChar != AValue || (FDefined & TStringsDefined.NameValueSeparator) == 0) {
                FDefined |= TStringsDefined.NameValueSeparator;
                FNameValueSeparator = AValue;
            }
        }
        
        private string GetValueFromIndex(int AIndex) 
        {
            if ((AIndex >= 0) && (AIndex < Count))
            {
                string sAll = Get(AIndex);
                string sName = Names(AIndex);
                if (sAll.Length - sName.Length > 1)
                {
                    return sAll.Substring(sName.Length + 1);
                }
            }            
            return string.Empty;
        }
        
        private void SetValueFromIndex(int AIndex, string AValue) {
            if (AValue != string.Empty) {
                if (AIndex < 0)
                {
                    AIndex = Add(string.Empty);
                }

                Put(AIndex, Names(AIndex) + NameValueSeparator + AValue);
            } else
                if (AIndex >= 0)
            {
                Delete(AIndex);
            }
        }
    
        protected void Error(string Msg, int Data) {
            throw new TStringListError(string.Format(Msg, Data));
        }
        
        protected string ExtractName(string s) {
            int p = s.IndexOf(NameValueSeparator);
            return p > 0 ? s.Substring(0, p) : "";
        }
        
        protected abstract string Get(int Index);

        protected virtual int GetCapacity() {
            return Count;
        }
        
        protected abstract int GetCount();
        
        protected virtual object GetObject(int Index) {
            return null;
        }
        
        [DebuggerStepThrough]
        protected virtual string GetTextStr() {
            string NL = Environment.NewLine;
            StringBuilder sb = new StringBuilder();
            int count = GetCount();
            
            for (int i = 0; i < count; ++i)
            {
                sb.Append(Get(i) + NL);
            }

            return sb.ToString();
        }
        
        protected virtual void Put(int AIndex, string AValue) {
            object dummy = GetObject(AIndex);
            Delete(AIndex);
            InsertObject(AIndex, AValue, dummy);
        }
        
        protected virtual void PutObject(int AIndex, object AObject) {
        }
        
        protected virtual void SetCapacity(int ACapacity) {
        }
        
        protected virtual void SetTextStr(string AText) {
            BeginUpdate();
            try {
                Clear();
                if (AText != null)
                {
                    string NL = Environment.NewLine;
                    for (int i = 0; i != -1 && i < AText.Length;) 
                    {
                        int oi = i;
                        i = AText.IndexOf(NL, i, StringComparison.OrdinalIgnoreCase);
                        if (i != -1)
                        {
                            Add(AText.Substring(oi, i - oi));
                        }
                        else 
                        {
                            Add(AText.Substring(oi));
                            break;
                        }
                        i += NL.Length;                    
                    }
                }
            } finally {
                EndUpdate();
            }
        }
        
        protected virtual void SetUpdateState(bool AUpdating) {
        }
        
        protected int UpdateCount {
            get { return FUpdateCount; }
        }
        
        protected virtual int CompareStrings(string s1, string s2) {
            return s1.CompareTo(s2);
        }
        
        // ICloneable
        public abstract object Clone();
        
        // IEnumerable
        public IEnumerator GetEnumerator() {
            return new TStringsEnumerator(this);
        }
        
        // ICollection
        public bool IsSynchronized {
            get { return false; }
        }
        
        public object SyncRoot {
            get { return this; }
        }
        
        public abstract void CopyTo(Array a, int i);
        
        public override string ToString() {
            return Text;
        }
        
        public virtual int Add(string s) {
            int result = GetCount();
            Insert(result, s);
            return result;
        }
        public virtual int AddObject(string s, object AObject) {
            int result = Add(s);
            PutObject(result, AObject);
            return result;
        }
        
        public void Append(string s) {
            Add(s);
        }
        
        public virtual void AddStrings(TStrings AStrings) {
            BeginUpdate();
            try {
                for (int i = 0; i < AStrings.Count; ++i)
                {
                    AddObject(AStrings[i], AStrings.Objects(i));
                }
            } finally {
                EndUpdate();
            }
        }
        
        public virtual void Assign(TStrings Source) {
            BeginUpdate();
            try {
                Clear();
                FDefined = Source.FDefined;
                FNameValueSeparator = Source.FNameValueSeparator;
                FQuoteChar = Source.FQuoteChar;
                FDelimiter = Source.FDelimiter;
                AddStrings(Source);
            } finally {
                EndUpdate();
            }
        }
        
        public void BeginUpdate() {
            if (FUpdateCount == 0)
            {
                SetUpdateState(true);
            }

            ++FUpdateCount;
        }
        
        public abstract void Clear();
        public abstract void Delete(int Index);
        
        public void EndUpdate() {
            --FUpdateCount;
            if (FUpdateCount == 0)
            {
                SetUpdateState(false);
            }
        }
        
        public override bool Equals(object o) {
            return o is TStrings ? Equals(o as TStrings) : false;
        }
        
        public bool Equals(TStrings AStrings) {
            if (Object.Equals(AStrings, null))
            {
                return false;
            }

            int count = AStrings.GetCount();
            if (count != AStrings.GetCount())
            {
                return false;
            }

            for (int i = 0; i < count; ++i)
            {
                if (Get(i) != AStrings.Get(i))
                {
                    return false;
                }
            }

            return true;
        }
        
        public override int GetHashCode() {
            return base.GetHashCode();
        }
        
        public static bool operator == (TStrings S1, TStrings S2) {
            if (Object.Equals(S1, null))
            {
                return false;
            }
            else
            {
                return S1.Equals(S2);
            }
        }
        
        public static bool operator !=(TStrings S1, TStrings S2) {
            if (Object.Equals(S1, null))
            {
                return false;
            }
            else
            {
                return !S1.Equals(S2);
            }
        }
        
        public virtual void Exchange(int AIndex1, int AIndex2) {
            BeginUpdate();
            try {
                string dummystr = Strings(AIndex1);
                object dummyobj = Objects(AIndex1);
                Strings(AIndex1, Strings(AIndex2));
                Objects(AIndex1, Objects(AIndex2));
                Strings(AIndex2, dummystr);
                Objects(AIndex2, dummyobj);
            } finally {
                EndUpdate();
            }
        }
        
        public virtual string GetText() {
            return GetTextStr();
        }
        
        public virtual int IndexOf(string AValue) {
            for (int i = 0; i < Count; ++i)
            {
                if (CompareStrings(AValue, Get(i)) == 0)
                {
                    return i;
                }
            }

            return -1;
        }
        
        public virtual int IndexOfName(string AName) {
            for (int i = 0; i < Count; ++i) {
                string S = Get(i);
                int P = S.IndexOf(NameValueSeparator);
                if (P > 0 && S.Substring(0, P) == AName)
                {
                    return i;
                }
            }
            
            return -1;
        }
        
        public virtual int IndexOfObject(object AObject) {
            for (int i = 0; i < Count; ++i)
            {
                if (GetObject(i) == AObject)
                {
                    return i;
                }
            }

            return -1;
        }

        public abstract void Insert(int AIndex, string AValue);
        
        public virtual void InsertObject(int AIndex, string AValue, object AObject) {
            Insert(AIndex, AValue);
            PutObject(AIndex, AObject);
        }
        
        public virtual void LoadFromFile(string AFileName) {
            using (Stream s = new FileStream(AFileName, FileMode.Open, 
                FileAccess.Read, FileShare.Read))
            {
                LoadFromStream(s);
            }
        }
        
        public virtual void LoadFromStream(System.IO.Stream AStream) {
            BeginUpdate();
            try {
                using (StreamReader sr = new StreamReader(AStream, Encoding.UTF8))
                {
                    string s = sr.ReadToEnd();
                    SetTextStr(s);
                }
            } finally {
                EndUpdate();
            }
        }

        public virtual void Move(int CurIndex, int NewIndex) {
            if (CurIndex != NewIndex) {
                BeginUpdate();
                try {
                    string dummystr = Get(CurIndex);
                    object dummyobj = GetObject(CurIndex);
                    Delete(CurIndex);
                    InsertObject(NewIndex, dummystr, dummyobj);
                } finally {
                    EndUpdate();
                }
            }
        }
        
        public virtual void SaveToFile(string AFileName) {
            using (Stream s = new FileStream(AFileName, FileMode.Create,
                FileAccess.Write, FileShare.None))
            {
                SaveToStream(s);
            }
        }
        
        public virtual void SaveToStream(Stream AStream) {
            using (StreamWriter sw = new StreamWriter(AStream, Encoding.UTF8))
            {
                sw.Write(GetTextStr());
            }
        }
        
        public virtual void SetText(string AText) {
            SetTextStr(AText);
        }
        
        public int Capacity {
            get => GetCapacity();
            set => SetCapacity(value);
        }

        public string CommaText {
            get => GetCommaText();
            set => SetCommaText(value);
        }

        public int Count {
            get { return GetCount(); }
        }
        
        public char Delimiter {
            get => GetDelimiter();
            set => SetDelimiter(value);
        }

        public string DelimitedText {
            get => GetDelimitedText();
            set => SetDelimitedText(value);
        }

        public string Names(int AIndex) {
            return GetName(AIndex);
        }
        
        public object Objects(int AIndex) {
            return GetObject(AIndex);
        }
        
        public void Objects(int AIndex, object AValue) {
            PutObject(AIndex, AValue);
        }
        
        public char QuoteChar {
            get => GetQuoteChar();
            set => SetQuoteChar(value);
        }

        public string Values(string AName) {
            return GetValue(AName);
        }
        
        public void Values(string AName, string AValue) {
            SetValue(AName, AValue);
        }
        
        public string ValueFromIndex(int AIndex) {
            return GetValueFromIndex(AIndex);
        }
        
        public void ValueFromIndex(int AIndex, string AValue) {
            SetValueFromIndex(AIndex, AValue);
        }
        
        public char NameValueSeparator {
            get => GetNameValueSeparator();
            set => SetNameValueSeparator(value);
        }

        public string Strings(int AIndex) {
            return Get(AIndex);
        }
        
        public void Strings(int AIndex, string AValue) {
            Put(AIndex, AValue);
        }
        
        public string this[int AIndex] {
            get => Get(AIndex);
            set => Put(AIndex, value);
        }

        public string Text {
            [DebuggerStepThrough]
            get => GetTextStr();
            [DebuggerStepThrough]
            set => SetTextStr(value);
        }
    }
}
