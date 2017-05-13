// 对network meassage的一些实用函数
using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net.Sockets;
using TKBase;
using UnityEngine;
using System.Collections.Generic;

public partial class Network
{

    public static void      PrintSendMsgProperties(object obj)
    {
        if (!NetworkConfig.IsDebugMode)
            return;

        LOG.Log("............... Send Msg Begin: " + obj + "...............");
        LOG.Log(DateTime.Now.ToString());
        PrintProperties(obj, 1);
        LOG.Log("............... Send Msg End: " + obj + "...............");
    }
    public static void      PrintRecivedMsgProperties(object obj)
    {
        if (!NetworkConfig.IsDebugMode)
            return;

        LOG.Log("************* Receive Msg Begin: " + obj + "*************");
        LOG.Log(DateTime.Now.ToString());
        LOG.Log("{");
        
        PrintProperties(obj, 1);

        LOG.Log("}");
        LOG.Log("************* Receive Msg End: " + obj + "*************");
    }

    public static void      PrintList(IList list, int indent)
    {
        if (list == null) return;

        int i = 0;
        foreach (object o in list)
        {
            string indentString = new string(' ', (indent - 1) * 3);
            indentString += "--";
            LOG.Log(indentString + o + "[" + i + "]");
            PrintProperties(o, indent + 2);
            i++;
        }
    }

    public static void PrintProperties(object obj, int indent)
    {
        if (obj == null) 
            return;
        
        string indentString = new string(' ', indent * 3);


        Type objType = obj.GetType();
        PropertyInfo[] properties = objType.GetProperties();
        foreach (PropertyInfo property in properties)
        {
            Attribute attri = Attribute.GetCustomAttribute(property, typeof(ProtoBuf.ProtoMemberAttribute));
            if (attri == null)
                continue;

            object propValue = property.GetValue(obj, null);
            if (propValue is IList)
            {
                LOG.Log(indentString + property.Name + "s Count = " + ((ICollection)propValue).Count + " :");
                PrintList((IList)propValue, indent + 2);
            }
            else if (property.PropertyType.Assembly == objType.Assembly && property.PropertyType.IsEnum == false)
            {
                LOG.Log(indentString + property.Name + ": " + propValue);
                if (indent != 0)
                {
                    LOG.Log(indentString + "{");
                }
                PrintProperties(propValue, indent + 2);
                if (indent != 0)
                {
                    LOG.Log(indentString + "}");
                }
            }
            else
            {
                LOG.Log(indentString + property.Name + ": " + propValue);
            }
        }


    }

    public static T         DeserializeMsg<T>(NetworkMsg msg, bool showLog)
    {
        T rsp;
        using (MemoryStream stream = new MemoryStream(msg.data))
        {
            rsp = ProtoBuf.Serializer.Deserialize<T>(stream);
        }

        if (showLog)
        {
            PrintRecivedMsgProperties(rsp);
        }
       

        return rsp;
    }
    public static T         DeserializeMsg<T>(NetworkMsg msg)
    {
        return DeserializeMsg<T>(msg, true);
    }

    public static bool Check_Str(string name)
    {
        for(int i = 0; i < name.Length; i++)
        {
            if((name[i] >= 'A' && name[i] <= 'Z') || (name[i] >= 'a' && name[i] <= 'z') || (name[i] >= '0' && name[i] <= '9')) { }
            else
            {
                return false;
            }
        }
        return true;
    }

    public static int Register(string name, string pwd)
    {
        string url = "http://td.joyyou.123u.com/meishi_mobile/register.php?username=" + name + "&password=" + pwd;
        string str = Get_Url(url);
        Dictionary<string, object> JsonGet = MiniJSON.Json.Deserialize(str) as Dictionary<string, object>;

        int status = Convert.ToInt32(JsonGet["status"]);
        if (status == 0)
        {
            Dictionary<string, object> data = JsonGet["data"] as Dictionary<string, object>;
            Network.UIN = Convert.ToInt32(data["uid"]);

            PlayerPrefs.SetString("AccountPre", name);
            PlayerPrefs.SetString("AccountPrePass", pwd);
            PlayerPrefs.SetString("AccountPreUid", Network.UIN.ToString());

            return 2;
        }
        else
        {
            return 0;
        }
    }

    public static int Login(string name, string pwd)
    {
        string url = "http://td.joyyou.123u.com/meishi_mobile/login.php?username=" + name + "&password=" + pwd;
        string str = Get_Url(url);
        Dictionary<string, object> JsonGet = MiniJSON.Json.Deserialize(str) as Dictionary<string, object>;

        int status = Convert.ToInt32(JsonGet["status"]);
        if (status == 0)
        {
            Dictionary<string, object> data = JsonGet["data"] as Dictionary<string, object>;
            Network.UIN = Convert.ToInt32(data["uid"]);

            PlayerPrefs.SetString("AccountPre", name);
            PlayerPrefs.SetString("AccountPrePass", pwd);
            PlayerPrefs.SetString("AccountPreUid", Network.UIN.ToString());

            return 2;
        }
        else
        {
            return 0;
        }
    }
}
