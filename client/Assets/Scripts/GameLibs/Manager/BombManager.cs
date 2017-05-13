using System.Collections.Generic;
using UnityEngine;
using TKGameView;
using TKBase;

namespace TKGame
{
    public class BombManager : ManagerBase, ITicker
    {
        public delegate void OnBombNewOrDelete(BaseBomb value,bool is_foucs);
        public event OnBombNewOrDelete OnBombNew;
        public event OnBombNewOrDelete OnBombDelete;

        private bool m_bIsStart = false;
        private Dictionary<int, BaseBomb> m_dicBombs;
        

        public BombManager()
            :base(ManagerType.BombManager)
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();
            m_dicBombs = new Dictionary<int, BaseBomb>();
        }
        public override void UnInitialize()
        {
            base.UnInitialize();

            m_dicBombs.Clear();
            m_dicBombs = null;
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
            if (false == m_bIsStart)
            {
                return;
            }
        }

        public BaseBomb GetBombByUID(int uid)
        {
            if(m_dicBombs.ContainsKey(uid))
            {
                return m_dicBombs[uid];
            }
            else
            {
                return null;
            }
        }

        private static object locker = new object();
        public void ThrowBomb(CharacterLogic pHost, AttackBombData pData,bool is_foucs)   //is_foucs 为camera是否跟踪
        {
            SkillINstructionData self_skill = GameGOW.Get().DataMgr.GetSkillINstructionDataByID(SkillManager.CurrentSkillId==0?0:100+SkillManager.CurrentSkillId);
            pData.damage = self_skill.damage;
            pData.center_damage = self_skill.center_damage;
            BombData pBombData = GameGOW.Get().DataMgr.GetBombDataByID(pData.bomb_config_id);
            if (pBombData != null)
            {
                int uid;
                lock (locker)
                {
                    uid = GameGOW.Get().GetNextUid();
                }
             //   Debug.Log("EEEEEEEEEEEEEEEEEEEEEEE-------------UID"+uid);
                BaseBomb m_stBombLogic = ObjectPools.CheckOut<BaseBomb>(uid, pHost, pBombData, pData);
              //  Debug.Log("EEEEEEEEEEEEEEEEEEEEEEE-------------UID" + m_stBombLogic.Config.m_iResourceID);
                m_stBombLogic.Position = pHost.GetWeaponPosition();
                float radian;
                if (SkillManager.CurrentSkillId == (int)SkillManager.SkillType.Rocket)
                {
                    radian = MathUtil.AngleToRadian(pHost.GetFireAngle()) * pHost.m_pInfo.m_iFacing;
                    GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iFireAngle = SkillManager.PreAngle;
                }
                else
                {
                    radian = MathUtil.AngleToRadian(pHost.GetFireAngle() + pHost.CalcObjectAngle()) * pHost.m_pInfo.m_iFacing;
                }

                float speedX = pHost.m_pInfo.m_iBombSpeed * Mathf.Cos(radian) * pHost.m_pInfo.m_iFacing;
                float speedY = pHost.m_pInfo.m_iBombSpeed * Mathf.Sin(radian);
           
                m_stBombLogic.Start();
                if(SkillManager.thunder_bomb_num>0)
                {
                    speedX = 0.0f;
                    speedY = 0.0f;

                   float val = GameGOW.Get().MapMgr.m_stBound.xMax - GameGOW.Get().MapMgr.m_stBound.xMin;
                    float add = val / 6;
                    m_stBombLogic.Position = new Vector2(GameGOW.Get().MapMgr.m_stBound.xMin + add* SkillManager.thunder_bomb_num, GameGOW.Get().MapMgr.m_stBound.yMax-10);
                 
                }
                m_stBombLogic.SetSpeedXY(speedX, speedY);
                m_dicBombs[uid] = m_stBombLogic;
             
                if (OnBombNew != null)
                {
                    OnBombNew(m_stBombLogic, is_foucs);
                }
            }

        }

        public void DeleteBomb(int uid)
        {
            if(m_dicBombs.ContainsKey(uid))
            {
                BaseBomb pBombData = m_dicBombs[uid];
                if (OnBombDelete != null) OnBombDelete(pBombData,true);
                ObjectPools.CheckIn(pBombData);
                m_dicBombs.Remove(uid);
            }
        }

        public void Clear()
        {
            foreach(KeyValuePair<int, BaseBomb> bomb in m_dicBombs)
            {
                ObjectPools.CheckIn(bomb.Value);
            }
            m_dicBombs.Clear();
        }
    }
}
