#define xue

using System.Collections.Generic;
using System;
using TKBase;
using UnityEngine;
using TKGameView;

namespace TKGame
{
    public enum BattleType
    {
        NORMAL_PVE,
        NORMAL_PVP,

        PVP_2V2,
        PVP_3V3,
        AI_1v1,
    }

    public class PlayerInfo
    {
        public int m_iPlayerUID;
        public int m_iTeam;
        public string m_strName;
        public int m_iInstructionID;
        public int m_iHP;
        public int m_iMoveEnergy;
        public int m_iAddEnergy;
        public int m_iRoomID;
    }

    public class TeamResult
    {
        public int m_iTeam;
        public int m_iRank;
        public int m_iAlivePlayer;
        public int m_iLastDeadTick;
        public int m_iSumHP;
        public HashSet<int> m_hsPlayerUIDs;
    }
    public class PlayerResult
    {
        public int m_iUID;
        public int m_iRank;
        public int m_iRemainBlood;
        public int m_iTotalDamage;
        public int m_iTeam;
        public int m_iDeadTick;
        public void Reset()
        {
            m_iUID = -1;
            m_iRemainBlood = -1;
            m_iTotalDamage = 0;
            m_iRank = 0;
        }
    }

    public class BattleInfo
    {
        public int m_iMapIndex; //地图
        public BattleType m_eBattleType; //战斗类型
        public int m_iRoundLimit; //回合限制
        public int m_iTimeLimit; //时间限制 单位秒
        public List<PlayerInfo> m_listPlayers;
        public BattleInfo()
        {
            m_listPlayers = new List<PlayerInfo>();
        }
    }



    public class BattleManager : ManagerBase, ITicker
    {
        public BattleInfo m_stBattleInfo;
        private int m_iPlayerCount;
        public int m_iCurrentRound;

        public int m_iCurrentRoundPlayerIndex = 0;
        public CharacterLogic m_pCurrentPlayer;

        public const int ROUND_TIME = 1200;
        public int m_iCurrentRoundTime = ROUND_TIME;

        public bool m_bIsInBattle = false;
        public bool Get_InBattle()
        {
            return m_bIsInBattle;
        }

        private Dictionary<int, TeamResult> m_dicTeamResult;
        private Dictionary<int, PlayerResult> m_dicPlayerResult;

        private int m_iBattleStartTick;
        private int m_iBattleEndTick;

        private int m_iBossUID;

        public BattleManager()
            :base(ManagerType.BattleManager)
        {

        }
        public override void Initialize()
        {
            base.Initialize();
            GameGOW.Get().TickerMgr.AddFixTicker(this);
            m_dicTeamResult = new Dictionary<int, TeamResult>();
            m_dicPlayerResult = new Dictionary<int, PlayerResult>();
        }

        public override void UnInitialize()
        {
            base.UnInitialize();
            m_dicTeamResult = null;
            m_dicPlayerResult = null;
        }

        public void Clear()
        {
            m_dicPlayerResult.Clear();
            m_dicTeamResult.Clear();

            m_bIsInBattle = false;
            m_stBattleInfo = null;
            m_iCurrentRoundPlayerIndex = 0;
            m_iCurrentRoundTime = ROUND_TIME;
            m_pCurrentPlayer = null;
            m_iPlayerCount = 0;
            m_iBattleStartTick = 0;
            m_iBattleEndTick = 0;
            m_iBossUID = 0;
            m_iWaitTime = 0;
        }

        public void InputKeyDown(int key)
        {
            if(key < InputDefine.MOVE_DOWN)
            {
                m_pCurrentPlayer.OnAttackAngleChange(key);
            }
            else if(key < InputDefine.MOVE_RIGHT)
            {
                m_pCurrentPlayer.OnDirectionKeyChanged(key);
            }
            else
            {
                m_pCurrentPlayer.OnFunctionKeyDown(key);
            }
        }

        public void InputKeyUp(int key)
        {
            if (key < InputDefine.MOVE_DOWN)
            {
                m_pCurrentPlayer.OnAttackAngleChange(key);
            }
            else if (key < InputDefine.MOVE_RIGHT)
            {
                m_pCurrentPlayer.OnDirectionKeyChanged(key);
            }
            else
            {
                m_pCurrentPlayer.OnFunctionKeyUp(key);
            }
        }

