using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace TKGame
{
    public class TickerManager: ManagerBase
    {
        private List<ITicker> m_lsFixTicker;
        private TickerList m_lsKeyTicker;
        private uint m_iTickCount;
        public uint TickCount
        {
            get { return m_iTickCount; }
        }
        private bool m_bIsPlaying;

        private float m_fTmpDetalTime;
        public TickerManager()
            :base(ManagerType.TickManager)
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();
            m_lsFixTicker = new List<ITicker>();
            m_lsKeyTicker = new TickerList();
            m_bIsPlaying = false;
            m_iTickCount = 0;
            m_fTmpDetalTime = 0;
        }

        public override void UnInitialize()
        {
            base.UnInitialize();
            m_lsFixTicker.Clear();
            m_lsKeyTicker.Clear();
            m_bIsPlaying = false;
            m_iTickCount = 0;
            m_fTmpDetalTime = 0;
        }

        public void Start()
        {
            m_iTickCount = 0;
            m_bIsPlaying = true;
        }

        public void Stop()
        {
            m_bIsPlaying = false;
            m_iTickCount = 0;
        }

        public void Pause()
        {
            m_bIsPlaying = false;

        }

        public void Resume()
        {
            m_bIsPlaying = true;
        }

        public void AddFixTicker(ITicker item)
        {
            m_lsFixTicker.Add(item);
        }

        public void AddKeyTicker(ITicker item)
        {
            m_lsKeyTicker.Add(item);
        }

        public void RemoveKeyTicker(ITicker item)
        {
            m_lsKeyTicker.Remove(item);
        }

        public void Clear()
        {
            m_lsKeyTicker.Clear();
        }

        public void Signal(float detalTime)
        {
            if (m_bIsPlaying == false)
            {
                return;
            }
            m_fTmpDetalTime += detalTime;
            while (m_fTmpDetalTime > GameDefine.FRAME_TIME_INTERVAL)
            {
                m_fTmpDetalTime -= GameDefine.FRAME_TIME_INTERVAL;
                m_iTickCount++;
                DoTick();
            }
        }

        private void DoTick()
        {
            for (int i = 0; i < m_lsFixTicker.Count; ++i)
            {
                m_lsFixTicker[i].Tick(m_iTickCount);
            }
            m_lsKeyTicker.DoTick(m_iTickCount);
        }
    }

}