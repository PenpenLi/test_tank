using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TKBase;
using TKGame;

namespace TKGameView
{
    public class LogView : MonoBehaviour 
    {
       
        static private List<string> m_Lines = new List<string>();
        static private List<LogType> m_LineTypes = new List<LogType>();
        void Start()
        {
            if(GameConfig.mode != GameConfig.MODE_RELEASE)
            {
                LOG.RegisterLogCallback(HandleLog);
            }
            else
            {
                this.enabled = false;
            }
        }

        void HandleLog(string logString, LogType type)
        {
            if (Application.isPlaying)
            {
                if (m_Lines.Count > 20)
                {
                    m_Lines.RemoveAt(0);
                    m_LineTypes.RemoveAt(0);
                }
                m_Lines.Add(logString);
                m_LineTypes.Add(type);
            }
        }

        void OnGUI()
        {
            
            for (int i = 0, imax = m_Lines.Count; i < imax; ++i)
            {
                if (m_LineTypes[i] == LogType.Warning || m_LineTypes[i] == LogType.Error)
                {
                    GUI.color = Color.red;
                }
                else
                {
                    GUI.color = Color.white;
                }
                GUILayout.Label(m_Lines[i]);
            }
        }
    }
}
