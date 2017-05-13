using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Reflection.Obfuscation(ApplyToMembers = true, Exclude = true, Feature = "renaming")]
public class Singleton<T>
{
    private static T m_instance = (T)Activator.CreateInstance(typeof(T), true);

    public static T Instance
    {
        get
        {
            return m_instance;
        }
    }
}


[System.Reflection.Obfuscation(ApplyToMembers = true, Exclude = true, Feature = "renaming")]
public class SingletonMono<T> : MonoBehaviour where T : UnityEngine.Component
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                //Debug.Log(typeof(T).Name);
                GameObject root = GameObject.Find(typeof(T).Name);
                
                if (root != null)
                {
                    //Debug.Log(root);
                    instance = root.GetComponent<T>();
                }
                else
                {
                    //Debug.Log(root);
                    root = GameObject.Find("/Client");

                    if (root == null)
                    {
                        root = GameObject.Find(typeof(T).Name);
                        if (root == null)
                        {
                            root = new GameObject(typeof(T).Name);
                        }
                    }

                    T[] list = root.GetComponentsInChildren<T>(true);
                    if (list != null && list.Length != 0)
                    {
                        instance = list[0];
                    }
                    else
                    {
                        instance = root.AddComponent<T>();
                    }
                }
            }
            return instance;
        }
    }

    public virtual void Awake()
    {
        //if (instance != null)
        //{
        //    Debug.LogError("already exist!");
        //}
        instance = (T)(System.Object)(this);
        //Debug.Log(instance);
    }
}