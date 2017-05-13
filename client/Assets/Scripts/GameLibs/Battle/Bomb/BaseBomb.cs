using UnityEngine;
using System.Collections.Generic;
using TKBase;
using TKGameView;

namespace TKGame
{
    public class BaseBomb : TKPhysicalObj, ITicker, IPoolableObect
    {
        private Texture2D m_stShape;
        private Texture2D m_stBorder;
        private CharacterLogic m_pHost;
        private BombData m_pConfig;
        public int cur_skill;
        public CharacterLogic Host
        {
            get { return m_pHost; }
        }

        public BombData Config
        {
            get { return m_pConfig; }
        }


        private AttackInfo m_pAttackInfo;

        public BaseBomb()
            : base(0, 1, 10.0f, 100.0f, 1.0f, 1.0f)
        {
            _testRect = new Rect(-3, -3, 9, 9);
            m_pConfig = null;
            m_pHost = null;
            m_stShape = null;
            m_stBorder = null;
            m_pAttackInfo = new AttackInfo();
        }

        override public void Update(float dt)
        {
            base.Update(dt);
        }

        public Vector2 Boom_pos = new Vector2(0f, 0f);
        public short Net_Boom_Status;
        public int Boom_Skill;
        public void Net_Boom(bool boom_kind)
        {
            Network.n_stShape = m_stShape;
            Network.n_stBorder = m_stBorder;
            if (GameGOW.Get().BattleMgr.m_pCurrentPlayer.ID == Network.playerid)
            {
                Network.Send_BOOM(boom_kind, _pos.x, _pos.y, this.ID);
            }
        }

        override protected void CollideGround()
        {
            if (SkillManager.CurrentSkillId == (int)SkillManager.SkillType.Rocket)
            {
                return;
            }
            Die();
            if (Network.gamemode == BattleType.AI_1v1)
            {
                Bomb();
            }
            else if (Network.NetworkMode == true)
            {
                Net_Boom(false);
            }
            else
            {
                Bomb();
            }
        }

        override protected void CollideObject(List<TKPhysicalObj> lst)
        {
            bool collided = false;
            Dictionary<int, bool> attackedMask = new Dictionary<int, bool>();
            for (int i = 0; i < lst.Count; ++i)
            {
                if (lst[i].CollideByObject(this))
                {
                    collided = true;
                    attackedMask[lst[i].ID] = true;
                }
            }

            //Debug.Log("xue ************" + lst.Count + "   "  + SkillManager.EnemyID +   "    " + Network.xue++);

            if (SkillManager.CurrentSkillId == (int)SkillManager.SkillType.Rocket)
            {
                if (attackedMask != null && attackedMask.ContainsKey(SkillManager.EnemyID))
                {

                }
                else
                {
                    return;
                }
            }


            if (collided)
            {
                Die();
                if (Network.gamemode == BattleType.AI_1v1)
                {
                    Bomb();
                }
                else if (Network.NetworkMode == true)
                {
                    Net_Boom(true);
                }
                else
                {
                    Bomb(attackedMask);
                }
            }
        }

        override protected void FlyOutMap()
        {
            Die();
            if (Network.gamemode == BattleType.AI_1v1)
            {
               Bomb();
            }
            else if (Network.NetworkMode == true)
            {
                Net_Boom(false);
            }
            else
            {
                Bomb();
            }
        }

        public int BombStatus = 0;
        public void Bomb(Dictionary<int, bool> attackedMask = null)
        {
            //  SounderManager.Get().PlaySound(1, "boom1");
            bool flag = true;
            if(GameGOW.Get().MapMgr.IsOutMap(this.Position))
            {
                flag = false;
            }
            if(flag)
            SounderManager.Get().PlaySound(1, "sound/yinxiao/pet_boom_" + (int)SkillManager.CurrentSkillId);
           
            if (SkillManager.DO_bomb_Action == true && flag)
                DigMap();

            Vector2 boom_pos = new Vector2();

            if (Network.gamemode == BattleType.AI_1v1)
            {
                boom_pos = this.Position;
            }
            else if (Network.NetworkMode == true)
            {
                boom_pos = Boom_pos;
            }
            else
            {
                boom_pos = this.Position;
            }
            for(int i=-1;i<=1;i++)
            {
                if (GameGOW.Get().MapMgr.IsEmpty((int)(boom_pos.x + i), (int)(boom_pos.y + 5)))
                {
                    boom_pos.x = (int)(boom_pos.x + i);
                    boom_pos.y = (int)((int)(boom_pos.y + 5));
                    break;
                    
                }
            }
            if (SkillManager.CurrentSkillId == 7) 
            {
                /*
                 * 能飞自杀
                    if(flag && boom_pos.y==0)
                    {
                        
                        
                    }
                    else
                    {
                        if (flag)
                        SelfCharacterWalkAction.Set_m_pCharacter(boom_pos);
                        else
                        {
                            Debug.Log("WWWWWWWWWWWWWWWWWWWWWWWWWWWWWW");
                            this.Stop();
                        }
                    }
                 */
                if (flag)
                SelfCharacterWalkAction.Set_m_pCharacter(boom_pos);
                else
                {
                    this.Stop();
                }
                   

            }



            List<TKPhysicalObj> bombList = GameGOW.Get().MapMgr.GetObjectsInMap(boom_pos, m_pConfig.m_fBombRange, null);
            int rr = 0;
            for (int i = 0; i < bombList.Count; ++i)
            {
                CharacterLogic pCharacter = bombList[i] as CharacterLogic;
                if (pCharacter != null)
                {

                    pCharacter.Falling(); //attackmask 
                    m_pAttackInfo.m_Position = boom_pos;
                    m_pAttackInfo.m_bIsCenterDamage = (attackedMask != null && attackedMask.ContainsKey(pCharacter.ID));
                    if (SkillManager.DO_bomb_Action == true || SkillManager.CurrentSkillId == 6)
                        BattleUtils.TriggerAttack(m_pAttackInfo, pCharacter, Boom_Skill);

                }
                
                if( (SkillManager.DO_bomb_Action == false && SkillManager.CurrentSkillId == 7 ) && flag)
                {
                    rr++;
                    if (rr < 2)
                        this.Stop();
                }
                
            }

            //xue move from BombView.cs
            BombStatus++;
            SkillManager.bomb_num--;
            if (Network.gamemode == BattleType.AI_1v1 && SkillManager.bomb_num <= 0) {
                if (GameGOW.Get().BattleMgr.m_bIsInBattle == true)
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
            }
            else if (Network.NetworkMode == true)
            {

            }
            else
            {
                if (SkillManager.bomb_num <= 0)
                {
                    GameGOW.Get().BattleMgr.ChangeController();
                }
            }
        }

