using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TKBase
{
    public class EventDispatcher
    {
        private static Dictionary<string, EventHandler> m_dicCallbacks = new Dictionary<string, EventHandler>();

        public static void AddListener(string eventName, EventHandler callback)
        {
            if(!m_dicCallbacks.ContainsKey(eventName))
            {
                m_dicCallbacks[eventName] = callback;
            }
            else
            {
                m_dicCallbacks[eventName] += callback;
            }
            
        }

        public static void RemoveListener(string eventName, EventHandler callback)
        {
            if (m_dicCallbacks.ContainsKey(eventName))
            {
                m_dicCallbacks[eventName] -= callback;
            }
        }

        public static void DispatchEvent(string eventName, object sender, EventArgs agrs)
        {
            if (m_dicCallbacks.ContainsKey(eventName) && m_dicCallbacks[eventName] != null)
            {
                m_dicCallbacks[eventName](sender, agrs);
            }
        }
    }
}
