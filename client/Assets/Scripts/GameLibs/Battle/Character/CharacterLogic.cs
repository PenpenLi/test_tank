using UnityEngine;
using System.Collections;
using TKBase;
using TKGameView;
namespace TKGame
{
    public class CharacterLogic : TKPhysicalObj, ITicker, IPoolableObect
    {
        /*
         *   zhan add
         */
        private bool isStartCall = false;  
        private bool isUpdateCall = false;
        private bool isLateUpdateCall = false;
        public static AttackBaseData AA;
        public  bool go_down = false;


     
        public CharacterInfo m_pInfo;
        private ActionManager m_pActionManager;
        private CharacterAnimiationLogic m_pBody;

        public CharacterLogic()
            : base(0)
        {
            m_pActionManager = new ActionManager();
            m_pInfo = new CharacterInfo();
            m_pBody = new CharacterAnimiationLogic(m_pInfo);
            CanCollided = true;
        }

        public CharacterAnimiationLogic Body
        {
            get { return m_pBody; }
        }


        public void Act(BaseAction action)
        {
            m_pActionManager.Act(action);
        }
        public int ActionCount
        {
            get { return m_pActionManager.ActionCount; }
        }

        public Vector2 GetWeaponPosition()
        {
            float x = Position.x + m_pInfo.m_pInstructionData.WeaponPosition.x * m_pInfo.m_iFacing;
            float y = Position.y + m_pInfo.m_pInstructionData.WeaponPosition.y;
            return new Vector2(x, y);
        }

        public float GetFireAngle()
        {

            //if(GameDefine.FACING_LEFT == m_pInfo.m_iFacing)
            //{
            //    ret = 180 - ret;
            //}
            float ret = m_pInfo.m_iFireAngle * m_pInfo.m_iFacing;
            return ret;
        }

        public void OnDirectionKeyChanged(int uiKey)
        {
            if (m_pInfo.m_iDirectionKeys != uiKey)
            {
                m_pInfo.m_iDirectionKeys = uiKey;
                //朝向
                if ((m_pInfo.m_iDirectionKeys & InputDefine.MOVE_RIGHT) != 0)
                {
                    m_pInfo.m_iFacing = GameDefine.FACING_RIGHT;
                }
                else if ((m_pInfo.m_iDirectionKeys & InputDefine.MOVE_LEFT) != 0)
                {
                    m_pInfo.m_iFacing = GameDefine.FACING_LEFT;
                }

                //移动
                Act(ObjectPools.CheckOut<SelfCharacterWalkAction>(this));
            }
        }

        public void OnFunctionKeyDown(int uiKey)
        {
            switch (uiKey)
            {
                case InputDefine.ATTACK:
                    if (m_pInfo.m_bIsInRound)
                    {
                        if (m_pInfo.m_bInBombSpeedUp == false)
                        {
                            m_pInfo.m_bInBombSpeedUp = true;
                            m_pInfo.m_iBombSpeed = 0;
                            Act(ObjectPools.CheckOut<CharacterAttackAction>(this));
                        }
                    }
                    break;
            }
        }

        public void OnFunctionKeyUp(int uiKey)
        {
            switch (uiKey)
            {
                case InputDefine.ATTACK:
                    m_pInfo.m_bInBombSpeedUp = false;

                    break;
            }
        }

        public void OnAttackAngleChange(int uiKey)
        {
            if ((uiKey & InputDefine.MOVE_UP) != 0)
            {
                m_pInfo.m_iFireAngle += CharacterDefine.ANGLE_STEP;
                if (m_pInfo.m_iFireAngle > m_pInfo.m_pInstructionData.high_fire_angle)
                {
                    m_pInfo.m_iFireAngle = m_pInfo.m_pInstructionData.high_fire_angle;
                }
            }
            else if ((uiKey & InputDefine.MOVE_DOWN) != 0)
            {
                m_pInfo.m_iFireAngle -= CharacterDefine.ANGLE_STEP;
                if (m_pInfo.m_iFireAngle < m_pInfo.m_pInstructionData.low_fire_angle)
                {
                    m_pInfo.m_iFireAngle = m_pInfo.m_pInstructionData.low_fire_angle;
                }
            }
            OnAttackForceChange();
            if (m_pInfo.m_icanattackmin < 0 || m_pInfo.m_icanattackmin > 1)
            {
                m_pInfo.m_icanattackmin = 0;
            }
            if (m_pInfo.m_icanattackmax < 0 || m_pInfo.m_icanattackmax > 1)
            {
                m_pInfo.m_icanattackmax = 1;
            }
        }

