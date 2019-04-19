using System;

namespace RiggVar.FR
{
    public class TBOPersistent
    {
        public virtual void Assign(object source)
        {
            if (source is TBOPersistent)
            {
                (source as TBOPersistent).AssignTo(this);
            }
            else
            {
                AssignError(null);
            }
        }

        public virtual void AssignTo(object dest)
        {
            if (dest is TBOPersistent)
            {
                (dest as TBOPersistent).AssignError(this);
            }
        }

        public void AssignError(object source)
        {

            string SourceName;
            if (source != null)
            {
                SourceName = source.GetType().Name;
            }
            else
            {
                SourceName = "null";
            }

            string ClassName = GetType().Name;
            string s = "cannot assign " + SourceName + " to " + ClassName;
            throw new Exception(s);
        }
    }

}