        protected void DigMap()
        {
            if (m_stShape != null && m_stShape.width > 0 && m_stShape.height > 0)
            {
                if (Network.gamemode == BattleType.AI_1v1) {
                    _map.Dig(_pos, m_stShape, m_stBorder);
                }
                else if (Network.NetworkMode == true)
                {
                    //_pos = new Vector2(1,1);
                    _map.Dig(Boom_pos, m_stShape, m_stBorder);
                }
                else
                {
                    _map.Dig(_pos, m_stShape, m_stBorder);
                }
            }
        }

        public void Start()
        {
            _isLiving = true;
            Net_Boom_Status = 0;
            BombStatus = 0;
            Boom_Skill = SkillManager.CurrentSkillId;
            AddToMap();
            GameGOW.Get().TickerMgr.AddKeyTicker(this);
            StartMoving();
        }

        public void Stop()
        {

            RemoveFromMap();
            GameGOW.Get().TickerMgr.RemoveKeyTicker(this);
            GameGOW.Get().BombMgr.DeleteBomb(this.ID);

            _vx.Clear();
            _vy.Clear();

            _ef = Vector2.zero;
        }

        #region(ITicker接口)
        private TickerFlag m_bIsInTickerList = TickerFlag.TICK_FLAG_NONE;
        public TickerFlag IsInTickerListFlag
        {
            set { m_bIsInTickerList = value; }
            get { return m_bIsInTickerList; }
        }
        public void Tick(uint tickCount)
        {
            DoTick(tickCount);

            //Debug.Log("xue@@@@@@@@@" + BombStatus + "---" + Net_Boom_Status + "----" + Network.xue++);

            if (BombStatus == 2)
            {
               if (SkillManager.CurrentSkillId != 7)
                    this.Stop();
            }
            if (Net_Boom_Status == 1)
            {
                _pos = Boom_pos;
                Net_Boom_Status = 0;
                Bomb();
            }
            else if (Net_Boom_Status == 2)
            {
                _pos = Boom_pos;
                Net_Boom_Status = 0;
                //Bomb(AttackedMask);

                Rect rect = GetCollideRect();
                rect.position += Boom_pos;
                List<TKPhysicalObj> lst = _map.GetObjectsInMap(rect, this);

                bool collided = false;
                Dictionary<int, bool> attackedMask = new Dictionary<int, bool>();
                for (int i = 0; i < lst.Count; ++i)
                {
                    if (lst[i].CollideByObject(this))
                    {
                        collided = true;
                        attackedMask[lst[i].ID] = true;
                    }
                }

                Network.xue++;
                if (collided)
                {
                    Bomb(attackedMask);
                }
            }
        }
        private void DoTick(uint tickCount)
        {
            Update(GameDefine.FRAME_TIME_INTERVAL);
        }
        #endregion
        #region(IPoolableObect接口)
        public bool Initialize(object[] args)
        {
            base.Initialize();

            _id = (int)args[0];
            m_pHost = args[1] as CharacterLogic;
            m_pConfig = args[2] as BombData;
            AttackBombData pData = args[3] as AttackBombData;

            m_pAttackInfo.m_pFight = m_pHost;
            m_pAttackInfo.m_iDamage = pData.damage;
            m_pAttackInfo.m_iCenterDamage = pData.center_damage;

            m_stShape = GameGOW.Get().ResourceMgr.GetRes<Texture2D>(m_pConfig.m_strShapePath);
            m_stBorder = GameGOW.Get().ResourceMgr.GetRes<Texture2D>(m_pConfig.m_strBorderPath);
            Network.n_stShape = m_stShape;
            Network.n_stBorder = m_stBorder;

            _mass = m_pConfig.m_fMass;
            _gravityFactor = m_pConfig.m_fGFactor;
            _windFactor = m_pConfig.m_fWindFactor;
            _airResitFactor = m_pConfig.m_fAirResitFactor;
            _testRect = m_pConfig.m_pAttackRect;
            return true;
        }
        public void UnInitialize()
        {

        }
        #endregion
    }
}
