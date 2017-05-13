
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaFramework;
using System.Text;
using TKBase;
using System.Runtime.InteropServices;
namespace TKGame
{
    public class SdkManager : MonoBehaviour
    {
        public static string wxName;
        public static string wxHeadUrl;
        public static int wxSex;
        public static bool wxLogin;

        void Start()
        {
            wxLogin = false;
            this.name = "NativeOsCallbacks";
        }
        
       #if UNITY_IOS
           [DllImport("__Internal")]
           private static extern void iOS_weixin_login();
           [DllImport("__Internal")]
           private static extern void iOS_weixin_invite(byte[] str1,byte[] str2,byte[] str3);
           [DllImport("__Internal")]
           private static extern void iOS_weixin_openweb(byte[] str);
           [DllImport("__Internal")]
           private static extern void iOS_weixin_scenetimeline(byte[] str1,byte[] str2,byte[] str3);
           [DllImport("__Internal")]
           private static extern void iOS_weixin_screenshot(byte[] bytes,int len);
           [DllImport("__Internal")]
           private static extern int iOS_askNetType();
           [DllImport("__Internal")]
           private static extern float iOS_askBattery();
           [DllImport("__Internal")]
           private static extern void iOS_weixin_phoneshake();
       #endif
               //ios转码
               private static byte[] GetUTF8BytesWithEnd(string str)
               {
                   byte[] res = Encoding.UTF8.GetBytes(str);
                   byte[] newRes = new byte[res.Length + 1];
                   for (int i = 0; i < res.Length; i++)
                       newRes[i] = res[i];
                   newRes[res.Length] = 0;
                   return newRes;
               }


               //----------------------------------收消息---------------------------------------------------------------

               IEnumerator GET(string url, Dictionary<string, string> dic)
               {
                   string Parameters;
                   bool first;
                   if (dic.Count > 0)
                   {
                       first = true;
                       Parameters = "?";
                       //从集合中取出所有参数，设置表单参数（AddField()).  
                       foreach (KeyValuePair<string, string> post_arg in dic)
                       {
                           if (first)
                               first = false;
                           else
                               Parameters += "&";

                           Parameters += post_arg.Key + "=" + post_arg.Value;
                       }
                   }
                   else
                   {
                       Parameters = "";
                   }
          //         LOG.Log("getURL :" + Parameters);

                   //直接URL传值就是get  
                   WWW www = new WWW(url + Parameters);
                   yield return www;
            //         LOG.Log("progress--- " + www.progress);

                   if (www.error != null)
                   {
                       //GET请求失败  
              //         LOG.Log("请求失败error :" + www.error);
                   }
                   else
                   {
                       //GET请求成功  
                //       LOG.Log("Get返回的数据：" + www.text);
                       json_UserInfo(www.text);
                   }
               }

               public void getATO(string json)
               {
               //    LOG.Log("getATO:   " + json);
                   Dictionary<string, object> JsonGet = MiniJSON.Json.Deserialize(json) as Dictionary<string, object>;
                   string openid = JsonGet["openid"].ToString();
                   string access_token = JsonGet["access_token"].ToString();
                   Dictionary<string, string> dic = new Dictionary<string, string>();
                   dic.Add("access_token", access_token);
                   dic.Add("openid", openid);
                //   LOG.Log("-----------start req-------");
                   StartCoroutine(GET("https://api.weixin.qq.com/sns/userinfo", dic));
       #if (UNITY_ANDROID) && (!UNITY_EDITOR)
               //    Network.ReqWXUserInfo(access_token,openid);
       #endif

       #if UNITY_IOS && (!UNITY_EDITOR)
                 //  Network.ReqWXUserInfo(access_token,openid);
       #endif
                   // json_AcToken(json);
               }


               public  void json_UserInfo(string str)
               {
                   //  string[] str2 = new string[] { "微信传回来的呃呃呃额额: " + str };
         //          LOG.Log("User info: " +  str);
                   Dictionary<string, object> JsonGet = MiniJSON.Json.Deserialize(str) as Dictionary<string, object>;
                   //txt.text = "解json之后 str:" + str;
             //      string openid = JsonGet["openid"].ToString();
                   wxName = JsonGet["nickname"].ToString();
                   //wxSex = int.Parse(JsonGet["sex"].ToString());
             //      string province = JsonGet["province"].ToString();
             //      string city = JsonGet["city"].ToString();
               //    string country = JsonGet["country"].ToString();
                       wxHeadUrl = JsonGet["headimgurl"].ToString();          //头像是一个链接还得下载下来.....
                       string unionid = JsonGet["unionid"].ToString();
                      PlayerPrefs.SetString("AccountWXheadUrl",wxHeadUrl);

                     //Network.wxName = nickname;
                      //Network.pic - wait for download
                  //Network.sex = sex;


            if (Network.Send_Wechat(unionid) == 1)
                   {
                       wxLogin = true;
             //          LOG.Log("ready to LuaHelper.SocketConnect(); ");
                       LuaHelper.SocketConnect();
                   }
                   else
                   {
                       Network.gameState = Network.SGameState.LoginFail;
                   }

                   //            Network.headimgurl = headimgurl;
                   //txt.text = "用户信息: " + "\nopenID:" + openid + "\nnickname:" + nickname
                   //   + "\nsex" + sex + "\nprovince" + province + "\ncity" + city + "\ncountry" + country
                   //  + "\nheadimgurl" + headimgurl + "\nunionID" + unionid;

               }

