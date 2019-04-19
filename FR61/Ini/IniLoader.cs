using System;

namespace RiggVar.FR
{
    public interface IIniLoader
    {
        void LoadSettings(TBaseIniImage ini);
        void SaveSettings(TBaseIniImage ini);
    }

    public class IniLoaderException : ApplicationException 
    {
        public IniLoaderException(string msg) : base(msg)
        {
        }
        public IniLoaderException(string msg, Exception ex) : base(msg, ex)
        {
        }
    }

    public class IniLoaderMock : IIniLoader
    {
        #region IniLoader Member

        public void LoadSettings(TBaseIniImage ini)
        {
        }

        public void SaveSettings(TBaseIniImage ini)
        {
        }

        #endregion

    }

}