        private void InitPlayerInfo(MapData pMapData)
        {
            Dictionary<int, int> m_dicTeamCount = new Dictionary<int, int>();
            for (int i = 0; i < m_stBattleInfo.m_listPlayers.Count; ++i)
            {
                PlayerInfo playerInfo = m_stBattleInfo.m_listPlayers[i];
                CharacterInstructionData pData = GameGOW.Get().DataMgr.GetCharacterInstructionByID(playerInfo.m_iInstructionID);
                //pData.default_name = playerInfo.m_strName;
                CharacterLogic logic = GameGOW.Get().CharacterMgr.CreateCharacter(playerInfo.m_iPlayerUID, pData);

                if (!m_dicTeamCount.ContainsKey(playerInfo.m_iTeam))
                {
                    m_dicTeamCount[playerInfo.m_iTeam] = 0;
                }
                BornData pBornData = pMapData.GetBornData(playerInfo.m_iTeam, m_dicTeamCount[playerInfo.m_iTeam]++);
                logic.Position = new UnityEngine.Vector2(pBornData.pos_x, pBornData.pos_y);
                logic.m_pInfo.m_iFacing = pBornData.face;
                logic.m_pInfo.m_iMoveEnergy = playerInfo.m_iMoveEnergy;
                logic.m_pInfo.m_iAddEnergy = playerInfo.m_iAddEnergy;
                logic.m_pInfo.m_iMaxMoveEnergy = playerInfo.m_iMoveEnergy;
                logic.m_pInfo.m_bIsInRound = m_iCurrentRoundPlayerIndex == i;
                logic.m_pInfo.m_iTeam = playerInfo.m_iTeam;
                logic.m_pInfo.m_iHP = playerInfo.m_iHP;
                logic.m_pInfo.m_iMaxHP = playerInfo.m_iHP;
                AddPlayerResult(playerInfo.m_iPlayerUID, playerInfo.m_iTeam);
            }
            m_iPlayerCount = m_stBattleInfo.m_listPlayers.Count;
        }

        private void AddEvents()
        {
            EventDispatcher.AddListener("EventCharacterDead", OnCharacterDead);
        }

        private void RemoveEvents()
        {
            EventDispatcher.RemoveListener("EventCharacterDead", OnCharacterDead);
        }
        public void StartBattle(BattleInfo info, MapData pMapData)
        {

            m_iCurrentRoundPlayerIndex = 0;
            m_stBattleInfo = info;
            m_iCurrentRound = 0;
            m_iWaitTime = 0;

            InitPlayerInfo(pMapData);

            m_pCurrentPlayer = GameGOW.Get().CharacterMgr.GetCharacterByUid(info.m_listPlayers[m_iCurrentRoundPlayerIndex].m_iPlayerUID);

            m_iCurrentRoundTime = ROUND_TIME; //info.m_iRoundLimit * 60; 

            AddEvents();
            m_bIsInBattle = true;
            EventDispatcher.DispatchEvent("EventBattleStart", this, new ChangeControllerEvent(info.m_listPlayers[m_iCurrentRoundPlayerIndex].m_iPlayerUID));
        }

        /// <summary>
        /// 结束战斗
        /// </summary>
        /// <param name="needCalcResult">是否需要计算结果, 玩家主动退出情况不需要计算结果</param>
        public void StopBattle(bool needCalcResult = true)
        {
         
            if (needCalcResult)
            {
                _CheckFinalBattleResult();
            }
            if (GameGOW.Get().Winner_Id == 2)
            {
                SounderManager.Get().PlaySound(1, "sound/win01");
            }
            else
            {
                SounderManager.Get().PlaySound(1, "sound/shibai_09");
            }
            RemoveEvents();

            m_bIsInBattle = false;

            m_stBattleInfo = null;
            m_iCurrentRound = 0;

            m_iBattleEndTick = (int)GameGOW.Get().TickerMgr.TickCount;

 
            EventDispatcher.DispatchEvent("EventBattleStop", this, null);
        }

        private TickerFlag m_bIsInTickerList = TickerFlag.TICK_FLAG_NONE;
        public TickerFlag IsInTickerListFlag
        {
            set { m_bIsInTickerList = value; }
            get { return m_bIsInTickerList; }
        }

        private int m_iWaitTime = 0;

