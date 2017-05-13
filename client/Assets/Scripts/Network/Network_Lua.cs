#define xue

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using TKBase;
using ProtoTest;
using UnityEngine;
using TKGame;

public partial class Network : Singleton<Network>
{
    public static BattleType gamemode;
    public enum LuaID
    {
        Init_Role_Res,
        Send_Check,
        Send_Create,
        Send_MapMatch,
        Send_MapMatch_Cancel,
        Send_GameReady,
        Send_Attach,
        Lua_Invitation_Friend,

        Off_Net,
        Send_Skip,
        Net_init,
        AI_1v1,
        NORMAL_PVE,
        PVP_2V2,
        NORMAL_PVP,
        PVP_3V3,

        Start_Game,
        Init_Battle_UI,

        Choosing,
        Send_Quit_Button,
        Lua_xue,
    }
    public delegate void Lua_fun();
    public static Dictionary<LuaID, Lua_fun> lua_fun = new Dictionary<LuaID, Lua_fun>();

    public static void Lua_Invitation_Friend()
    {
        Network.Send_Invitation_Friend(Network.Invitation_name);
    }

    public static void Lua_xue()
    {
        //模拟查找好友
        //Find_Friend.sex = 1;
        //Find_Friend.exp = 2;
        //Find_Friend.game_cnt = 3;
        //Find_Friend.game_win = 4;
        //EventDispatcher.DispatchEvent("EventShowInfomationUI", null, null);


        //模拟已有好友列表
        //PlayerInformation p1 = new PlayerInformation();
        //p1.name = "xue";
        //p1.sex = 1;
        //p1.status = 1;
        //Friend_list[Friend_cnt++] = p1;

        //PlayerInformation p2 = new PlayerInformation();
        //p2.name = "rui";
        //p2.sex = 2;
        //p2.status = 2;
        //Friend_list[Friend_cnt++] = p2;
        //EventDispatcher.DispatchEvent("EventUpdateFriendList", null, null);

        //模拟好友通知
        //EventDispatcher.DispatchEvent("EventAddFriendNotify", null, null);
    }

    public static void Send_Quit_Button()
    {
        //GameGOW.Get().BattleMgr.StopBattle();
        GameGOW.Get().BattleMgr.m_bIsInBattle = false;
        if (Network.gamemode == BattleType.AI_1v1)
        {

        }
        else if(Network.NetworkMode == true)
        {
            Network.Send_Quit();
        }

    }

    public static void Choosing()
    {
        Network.GameChooseStatus = 0;
    }

    public static void Init_Battle_UI()
    {
        if(Network.NetworkMode == true)
        {
            if(GameGOW.battleinfo.m_listPlayers[0].m_iPlayerUID != Network.playerid)
            {
                EventDispatcher.DispatchEvent("EventHideBattleJoyStickUI", null, null);
            }
        }
    }

    public static void Start_Game()
    {
        GameGOW.Get().Start_Game();
    }

    public static void Init_Role_Res()
    {
        Network.role_res = -1;
    }

    public static void Send_Check()
    {
        Network.Send_CreateCharacter(true);
    }

    public static void Send_Create()
    {
        Network.Send_CreateCharacter(false);
    }

    public static void Off_Net()
    {
        Network.Instance.Disonnect();
        NetworkConfig.Click_Exit_Button = true;
        Network.NetworkMode = false;
        if (SdkManager.wxLogin == true)
        {
            PlayerPrefs.SetString("AccountWXuin2", null);
            SdkManager.wxLogin = false;
        }
        Network.gameState = SGameState.Offline;
    }

    public static void Send_Skip()
    {
        if (Network.gamemode == BattleType.AI_1v1) {
            if (Network.Attacking == 0)
            {
                if (GameGOW.Get().BattleMgr.m_pCurrentPlayer.ID == Network.playerid)
                {
                    EventDispatcher.DispatchEvent("EventHideBattleJoyStickUI", null, null);
                }
                else
                {
                    EventDispatcher.DispatchEvent("EventShowBattleJoyStickUI", null, null);
                }
                GameGOW.Get().BattleMgr.ChangeController_Quick();
                if (GameGOW.Get().BattleMgr.Get_InBattle() == true && GameGOW.Get().BattleMgr.m_iCurrentRoundPlayerIndex % 2 == 1)
                {
                    GameGOW.Get().AIMgr.Act_AI_1v1();
                }
            }
        }
        else if (Network.NetworkMode == true)
        {
            Network.Send_SKIP();
        }
        else
        {
            GameGOW.Get().BattleMgr.ChangeController();
        }
    }



    public static void Net_init()
    {
        Network.Init();
    }

    public static void AI_1v1()
    {
        Network.Player_tank = UnityEngine.Random.Range(3, 5);
        Network.Ai_tank = UnityEngine.Random.Range(3, 5);
        Network.InitRound();
        Network.gamemode = BattleType.AI_1v1;
    }

    public static void NORMAL_PVE()
    {
        Network.gamemode = BattleType.NORMAL_PVE;
    }

    public static void PVP_2V2()
    {
        Network.gamemode = BattleType.PVP_2V2;
    }

    public static void PVP_3V3()
    {
        Network.gamemode = BattleType.PVP_3V3;
    }