        public void OnAttackForceChange()
        {
            int preSpeed = m_pInfo.m_iBombSpeed;
            m_pInfo.m_icanattackmax = m_pInfo.m_icanattackmin = -1f;
            float midforce = 100000000f;
            int midforcei = 0;
            var characterList = GameGOW.Get().CharacterMgr.GetCharacterByTeam(m_pInfo.m_iTeam, false, true);
            if (characterList.Count == 0)
            {
                return;
            }
            float x1 = characterList[0].Position.x;
            float y1 = characterList[0].Position.y;
            //Debug.Log("try----------" + characterList[0].Position.x);

            float radian = MathUtil.AngleToRadian(GetFireAngle() + CalcObjectAngle()) * m_pInfo.m_iFacing;
            float x0 = GetWeaponPosition().x, y0 = GetWeaponPosition().y;
            for (int i = 1; i <= 100; i++)
            {
                m_pInfo.m_iBombSpeed = (int)System.Math.Ceiling(1f * i / 100 * m_pInfo.m_pInstructionData.fire_range);
                float speedX = m_pInfo.m_iBombSpeed * Mathf.Cos(radian) * m_pInfo.m_iFacing;
                float speedY = m_pInfo.m_iBombSpeed * Mathf.Sin(radian);
                float t = (x1 - x0) / speedX / GameDefine.FRAME_TIME_INTERVAL;
                if (t <= 0)
                {
                    continue;
                }
                if (Mathf.Abs(speedY * t + 0.5f * _gravityFactor * _map.Gravity * t * t - y1 + y0) <= midforce)
                {
                    midforce = Mathf.Abs(speedY * t + 0.5f * _gravityFactor * _map.Gravity * t * t - y1 + y0);
                    midforcei = i;
                }
            }

            //Debug.Log("-----------------------------");
            //Debug.Log(x1);
            //Debug.Log(y1);
            //Debug.Log(radian);
            //Debug.Log(_gravityFactor * _map.Gravity);
            //Debug.Log(x0);
            //Debug.Log(y0);
            //Debug.Log(m_pInfo.m_pInstructionData.fire_range);
            //Debug.Log(midforcei);
            //Debug.Log("+++++++++++++++++++++++++++++++++++++++++++++");

            m_pInfo.m_icanattackmin = 1f * midforcei / 100;
            m_pInfo.m_icanattackmax = 1f * midforcei / 100 + 0.1f;
            m_pInfo.m_iBombSpeed = preSpeed;
        }

        public void ChangeInstruction(CharacterInstructionData pData)
        {
            m_pInfo.m_pInstructionData = pData;
            m_pInfo.m_iFireAngle = pData.low_fire_angle;
            SetCollideRect(pData.be_attack_box_min_x,
                pData.be_attack_box_min_y,
                pData.be_attack_box_max_x,
                pData.be_attack_box_max_y);
            m_pBody.DoAction(CharacterStateType.IDLE1);
        }

        private void CheckIsAttack()
        {
            if(Network.gamemode == BattleType.AI_1v1)
            {
                if (m_pInfo.m_pCurrentStateData.AttackData.ContainsKey(m_pInfo.m_iCurrentFrame))
                {
                    AttackBaseData attack = m_pInfo.m_pCurrentStateData.AttackData[m_pInfo.m_iCurrentFrame];
                    if (attack.Type == CharacterAttackType.BOMB && this.IsLiving == true)
                    {
                        AA = attack;

                        //SkillManager.CurrentSkillId = (int)SkillManager.SkillType.Normal;
                        //GameGOW.Get().PlaySkill(SkillManager.SkillType.Normal, this, attack)
                        GameGOW.Get().PlaySkill((SkillManager.SkillType)SkillManager.CurrentSkillId, this, attack);

                        // GameGOW.Get().BombMgr.ThrowBomb(this, attack as AttackBombData);
                    }
                }
            }
            if (Network.NetworkMode == true)
            {

            }
            else
            {
                if (m_pInfo.m_pCurrentStateData.AttackData.ContainsKey(m_pInfo.m_iCurrentFrame))
                {
                    AttackBaseData attack = m_pInfo.m_pCurrentStateData.AttackData[m_pInfo.m_iCurrentFrame];
                    if (attack.Type == CharacterAttackType.BOMB && this.IsLiving == true)
                    {
                        AA = attack;

                        //SkillManager.CurrentSkillId = (int)SkillManager.SkillType.Normal;
                        //GameGOW.Get().PlaySkill(SkillManager.SkillType.Normal, this, attack)
                        GameGOW.Get().PlaySkill((SkillManager.SkillType)SkillManager.CurrentSkillId, this, attack);

                        // GameGOW.Get().BombMgr.ThrowBomb(this, attack as AttackBombData);
                    }
                }
            }
        }