        public void Tick(uint tickCount)
        {
            if (!m_bIsInBattle)
            {
                return;
            }

            if (tickCount - m_iBattleStartTick > m_stBattleInfo.m_iTimeLimit)
            {
                if (Network.gamemode == BattleType.AI_1v1)
                {
                    GameGOW.Get().BattleMgr.m_bIsInBattle = false;
                    //StopBattle();
                }
                else if (Network.NetworkMode == true)
                {
                    //GameGOW.Get().BattleMgr.m_bIsInBattle = false;
                    //Network.Send_Over(Network.Pid_Tid[Network.playerid], GameGOW.Get().Winner_Id != Network.Pid_Tid[Network.playerid]);
                }
                else
                {
                    StopBattle();
                }
            }

            if(m_iWaitTime >= 0)
            {
                --m_iWaitTime;
                if(m_iWaitTime == 0)
                {
                    DoChangeController();
                }
            }

            if (Network.gamemode == BattleType.AI_1v1) {
                if (Network.Attacking != 2)
                {
                    --m_iCurrentRoundTime;
                }
            }
            else if (Network.NetworkMode == true)
            {
                if (Network.Attacking != 2)
                {
                    --m_iCurrentRoundTime;
                }
            }
            else
            {
                --m_iCurrentRoundTime;
            }
            if(m_iCurrentRoundTime>60 && m_iCurrentRoundTime <=360 && m_iCurrentRoundTime%60==0 && m_bIsInBattle)
            {
               // Debug.Log("++++++++++++++++++++++++++++++++++++++++++++"+m_bIsInBattle);
                SounderManager.Get().PlaySound(3,"sound/yinxiao/08");
            }

            if(m_iCurrentRoundTime <= 60)
            {//时间到, 切换行动对象
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
                    else if(Network.Attacking == 1)
                    {
                        Network.Attacking = 2;
                        GameGOW.Get().BattleMgr.m_pCurrentPlayer.OnFunctionKeyUp(0x11);
                    }
                }
                if (Network.NetworkMode == true)
                {
                    int index = GameGOW.Get().BattleMgr.m_iCurrentRoundPlayerIndex;
                    int id = GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[index].m_iPlayerUID;
                    if (Network.playerid == id && Network.Moving == false && Network.PreMove != 0)
                    {
                        Network.Moving = true;
                        var m_pCharacterLogic = GameGOW.Get().BattleMgr.m_pCurrentPlayer;
                        Network.Send_Move(false, 0, 0, m_pCharacterLogic.Position.x, m_pCharacterLogic.Position.y);
                    }

                    if (Network.playerid == id && Network.Attacking == 1)
                    {
                        Network.Attacking = 2;
                        var m_pCharacterLogic = GameGOW.Get().BattleMgr.m_pCurrentPlayer;
                        Network.Send_Attach(false);
                    }
                }
                else
                {
                    ChangeController();
                }
            }
        }

        public void ChangeController_Quick()
        {
            m_pCurrentPlayer.m_pInfo.m_bIsInRound = false;
            DoChangeController();
        }

        public void ChangeController()
        {
            Debug.Log("HHHHHHHHHHHHHH                  0"+"here");
            m_pCurrentPlayer.m_pInfo.m_bIsInRound = false;
            m_iWaitTime = 1;
        }

