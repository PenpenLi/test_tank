using System;
using System.Collections.Generic;
using System.Xml;
using TKBase;
using UnityEngine;

namespace TKGame
{
    public class CharacterStateData : DataBase
    {
        public string animation_name;
        public int total_frame;
        public Dictionary<int, AttackBaseData> AttackData;
        public CharacterStateData()
        {
            AttackData = new Dictionary<int, AttackBaseData>();
        }

        public override void FromXml(XmlElement xml)
        {
            XmlRead.Attr(xml, "animation_name", ref animation_name);
            XmlRead.Attr(xml, "total_frame", ref total_frame);

            XmlNodeList xmlList = xml.GetElementsByTagName("attack");
            int frame = -1;
            foreach (XmlNode node in xmlList)
            {
                XmlElement ele = node as XmlElement;
                CharacterAttackType eType = CharacterAttackType.NONE;

                XmlRead.AttrEnum<CharacterAttackType>(ele, "attack_type", ref eType);
                XmlRead.Attr(ele, "frame_index", ref frame);

                AttackBaseData pData = AttackBaseData.CreateInstance(eType);
                pData.FromXml(ele);
                AttackData[frame] = pData;
                Network.attackbasedata = pData;
            }
        }

        public void FromPrefabs(CharacterEditorData.CharacterEditorStateData editStateDt)
        {
            this.animation_name = editStateDt.m_animationName;
            this.total_frame = editStateDt.m_totFrame;
            foreach(CharacterEditorData.CharacterEditorBombAttackData item in editStateDt.m_attackBmDatas)
            {
                int frame = item.m_iFrame;
                AttackBombData pData = (AttackBombData)AttackBaseData.CreateInstance(item.m_attackType);
                pData.damage = item.m_damage;
                pData.center_damage = item.m_centerDamage;
                pData.bomb_config_id = item.m_bombCofigID;
                AttackData[frame] = pData;
            }
            foreach (CharacterEditorData.CharacterEditorNormalAttackData item in editStateDt.m_attackNmDatas)
            {
                int frame = item.m_iFrame;
                AttackNormalData pData = (AttackNormalData)AttackBaseData.CreateInstance(item.m_attackType);
                AttackData[frame] = pData;
            }
        }
    }
}