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
using LuaFramework;
using TKGameView;

public partial class Network : Singleton<Network>
{
    public static void CMD_INVALID(NetworkMsg msg)
    {

    }

    public static void CMD_LOGIN(NetworkMsg msg)
    {
        Res_login xmsg = new Res_login();
        using (MemoryStream stream = new MemoryStream(msg.data))
        {
            xmsg = ProtoBuf.Serializer.Deserialize<Res_login>(stream);
        }
        Network.Myplayer.m_iPlayerUID = xmsg.m_nPlayerID;
        Network.playerid = xmsg.m_nPlayerID;

        if (xmsg.m_iHaveRole == false)
        {
            Network.gameState = Network.SGameState.CreateCharacter;
        }
        else
        {
            //Network.Instance.Send_EnterRoom();
            //Network.NetworkMode = true;
        }
    }

    public static void CMD_ROLE(NetworkMsg msg)
    {
        Res_roleInfo xmsg = new Res_roleInfo();
        using (MemoryStream stream = new MemoryStream(msg.data))
        {
            xmsg = ProtoBuf.Serializer.Deserialize<Res_roleInfo>(stream);
        }

        Network.roleinfo = xmsg;
        Network.role_res = xmsg.resultID;

        int val = xmsg.resultID;
        if (val == 0)
        {
            Network.name = xmsg.name;
            Network.exp = xmsg.exp;
            Network.gold = xmsg.gold;
            Network.diamond = xmsg.diamond;
            Network.sex = xmsg.sex;
            Network.Instance.Send_EnterRoom();
            //Network.NetworkMode = true;
        }
    }

    public static void CMD_ENTERROOM(NetworkMsg msg)
    {
        Res_enter xmsg = new Res_enter();
        using (MemoryStream stream = new MemoryStream(msg.data))
        {
            xmsg = ProtoBuf.Serializer.Deserialize<Res_enter>(stream);
        }
        Network.Myplayer.m_iRoomID = xmsg.m_iRoomID;
        CMD_ENTERROOM_result = xmsg.m_nResultID;

        
    }

    public static void CMD_MAP_MATCH(NetworkMsg msg)
    {
        Res_merge xmsg = new Res_merge();
        using (MemoryStream stream = new MemoryStream(msg.data))
        {
            xmsg = ProtoBuf.Serializer.Deserialize<Res_merge>(stream);
        }

        CMD_MAP_MATCH_result = xmsg.m_nresultid;
        if (CMD_MAP_MATCH_result == (int)ResultID.result_id_success)
        {
            gameInfo = xmsg;
        }
    }

    public static void CMD_SITDOWN(NetworkMsg msg)
    {
        SitDown xmsg = new SitDown();
        using (MemoryStream stream = new MemoryStream(msg.data))
        {
            xmsg = ProtoBuf.Serializer.Deserialize<SitDown>(stream);
        }

        List<bool> wxLogin = xmsg.wxLogin;
        List<string> wxHead = xmsg.wxHead;
        
        for(int i = 0; i < xmsg.m_iPlayerCount; i++)
        {
            Battle_wxLogin[i] = wxLogin[i];
            if(wxLogin[i] == true)
            {
                Battle_wxHead[i] = wxHead[i];
            }
        }

        CMD_MAP_MATCH_result = (int)ResultID.result_id_success;
        if (CMD_MAP_MATCH_result == (int)ResultID.result_id_success)
        {
            gameInfo_sitdown = xmsg;
            EventDispatcher.DispatchEvent("EventShowPrepareUI", null, null);
        }
        Network.seat = xmsg.m_cSeatID;
    }

