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
    private static object locker = new object();
    public static ClientHeader GetHeader(MessageID x, int len)
    {
        ClientHeader header = new ClientHeader();
        header.m_iUin = UIN;
        header.m_cSHFlag = 0;
        header.m_nOptionLength = 0;
        header.m_szOption = null;
        header.m_iMessageID = (short)x;
        header.m_nPlayerID = (short)Network.Myplayer.m_iPlayerUID;
        header.m_nGroupID = 1;
        header.m_nPlatformID = 0;
        
        lock (locker)
        {
            header.m_iSequenceID = SequenceID;
            Network.SequenceID++;
        }

        header.m_iPackageLength = 23 + header.m_nOptionLength + len;

        return header;
    }

    public void Send_Login()
    {
        Login xmsg = new Login();
        xmsg.uin = Network.UIN;
        byte[] msgBytes;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, xmsg);
            msgBytes = stream.ToArray();
        }

        ClientHeader header = new ClientHeader();
        //header = GetHeader(MessageID.CMD_LOGIN, msgBytes.Length);

        header.m_iUin = Network.UIN;
        header.m_cSHFlag = 0;
        header.m_nOptionLength = 0;
        header.m_szOption = null;
        header.m_iMessageID = (short)MessageID.CMD_LOGIN;
        header.m_nPlayerID = -1;
        header.m_nGroupID = 1;
        header.m_nPlatformID = 0;
        Network.SequenceID = 0;
        header.m_iSequenceID = 0;

        Network.SequenceID++;
        header.m_iPackageLength = 23 + header.m_nOptionLength + msgBytes.Length;

        connector.SendMsg(header, msgBytes);
    }

    public static void Send_CreateCharacter(bool check)
    {
        Create_role xmsg = new Create_role();
        xmsg.name = Network.name;
        xmsg.sex = Network.sex;
        xmsg.uin = Network.UIN;
        xmsg.checkName = check;
        xmsg.wxLogin = SdkManager.wxLogin;
        if (SdkManager.wxLogin == true)
        {
            xmsg.wxHead = SdkManager.wxHeadUrl;
        }

        byte[] msgBytes;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, xmsg);
            msgBytes = stream.ToArray();
        }

        ClientHeader header = new ClientHeader();
        header = GetHeader(MessageID.CMD_ROLE, msgBytes.Length);

        CMD_MAP_MATCH_result = -1;
        connector.SendMsg(header, msgBytes);
    }

    public void Send_EnterRoom()
    {
        enter xmsg = new enter();
        xmsg.m_nRoomID = 0;
        byte[] msgBytes;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, xmsg);
            msgBytes = stream.ToArray();
        }

        ClientHeader header = new ClientHeader();
        header = GetHeader(MessageID.CMD_ENTERROOM, msgBytes.Length);

        connector.SendMsg(header, msgBytes);
    }

    public static void Send_MapMatch()
    {
        Network.seat_res = 0;
        Merge xmsg = new Merge();
        if (Network.gamemode == BattleType.NORMAL_PVP)
        {
            xmsg.m_iModeID = 1;
        }
        else if(Network.gamemode == BattleType.PVP_2V2)
        {
            xmsg.m_iModeID = 2;
        }
        else if(Network.gamemode ==  BattleType.PVP_3V3)
        {
            xmsg.m_iModeID = 3;
        }
        else {
            xmsg.m_iModeID = 1;
        }
        xmsg.cancel = false;

        byte[] msgBytes;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, xmsg);
            msgBytes = stream.ToArray();
        }

        ClientHeader header = new ClientHeader();
        header = GetHeader(MessageID.CMD_MAP_MATCH, msgBytes.Length);

        CMD_MAP_MATCH_result = -1;
        connector.SendMsg(header, msgBytes);
    }

    public static void Send_MapMatch_Cancel()
    {
        Merge xmsg = new Merge();
        if (Network.gamemode == BattleType.NORMAL_PVP)
        {
            xmsg.m_iModeID = 1;
        }
        else if (Network.gamemode == BattleType.PVP_2V2)
        {
            xmsg.m_iModeID = 2;
        }
        else
        {
            xmsg.m_iModeID = 1;
        }
        xmsg.cancel = true;

        byte[] msgBytes;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, xmsg);
            msgBytes = stream.ToArray();
        }

        ClientHeader header = new ClientHeader();
        header = GetHeader(MessageID.CMD_MAP_MATCH, msgBytes.Length);

        CMD_MAP_MATCH_result = -1;
        connector.SendMsg(header, msgBytes);
    }

    public static void Send_GameReady()
    {
        OnGameReady xmsg = new OnGameReady();
        xmsg.m_strName = Network.name;
        xmsg.m_iPic = Network.sex;

        byte[] msgBytes;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, xmsg);
            msgBytes = stream.ToArray();
        }


        OnGame ongame = new OnGame();
        ongame.gameData = msgBytes;
        ongame.m_iRoomID = Network.Myplayer.m_iRoomID;
        ongame.m_iTableID = Network.gameInfo_sitdown.m_iTableID;
        ongame.m_icmd = (int)GameID.Ready;
        ongame.gameDataLength = msgBytes.Length;

        byte[] res;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, ongame);
            res = stream.ToArray();
        }

        ClientHeader header = new ClientHeader();
        header = GetHeader(MessageID.CMD_PLAYGAME, res.Length);

        connector.SendMsg(header, res);
    }

    public static void Send_GameChoose(int num) //服务器 -x：选择x   客户端 100+x  选择x
    {
        OnGameChoose xmsg = new OnGameChoose();
        //xmsg.m_iTankID = UnityEngine.Random.Range(1, 5);
        //xmsg.m_iTankID = xmsg.m_iTankID * 100000 + 1;
        if(num > 100)
        {
            num = -(num - 100);
        }
        xmsg.m_iTankID = num;
        xmsg.m_iSkill1 = Network.skill1;
        xmsg.m_iSkill2 = Network.skill2;

        byte[] msgBytes;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, xmsg);
            msgBytes = stream.ToArray();
        }


        OnGame ongame = new OnGame();
        ongame.gameData = msgBytes;
        ongame.m_iRoomID = Network.Myplayer.m_iRoomID;
        ongame.m_iTableID = Network.gameInfo_sitdown.m_iTableID;
        ongame.m_icmd = (int)GameID.Choose;
        ongame.gameDataLength = msgBytes.Length;

        byte[] res;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, ongame);
            res = stream.ToArray();
        }

        ClientHeader header = new ClientHeader();
        header = GetHeader(MessageID.CMD_PLAYGAME, res.Length);

        connector.SendMsg(header, res);
    }

    public static void Send_GameLoading(int len)
    {
        OnGameLoading xmsg = new OnGameLoading();
        xmsg.m_iOptID = Network.playerid;
        xmsg.m_ilen = len;

        byte[] msgBytes;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, xmsg);
            msgBytes = stream.ToArray();
        }


        OnGame ongame = new OnGame();
        ongame.gameData = msgBytes;
        ongame.m_iRoomID = Network.Myplayer.m_iRoomID;
        ongame.m_iTableID = Network.gameInfo_sitdown.m_iTableID;
        ongame.m_icmd = (int)GameID.Load;
        ongame.gameDataLength = msgBytes.Length;

        byte[] res;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, ongame);
            res = stream.ToArray();
        }

        ClientHeader header = new ClientHeader();
        header = GetHeader(MessageID.CMD_PLAYGAME, res.Length);

        connector.SendMsg(header, res);
    }

    public static void Send_Attach(bool boom)
    {
        OnGameAttack xmsg = new OnGameAttack();
        if (boom == false)
        {
            xmsg.m_iPower = GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iBombSpeed;
            xmsg.m_iAngle = GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iFireAngle;
        }
        else
        {
            xmsg.m_iPower = 0;
            xmsg.m_iAngle = 0;
        }
        xmsg.m_iHasBlast = boom;
        xmsg.m_iBombType = SkillManager.CurrentSkillId;

        if(xmsg.m_iBombType == (int)SkillManager.SkillType.DoubleBomb)
        {
            xmsg.m_iBombCnt = 2;
        }
        else if (xmsg.m_iBombType == (int)SkillManager.SkillType.TripleBomb)
        {
            xmsg.m_iBombCnt = 3;
        }
        else if(xmsg.m_iBombType == (int)SkillManager.SkillType.ThunderBomb)
        {
            xmsg.m_iBombCnt = 5;
        }
        else
        {
            xmsg.m_iBombCnt = 1;
        }

        byte[] msgBytes;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, xmsg);
            msgBytes = stream.ToArray();
        }

        OnGame ongame = new OnGame();
        ongame.gameData = msgBytes;
        ongame.m_iRoomID = Network.Myplayer.m_iRoomID;
        ongame.m_iTableID = Network.gameInfo_sitdown.m_iTableID;
        ongame.m_icmd = (int)GameID.Attach;
        ongame.gameDataLength = msgBytes.Length;

        byte[] res;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, ongame);
            res = stream.ToArray();
        }

        ClientHeader header = new ClientHeader();
        header = GetHeader(MessageID.CMD_PLAYGAME, res.Length);
        connector.SendMsg(header, res);
    }

    public static void Send_BOOM(bool boom, float x, float y, int boomid)
    {
        OnGameBoom xmsg = new OnGameBoom();
        xmsg.m_ix = x;
        xmsg.m_iy = y;
        xmsg.Kind = boom;
        xmsg.BombID = boomid;

        byte[] msgBytes;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, xmsg);
            msgBytes = stream.ToArray();
        }

        OnGame ongame = new OnGame();
        ongame.gameData = msgBytes;
        ongame.m_iRoomID = Network.Myplayer.m_iRoomID;
        ongame.m_iTableID = Network.gameInfo_sitdown.m_iTableID;
        ongame.m_icmd = (int)GameID.Boom;
        ongame.gameDataLength = msgBytes.Length;

        byte[] res;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, ongame);
            res = stream.ToArray();
        }
        
        ClientHeader header = new ClientHeader();
        header = GetHeader(MessageID.CMD_PLAYGAME, res.Length);
        connector.SendMsg(header, res);
    }

    public static void Send_Damage(int m_iPlayerID, int damage)
    {
        OnGame_HP xmsg = new OnGame_HP();
        xmsg.m_iPlayerID = m_iPlayerID;
        xmsg.hpValues = damage;

        byte[] msgBytes;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, xmsg);
            msgBytes = stream.ToArray();
        }

        OnGame ongame = new OnGame();
        ongame.gameData = msgBytes;
        ongame.m_iRoomID = Network.Myplayer.m_iRoomID;
        ongame.m_iTableID = Network.gameInfo_sitdown.m_iTableID;
        ongame.m_icmd = (int)GameID.Damage;
        ongame.gameDataLength = msgBytes.Length;

        byte[] res;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, ongame);
            res = stream.ToArray();
        }

        ClientHeader header = new ClientHeader();
        header = GetHeader(MessageID.CMD_PLAYGAME, res.Length);
        connector.SendMsg(header, res);
    }

    public static void Send_SKIP()
    {
        OnGame ongame = new OnGame();
        ongame.gameData = Encoding.ASCII.GetBytes("");
        ongame.m_iRoomID = Network.Myplayer.m_iRoomID;
        ongame.m_iTableID = Network.gameInfo_sitdown.m_iTableID;
        ongame.m_icmd = (int)GameID.Skip;
        ongame.gameDataLength = 0;

        byte[] res;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, ongame);
            res = stream.ToArray();
        }

        ClientHeader header = new ClientHeader();
        header = GetHeader(MessageID.CMD_PLAYGAME, res.Length);
        connector.SendMsg(header, res);
    }

    public static void Send_Move(bool keydown, int direction, float time, float x, float y)
    {
        OnGameMove xmsg = new OnGameMove();
        xmsg.m_iKeyDown = keydown;
        xmsg.m_iDirection = direction;
        xmsg.m_iIntervalTime = time;

        xmsg.m_iPosX = x;
        xmsg.m_iPosY = y;
        xmsg.m_iOptID = Network.playerid;

        byte[] msgBytes;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, xmsg);
            msgBytes = stream.ToArray();
        }

        OnGame ongame = new OnGame();
        ongame.gameData = msgBytes;
        ongame.m_iRoomID = Network.Myplayer.m_iRoomID;
        ongame.m_iTableID = Network.gameInfo_sitdown.m_iTableID;
        ongame.m_icmd = (int)GameID.Move;
        ongame.gameDataLength = msgBytes.Length;

        byte[] res;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, ongame);
            res = stream.ToArray();
        }

        ClientHeader header = new ClientHeader();
        header = GetHeader(MessageID.CMD_PLAYGAME, res.Length);
        connector.SendMsg(header, res);
    }

    public static void Send_Over(int teamid, bool win)
    {
        OnGameOver xmsg = new OnGameOver();
        xmsg.m_iTeamID = teamid;
        xmsg.m_iWin = win;

        byte[] msgBytes;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, xmsg);
            msgBytes = stream.ToArray();
        }

        OnGame ongame = new OnGame();
        ongame.gameData = msgBytes;
        ongame.m_iRoomID = Network.Myplayer.m_iRoomID;
        ongame.m_iTableID = Network.gameInfo_sitdown.m_iTableID;
        ongame.m_icmd = (int)GameID.Die;
        ongame.gameDataLength = msgBytes.Length;

        byte[] res;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, ongame);
            res = stream.ToArray();
        }

        ClientHeader header = new ClientHeader();
        header = GetHeader(MessageID.CMD_PLAYGAME, res.Length);
        connector.SendMsg(header, res);
    }

    public static void Send_Chat(int id)
    {
        OnGame_chat xmsg = new OnGame_chat();
        xmsg.m_iChatID = id;

        byte[] msgBytes;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, xmsg);
            msgBytes = stream.ToArray();
        }

        OnGame ongame = new OnGame();
        ongame.gameData = msgBytes;
        ongame.m_iRoomID = Network.Myplayer.m_iRoomID;
        ongame.m_iTableID = Network.gameInfo_sitdown.m_iTableID;
        ongame.m_icmd = (int)GameID.Chat;
        ongame.gameDataLength = msgBytes.Length;

        byte[] res;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, ongame);
            res = stream.ToArray();
        }

        ClientHeader header = new ClientHeader();
        header = GetHeader(MessageID.CMD_PLAYGAME, res.Length);
        connector.SendMsg(header, res);
    }

    public static void Send_PVE_Over(bool status)
    {
        PVE_over xmsg = new PVE_over();
        xmsg.m_iWin = status;

        byte[] msgBytes;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, xmsg);
            msgBytes = stream.ToArray();
        }

        ClientHeader header = new ClientHeader();
        header = GetHeader(MessageID.MSG_PVE_OVER, msgBytes.Length);

        CMD_MAP_MATCH_result = -1;
        connector.SendMsg(header, msgBytes);
    }

    public static void Send_Quit()
    {
        OnGame ongame = new OnGame();
        ongame.gameData = Encoding.ASCII.GetBytes("x");
        ongame.m_iRoomID = Network.Myplayer.m_iRoomID;
        ongame.m_iTableID = Network.gameInfo_sitdown.m_iTableID;
        ongame.m_icmd = (int)GameID.Quit;
        ongame.gameDataLength = 1;

        byte[] res;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, ongame);
            res = stream.ToArray();
        }

        ClientHeader header = new ClientHeader();
        header = GetHeader(MessageID.CMD_PLAYGAME, res.Length);
        connector.SendMsg(header, res);
    }

    public static void Send_Heart()
    {
        byte[] res = Encoding.ASCII.GetBytes("");
        ClientHeader header = new ClientHeader();
        header = GetHeader(MessageID.CMD_HEART_BEAT_RSP, 0);
        connector.SendMsg(header, res);
    }

    
    public static void Send_Add_Friend(string name)
    {
        if(Add_Friend_List.ContainsKey(name))
        {
            return;
        }
        Add_Friend_List.Add(name, 0);
        Friend_add_del xmsg = new Friend_add_del();
        xmsg.m_NameOther = name;

        byte[] msgBytes;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, xmsg);
            msgBytes = stream.ToArray();
        }

        ClientHeader header = new ClientHeader();
        header = GetHeader(MessageID.MSG_FRIEND_ADD, msgBytes.Length);
        connector.SendMsg(header, msgBytes);
    }

    public static void Send_Del_Friend(string name)
    {
        if (Add_Friend_List.ContainsKey(name))
        {
            Add_Friend_List.Remove(name);
        }
        Friend_add_del xmsg = new Friend_add_del();
        xmsg.m_NameOther = name;

        byte[] msgBytes;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, xmsg);
            msgBytes = stream.ToArray();
        }

        ClientHeader header = new ClientHeader();
        header = GetHeader(MessageID.MSG_FRIEND_DEL, msgBytes.Length);
        connector.SendMsg(header, msgBytes);
    }

    public static void Send_Add_Notify_Friend(string name1, string name2, bool accepted)
    {
        Friend_resAddNotify xmsg = new Friend_resAddNotify();
        xmsg.Name1 = name1;
        xmsg.Name2 = name2;
        xmsg.accept = accepted;

        byte[] msgBytes;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, xmsg);
            msgBytes = stream.ToArray();
        }

        ClientHeader header = new ClientHeader();
        header = GetHeader(MessageID.MSG_FRIEND_OPT_ADD, msgBytes.Length);
        connector.SendMsg(header, msgBytes);
    }

    public static void Send_Find_Friend(string name)
    {
        Friend_getInfo xmsg = new Friend_getInfo();
        xmsg.m_NameOther = name;

        byte[] msgBytes;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, xmsg);
            msgBytes = stream.ToArray();
        }

        ClientHeader header = new ClientHeader();
        header = GetHeader(MessageID.MSG_FRIEND_GETINFO, msgBytes.Length);
        connector.SendMsg(header, msgBytes);
    }

    public static void Send_Invitation_Friend(string name)
    {
        Network.gamemode = BattleType.NORMAL_PVP;
        Network.Init();
        Friend_invitation xmsg = new Friend_invitation();
        xmsg.m_NameOther = name;
        xmsg.m_iOptIDself = Network.playerid;

        byte[] msgBytes;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, xmsg);
            msgBytes = stream.ToArray();
        }

        ClientHeader header = new ClientHeader();
        header = GetHeader(MessageID.MSG_FRIEND_INVITATION, msgBytes.Length);
        connector.SendMsg(header, msgBytes);
    }

    public static void Send_Invitation_Notify_Friend(bool accepted)
    {
        Friend_resInvNotify xmsg = new Friend_resInvNotify();
        xmsg.m_iOptID1 = Network.playerid;
        xmsg.m_iOptID2 = Network.Invitation_id;
        xmsg.accept = accepted;

        byte[] msgBytes;
        using (MemoryStream stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, xmsg);
            msgBytes = stream.ToArray();
        }

        ClientHeader header = new ClientHeader();
        header = GetHeader(MessageID.MSG_FRIEND_OPT_INV, msgBytes.Length);
        connector.SendMsg(header, msgBytes);
    }
}