    public static void NORMAL_PVP()
    {
        Network.gamemode = BattleType.NORMAL_PVP;
    }


    public enum LuaGetID
    {
        Get_Seat,
        Get_Seat_Binary,
        Get_Role,
        GetVersion,
        Get_Sex,

        Send_Youke,

        GetLoading,
        My_Index,

        Choose_Status,
    }
    public delegate int Lua_get();
    public static Dictionary<LuaGetID, Lua_get> lua_get = new Dictionary<LuaGetID, Lua_get>();

    public static int Choose_Status()
    {
        return Network.GameChooseStatus;
    }

    public static int My_Index()
    {
        int index = 1;
        for(int i = 0; i < Network.battleinfo.m_listPlayers.Count; i++)
        {
            if(Network.battleinfo.m_listPlayers[i].m_iPlayerUID == Network.playerid)
            {
                index = i + 1;
                break;
            }
        }
        return index;
    }

    public static int GetLoading()
    {
        return Network.GameLoading;
    }

    public static int Send_Youke()
    {
        if (PlayerPrefs.GetString("AccountYouke") != null && PlayerPrefs.GetString("AccountYouke") != "")
        {
            Network.UIN = int.Parse(PlayerPrefs.GetString("AccountYoukeUid"));
        }
        else
        {
            int cnt = 0;
            bool flag = false;
            while (cnt++ < 10)
            {
                string name = (UnityEngine.Random.Range(1, 10000000)).ToString();
                string password = (UnityEngine.Random.Range(1, 10000000)).ToString();
                string url = "http://td.joyyou.123u.com/meishi_mobile/register.php?username=" + name + "&password=" + password;
                string str = Get_Url(url);
                Dictionary<string, object> JsonGet = MiniJSON.Json.Deserialize(str) as Dictionary<string, object>;

                int status = Convert.ToInt32(JsonGet["status"]);
                if (status == 0)
                {
                    Dictionary<string, object> data = JsonGet["data"] as Dictionary<string, object>;
                    Network.UIN = Convert.ToInt32(data["uid"]);

                    PlayerPrefs.SetString("AccountYouke", name);
                    PlayerPrefs.SetString("AccountYoukePass", password);
                    PlayerPrefs.SetString("AccountYoukeUid", Network.UIN.ToString());

                    flag = true;
                    break;
                }
            }

            if(flag == false)
            {
                return 0;
            }
        }
        return 1;
    }

    public static void ReqWXUserInfo(string access_token, string openid)
    {
        string url = "https://api.weixin.qq.com/sns/userinfo?access_token=" + access_token + "&openid=" + openid;
        LOG.Log("xue:url:" + url);
        string str = Get_Url(url);
        LOG.Log("Rsp str:  " + str);
   //     SdkManager.json_UserInfo(str);
    }

    public static int Send_Wechat(string unionID)
    {       
        int cnt = 0;
        bool flag = false;
        while (cnt < 3)
        {
            ++cnt;
            string url = "http://td.joyyou.123u.com/meishi_mobile/get_user_uin.php?unionID=" + unionID;
            string str = Get_Url(url);
            Dictionary<string, object> JsonGet = MiniJSON.Json.Deserialize(str) as Dictionary<string, object>;

            int status = Convert.ToInt32(JsonGet["status"]);
            if (status == 0)
            {
                Network.UIN = Convert.ToInt32(JsonGet["uin"]);
                PlayerPrefs.SetString("unionID", unionID);
                PlayerPrefs.SetString("AccountWXuin2", Network.UIN.ToString());
                flag = true;
                break;
            }
        }
        if (flag == false)
            return 0;
        else
            return 1;
    }

    public static string Get_Url(string url)
    {
       
       WebRequest wRequest = WebRequest.Create(url);
        wRequest.Method = "GET";
        wRequest.ContentType = "text/html;charset=UTF-8";
        WebResponse wResponse = wRequest.GetResponse();
        Stream stream = wResponse.GetResponseStream();
        StreamReader reader = new StreamReader(stream, System.Text.Encoding.Default);
        string str = reader.ReadToEnd();   //url返回的值 
        reader.Close();
        wResponse.Close();
        return str;
    }

    public static int Get_Seat_Binary()
    {
        return Network.seat_binary;
    }

    public static int Get_Seat()
    {
        return Network.seat_res;
    }

    public static int Get_Role()
    {
        return Network.role_res;
    }

    public static int GetVersion()
    {
        if (Network.NetworkMode == true) return 1;
        else return 0;
    }

    public static int Get_Sex()
    {
        return Network.sex;
    }


    public enum LuaSetID
    {
        Send_Choose, 
        Send_Chat_Id,
        Send_Invitation,
    }
    public delegate void Lua_set(int x);
    public static Dictionary<LuaSetID, Lua_set> lua_set = new Dictionary<LuaSetID, Lua_set>();

    public static void Send_Invitation(int x)
    {
        if(x == 1)
        {
            Network.Send_Invitation_Notify_Friend(true);
        }
        else
        {
            Network.Send_Invitation_Notify_Friend(false);
        }
    }

    public static void Send_Chat_Id(int x)
    {
        Send_Chat(x);
    }

    public static void Send_Choose(int x)
    {
        Network.Send_GameChoose(x);
    }
}