    public static void CMD_PUSHGAMEDATA(NetworkMsg msg)
    {
        Res_game xmsg = new Res_game();
        using (MemoryStream stream = new MemoryStream(msg.data))
        {
            xmsg = ProtoBuf.Serializer.Deserialize<Res_game>(stream);
        }

        //Debug.Log("xue****sdid****:" + xmsg.m_iRoomID + ":" + xmsg.m_iTableID + ":" + xmsg.m_icmd + ":" + Enum.GetName(typeof(GameID), xmsg.m_icmd) + ":------" + Network.xue++);

        if (xmsg.m_icmd == (int)GameID.Ready)
        {
            Res_ready res = new Res_ready();
            using (MemoryStream stream = new MemoryStream(xmsg.gameData))
            {
                res = ProtoBuf.Serializer.Deserialize<Res_ready>(stream);
            }
            Network.battleinfo.m_iMapIndex = res.m_iMapID;
#if xue
            Network.battleinfo.m_eBattleType = BattleType.NORMAL_PVP;
            Network.battleinfo.m_iTimeLimit = GameDefine.INFINITE;
#endif
            int ma = res.m_iSeatSum;
            List<string> username = res.m_strName;
            List<int> teamid = res.m_iTeamID;
            List<int> playerid = res.m_aiSeatPlayer;

            for (int i = 0; i < ma; i++)
            {
                PlayerInfo p1 = new PlayerInfo();
                p1.m_strName = username[i];
                p1.m_iPlayerUID = playerid[i];
                p1.m_iTeam = teamid[i];
                Network.Pid_Tid.Add(playerid[i], teamid[i]);
#if xue
                //Debug.Log("xue*****************:" + ":name:" + username[i] + ":playerid:" + playerid[i] + "teamid" + teamid[i] + "mapid:" + res.m_iMapID);
#endif
                Network.battleinfo.m_listPlayers.Add(p1);

                if (Network.playerid == playerid[i])
                {
                    Network.playerindex = i;
                }
                else
                {
                    Network.playerid2 = playerid[i];
                }
            }

            CMD_MAP_MATCH_result = (int)ResultID.result_enter_game;
        }
        else if (xmsg.m_icmd == (int)GameID.Ready_Notify)
        {
            Notify_ready res = new Notify_ready();
            using (MemoryStream stream = new MemoryStream(xmsg.gameData))
            {
                res = ProtoBuf.Serializer.Deserialize<Notify_ready>(stream);
            }
            Network.seat_binary = (Network.seat_binary) | (1 << (res.m_iSeat));
            Network.GameReady_pic[res.m_iSeat] = res.m_iPic;
            Network.seat_res++;
        }
        else if (xmsg.m_icmd == (int)GameID.Choose)
        {
            Res_choose res = new Res_choose();
            using (MemoryStream stream = new MemoryStream(xmsg.gameData))
            {
                res = ProtoBuf.Serializer.Deserialize<Res_choose>(stream);
            }

            Network.battleinfo.m_iRoundLimit = res.m_iRound;
#if xue
            Network.battleinfo.m_eBattleType = BattleType.NORMAL_PVP;
            Network.battleinfo.m_iTimeLimit = GameDefine.INFINITE;
#endif
            int ma = Network.battleinfo.m_listPlayers.Count;
            List<int> tankid = res.m_iTankID;


            for (int i = 0; i < ma; i++)
            {
                Network.battleinfo.m_listPlayers[i].m_iInstructionID = tankid[i];
                Network.battleinfo.m_listPlayers[i].m_iHP = GameGOW.Get().DataMgr.GetCharacterInstructionByID(tankid[i]).m_iHP;
                Network.battleinfo.m_listPlayers[i].m_iMoveEnergy = GameGOW.Get().DataMgr.GetCharacterInstructionByID(tankid[i]).m_iMoveEnergy;
                Network.battleinfo.m_listPlayers[i].m_iAddEnergy = GameGOW.Get().DataMgr.GetCharacterInstructionByID(tankid[i]).m_iAddEnergy;

#if xue
                //Debug.Log("xue*****************:tankid:" + tankid[i]);
#endif
            }

            CMD_MAP_MATCH_result = (int)ResultID.result_choose_done;
        }
        else if (xmsg.m_icmd == (int)GameID.Choose_Notify)
        {
            Notify_choose res = new Notify_choose();
            using (MemoryStream stream = new MemoryStream(xmsg.gameData))
            {
                res = ProtoBuf.Serializer.Deserialize<Notify_choose>(stream);
            }

            //Debug.Log("xue^^^^^^^^^^^^^^^^^  " + res.m_iSeat + ":" + res.m_iTankID + "         " + Network.xue++);
            if (res.m_iTankID < 0)
            {
                Network.GameChoose_num[res.m_iSeat + 1] = (-res.m_iTankID) + 100;
            }
            else
            {
                Network.GameChoose_num[res.m_iSeat + 1] = res.m_iTankID;
                Network.GameChooseStatus = -1;
            }
        }
        else if (xmsg.m_icmd == (int)GameID.Load)
        {
            Res_loading res = new Res_loading();
            using (MemoryStream stream = new MemoryStream(xmsg.gameData))
            {
                res = ProtoBuf.Serializer.Deserialize<Res_loading>(stream);
            }

            int index = 1;
            for (int i = 0; i < Network.battleinfo.m_listPlayers.Count; i++)
            {
                if (Network.battleinfo.m_listPlayers[i].m_iPlayerUID == res.m_iOptID)
                {
                    index = i;
                    break;
                }
            }

            Network.GameLoading_num[index + 1] = res.m_ilen;

            if (res.m_ilen == 200)
            {
                Network.GameLoading = 10;
            }
            else if (Network.playerid == res.m_iOptID)
            {
                Network.GameLoading++;
            }
        }
        else if (xmsg.m_icmd == (int)GameID.Move)
        {
            Res_move res = new Res_move();
            using (MemoryStream stream = new MemoryStream(xmsg.gameData))
            {
                res = ProtoBuf.Serializer.Deserialize<Res_move>(stream);
            }

            CharacterLogic m_pCurrentPlayer = GameGOW.Get().CharacterMgr.GetCharacterByUid(res.m_iOptID);

            int type = res.m_iDirection;
            //var m_pCharacterLogic = GameGOW.Get().BattleMgr.m_pCurrentPlayer;

            m_pCurrentPlayer.OnDirectionKeyChanged(type);

            if (type == 0 || res.m_iIntervalTime == 1)
            {
                Vector2 v2 = new Vector2(res.m_iPosX, res.m_iPosY);
                m_pCurrentPlayer.Position = v2;
            }

        }
        else if (xmsg.m_icmd == (int)GameID.Round)
        {
            Res_round res = new Res_round();
            using (MemoryStream stream = new MemoryStream(xmsg.gameData))
            {
                res = ProtoBuf.Serializer.Deserialize<Res_round>(stream);
            }
            Network.Round = res.m_iRound;
            Network.Wind = res.m_iWind;
            Network.next_playerid = res.m_iOptID;

            if (res.m_iOptID != Network.playerid)
            {
                EventDispatcher.DispatchEvent("EventHideBattleJoyStickUI", null, null);
            }

            //技能冷却
            /*
            if (Network.playerid == GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[GameGOW.Get().BattleMgr.m_iCurrentRoundPlayerIndex].m_iPlayerUID)
            {
                if (SkillManager.CurrentSkillId != 0)
                {
                    Network.Skill_CD[SkillManager.CurrentSkillId]--;
                }

                for (int i = 0; i < 20; i++)
                {
                    if (i == SkillManager.CurrentSkillId)
                    {
                        if (Network.Skill_CD[i] == -1)
                        {
                            Network.Skill_CD[i] = 2;
                        }
                        continue;
                    }
                    else if (Network.Skill_CD[i] > 0)
                    {
                        Network.Skill_CD[i]--;
                    }
                    //Debug.Log("xue^^^^^" + i + "   " + Network.Skill_CD[i] + "   " + Network.xue++);
                }
            }
            */
            SkillManager.CurrentSkillId = 0;

            if (res.m_iOptID == Network.playerid)
            {
                EventDispatcher.DispatchEvent("EventShowBattleJoyStickUI", null, null);
            }
            Network.InitRound();

            GameGOW.Get().BattleMgr.ChangeController_Quick();
        }
        else if (xmsg.m_icmd == (int)GameID.Attach)
        {
            Res_attack res = new Res_attack();
            using (MemoryStream stream = new MemoryStream(xmsg.gameData))
            {
                res = ProtoBuf.Serializer.Deserialize<Res_attack>(stream);
            }

            int speed = res.m_iPower;
            float angle = res.m_iAngle;

            SkillManager.CurrentSkillId = res.m_iBombType;

            CharacterLogic m_pCurrentPlayer = GameGOW.Get().CharacterMgr.GetCharacterByUid(res.m_iOptID);

            //var m_pCharacterLogic = GameGOW.Get().BattleMgr.m_pCurrentPlayer;
            /**
            m_pCurrentPlayer.OnFunctionKeyDown(0x11);

            GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iBombSpeed = speed;
            GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iFireAngle = angle;
            Network.Attacking = 2;
            m_pCurrentPlayer.OnFunctionKeyUp(0x11);
    **/
            Network.Round = res.m_iRound;

            m_pCurrentPlayer.m_pInfo.m_iBombSpeed = speed;
            m_pCurrentPlayer.m_pInfo.m_iFireAngle = angle;
            Network.Attacking = 2;

            m_pCurrentPlayer.m_pInfo.m_bInBombSpeedUp = false;
            GameGOW.Get().PlaySkill((SkillManager.SkillType)SkillManager.CurrentSkillId, m_pCurrentPlayer, Network.attackbasedata);
        }
        else if (xmsg.m_icmd == (int)GameID.Boom)
        {
            Res_Boom res = new Res_Boom();
            using (MemoryStream stream = new MemoryStream(xmsg.gameData))
            {
                res = ProtoBuf.Serializer.Deserialize<Res_Boom>(stream);
            }

            float x = res.m_ix;
            float y = res.m_iy;

            SounderManager.Get().FunNet(res.BombID, res.Kind, x, y);
            /*
            GameGOW.Get().BombMgr.GetBombByUID(res.BombID).Boom_pos = new Vector2(x, y);
            Debug.Log("xue^^^^" + res.BombID + "   " + GameGOW.Get().BombMgr.GetBombByUID(res.BombID).Boom_pos.x + "    " +
                GameGOW.Get().BombMgr.GetBombByUID(res.BombID).Boom_pos.y + "    " + Network.xue++);
            if (res.Kind == false)
            {
                GameGOW.Get().BombMgr.GetBombByUID(res.BombID).Net_Boom_Status = 1;
                //BaseBomb.Net_Boom_Status = 1;
            }
            else
            {
                GameGOW.Get().BombMgr.GetBombByUID(res.BombID).Net_Boom_Status = 2;
                //BaseBomb.Net_Boom_Status = 2;
            }
            */
        }
        else if (xmsg.m_icmd == (int)GameID.Damage)
        {

            OnGame_HP res = new OnGame_HP();
            using (MemoryStream stream = new MemoryStream(xmsg.gameData))
            {
                res = ProtoBuf.Serializer.Deserialize<OnGame_HP>(stream);
            }

            //执行掉血
            DamageType eType = DamageType.NORMAL;
            CharacterLogic dstObject = GameGOW.Get().CharacterMgr.GetCharacterByUid(res.m_iPlayerID);
            int hp = dstObject.m_pInfo.m_iHP;
            float DD = res.hpValues;
            hp -= (int)DD;
            if (hp > dstObject.m_pInfo.m_iMaxHP)
            {
                hp = dstObject.m_pInfo.m_iMaxHP;
            }
            dstObject.OnDamage(hp, res.hpValues, eType);

        }
        else if (xmsg.m_icmd == (int)GameID.Die)
        {
            Notify_disconnect res = new Notify_disconnect();
            using (MemoryStream stream = new MemoryStream(xmsg.gameData))
            {
                res = ProtoBuf.Serializer.Deserialize<Notify_disconnect>(stream);
            }
            GameGOW.Get().CharacterMgr.GetCharacterByUid(res.m_iOptID).Dead();
        }
        else if (xmsg.m_icmd == (int)GameID.Disconnect)
        {
            Notify_disconnect res = new Notify_disconnect();
            using (MemoryStream stream = new MemoryStream(xmsg.gameData))
            {
                res = ProtoBuf.Serializer.Deserialize<Notify_disconnect>(stream);
            }
            //处理掉线
        }
        else if (xmsg.m_icmd == (int)GameID.Over)
        {
            Res_over res = new Res_over();
            using (MemoryStream stream = new MemoryStream(xmsg.gameData))
            {
                res = ProtoBuf.Serializer.Deserialize<Res_over>(stream);
            }

            if (res.m_iOverStatus == (int)GameResID.game_result_win)
            {
                GameGOW.Get().Winner_Id = 2;
                GameGOW.Get().BattleMgr.StopBattle();
            }
            else if (res.m_iOverStatus == (int)GameResID.game_result_fail)
            {
                GameGOW.Get().Winner_Id = 1;
                GameGOW.Get().BattleMgr.StopBattle();
            }
            else if (res.m_iOverStatus == (int)GameResID.game_result_exception)
            {
                EventDispatcher.DispatchEvent("EventBackHost", null, null);
            }
            else
            {
                GameGOW.Get().Winner_Id = 1;
                GameGOW.Get().BattleMgr.StopBattle();
            }

            Network.addgold = res.m_iGetGold;
            Network.addexp = res.m_iGetExp;
            Network.adddiamond = res.m_iGetDiamond;

            Network.gold += Network.addgold;
            Network.exp += Network.addexp;
            Network.diamond += Network.adddiamond;
            Add_Friend_List = new Dictionary<string, int>();
            Invite_Friend_List = new Dictionary<string, int>();
        }
        else if (xmsg.m_icmd == (int)GameID.Quit)
        {
            GameGOW.Get().BattleMgr.m_bIsInBattle = false;
        }
        else if (xmsg.m_icmd == (int)GameID.Chat)
        {
            Notify_chat res = new Notify_chat();
            using (MemoryStream stream = new MemoryStream(xmsg.gameData))
            {
                res = ProtoBuf.Serializer.Deserialize<Notify_chat>(stream);
            }

            if (res.m_iChatID >= 1000 && res.m_iChatID < 2000)
            {
                var info = GameGOW.Get().CharacterMgr.GetCharacterByUid(res.m_iOptID).m_pInfo;
                info.EmojiID = res.m_iChatID;
            }
            else
            {
                LuaHelper.Play_Sound(2, "sound/girlsound/girl_0" + (res.m_iChatID-2000));
                var info = GameGOW.Get().CharacterMgr.GetCharacterByUid(res.m_iOptID).m_pInfo;
                info.MessageID = res.m_iChatID;
            }
        }
    }