               //---------------------------------------------------发消息--------------------------------------------------------------
               public static void SdkScenetimeline(string str1, string str2, string str3)
               {
               //    string[] str = new string[] { "http://blog.csdn.net/axuan_k", "---title----", "---description---" };
       #if (UNITY_ANDROID) && (!UNITY_EDITOR)
                   _CallSdkApi("SdkScenetimeline",str1,str2,str3);
       #endif

         //          Debug.Log("SdkScenetimeline++++++++");

       #if (UNITY_IOS) && (!UNITY_EDITOR)
                   iOS_weixin_scenetimeline(GetUTF8BytesWithEnd(str1),GetUTF8BytesWithEnd(str2),GetUTF8BytesWithEnd(str3));
       #endif
               }

               public static int SdkInvitation(string str1,string str2, string str3)
               {
       #if (UNITY_ANDROID) && (!UNITY_EDITOR)
                   _CallSdkApi("SdkInvitation",str1,str2,str3);  
       #endif

       #if (UNITY_IOS) && (!UNITY_EDITOR)
                   iOS_weixin_invite(GetUTF8BytesWithEnd(str1),GetUTF8BytesWithEnd(str2),GetUTF8BytesWithEnd(str3));      
       #endif
                   return 0;
               }


                public static int wechatLogin()
                {
                    if (PlayerPrefs.GetString("AccountWXuin2") != null&& PlayerPrefs.GetString("AccountWXuin2") !="")
                    {
                        wxLogin = true;
                        Network.UIN = int.Parse(PlayerPrefs.GetString("AccountWXuin2"));
                        wxHeadUrl = PlayerPrefs.GetString("AccountWXheadUrl");
                        LuaHelper.SocketConnect();
                        return -1;
                    }
        #if (UNITY_ANDROID) && (!UNITY_EDITOR)
                           int ret = _CallSdkApiReturn<int>("SdkLogin");
                           if(ret == -1)
                                return 1;
                           return -1;
        #endif
                    //     Debug.Log("Login!!!!!!\n");


        #if (UNITY_IOS) && (!UNITY_EDITOR)
                           iOS_weixin_login();
                           return -2;
        #endif
                    return 0;

                }


        public static void getNetType()
               {
                   //txt.text = "按钮5   " + "type （-1：没有网络）  （1：WIFI网络）（2：cmwap网络）（3：cmnet网络 ） \n" + "type: " + netType.ToString
                   int temp = 1;
       #if (UNITY_ANDROID) && (!UNITY_EDITOR)
                   temp = _CallSdkApiReturn<int>("SdkAskNetType");
       #endif

       #if UNITY_IOS && (!UNITY_EDITOR)
                   temp = iOS_askNetType();
       #endif

               }
               public static void getNetSignal()
               {
                   float temp = 40;
       #if (UNITY_ANDROID) && (!UNITY_EDITOR)
                   temp = _CallSdkApiReturn<float>("SdkAskNetSignal");
       #endif

       #if UNITY_IOS && (!UNITY_EDITOR)
                   temp = 40;
       #endif
                   // txt.text = "按钮6" + "信号范围-100 到 0， 0信号最好    value: " + value.ToString();
               }
               public static void getBattery()
               {
                   float temp = 1;
       #if (UNITY_ANDROID) && (!UNITY_EDITOR)
                   temp = _CallSdkApiReturn<float>("SdkAskBattery");
       #endif

       #if UNITY_IOS && (!UNITY_EDITOR)
                   temp = iOS_askBattery();
       #endif
                   //txt.text = "按钮7  value:" + v.ToString();
               }

               //设置手机超时震动
               public static void SdkPhoneshake()
               {
       #if (UNITY_ANDROID) && (!UNITY_EDITOR)
                   _CallSdkApi("SdkPhoneshake");
       #endif

                   Debug.Log("SdkPhoneshake+++++");

       #if UNITY_IOS && (!UNITY_EDITOR)
                   iOS_weixin_phoneshake();
       #endif

               }


       #if UNITY_ANDROID
               protected static AndroidJavaObject _GetCurrentAndroidJavaObject()                //要有这个AndroidJavaObject才能Call
               {
                   AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                   return jc.GetStatic<AndroidJavaObject>("currentActivity");
               }

               protected static void _CallSdkApi(string apiName, params object[] args)             //没有返回值的Call
               {

                   AndroidJavaObject jo = _GetCurrentAndroidJavaObject();
                   jo.Call(apiName, args);
               }
               protected static T _CallSdkApiReturn<T>(string apiName, params object[] args)          //模板只是返回参数类型不同
               {

                   AndroidJavaObject jo = _GetCurrentAndroidJavaObject();
                   return jo.Call<T>(apiName, args);
               }
       #endif
               
        }
}
