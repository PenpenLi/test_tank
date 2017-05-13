using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TKGame
{
    public class TickerList
    {
        private List<ITicker> m_stList;
        private List<ITicker> m_lsAddBuffer;


        public TickerList()
        {
            m_stList = new List<ITicker>();
            m_lsAddBuffer = new List<ITicker>();
        }

        public void Add(ITicker item)
        {
            switch (item.IsInTickerListFlag)
            {
                case TickerFlag.TICK_FLAG_NONE:
                    m_lsAddBuffer.Add(item);
                    item.IsInTickerListFlag = TickerFlag.TICK_FLAG_IN_BUFFER;
                    break;
                case TickerFlag.TICK_FLAG_IN_BUFFER:
                    if (GameConfig.mode == GameConfig.MODE_DEBUG)
                    {
                        throw new Exception("add tick twice!");
                    }
                    break;
                case TickerFlag.TICK_FLAG_IN_LIST:
                    if (GameConfig.mode == GameConfig.MODE_DEBUG)
                    {
                        throw new Exception("add tick twice!");
                    }
                    break;
            }
        }

        public void Remove(ITicker item)
        {
            if(item.IsInTickerListFlag == TickerFlag.TICK_FLAG_IN_LIST)
            {
                item.IsInTickerListFlag = TickerFlag.TICK_FLAG_NONE;
            }
            else if(item.IsInTickerListFlag == TickerFlag.TICK_FLAG_IN_BUFFER)
            {
                item.IsInTickerListFlag = TickerFlag.TICK_FLAG_NONE;
                m_lsAddBuffer.Remove(item);
            }
            
        }

        private void LaterOperation()
        {
            for (int i = m_stList.Count - 1; i >= 0; --i)
            {
                if (m_stList[i].IsInTickerListFlag != TickerFlag.TICK_FLAG_IN_LIST)
                {
                    m_stList[i].IsInTickerListFlag = TickerFlag.TICK_FLAG_NONE;
                    m_stList.RemoveAt(i);
                }
            }
            for (int i = 0; i < m_lsAddBuffer.Count; ++i)
            {
                m_lsAddBuffer[i].IsInTickerListFlag = TickerFlag.TICK_FLAG_IN_LIST;
                m_stList.Add(m_lsAddBuffer[i]);
            }
            m_lsAddBuffer.Clear();
        }

        public void DoTick(uint tickCount)
        {
            int count = m_stList.Count;
            for (int i = 0; i < count; ++i)
            {
                if (m_stList[i].IsInTickerListFlag == TickerFlag.TICK_FLAG_IN_LIST)
                {
                    m_stList[i].Tick(tickCount);
                }
            }
            LaterOperation();
        }

        public void Clear()
        {
            m_stList.Clear();
            m_lsAddBuffer.Clear();
        }
    }
}