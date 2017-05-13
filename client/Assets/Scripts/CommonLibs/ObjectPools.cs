using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
namespace TKBase
{
    public sealed class ObjectPools
    {

        private static Dictionary<Type, ArrayList> m_dicObject =
            new Dictionary<Type, ArrayList>();

        /**
         * 回收对象
         * @param obj 被回收的对象
         */
        public static void CheckIn(object obj)
        {
            Type t = obj.GetType();
            if (obj is GameObject)
            {
                (obj as GameObject).SetActive(false);
            }
            if(obj is IPoolableObect)
            {
                (obj as IPoolableObect).UnInitialize();
            }
            if (false == m_dicObject.ContainsKey(t))
            {
                m_dicObject.Add(t, new ArrayList());
            }

            m_dicObject[t].Add(obj);
        }

        public static T CheckOut<T>(params object[] args) where T : IPoolableObect
        {
            Type t = typeof(T);
            IPoolableObect obj;
            if (false == m_dicObject.ContainsKey(t)
                || 0 == m_dicObject[t].Count)
            {
                try
                {
                    obj = Activator.CreateInstance(t) as IPoolableObect;
                    obj.Initialize(args);
                    return (T)obj;
                }
                catch (Exception ee)
                { //分配内存失败！
                    throw ee;
                }

            }
            obj = m_dicObject[t][0] as IPoolableObect;
            m_dicObject[t].RemoveAt(0);
            obj.Initialize(args);
            return (T)obj;
        }

        /**
         * 从对象池取一个对象
         * @param t 取出对象的类型
         * @param args 初始化对象参数
         */
        public static IPoolableObect CheckOut(Type t, params object[] args)
        {
            IPoolableObect obj;
            if (false == m_dicObject.ContainsKey(t)
                || 0 == m_dicObject[t].Count)
            {
                try
                {
                    obj = Activator.CreateInstance(t) as IPoolableObect;
                    obj.Initialize(args);
                    return obj;
                }
                catch (Exception ee)
                { //分配内存失败！
                    throw ee;
                }

            }
            obj = m_dicObject[t][0] as IPoolableObect;
            m_dicObject[t].RemoveAt(0);
            obj.Initialize(args);
            return obj;
        }

        public static GameObject CheckOut(GameObject output)
        {
            Type t = output.GetType();
            if (false == m_dicObject.ContainsKey(t)
                || 0 == m_dicObject[t].Count)
            {
                try
                {
                    output = GameObject.Instantiate(output) as GameObject;
                    return output;
                }
                catch (Exception ee)
                {//分配内存失败！
                    throw ee;
                }

            }
            output = m_dicObject[t][0] as GameObject;
            m_dicObject[t].RemoveAt(0);
            return output;
        }
    }
}