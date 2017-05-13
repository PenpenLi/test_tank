using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TKBase;
using TKGameView;
using UnityEngine;

namespace TKGame
{
    class CharacterAttackAction: BaseAction
    {
        public enum AttackState
        {
            PrepareAttack,
            InAttack,
            FinishAttack
        }

        private CharacterLogic m_pCharacter;
        private int m_iSpeedInterval;
        private int m_iSpeedFlag;          //控制力度条正方向/反方向移动

        private AttackState m_eAttackState;
        public AttackState _AttackState
        {
            get { return m_eAttackState; }
            set { m_eAttackState = value; }
        }
        
        public CharacterAttackAction()
            :base()
        {

        }

        public override bool Connect(BaseAction action)
        {
            if (!isFinished && (action is CharacterAttackAction || action is SelfCharacterWalkAction))
            {
                return true;
            }
            return false;
        }

        public override void Prepare()
        {
            if (_isPrepare) return;
            _isPrepare = true;
            m_eAttackState = AttackState.PrepareAttack;
            if (m_pCharacter == null)
            {
                Finish();
                return;
            }
        }

        public override void Execute()
        {
            //有剩余时间力度条才会增加减少，
            if(m_eAttackState == AttackState.PrepareAttack)
            {
                if (m_pCharacter.m_pInfo.m_bInBombSpeedUp && GameGOW.Get().BattleMgr.m_iCurrentRoundTime > 0)
                {
                    if(m_pCharacter.m_pInfo.m_iBombSpeed < 0)
                    {
                        m_iSpeedFlag = -m_iSpeedFlag;
                        m_pCharacter.m_pInfo.m_iBombSpeed = 0;
                    }
                    else if (m_pCharacter.m_pInfo.m_iBombSpeed > m_pCharacter.m_pInfo.m_pInstructionData.fire_range)
                    {
                        m_iSpeedFlag = -m_iSpeedFlag;
                        m_pCharacter.m_pInfo.m_iBombSpeed = m_pCharacter.m_pInfo.m_pInstructionData.fire_range;
                    }
                    else
                    {
                        m_pCharacter.m_pInfo.m_iBombSpeed += m_iSpeedInterval * m_iSpeedFlag;
                    }
                
                }
                else//时间到不开炮自动切换对象
                {
                    if (GameGOW.Get().BattleMgr.m_iCurrentRoundTime > 0 && m_pCharacter.IsLiving==true)   //
                    {
                        SounderManager.Get().PlaySound(1, "sound/shooting04");
                        m_pCharacter.Body.DoAction(CharacterStateType.ATTACK);
                        LOG.Log("CharacterStateType.ATTACK");
                        m_eAttackState = AttackState.InAttack;
                    }
                    else
                    {
                        Finish();
                    }
                }
            }
            else if(m_eAttackState == AttackState.InAttack)
            {
                if(m_pCharacter.m_pInfo.m_eCurrentStateType != CharacterStateType.ATTACK ||
                    m_pCharacter.m_pInfo.m_iCurrentFrame == m_pCharacter.m_pInfo.m_pCurrentStateData.total_frame)
                {
                    Finish();
                }
            }
        }

        private void SetSelfAttackFinish()
        {
            if (Network.NetworkMode == true)
            {
                //Network.Send_Attach(true);
            }
            else if (Network.gamemode == BattleType.AI_1v1)
            {
                /***
                if (GameGOW.Get().BattleMgr.m_pCurrentPlayer.ID == Network.playerid)
                {
                    EventDispatcher.DispatchEvent("EventHideBattleJoyStickUI", null, null);
                }
                else
                {
                    EventDispatcher.DispatchEvent("EventShowBattleJoyStickUI", null, null);
                }
                GameGOW.Get().BattleMgr.ChangeController_Quick();
                if(GameGOW.Get().BattleMgr.Get_InBattle() == true && GameGOW.Get().BattleMgr.m_iCurrentRoundPlayerIndex % 2 == 1)
                {
                    AIManager.Act_AI_1v1();
                }
                **/
            }
            else
            {
                //GameGOW.Get().BattleMgr.ChangeController();
            }
        }

        private void Finish()
		{
			_isFinished = true;
            m_eAttackState = AttackState.FinishAttack;
            if (m_pCharacter.m_pInfo.m_eCurrentStateType == CharacterStateType.ATTACK)
            {
                m_pCharacter.Body.DoAction(CharacterStateType.IDLE);
            }
            SetSelfAttackFinish();
        }

        #region(IPoolableObect接口)
        override public bool Initialize(object[] args)
        {
            base.Initialize(args);
            m_pCharacter = args[0] as CharacterLogic;
            m_iSpeedInterval =(int)Math.Ceiling(0.01 * m_pCharacter.m_pInfo.m_pInstructionData.fire_range);
            m_iSpeedFlag = 1;
            m_eAttackState = AttackState.FinishAttack;
            return true;
        }
        override public void UnInitialize()
        {
            base.UnInitialize();
        }
        #endregion
    }
}