        private void DoChangeController()
        {
            Debug.Log("HHHHHHHHHHHHHH                  1" + "here");
            if (!m_bIsInBattle)
            {
                return;
            }

            m_iCurrentRoundTime = ROUND_TIME;
            m_iCurrentRoundPlayerIndex++;

            GameGOW.Get().MapMgr.RandomWind();
          //  Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"+ GameGOW.Get().MapMgr.WindPercent+"BBBBBBBBBBB"+Network.Wind);
            GameGOW.Get().PlayWind(2, -90f * GameGOW.Get().MapMgr.WindPercent, true);

            //   Debug.Log("CCCCCC  Wind wind_range wind_step "+ GameGOW.Get().MapMgr.WindPercent+" "+  GameGOW.Get().MapMgr.Wind + " "+ GameGOW.Get().MapMgr.wind_range + "  "+ GameGOW.Get().MapMgr.wind_step );


            if (m_iCurrentRoundPlayerIndex == m_iPlayerCount)
            {
                m_iCurrentRoundPlayerIndex = 0;
                m_iCurrentRound++;
            }
#if xue
            while (m_stBattleInfo != null && m_stBattleInfo.m_listPlayers != null && GameGOW.Get().CharacterMgr.GetCharacterByUid(m_stBattleInfo.m_listPlayers[m_iCurrentRoundPlayerIndex].m_iPlayerUID) != null
                && GameGOW.Get().CharacterMgr.GetCharacterByUid(m_stBattleInfo.m_listPlayers[m_iCurrentRoundPlayerIndex].m_iPlayerUID).IsLiving == false)
            {
                m_iCurrentRoundPlayerIndex++;
                if (m_iCurrentRoundPlayerIndex == m_iPlayerCount)
                {
                    m_iCurrentRoundPlayerIndex = 0;
                    m_iCurrentRound++;
                }
            }
#endif

            int uid;
            if (Network.gamemode == BattleType.AI_1v1)
            {
                uid = m_stBattleInfo.m_listPlayers[m_iCurrentRoundPlayerIndex].m_iPlayerUID;
            }
            else if (Network.NetworkMode == true)
            {
                uid = Network.next_playerid;
                for(int i = 0; i < GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers.Count; i++)
                {
                    if(GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[i].m_iPlayerUID == uid)
                    {
                        m_iCurrentRoundPlayerIndex = i;
                        break;
                    }
                }
            }
            else
            {
                uid = m_stBattleInfo.m_listPlayers[m_iCurrentRoundPlayerIndex].m_iPlayerUID;
            }

            SkillManager.CurrentSkillId = 0;
            Network.InitRound();
            m_pCurrentPlayer = GameGOW.Get().CharacterMgr.GetCharacterByUid(uid);
            m_pCurrentPlayer.m_pInfo.m_bIsInRound = true;
            //m_pCurrentPlayer.m_pInfo.m_iMoveEnergy = m_pCurrentPlayer.m_pInfo.m_iMaxMoveEnergy;
            m_pCurrentPlayer.m_pInfo.m_iMoveEnergy = Math.Min(m_pCurrentPlayer.m_pInfo.m_iMaxMoveEnergy, 
            m_pCurrentPlayer.m_pInfo.m_iMoveEnergy + m_pCurrentPlayer.m_pInfo.m_iAddEnergy);
            EventDispatcher.DispatchEvent("EventChangeController", null, new ChangeControllerEvent(uid));

        }
        /*
        public List<CharacterLogic> GetPlayerByTeam()
        {
            //m_stBattleInfo.m_listPlayers[i].m_iTeam
             GameGOW.Get().CharacterMgr.GetCharacterByUid()
        }
        */
#region (战斗结算)
        private void AddPlayerResult(int uid, int team)
        {
            if (!m_dicTeamResult.ContainsKey(team))
            {
                TeamResult teamResult = new TeamResult();
                teamResult.m_iTeam = team;
                teamResult.m_iRank = 0;
                teamResult.m_iLastDeadTick = m_stBattleInfo.m_iTimeLimit;
                teamResult.m_hsPlayerUIDs = new HashSet<int>();
                teamResult.m_hsPlayerUIDs.Add(uid);
                teamResult.m_iAlivePlayer = teamResult.m_hsPlayerUIDs.Count;
                m_dicTeamResult[team] = teamResult;
            }
            else
            {
                TeamResult teamResult = m_dicTeamResult[team];
                teamResult.m_hsPlayerUIDs.Add(uid);
                teamResult.m_iAlivePlayer = teamResult.m_hsPlayerUIDs.Count;
            }

            PlayerResult playerResult = new PlayerResult();
            playerResult.Reset();
            playerResult.m_iUID = uid;
            playerResult.m_iTeam = team;
            playerResult.m_iDeadTick = m_stBattleInfo.m_iTimeLimit;
            m_dicPlayerResult[uid] = playerResult;
        }

        public void OnCharacterDead(object sender, EventArgs e)
        {
            if (!m_bIsInBattle)
                return;

            CharacterDeadEvent evt = e as CharacterDeadEvent;
            if(evt != null)
            {
                if(_IsPvEBossDead(evt.m_iUniqueID))
                {
                    if (Network.gamemode == BattleType.AI_1v1)
                    {
                        GameGOW.Get().BattleMgr.m_bIsInBattle = false;
                        //StopBattle();
                    }
                    else if (Network.NetworkMode == true)
                    {
                        //GameGOW.Get().BattleMgr.m_bIsInBattle = false;
                        //Network.Send_Over(Network.Pid_Tid[Network.playerid], GameGOW.Get().Winner_Id != Network.Pid_Tid[Network.playerid]);
                    }
                    else
                    {
                        StopBattle();
                    }
                    return;
                }

                if (m_dicPlayerResult.ContainsKey(evt.m_iUniqueID))
                {
                    PlayerResult playerResult = m_dicPlayerResult[evt.m_iUniqueID];
                    playerResult.m_iDeadTick = (int)GameGOW.Get().TickerMgr.TickCount;

                    TeamResult teamResult = m_dicTeamResult[playerResult.m_iTeam];
                    teamResult.m_iAlivePlayer--;
                    if (teamResult.m_iAlivePlayer == 0)
                    {
                        teamResult.m_iLastDeadTick = playerResult.m_iDeadTick;
                    }

                    if (m_stBattleInfo.m_eBattleType == BattleType.NORMAL_PVE)
                    {
                        if (teamResult.m_iAlivePlayer == 0)
                        {
                            if (Network.gamemode == BattleType.AI_1v1)
                            {
                                GameGOW.Get().BattleMgr.m_bIsInBattle = false;
                                //StopBattle();
                            }
                            else if (Network.NetworkMode == true)
                            {
                                GameGOW.Get().BattleMgr.m_bIsInBattle = false;
                                //Network.Send_Over(Network.Pid_Tid[Network.playerid], GameGOW.Get().Winner_Id != Network.Pid_Tid[Network.playerid]);
                            }
                            else
                            {
                                StopBattle();
                            }
                        }
                    }
                    else
                    {
                        if (_IsPvPBattleEnd())
                        {
                            if (Network.gamemode == BattleType.AI_1v1)
                            {
                                //StopBattle();
                                GameGOW.Get().BattleMgr.m_bIsInBattle = false;
                            }
                            else if (Network.NetworkMode == true)
                            {
                                //GameGOW.Get().BattleMgr.m_bIsInBattle = false;
                                //Network.Send_Over(Network.Pid_Tid[Network.playerid], GameGOW.Get().Winner_Id != Network.Pid_Tid[Network.playerid]);
                            }
                            else
                            {
                                StopBattle();
                            }
                        }
                    }
                }
            }
        }

