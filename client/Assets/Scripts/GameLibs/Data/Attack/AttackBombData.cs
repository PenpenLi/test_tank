using UnityEngine;
using System.Collections;
using System.Xml;
using TKBase;

namespace TKGame
{
    public class AttackBombData : AttackBaseData
    {
        public int bomb_config_id;
        public int damage;
        public int center_damage;
        public AttackBombData()
            :base(CharacterAttackType.BOMB)
        {

        }

        public override void FromXml(XmlElement xml)
        {
            XmlRead.Attr(xml, "bomb_config_id", ref bomb_config_id);
            XmlRead.Attr(xml, "damage", ref damage);
            XmlRead.Attr(xml, "center_damage", ref center_damage);
        }
    }
}