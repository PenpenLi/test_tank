using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TKGame
{
    public class AttackNormalData : AttackBaseData
    {
        public int m_iDamage;
        public AttackNormalData()
            : base(CharacterAttackType.NORMAL)
        {

        }
    }
}