        private bool _IsPvEBossDead(int uid)
        {
            if(uid == m_iBossUID && 
                m_stBattleInfo.m_eBattleType == BattleType.NORMAL_PVE)
            {
                return true;
            }
            return false;
        }

        private bool _IsPvPBattleEnd()
        {
            int aliveTeam = 0;
            foreach(var stTeamResult in m_dicTeamResult)
            {
                if(stTeamResult.Value.m_iAlivePlayer > 0)
                {
                    aliveTeam++;
                }
            }
            return aliveTeam <= 1;
        } 

        private void _CheckFinalBattleResult()
        {
            if(m_stBattleInfo.m_eBattleType == BattleType.NORMAL_PVE)
            {
                _CalcPvEBattleResult();
            }
            else if(m_stBattleInfo.m_eBattleType == BattleType.NORMAL_PVP)
            {
                _CalcPvPBattleResult();
            }
            else
            {
                _CalcPvPBattleResult();
            }
        }

        private int _GetPlayerHp(int uid)
        {
            var stPlayer = GameGOW.Get().CharacterMgr.GetCharacterByUid(uid);
            if(stPlayer != null)
            {
                return stPlayer.m_pInfo.m_iHP;
            }
            else
            {
                return 0;
            }
        }

        private void _CalcPvEBattleResult()
        {
            foreach(var player in m_dicPlayerResult)
            {
                player.Value.m_iRank = m_dicTeamResult[player.Value.m_iTeam].m_iAlivePlayer > 0 ? 1 : 2;
                player.Value.m_iRemainBlood = _GetPlayerHp(player.Value.m_iUID);
            }
        }


        static int CompareTeamResult(TeamResult a, TeamResult b)
        {
            if (a.m_iLastDeadTick != b.m_iLastDeadTick)
            {
                return a.m_iLastDeadTick - b.m_iLastDeadTick;
            }

            if (a.m_iAlivePlayer != b.m_iAlivePlayer)
            {
                return a.m_iAlivePlayer - b.m_iAlivePlayer;
            }

            if (a.m_iSumHP != b.m_iSumHP)
            {
                return a.m_iSumHP - b.m_iSumHP;
            }

            return a.m_hsPlayerUIDs.GetEnumerator().Current - b.m_hsPlayerUIDs.GetEnumerator().Current;
        }

        private void _CalcPvPBattleResult()
        {
            List<TeamResult> lstTeamResult = new List<TeamResult>();
            foreach(var teamResult in m_dicTeamResult)
            {
                teamResult.Value.m_iSumHP = 0;
                foreach(var uid in teamResult.Value.m_hsPlayerUIDs)
                {
                    int hp = _GetPlayerHp(uid);
                    m_dicPlayerResult[uid].m_iRemainBlood = hp;
                    teamResult.Value.m_iSumHP += hp;
                }
                lstTeamResult.Add(teamResult.Value);
            }

            lstTeamResult.Sort(CompareTeamResult);

            int rank = 1;
            foreach(var teamResult in m_dicTeamResult)
            {
                teamResult.Value.m_iRank = rank;
                rank++;
            }

            foreach(var playerResult in m_dicPlayerResult)
            {
                playerResult.Value.m_iRank = m_dicTeamResult[playerResult.Value.m_iTeam].m_iRank;
            }
        }
        #endregion
    }

}