        public bool CanStand(int x, int y)
        {
            return !_map.IsEmpty(x - 1, y) || !_map.IsEmpty(x + 1, y);
        }
        public bool CanMoveDirection(int dir)
        {
            return !_map.IsOutMap(new Vector2(Position.x + (15 + m_pInfo.m_pInstructionData.walk_speed_x) * dir, Position.y));
        }
        public Vector2 GetNextWalkPoint(int dir)
        {
            if (CanMoveDirection(dir))
            {
                return _map.findNextWalkPoint((int)Position.x, (int)Position.y, dir,
                    m_pInfo.m_pInstructionData.walk_speed_x, m_pInfo.m_pInstructionData.walk_speed_y);
            }
            return Vector2.zero;
        }

        public override bool CollideByObject(TKPhysicalObj obj)
        {
            if (obj is BaseBomb)
            {
                return true;
            }
            return false;
        }

        public void Falling()
        {
            Act(ObjectPools.CheckOut<CharacterCheckFallingAction>(this));
        }

        public void OnDamage(int curHP, int iDamage, DamageType eType)
        {
            m_pInfo.m_iHP = curHP;
            if (m_pInfo.m_iHP <= 0)
            {
                if (IsLiving == true)
                {
                    if (Network.gamemode == BattleType.AI_1v1)
                    {
                        Dead();
                    }
                    else if (Network.NetworkMode == true)
                    {
                        if (Network.playerid == this.ID)
                        {
                            Network.Send_Over(Network.Pid_Tid[this.ID], false);
                        }
                    }
                    else
                    {
                        Dead();
                    }
                    
                }

            }
            else
            {
               if(SkillManager.CurrentSkillId!=6)
                m_pBody.DoAction(CharacterStateType.BEATTACK);
            }
        }

        public void Dead()
        {
            if (Network.gamemode == BattleType.AI_1v1)
            {
                GameGOW.Get().Winner_Id = Network.playerid == this.ID ? 1 : 2;
            }
            else if (Network.NetworkMode == true)
            {
                GameGOW.Get().Winner_Id = Network.Pid_Tid[this.ID];
            }
            else
            {
                GameGOW.Get().Winner_Id = (this.ID) == 1 ? 2 : 1;
            }
          //  Debug.Log("Position +++++++++++++++++++++++"+Position.x+"-"+Position.y);
            _isLiving = false;
            //m_pInfo.m_iHP = 0;
            m_pBody.DoAction(CharacterStateType.DEAD);
           
           
            Die();



            EventDispatcher.DispatchEvent("EventCharacterDead", this, new CharacterDeadEvent(this.ID));

            if (Network.gamemode == BattleType.AI_1v1)
            {
                Network.Send_PVE_Over(GameGOW.Get().Winner_Id == 2);
            }
            else if(Network.NetworkMode == true)
            {
                if (Network.playerid == this.ID)
                {
                    //Network.Send_Over(Network.Pid_Tid[this.ID], false);
                }
            }
        }

        #region(ITicker接口)
        private TickerFlag m_bIsInTickerList;
        public TickerFlag IsInTickerListFlag
        {
            set { m_bIsInTickerList = value; }
            get { return m_bIsInTickerList; }
        }
        public void Tick(uint tickCount)
        {
            DoTick(tickCount);
        }
        private void DoTick(uint tickCount)
        {
            m_pActionManager.Execute();
            if ((tickCount & 1) == 0)
            {
                m_pBody.Tick(tickCount);
                CheckIsAttack();
            }
        }
        #endregion
        #region(IPoolableObect接口)
        public bool Initialize(object[] args)
        {
            base.Initialize();
            _id = (int)args[0];
            ChangeInstruction(args[1] as CharacterInstructionData);
            m_pBody.DoAction(CharacterStateType.IDLE);

            AddToMap();
            GameGOW.Get().TickerMgr.AddKeyTicker(this);

            return true;
        }
        public void UnInitialize()
        {
            m_pInfo.UnInitialize();
            m_pActionManager.Clear();
            m_pBody.UnInitialize();
            RemoveFromMap();
            GameGOW.Get().TickerMgr.RemoveKeyTicker(this);
        }
        #endregion
    }
   
}