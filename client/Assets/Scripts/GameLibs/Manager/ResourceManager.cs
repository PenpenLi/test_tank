using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TKBase;

namespace TKGame
{
    public class ResourceManager:ManagerBase
    {

        Dictionary<string, UnityEngine.Object> m_mResList;

        public ResourceManager()
            :base(ManagerType.ResourceManager)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            m_mResList = new Dictionary<string, UnityEngine.Object>();
        }
        public override void UnInitialize()
        {
            base.UnInitialize();
            m_mResList.Clear();
            m_mResList = null;
        }

        public GameObject GetRes(string sPath)
        {
            UnityEngine.Object obj;
            if (m_mResList.ContainsKey(sPath))
            {
                obj = m_mResList[sPath];
            }
            else
            {
                obj = Resources.Load(sPath, typeof(GameObject));
                if (obj != null)
                    m_mResList.Add(sPath, obj);
            }

            if (obj == null)
            {
                LOG.Error("No Resource In Path=" + sPath);
                return null;
            }

            var gameObject = (GameObject)GameObject.Instantiate(obj);
            return gameObject;
        }

        public T GetRes<T>(string sPath) where T : UnityEngine.Object
        {
            UnityEngine.Object obj;
            if (m_mResList.ContainsKey(sPath))
            {
                obj = m_mResList[sPath];
            }
            else
            {
                obj = Resources.Load(sPath);
                if (obj != null)
                    m_mResList.Add(sPath, obj);
            }
            if (obj != null)
                return (T)obj;
            else
            {
                LOG.Error(sPath + " resources  is  null");
                return null;
            }

        }
    }
}