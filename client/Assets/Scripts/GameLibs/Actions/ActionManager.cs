using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TKBase;

namespace TKGame
{
    public class ActionManager
    {
        private List<BaseAction> m_actionQueue;
        public ActionManager()
        {
            m_actionQueue = new List<BaseAction>();
        }

        public void Act(BaseAction action)
        {

            for (int i = 0; i < m_actionQueue.Count; ++i)
            {
                BaseAction c = m_actionQueue[i];
                if (c.Connect(action))
                {
                    ObjectPools.CheckIn(action);
                    return;
                }
                else if (c.CanReplace(action))
                {
                    ObjectPools.CheckIn(c);
                    action.Prepare();
                    m_actionQueue[i] = action;
                    return;
                }
            }

            m_actionQueue.Add(action);
            if (m_actionQueue.Count == 1)
            {
                action.Prepare();
            }
        }
        public void Execute()
        {
            if (m_actionQueue.Count > 0)
            {
                BaseAction c = m_actionQueue[0];
                if (!c.isFinished)
                {
                    c.Execute();
                }
                else
                {
                    ObjectPools.CheckIn(c);
                    m_actionQueue.RemoveAt(0);
                    if (m_actionQueue.Count > 0)
                    {
                        m_actionQueue[0].Prepare();
                    }
                }
            }
        }

        public void TraceAllRemainAction()
        {
            if (GameConfig.mode == GameConfig.MODE_DEBUG)
            {
                LOG.Log("-----------------TraceAllRemainAction------------------");
                for (int i = 0; i < m_actionQueue.Count; ++i)
                {
                    LOG.Log(m_actionQueue[i].GetType().ToString());
                }
                LOG.Log("--------------TraceAllRemainAction  end------------------");
            }
        }

        public int ActionCount
        {
            get { return m_actionQueue.Count; }
        }

        public void ExecuteAtOnce()
        {
            foreach(BaseAction action in m_actionQueue)
            {
                action.ExecuteAtOnce();
            }
        }

        public void Clear()
        {
            foreach (BaseAction action in m_actionQueue)
            {
                action.Cancel();
                ObjectPools.CheckIn(action);
            }
            m_actionQueue.Clear();
        }
    }
}
