using System;

namespace RiggVar.FR
{
    /// <summary>
    /// usage: call DoOnIdle() often,
    /// this class will limit the frequency of calls to the attached OnUpdateView EventHandler.
    /// </summary>
    public class TGridUpdate
    {
        public DateTime FLastUpdateTime = DateTime.Now;
        public bool Invalidated;

        public bool NeedFullUpdate = false;

        public bool Enabled;

        public TGridUpdate()
        {
        }

        public bool NeedDraw = false;
        public EventHandler OnDraw { get; set; }
        private void Draw()
        {
            OnDraw?.Invoke(this, null);
        }

        public EventHandler OnUpdate { get; set; }

        private void Update()
        {
            OnUpdate?.Invoke(this, null);
        }

        /// <summary>
        /// extend the passive period
        /// </summary>    
        public void DelayUpdate()
        {
            if (!Invalidated)
            {
                FLastUpdateTime = DateTime.Now;
                //Caption := Caption + 'D';
            }
            else
            {
                Invalidated = false;
            }
        }

        /// <summary>
        /// make sure assigned EventHandler is called from within next call to DoOnIdle
        /// </summary>
        public void InvalidateView()
        {
            NeedDraw = true;
            TimeSpan ts = new TimeSpan(0, 0, 0, 1, 100);
            FLastUpdateTime = FLastUpdateTime - ts;
            //Caption = Caption + 'I';
            Invalidated = true;
        }

        /// <summary>
        /// call assigned EventHandler, if any - but only if not called recently
        /// </summary>
        public void DoOnIdle()
        {
            if (Enabled)
            {                
                TimeSpan ts = DateTime.Now - FLastUpdateTime;
                if (ts.Seconds > 0)
                {
                    if (NeedFullUpdate)
                    {
                        Update();
                    }
                    else if (NeedDraw)
                    {
                        Draw();
                    }

                    NeedDraw = false;
                    FLastUpdateTime = DateTime.Now;
                }
            }
        }

        public void DrawNow()
        {
            if (Enabled)
            {
                Draw();
            }
        }

        public void UpdateNow()
        {
            if (Enabled)
            {
                Update();
            }
        }

        public void ScheduleDraw()
        {
            NeedDraw = true;
        }

        public void ScheduleFullUpdate()
        {
            NeedFullUpdate = true;
        }

    }
    
}
