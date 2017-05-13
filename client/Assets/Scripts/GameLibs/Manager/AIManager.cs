using UnityEngine;
using System.Collections;
using TKBase;
using System;

namespace TKGame
{
    public class AIManager : ManagerBase, ITicker
    {
        private bool m_bIsStart = false;

        public AIManager()
            :base(ManagerType.AIManager)
        {
             
        }
        ~AIManager()
        {

        }

        private TickerFlag m_bIsInTickerList;
        public TickerFlag IsInTickerListFlag
        {
            set
            {
                m_bIsInTickerList = value;
            }
            get
            {
                return m_bIsInTickerList;
            }
        }
        // Update is called once per frame
        public void Tick(uint tickCount)
        {
            if (GameGOW.Get().BattleMgr.m_bIsInBattle == false)
            {
                GameGOW.Get().TickerMgr.RemoveKeyTicker(this);
                return;
            }
            if (false == m_bIsStart)
            {
                return;
            }

            if (ai_pre == true) {
                if (time_cnt <= 60)
                {
                    time_cnt++;
                    return;
                }
                else
                {
                    float ai_x = GameGOW.Get().CharacterMgr.GetCharacterByUid(GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[1].m_iPlayerUID).Position.x;
                    float en_x = GameGOW.Get().CharacterMgr.GetCharacterByUid(GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[0].m_iPlayerUID).Position.x;
                    if (ai_x - en_x <= 1e-5)
                    {
                        GameGOW.Get().BattleMgr.m_pCurrentPlayer.OnDirectionKeyChanged(0x8);
                    }
                    else
                    {
                        GameGOW.Get().BattleMgr.m_pCurrentPlayer.OnDirectionKeyChanged(0x4);
                    }
                    ai_pre = false;
                }
            }

            if (ai_move == true)
            {
                time_move++;

                float ai_x = GameGOW.Get().CharacterMgr.GetCharacterByUid(GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[1].m_iPlayerUID).Position.x;
                float en_x = GameGOW.Get().CharacterMgr.GetCharacterByUid(GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[0].m_iPlayerUID).Position.x;
                if (Math.Abs(ai_x - en_x) <= 200)
                {
                    GameGOW.Get().BattleMgr.m_pCurrentPlayer.OnDirectionKeyChanged(0x0);
                    ai_move = false;
                    float ai_y = GameGOW.Get().CharacterMgr.GetCharacterByUid(GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[1].m_iPlayerUID).Position.y;
                    float en_y = GameGOW.Get().CharacterMgr.GetCharacterByUid(GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[0].m_iPlayerUID).Position.y;
                    if (Math.Abs(ai_y - en_y) >= 200)
                    {
                        SkillManager.CurrentSkillId = (int)SkillManager.SkillType.Rocket;
                    }
                    return;
                }

                if (ai_x - en_x <= 1e-5)
                {
                    GameGOW.Get().BattleMgr.m_pCurrentPlayer.OnDirectionKeyChanged(0x8);
                }
                else
                {
                    GameGOW.Get().BattleMgr.m_pCurrentPlayer.OnDirectionKeyChanged(0x4);
                }

                if (time_move >= 90)
                {
                    GameGOW.Get().BattleMgr.m_pCurrentPlayer.OnDirectionKeyChanged(0x0);
                    ai_move = false;
                }
                return;
            }

            time_attack++;
            if(time_attack >= 30)
            {
                var m_pCharacterLogic = GameGOW.Get().BattleMgr.m_pCurrentPlayer;
                m_pCharacterLogic.OnAttackForceChange();
                m_pCharacterLogic.OnFunctionKeyDown(0x11);

                //GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iBombSpeed = 880;
                float speed = (GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_icanattackmin + GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_icanattackmax) / 2.0f;
                float ma = GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_pInstructionData.fire_range;
                GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iBombSpeed = (int)(ma * speed);
                float ai_y = GameGOW.Get().CharacterMgr.GetCharacterByUid(GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[1].m_iPlayerUID).Position.y;
                float en_y = GameGOW.Get().CharacterMgr.GetCharacterByUid(GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[0].m_iPlayerUID).Position.y;
                if (ai_y < en_y)
                {
                    GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iFireAngle = GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_pInstructionData.high_fire_angle;
                }
                else
                {
                    GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iFireAngle = GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_pInstructionData.low_fire_angle;
                }

                if (SkillManager.CurrentSkillId == (int)SkillManager.SkillType.Cure)
                {
                    GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iBombSpeed = 0;
                }
                m_pCharacterLogic.OnFunctionKeyUp(0x11);
                

                m_bIsStart = false;
                GameGOW.Get().TickerMgr.RemoveKeyTicker(this);
            }
            
        }

        public static bool ai_pre = false;
        public static int time_cnt = 0;
        public static int time_move = 0;
        public static bool ai_move;
        public static int time_attack = 0;
        public static bool ai_attack = true;

        public void Act_AI_1v1()
        {
            m_bIsStart = true;
            ai_pre = true;
            time_cnt = 0;
            time_move = 0;
            time_attack = 0;
            ai_move = true;
            ai_attack = true;


            if (GameGOW.Get().BattleMgr.m_iCurrentRound % 2 == 1)
            {
                SkillManager.CurrentSkillId = UnityEngine.Random.Range(1, 8);
                if(SkillManager.CurrentSkillId == (int)SkillManager.SkillType.Robot)
                {
                    SkillManager.CurrentSkillId++;
                }
            }
            

            GameGOW.Get().TickerMgr.AddKeyTicker(this);
        }

        IEnumerator Delay(float delay_time) //延迟发炮,delay_time 为延迟时间
        {
            yield return new WaitForSeconds(delay_time);
        }
    }
}
