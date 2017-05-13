using System;
using UnityEngine;
using System.IO;
using System.Text;

namespace TKBase
{

    public class LOG
    {
        public static bool m_bIsHideLog = false;
        public static bool m_bIsShowLogWin = false;
        private static string logFilePath = "";
        public delegate void LogCallback(string msg, LogType type);
        private static event LogCallback logCallback;
        public static void RegisterLogCallback(LogCallback handle)
        {
            logCallback += handle;
        }
       
        public static void Log(string message)
        {
            if (m_bIsHideLog)
                return;
            //message = "tick=" + TickerMgr.Get().GetCountTick() + "  " + message;
            message = "[info][" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "]" + message;
            Debug.Log(message);
            //if(m_bIsShowLogWin)
            //LogWindowUI.Get ().AddLog (message, LogWindowUI.LogType.kLog);
            PrintToFile(message);
            if (logCallback != null) logCallback(message, LogType.Log);
        }

        public static void KeyLog(string message)
        {
            if (m_bIsHideLog)
                return;
            //message = "tick=" + TickerMgr.Get().GetCountTick() + "  " + message;
            message = "[key][" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "]" + message;
            Debug.Log(message);
            //if(m_bIsShowLogWin)
            //LogWindowUI.Get ().AddLog (message, LogWindowUI.LogType.kLog);
            PrintToFile(message);
            if (logCallback != null) logCallback(message, LogType.Assert);
        }

        public static void Warning(string message)
        {
            if (m_bIsHideLog)
                return;
            message = "[warning][" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "]" + message;
            Debug.LogWarning(message);
            //if(m_bIsShowLogWin)
            //LogWindowUI.Get ().AddLog (message, LogWindowUI.LogType.kWarning);
            PrintToFile(message);
            if (logCallback != null) logCallback(message, LogType.Warning);
        }

        public static void Error(string message)
        {
            if (m_bIsHideLog)
                return;
            message = "[error][" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "]" + message;
            Debug.Assert(false, message);
            PrintToFile(message);
            if (logCallback != null) logCallback(message, LogType.Error);
            //if(m_bIsShowLogWin)
            //LogWindowUI.Get ().AddLog (message, LogWindowUI.LogType.kError);
        }

        public static void Assert(bool condition, string message)
        {
			if (m_bIsHideLog)
				return;
            Debug.Assert(condition, message);
			//if(!condition && m_bIsShowLogWin)
				//LogWindowUI.Get ().AddLog (message, LogWindowUI.LogType.kError);
        }

        private static void PrintToFile(string message)
        {

            if(logFilePath == "")
            {
#if UNITY_ANDROID || UNITY_IPHONE
                string logPath = Application.persistentDataPath + "/log/";
#else
                string logPath = Application.dataPath + "/../log/";
#endif
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }
                logFilePath = logPath + DateTime.Today.ToString("yyyy_MM_dd") + ".txt";
            }

                using (StreamWriter sw = new StreamWriter(logFilePath, true, Encoding.UTF8))
            {
                sw.WriteLine(message);
            }
        }
    }
}