    public static void MSG_PVE_OVER(NetworkMsg msg)
    {
        Res_PVEover res = new Res_PVEover();
        using (MemoryStream stream = new MemoryStream(msg.data))
        {
            res = ProtoBuf.Serializer.Deserialize<Res_PVEover>(stream);
        }

        if (res.m_iOverStatus == (int)GameResID.game_result_win)
        {
            GameGOW.Get().Winner_Id = 2;
            GameGOW.Get().BattleMgr.StopBattle();
        }
        else if (res.m_iOverStatus == (int)GameResID.game_result_fail)
        {
            GameGOW.Get().Winner_Id = 1;
            GameGOW.Get().BattleMgr.StopBattle();
        }
        else if (res.m_iOverStatus == (int)GameResID.game_result_exception)
        {
            EventDispatcher.DispatchEvent("EventBackHost", null, null);
        }
        else
        {
            GameGOW.Get().Winner_Id = 1;
            GameGOW.Get().BattleMgr.StopBattle();
        }

        Network.addgold = res.m_iGetGold;
        Network.addexp = res.m_iGetExp;
        Network.adddiamond = res.m_iGetDiamond;

        Network.gold += Network.addgold;
        Network.exp += Network.addexp;
        Network.diamond += Network.adddiamond;
    }

    public static void MSG_FRIEND_ADD(NetworkMsg msg)
    {
        Friend_add_del res = new Friend_add_del();
        using (MemoryStream stream = new MemoryStream(msg.data))
        {
            res = ProtoBuf.Serializer.Deserialize<Friend_add_del>(stream);
        }
        if (res.ResultID == (int)ResultID.result_id_success)
        {
            //消息发送成功
        }
        else
        {
            //弹窗用户不存在或者非法
        }
    }

