using UnityEngine;
using TKBase;
using TKGameView;
namespace TKGame
{
    class SelfCharacterWalkAction : BaseAction
    {
        public static CharacterLogic m_pCharacter;
        private Vector2 m_pEnd;
        private int m_iCount;
        private const int SEND_MSG_COUNT = 20;
        public bool do_dead_act=false;
        public SelfCharacterWalkAction()
            :base()
        {
        }
        public static void Set_m_pCharacter(Vector2 p)
        {
            
            //Debug.Log("bomb vector " + p.x + "-" + p.y + "   Character " + GameGOW.Get().BattleMgr.m_pCurrentPlayer.Position.x + "-" + GameGOW.Get().BattleMgr.m_pCurrentPlayer.Position.y);
            bool flag = false;
            p.x = (int)p.x; p.y = (int)p.y;
            if(GameGOW.Get().MapMgr.IsEmpty((int)p.x, (int)p.y) && !GameGOW.Get().MapMgr.IsOutMap(p))
            {
                GameGOW.Get().BattleMgr.m_pCurrentPlayer.Position = p;
                flag = true;
            }
            for (int i=(int)p.x-30;i<=(int)p.x+30;i++)
            {
                if(flag)
                {
                    break;
                }
                for(int j=(int)p.y-30;j<=(int)p.y+30;j++)
                {
                    if(flag)
                    {
                        break;
                    }
                    Vector2 temp = new Vector2(i, j);
                    if (GameGOW.Get().MapMgr.IsOutMap(temp) == false && GameGOW.Get().MapMgr.IsEmpty(i, j))
                    {
                        GameGOW.Get().BattleMgr.m_pCurrentPlayer.Position = temp;
                        flag = true;
                    }
                }
            }
            
            
           // GameGOW.Get().BattleMgr.m_pCurrentPlayer.Position = p;
            GameGOW.Get().BattleMgr.m_pCurrentPlayer.Body.DoAction(CharacterStateType.IDLE1);
        }
        override public bool Connect(BaseAction action)
        {
            if (!_isFinished)
            {
                return action is SelfCharacterWalkAction;
            }
            else
            {
                return false;
            }
        }

        override public bool CanReplace(BaseAction action)
        {
            return action is CharacterAttackAction;
        }

        override public void Prepare()
        {
            m_pCharacter.StartMoving();
        }

