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
using TKGame;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public partial class Network : Singleton<Network>
{
    public static Res_Boom res_boom = new Res_Boom();
    public static int xue = 0;

    public static bool NetworkMode = false;

    public static PlayerInfo Myplayer = new PlayerInfo();
    public static int UIN = UnityEngine.Random.Range(1, 10000);
    public static int SequenceID = 0;

    public static int CMD_ENTERROOM_result;
    public static int CMD_MAP_MATCH_result = -1;

    public static Res_merge gameInfo = new Res_merge();
    public static SitDown gameInfo_sitdown = new SitDown();

    public static BattleInfo battleinfo = new BattleInfo();

    //人物信息
    public static string name;          
    public static int exp;
    public static int gold= 0;
    public static int diamond;

    public static int addexp;
    public static int addgold;
    public static int adddiamond;
    //个人信息
    public static int sex;//1男生 2女生

    public static int playerid = 0;
    public static int playerindex = 0;
    public static int playerid2 = 0;
    public static int next_playerid = 0;
    public static int seat = 0;
    public static int seat_res = 0;    //坐下总人数
    public static int seat_binary = 0;  //坐下二进制
    public static int role_res = -1;

    public static int default_skill = 1;
    public static int skill1 = 2;
    public static int skill2 = 7;
    

    //游戏中
    public static int Round = 1;
    public static int Wind;
    public static int PreMove = 0;
    public static int MoveCnt = 0;
    public static Dictionary<int, int> Pid_Tid = new Dictionary<int, int>();
    public static int []Skill_CD = new int[20];

    public static int Attacking = 0;
    public static bool Moving = false;

    //本次炸弹爆炸属性
    public static Texture2D n_stShape;
    public static Texture2D n_stBorder;

    //攻击网络bug临时解决方案
    public static AttackBaseData attackbasedata = new AttackBaseData(CharacterAttackType.BOMB);


    public static float Heart = 0;

    public static Res_roleInfo roleinfo = new Res_roleInfo();


    //好友
    public static int Friend_cnt;
    public static PlayerInformation Find_Friend = new PlayerInformation();
    public static PlayerInformation []Friend_list = new PlayerInformation[55];
    public static string Add_Friend_Name;
    public static PlayerInformation Added_Friend = new PlayerInformation();
    public static string Invitation_name;
    public static int Invitation_id;
    public static Dictionary<String, int> Add_Friend_List = new Dictionary<string, int>();
    public static Dictionary<String, int> Invite_Friend_List = new Dictionary<string, int>();


    //选择
    public static int GameChooseStatus = -1;

    //加载
    public static int []InfoLevel = new int[11];
    public static int GameLoading = 0;
    public static int GameLoading_Cnt = 0;
    public static int []GameLoading_num  = new int [11];
    public static int []GameChoose_num = new int[11];
    public static int []GameReady_pic = new int[11];

    //坐下
    public static bool[] Battle_wxLogin = new bool[7];
    public static string[] Battle_wxHead = new string[7];

    //AI
    public static int Player_tank = 1;
    public static int Ai_tank = 1;

    public static float BombEnergy = 0;

    public static string ToLua_str = null;

    /// <summary>
    /// 二次匹配初始化
    /// </summary>
    public static void Init()
    {
        CMD_MAP_MATCH_result = -1;
        role_res = -1;
        seat_res = 0;
        seat_binary = 0;

        gameInfo = new Res_merge();
        gameInfo_sitdown = new SitDown();
        battleinfo = new BattleInfo();

        playerid2 = 0;
        Round = 1;
        PreMove = 0;
        Pid_Tid = new Dictionary<int, int>();

        Attacking = 0;
        Moving = false;

        GameChooseStatus = -1;

        GameLoading = 0;
        GameLoading_Cnt = 0;
        for (int i = 1; i <= 10; i++)
        {
            GameLoading_num[i] = 0;
            GameChoose_num[i] = 0;
        }

        for(int i = 0; i < 20; i++)
        {
            Skill_CD[i] = 0;
        }
    }

    public static void mySort()
    {
        SortByMode cmp = new SortByMode();
        Array.Sort(Friend_list, 0, Friend_cnt ,cmp);
    }


    public static void InitRound()
    {
        Attacking = 0;
        Moving = false;
        PreMove = 0;
        MoveCnt = 0;
        SkillManager.CurrentSkillId = 0;
        BombEnergy = 0;
    }

    public static bool Check_Name(string name)
    {
        string[] x = { "！", "￥", "…", "（", "）", "—", "，", "。", "《", "》", "？", "》", "‘", "’", "“", "”", "：", "；", "【", "】" };

        for (int i = 0; i < x.Length; i++)
        {
            if (name.IndexOf(x[i]) > -1)
            {
                return false;
            }
        }
        return true;
    }

    public static void Start_xue()
    {
        for (int i = 0; i < 55; i++)
        {
            Friend_list[i] = new PlayerInformation();
        }
        //for (int i = 0; i < 10; i++)
        //{
        //    Debug.Log(UnityEngine.Random.Range(1, 7) + "   " + Network.xue++);
        //}
        //PlayerPrefs.DeleteAll();
        /*
        string url = "http://td.joyyou.123u.com/meishi_mobile/login.php?username=zpc522&password=1234567890";
        WebRequest wRequest = WebRequest.Create(url);
        wRequest.Method = "GET";
        wRequest.ContentType = "text/html;charset=UTF-8";
        WebResponse wResponse = wRequest.GetResponse();
        Stream stream = wResponse.GetResponseStream();
        StreamReader reader = new StreamReader(stream, System.Text.Encoding.Default);
        string str = reader.ReadToEnd();   //url返回的值 
        reader.Close();
        wResponse.Close();

        Dictionary<string, object> JsonGet = MiniJSON.Json.Deserialize(str) as Dictionary<string, object>;

        int status = Convert.ToInt32(JsonGet["status"]);
        if (status == 0)
        {
            Dictionary<string, object> data = JsonGet["data"] as Dictionary<string, object>;

            int uid = Convert.ToInt32(data["uid"]);

            Debug.Log("xue" + uid);
        }
        else
        {
            string ans = JsonGet["msg"].ToString();

            Debug.Log("xue::::::" + ans);
        }
        */

    }

    public static byte[] ObjectToBytes(object obj)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            IFormatter formatter = new BinaryFormatter(); formatter.Serialize(ms, obj); return ms.GetBuffer();
        }
    }
}

public class SortByMode : IComparer
{

    public int Compare(object x, object y)
    {
        PlayerInformation a = (PlayerInformation)x;
        PlayerInformation b = (PlayerInformation)y;
        
        if (a.status != b.status )
        {
            return b.status.CompareTo(a.status);
        }
        return a.name.CompareTo(b.name);
    }
}

public class PlayerInformation {
    public string name;
    public int sex;
    public int game_cnt;
    public int game_win;
    public int exp;
    public int tank_cnt;
    public int[] tank_own = new int[10];
    public int skill_cnt;
    public int[] skill_own = new int[10];
    public int relation;   // 0 myself  1 friend  2 not friend
    public int status;   //0 offnet  1. online
    public bool wxLogin;
    public string wxHead;
}

