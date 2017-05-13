using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
namespace TKGame
{
    public enum DamageType
    {
        NORMAL,
        CRIT,
        MISS,
    };
    class BattleUtils
    {
         public static double get_distance(Vector2 A,Vector2 B)
        {
            return Math.Sqrt((A.x - B.x) * (A.x - B.x) + (A.y - B.y) * (A.y - B.y));
          //  return (A.x - B.x) * (A.x - B.x) + (A.y - B.y) * (A.y - B.y);
        }
        static public int TriggerAttack(AttackInfo attackInfo, CharacterLogic dstObject, int Boom_Skill)
        {
            CharacterLogic pSrcObject = attackInfo.m_pFight;
            int iDamage = 0;
            DamageType eType = DamageType.NORMAL;
            float Range_boom = GameGOW.Get().DataMgr.GetBombDataByID(1).m_fBombRange;
            
           // SkillManager._debug("WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW  WWWW   " + attackInfo.m_Position.x + "-" + attackInfo.m_Position.y);
           // SkillManager._debug("WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW  WWWW   " + dstObject.Position.x + "-" + dstObject.Position.y);
           // SkillManager._debug("CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC CCC  " + get_distance(attackInfo.m_Position, dstObject.Position));
            
            if(get_distance(attackInfo.m_Position, dstObject.Position)<Range_boom*1.7/3)  //
            {
               iDamage = GameGOW.Get().DataMgr.GetSkillINstructionDataByID(Boom_Skill == 0 ? 0 : 100 + SkillManager.CurrentSkillId).center_damage;
            }
               
            else if(get_distance(attackInfo.m_Position, dstObject.Position) < Range_boom  )
            {
                double temp = (Range_boom - get_distance(attackInfo.m_Position, dstObject.Position))*(3/1.7) / (Range_boom);
                iDamage = (int)(GameGOW.Get().DataMgr.GetSkillINstructionDataByID(Boom_Skill == 0 ? 0 : 100 + SkillManager.CurrentSkillId).center_damage * temp);
                if(iDamage >= 0 && iDamage<28)
                {
                    iDamage = 28;
                }
            }
            else
            {
                if (Boom_Skill == 6)
                {
                    iDamage = -28;
                }
                else
                {
                    iDamage = 28;
                }
            }
            /*
            if (attackInfo.m_bIsCenterDamage)
            {
                iDamage = GameGOW.Get().DataMgr.GetSkillINstructionDataByID(SkillManager.CurrentSkillId == 0 ? 0 : 100 + SkillManager.CurrentSkillId).center_damage;
            }
            else
            {
                iDamage = GameGOW.Get().DataMgr.GetSkillINstructionDataByID(SkillManager.CurrentSkillId == 0 ? 0 : 100 + SkillManager.CurrentSkillId).damage;
            }
            /*
            if(iDamage < 0)
            {
                iDamage = 0;
            }
            */

            
            if(Network.gamemode == BattleType.AI_1v1 || Network.NetworkMode == false) 
            {
                int hp = dstObject.m_pInfo.m_iHP;
                float DD = iDamage;
                hp -= (int)DD;
                if (hp > dstObject.m_pInfo.m_iMaxHP)
                {
                    hp = dstObject.m_pInfo.m_iMaxHP;
                }
                dstObject.OnDamage(hp, iDamage, eType);
            }
            else if(Network.NetworkMode == true)
            {
                if (GameGOW.Get().BattleMgr.m_pCurrentPlayer.ID == Network.playerid)
                {
                    Network.Send_Damage(dstObject.ID, iDamage);
                }
            }

            return iDamage;
        }
    }
}