    public static void MSG_FRIEND_DEL(NetworkMsg msg)
    {
        Friend_add_del res = new Friend_add_del();
        using (MemoryStream stream = new MemoryStream(msg.data))
        {
            res = ProtoBuf.Serializer.Deserialize<Friend_add_del>(stream);
        }

        if (res.ResultID == (int)ResultID.result_id_success)
        {
            //消息发送成功
        }
        else
        {
            //消息发送失败
        }
    }

    public static void MSG_FRIEND_ARRAY(NetworkMsg msg)
    {
        Friend_array res = new Friend_array();
        using (MemoryStream stream = new MemoryStream(msg.data))
        {
            res = ProtoBuf.Serializer.Deserialize<Friend_array>(stream);
        }

        List<string> username = res.name;
        List<int> sex = res.sex;
        List<int> exp = res.exp;
        List<bool> online = res.online;
        List<bool> wxLogin = res.wxLogin;
        Network.Friend_cnt = res.friendCnt;
        for (int i = 0; i < Network.Friend_cnt; i++)
        {
            Friend_list[i].name = username[i];
            Friend_list[i].sex = sex[i];
            Friend_list[i].exp = exp[i];
            Friend_list[i].status = online[i] ? 1 : 0;
            Friend_list[i].wxLogin = wxLogin[i];
            if (Friend_list[i].wxLogin == true)
            {
                Friend_list[i].wxHead = res.wxHead[i];
            }
        }
        Network.mySort();
        Network.gameState = Network.SGameState.LoginOK;
        EventDispatcher.DispatchEvent("EventUpdateFriendList", null, null);
    }

