using UnityEngine;
using System.Collections;

namespace TKGame
{
    public class AttackBaseData : DataBase
    {
        public CharacterAttackType Type;
        public AttackBaseData(CharacterAttackType eType)
        {
            Type = eType;
        }

        public static AttackBaseData CreateInstance(CharacterAttackType eType)
        {
            switch (eType)
            {
                case CharacterAttackType.BOMB:
                    return AttackBaseData.CreateInstance<AttackBombData>();
                case CharacterAttackType.NORMAL:
                    return AttackBaseData.CreateInstance<AttackNormalData>();
            }
            return null;
        }
    }
}