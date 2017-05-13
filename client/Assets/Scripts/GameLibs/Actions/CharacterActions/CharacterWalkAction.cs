using UnityEngine;

namespace TKGame
{
    class CharacterWalkAction : BaseAction
    {
        private CharacterLogic m_pCharacter;
        private Vector2 m_pTarget;
        private int m_iDir;

        public CharacterWalkAction()
            :base()
        {
        }
        override public bool Connect(BaseAction action)
        {
            CharacterWalkAction walk = action as CharacterWalkAction;
            if(walk != null)
            {
                m_pTarget = walk.m_pTarget;
                m_iDir = walk.m_iDir;
                return true;
            }
            else
            {
                return false;
            }
        }

        override public void Prepare()
        {
            if (_isPrepare) return;
            _isPrepare = true;
            if(m_pCharacter.IsLiving)
            {
                m_pCharacter.StartMoving(); m_pCharacter.Body.DoAction(CharacterStateType.WALK);
            }
            else
            {
                Finish();
            }
        }

        override public void Execute()
        {
            if(Vector2.Distance(m_pCharacter.Position, m_pTarget) <= m_pCharacter.m_pInfo.m_pInstructionData.walk_speed_x ||
                m_pTarget.x == m_pCharacter.Position.x)
            {
                Finish();
            }
            else
            {
                m_pCharacter.m_pInfo.m_iFacing = m_pTarget.x > m_pCharacter.Position.x ? GameDefine.FACING_RIGHT : GameDefine.FACING_LEFT;
                Vector2 p = m_pCharacter.GetNextWalkPoint(m_pCharacter.m_pInfo.m_iFacing);
                if(p == Vector2.zero || //找不到路 
                    m_pCharacter.m_pInfo.m_iFacing == GameDefine.FACING_RIGHT && m_pCharacter.Position.x >= m_pTarget.x ||//已经走过头了
                    m_pCharacter.m_pInfo.m_iFacing == GameDefine.FACING_LEFT && m_pCharacter.Position.x <= m_pTarget.x)//已经走过头了
                {
                    Finish();
                }
                else
                {
                    m_pCharacter.Position = p;
                    m_pCharacter.Body.DoAction(CharacterStateType.WALK);
                }
            }
        }

        override public void ExecuteAtOnce()
        {
            base.ExecuteAtOnce();
            m_pCharacter.Position = m_pTarget;
            m_pCharacter.m_pInfo.m_iFacing = m_iDir;
            m_pCharacter.StopMoving();
            m_pCharacter.Body.DoAction(CharacterStateType.IDLE);
        }

        private void Finish()
        {
            m_pCharacter.Position = m_pTarget;
            m_pCharacter.m_pInfo.m_iFacing = m_iDir;
            m_pCharacter.StopMoving();
            m_pCharacter.Body.DoAction(CharacterStateType.IDLE);
            _isFinished = true;
        }

        #region(IPoolableObect接口)
        override public bool Initialize(object[] args)
        {
            base.Initialize(args);
            m_pCharacter = args[0] as CharacterLogic;
            m_iDir = (int)args[1];
            m_pTarget = (Vector2)args[2];
            _isFinished = false;
            return true;
        }
        override public void UnInitialize()
        {
            base.UnInitialize();
        }
        #endregion
    }
}