    public static void MSG_FRIEND_ADD_NOTIFY(NetworkMsg msg)
    {
        Friend_addNotify res = new Friend_addNotify();
        using (MemoryStream stream = new MemoryStream(msg.data))
        {
            res = ProtoBuf.Serializer.Deserialize<Friend_addNotify>(stream);
        }

        Network.Add_Friend_Name = res.m_NameOther;
        Network.Added_Friend.sex = res.m_iSex;
        Network.Added_Friend.name = res.m_NameOther;
        Network.Added_Friend.status = res.online ? 1 : 0;
        Network.Added_Friend.wxLogin = res.wxLogin;
        if(res.wxLogin == true)
        {
            Network.Added_Friend.wxHead = res.wxHead;
        }

        //弹窗通知有人要加你为好友
        EventDispatcher.DispatchEvent("EventAddFriendNotify", null, null);
    }

    public static void MSG_FRIEND_DEL_NOTIFY(NetworkMsg msg)
    {
        Friend_delNotify res = new Friend_delNotify();
        using (MemoryStream stream = new MemoryStream(msg.data))
        {
            res = ProtoBuf.Serializer.Deserialize<Friend_delNotify>(stream);
        }

        //删除
        int index = 0;
        for (int i = 0; i < Network.Friend_cnt; i++)
        {
            if (Network.Friend_list[i].name == res.m_NameOther)
            {
                index = i;
                break;
            }
        }
        for (int i = index; i < Network.Friend_cnt - 1; i++)
        {
            Network.Friend_list[i].name = Network.Friend_list[i + 1].name;
            Network.Friend_list[i].sex = Network.Friend_list[i + 1].sex;
            Network.Friend_list[i].exp = Network.Friend_list[i + 1].exp;
            Network.Friend_list[i].status = Network.Friend_list[i + 1].status;
            Network.Friend_list[i].wxLogin = Network.Friend_list[i + 1].wxLogin;
            Network.Friend_list[i].wxHead = Network.Friend_list[i + 1].wxHead;
        }
        Network.Friend_cnt--;
        Network.mySort();
        EventDispatcher.DispatchEvent("EventUpdateFriendList", null, null);
    }

