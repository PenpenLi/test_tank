using UnityEngine;
using TKBase;

namespace TKGame
{
    class CharacterCheckFallingAction : BaseAction
    {
        private CharacterLogic m_pCharacter;
        public CharacterCheckFallingAction()
            :base()
        {

        }

        public override void Prepare()
        {
            
        }

        public override void Execute()
        {
            /*
            bool flag = true, living = false;
            int startx = (int)m_pCharacter.Position.x;
            int endx = startx + ((int)m_pCharacter.m_pInfo.m_iFacing) * (m_pCharacter.m_pInfo.m_pInstructionData.be_attack_box_max_x - m_pCharacter.m_pInfo.m_pInstructionData.be_attack_box_min_x) / 2;
            int nowx = startx;
            int stepx = endx > nowx ? 1 : -1;
            MapManager pMap = GameGOW.Get().MapMgr;
            Vector2 p1 = Vector2.zero;
            do
            {
                Vector2 p = pMap.FindYLineNotEmptyPointUp(nowx, (int)m_pCharacter.Position.y + m_pCharacter.m_pInfo.m_pInstructionData.walk_speed_y,
                    (int)pMap.m_stBound.height);
                if (m_pCharacter.Position == p) flag = false;
                if (p != Vector2.zero) living = true;
                if (nowx == startx) p1 = p;
                nowx = nowx + stepx;
            } while (nowx != endx);
            if (flag)
            {
                if (living)
                {
                    BaseAction act = ObjectPools.CheckOut<CharacterFallingAction>(m_pCharacter, p1, true, false);
                    m_pCharacter.Act(act);
                }
                else
                {
                    p1 = new Vector2(startx, pMap.m_stBound.yMin);
                    BaseAction act = ObjectPools.CheckOut<CharacterFallingAction>(m_pCharacter, p1, false, false);
                    m_pCharacter.Act(act);
                }
            }
            */
            
            int tx = (int)m_pCharacter.Position.x;
            MapManager pMap = GameGOW.Get().MapMgr;
            Vector2 p = pMap.FindYLineNotEmptyPointUp_(tx, (int)m_pCharacter.Position.y + m_pCharacter.m_pInfo.m_pInstructionData.walk_speed_y,
                (int)pMap.m_stBound.height);
           
           // Debug.Log("xxxxxxXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"+p.x+"-"+p.y+"-----"+GameGOW.Get().MapMgr.IsEmpty((int)p.x,(int)p.y)+" "+ GameGOW.Get().MapMgr.IsEmpty((int)p.x-3, (int)p.y)+"  "+ GameGOW.Get().MapMgr.IsEmpty((int)p.x+3, (int)p.y));
            //for(int i=0;i)
         //   p.x -= 3;

            if(m_pCharacter.Position != p)
            {
                if (p != Vector2.zero)
                {
                    BaseAction act = ObjectPools.CheckOut<CharacterFallingAction>(m_pCharacter, p, true, false);
                    m_pCharacter.Act(act);
                }
                else
                {
                    p = new Vector2(tx, pMap.m_stBound.yMin);
                    BaseAction act = ObjectPools.CheckOut<CharacterFallingAction>(m_pCharacter, p, false, false);
                    m_pCharacter.Act(act);
                }
            }

            _isFinished = true;
        }
        #region(IPoolableObect接口)
        override public bool Initialize(object[] args)
        {
            base.Initialize(args);
            m_pCharacter = args[0] as CharacterLogic;
            return true;
        }
        override public void UnInitialize()
        {
            base.UnInitialize();
        }
        #endregion
    }
}
