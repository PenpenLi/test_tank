using UnityEngine;

namespace TKGame
{
    class CharacterFallingAction : BaseAction
    {
        private CharacterLogic m_pCharacter;
        private Vector2 m_pTarget;
        private bool m_bIsLiving;
        private bool m_bCanIgnore;
        public CharacterFallingAction()
            :base()
        {
        }

        override public bool Connect(BaseAction action)
		{
			CharacterFallingAction ac = action as CharacterFallingAction;
			if(ac != null && ac.m_pTarget.y > m_pTarget.y)
			{
				//过滤掉掉落点比现在点高的FallingAction
				return true;
			}
			return false;
		}

        override public bool CanReplace(BaseAction action)
		{
			if(action is CharacterWalkAction)
			{
				if(m_bCanIgnore)
				{
					return true;
				}
			}
			return false;
		}
		
		override public void Prepare()
		{
			if(_isPrepare) return;
			_isPrepare = true;
			if(m_pCharacter.IsLiving)
			{
				if(m_pCharacter.Position.x == m_pTarget.x || !m_bCanIgnore)
				{
                    m_pCharacter.StartMoving();
				}
				else
				{
                    Finish();
				}
			}
			else
			{
                Finish();
			}
		}

		override public void Execute()
		{
            if (Mathf.Abs(m_pTarget.y - m_pCharacter.Position.y) <= Mathf.Abs(GameGOW.Get().MapMgr.Gravity))
            {
                ExecuteAtOnce();
			}
			else
			{
				m_pCharacter.Position = new Vector2(m_pTarget.x, m_pCharacter.Position.y + GameGOW.Get().MapMgr.Gravity*0.5f);
			}
		}
		
		override public void ExecuteAtOnce()
		{
			base.ExecuteAtOnce();
            m_pCharacter.Position = m_pTarget;
//			_player.needFocus();
			if(!m_bIsLiving)
			{
                if (m_pCharacter.IsLiving == true)
                {
                    if (Network.gamemode == BattleType.AI_1v1)
                    {
                        m_pCharacter.Dead();
                    }
                    else if (Network.NetworkMode == true)
                    {
                        if (Network.playerid == m_pCharacter.ID)
                        {
                            Network.Send_Over(Network.Pid_Tid[m_pCharacter.ID], false);
                        }
                    }
                    else
                    {
                        m_pCharacter.Dead();
                    }
                    
                }
                if(Network.NetworkMode == false)
                {
                    GameGOW.Get().BattleMgr.ChangeController();
                }
            }

            Finish();

        }
		
		private void Finish()
		{
//			trace("FallingAction Finish _player.x :"+_player.pos.x + "   _player.y"+_player.pos.y);
			_isFinished = true;
            m_pCharacter.StopMoving();
		}

        public override bool Initialize(object [] args)
        {
            base.Initialize(args);
            m_pCharacter = args[0] as CharacterLogic;
            m_pTarget = (Vector2) args[1];
            m_bIsLiving = (bool)args[2];
            m_bCanIgnore = (bool)args[3];
            return true;
        }
    }
}