    public static void MSG_FRIEND_OPT_ADD(NetworkMsg msg)
    {
        Friend_resAddNotify res = new Friend_resAddNotify();
        using (MemoryStream stream = new MemoryStream(msg.data))
        {
            res = ProtoBuf.Serializer.Deserialize<Friend_resAddNotify>(stream);
        }

        if (res.resultID == (int)ResultID.result_id_success && res.accept == true)
        {
            if (Network.name == res.Name2)
            {
                Network.Friend_list[Network.Friend_cnt].name = Network.Find_Friend.name;
                Network.Friend_list[Network.Friend_cnt].sex = Network.Find_Friend.sex;
                Network.Friend_list[Network.Friend_cnt].exp = Network.Find_Friend.exp;
                Network.Friend_list[Network.Friend_cnt].status = Network.Find_Friend.status;
                Network.Friend_list[Network.Friend_cnt].wxLogin = Network.Find_Friend.wxLogin;
                if (Network.Find_Friend.wxLogin == true)
                {
                    Network.Friend_list[Network.Friend_cnt].wxHead = Network.Find_Friend.wxHead;
                }
                Network.Friend_cnt++;
            }
            else
            {
                //Network.Find_Friend.name = res.Name2;
                //Network.Find_Friend.sex = 1;
                //Network.Find_Friend.exp = 0;
                //Network.Find_Friend.status = 0;
                Network.Friend_list[Network.Friend_cnt].name = Network.Added_Friend.name;
                Network.Friend_list[Network.Friend_cnt].sex = Network.Added_Friend.sex;
                Network.Friend_list[Network.Friend_cnt].exp = Network.Added_Friend.exp;
                Network.Friend_list[Network.Friend_cnt].status = Network.Added_Friend.status;
                Network.Friend_list[Network.Friend_cnt].wxLogin = Network.Added_Friend.wxLogin;
                if (Network.Added_Friend.wxLogin == true)
                {
                    Network.Friend_list[Network.Friend_cnt].wxHead = Network.Added_Friend.wxHead;
                }
                Network.Friend_cnt++;
            }
            Network.mySort();
            EventDispatcher.DispatchEvent("EventUpdateFriendList", null, null);

        }
        else if(res.resultID == (int)ResultID.result_id_success && res.accept == false)   //拒绝
        {
            if (Network.name == res.Name2)
            {
                EventDispatcher.DispatchEvent("EventShowMessageUIwithRefuse", null, null);

            }
        }
    }