        override public void Execute()
        {
            if (Network.NetworkMode == false)
            {
                if (m_pCharacter.m_pInfo.m_iDirectionKeys != 0 && //方向键已经按下
                    m_pCharacter.m_pInfo.m_iMoveEnergy > 0 && //还有移动能量
                    m_pCharacter.m_pInfo.m_bIsInRound
                    )
                {
                    Vector2 p = m_pCharacter.GetNextWalkPoint(m_pCharacter.m_pInfo.m_iFacing);
                    if (p != Vector2.zero)
                    {
                        --m_pCharacter.m_pInfo.m_iMoveEnergy;
                        m_pCharacter.Position = p;
                        m_pCharacter.Body.DoAction(CharacterStateType.WALK);
                        m_iCount++;
                        if (m_iCount >= SEND_MSG_COUNT)
                        {
                            SendAction();
                        }
                    }
                    else
                    {
                        SendAction();
                        Finish();
                        int tx = (int)m_pCharacter.Position.x + m_pCharacter.m_pInfo.m_iFacing * m_pCharacter.m_pInfo.m_pInstructionData.walk_speed_x;
                        if (m_pCharacter.CanMoveDirection(m_pCharacter.m_pInfo.m_iFacing) && m_pCharacter.CanStand(tx, (int)m_pCharacter.Position.y) == false)
                        {
                            MapManager pMap = GameGOW.Get().MapMgr;
                            p = pMap.FindYLineNotEmptyPointUp(tx, (int)m_pCharacter.Position.y + m_pCharacter.m_pInfo.m_pInstructionData.walk_speed_y,
                                (int)pMap.m_stBound.height);
                            if (p != Vector2.zero)
                            {
                                BaseAction act = ObjectPools.CheckOut<CharacterFallingAction>(m_pCharacter, p, true, false);
                                m_pCharacter.Act(act);
                            }
                            else
                            {
                                p = new Vector2(tx, pMap.m_stBound.yMin);
                              
                                
                                BaseAction act = ObjectPools.CheckOut<CharacterFallingAction>(m_pCharacter, p, false, false);
                                m_pCharacter.go_down = true;
                                m_pCharacter.Act(act);
                            }
                        }
                    }
                }
                else
                {
                    if (m_pCharacter.m_pInfo.m_iMoveEnergy <= 0)
                    {

                    }
                    SendAction();
                    Finish();
                }
            }
            else
            {
                if (m_pCharacter.m_pInfo.m_iDirectionKeys != 0 && //方向键已经按下
                    m_pCharacter.m_pInfo.m_iMoveEnergy > 0  //还有移动能量
                    )
                {
                    Vector2 p = m_pCharacter.GetNextWalkPoint(m_pCharacter.m_pInfo.m_iFacing);
                    if (p != Vector2.zero)
                    {
                        --m_pCharacter.m_pInfo.m_iMoveEnergy;
                        m_pCharacter.Position = p;
                        m_pCharacter.Body.DoAction(CharacterStateType.WALK);
                        m_iCount++;
                        if (m_iCount >= SEND_MSG_COUNT)
                        {
                            SendAction();
                        }
                    }
                    else if (Network.gamemode != BattleType.AI_1v1 || GameGOW.Get().BattleMgr.m_pCurrentPlayer.ID == Network.playerid)
                    {
                        SendAction();
                        Finish();
                        int tx = (int)m_pCharacter.Position.x + m_pCharacter.m_pInfo.m_iFacing * m_pCharacter.m_pInfo.m_pInstructionData.walk_speed_x;
                        if (m_pCharacter.CanMoveDirection(m_pCharacter.m_pInfo.m_iFacing) && m_pCharacter.CanStand(tx, (int)m_pCharacter.Position.y) == false)
                        {
                            MapManager pMap = GameGOW.Get().MapMgr;
                            p = pMap.FindYLineNotEmptyPointUp(tx, (int)m_pCharacter.Position.y + m_pCharacter.m_pInfo.m_pInstructionData.walk_speed_y,
                                (int)pMap.m_stBound.height);
                            if (p != Vector2.zero)
                            {
                                BaseAction act = ObjectPools.CheckOut<CharacterFallingAction>(m_pCharacter, p, true, false);
                                m_pCharacter.Act(act);
                            }
                            else
                            {
                                p = new Vector2(tx, pMap.m_stBound.yMin);
                                BaseAction act = ObjectPools.CheckOut<CharacterFallingAction>(m_pCharacter, p, false, false);
                                m_pCharacter.go_down = true;
                                m_pCharacter.Act(act);
                            }
                        }
                    }
                }
                else
                {
                    if (m_pCharacter.m_pInfo.m_iMoveEnergy <= 0)
                    {

                    }
                    SendAction();
                    Finish();
                }
            }
        }

        private void SendAction()
		{
			//GameInSocketOut.sendGameStartMove(0,_player.x,_player.y,_player.info.direction,_player.isLiving,_player.map.currentTurn);
			//发送到服务器广播
			m_iCount = 0;
		}

        private void Finish()
        {
            m_pCharacter.StopMoving();
            if (m_pCharacter.go_down ==false ) 
            m_pCharacter.Body.DoAction(CharacterStateType.IDLE);
            else
            {
                m_pCharacter.Body.DoAction(CharacterStateType.DEAD);
                m_pCharacter.go_down = false;
            }
              
            _isFinished = true;
        }

        #region(IPoolableObect接口)
        override public bool Initialize(object[] args)
        {
            base.Initialize(args);
            m_pCharacter = args[0] as CharacterLogic;
            m_pEnd = Vector2.zero;
            m_iCount = 0;
            return true;
        }
        override public void UnInitialize()
        {
            base.UnInitialize();
        }
        #endregion


        
    }
}