    public static void MSG_FRIEND_GETINFO(NetworkMsg msg)
    {
        Friend_resGetInfo res = new Friend_resGetInfo();
        using (MemoryStream stream = new MemoryStream(msg.data))
        {
            res = ProtoBuf.Serializer.Deserialize<Friend_resGetInfo>(stream);
        }


        if (res.resultID == (int)ResultID.result_id_success)
        {
            Find_Friend.name = res.m_NameOther;
            Find_Friend.sex = res.sex;
            Find_Friend.exp = res.exp;
            Find_Friend.game_cnt = res.battleCnt;
            Find_Friend.game_win = res.winCnt;
            Find_Friend.status = res.isOnline ? 1 : 0;

            if (Find_Friend.name == Network.name)
            {
                Find_Friend.relation = 0;
            }
            else
            {
                Find_Friend.relation = 2;
                for (int i = 0; i < Friend_cnt; i++)
                {
                    if (Find_Friend.name == Friend_list[i].name)
                    {
                        Find_Friend.relation = 1;
                        break;
                    }
                }
            }
            Find_Friend.wxLogin = res.wxLogin;
            if(res.wxLogin == true)
            {
                Find_Friend.wxHead = res.wxHead;
            }
            
            //弹窗显示信息
            EventDispatcher.DispatchEvent("EventShowInfomationUI", null, null);
            
        }
        else
        {
            //用户不存在或者输入不合法
            EventDispatcher.DispatchEvent("EventShowMessageUIwithString", null, null);
        }
    }

    public static void MSG_FRIEND_STATUS(NetworkMsg msg)
    {
        Friend_notifyStatus res = new Friend_notifyStatus();
        using (MemoryStream stream = new MemoryStream(msg.data))
        {
            res = ProtoBuf.Serializer.Deserialize<Friend_notifyStatus>(stream);
        }

        for(int i = 0; i < Friend_cnt; i++)
        {
            if(Friend_list[i].name == res.m_Name)
            {
                Friend_list[i].status = res.online ? 1 : 0;
                break;
            }
        }

        Network.mySort();
        EventDispatcher.DispatchEvent("EventUpdateFriendList", null, null);
    }


    /// <summary>
    /// 邀请相关
    /// </summary>
    public static void MSG_FRIEND_INVITATION(NetworkMsg msg)
    {
        Friend_invitation res = new Friend_invitation();
        using (MemoryStream stream = new MemoryStream(msg.data))
        {
            res = ProtoBuf.Serializer.Deserialize<Friend_invitation>(stream);
        }
        if (res.ResultID == (int)ResultID.result_id_success)
        {
            //消息发送成功
        }
        else
        {
            //弹窗用户不存在或者非法
        }
    }

    public static void MSG_FRIEND_INV_NOTIFY(NetworkMsg msg)
    {
        Friend_invNotify res = new Friend_invNotify();
        using (MemoryStream stream = new MemoryStream(msg.data))
        {
            res = ProtoBuf.Serializer.Deserialize<Friend_invNotify>(stream);
        }

        if (Invite_Friend_List.ContainsKey(res.m_NameOther))
        {
            return;
        }
        Invite_Friend_List.Add(res.m_NameOther, 0);

        Network.Invitation_name = res.m_NameOther;
        Network.Invitation_id = res.m_iOptIDother;
        Network.gamemode = BattleType.NORMAL_PVP;
        Network.Init();
        //弹窗通知有人要一起邀请你
        EventDispatcher.DispatchEvent("EventInvitationFriendNotify", null, null);
        //Network.Send_Invitation_Notify_Friend(true);
    }

    public static void MSG_FRIEND_OPT_INV(NetworkMsg msg)
    {
        Friend_resInvNotify res = new Friend_resInvNotify();
        using (MemoryStream stream = new MemoryStream(msg.data))
        {
            res = ProtoBuf.Serializer.Deserialize<Friend_resInvNotify>(stream);
        }

        if (res.resultID == (int)ResultID.result_id_success && res.accept == true)
        {
            //准备界面
            //Network.gamemode = BattleType.NORMAL_PVP;
            //EventDispatcher.DispatchEvent("EventShowPrepareUI", null, null);
        }
        else if (res.resultID == (int)ResultID.result_id_success && res.accept == false)   //拒绝
        {
            if (Network.playerid == res.m_iOptID2)
            {
                //弹窗被拒绝
                EventDispatcher.DispatchEvent("EventShowMessageUIwithRefuseInvitation", null, null);
            }
        }
    }

}