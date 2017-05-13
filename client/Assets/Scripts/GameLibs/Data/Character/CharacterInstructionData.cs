using System.Collections.Generic;
using System;
using UnityEngine;
using System.Xml;
using TKBase;

namespace TKGame
{
    /**
     * 人物的使用说明书，描述该角色的各项动画，攻击判定等等
     * 由动作编辑器生成
     */
    public class CharacterInstructionData : DataBase
    {
        public int id = 100001;
        public int resource_id = 100001;
        public string default_name;
        public string description;
        public float scale;

        public int walk_speed_x = 2;
        public int walk_speed_y = 8;
        public float Hatred = 0; //仇恨值， 越高越吸引攻击
        public float low_fire_angle = 25;
        public float high_fire_angle = 45;
        public int fire_range = 3000;//射程

        public Vector2 WeaponPosition;

        public int be_attack_box_min_x = 0;
        public int be_attack_box_min_y = 0;
        public int be_attack_box_max_x = 0;
        public int be_attack_box_max_y = 0;

        public int self_skill=101;
        public int m_iHP;
        public int m_iMoveEnergy;
        public int m_iAddEnergy;
        public Dictionary<CharacterStateType, CharacterStateData> Actions;
        public CharacterInstructionData()
        {
            Actions = new Dictionary<CharacterStateType, CharacterStateData>();
        }

        public override void FromXml(XmlElement xml)
        {
            XmlRead.Attr(xml, "id", ref id);
            XmlRead.Attr(xml, "default_name", ref default_name);
            XmlRead.Attr(xml, "description", ref description);
            XmlRead.Attr(xml, "resource_id", ref resource_id);
            XmlRead.Attr(xml, "scale", ref scale);
            XmlRead.Attr(xml, "walk_speed_x", ref walk_speed_x);
            XmlRead.Attr(xml, "walk_speed_y", ref walk_speed_y);
            XmlRead.Attr(xml, "low_fire_angle", ref low_fire_angle);
            XmlRead.Attr(xml, "high_fire_angle", ref high_fire_angle);
            XmlRead.Attr(xml, "fire_range", ref fire_range);
            int weapon_pos_x = 0;
            XmlRead.Attr(xml, "weapon_pos_x", ref weapon_pos_x);
            int weapon_pos_y = 0;
            XmlRead.Attr(xml, "weapon_pos_y", ref weapon_pos_y);
            WeaponPosition = new Vector2(weapon_pos_x, weapon_pos_y);

            XmlRead.Attr(xml, "be_attack_box_min_x", ref be_attack_box_min_x);
            XmlRead.Attr(xml, "be_attack_box_min_y", ref be_attack_box_min_y);
            XmlRead.Attr(xml, "be_attack_box_max_x", ref be_attack_box_max_x);
            XmlRead.Attr(xml, "be_attack_box_max_y", ref be_attack_box_max_y);
            XmlRead.Attr(xml, "self_skill", ref self_skill);
            XmlRead.Attr(xml, "m_iHP",ref m_iHP);
            XmlRead.Attr(xml, "m_iMoveEnergy", ref m_iMoveEnergy);
            XmlRead.Attr(xml, "m_iAddEnergy", ref m_iAddEnergy);

            XmlNodeList xmlList = xml.GetElementsByTagName("state");
            foreach (XmlNode node in xmlList)
            {
                XmlElement ele = node as XmlElement;
                CharacterStateData pData = DataBase.CreateInstance<CharacterStateData>();
                pData.FromXml(ele);
                CharacterStateType eType = CharacterStateType.UNDEFINE;
                XmlRead.AttrEnum<CharacterStateType>(ele, "state_type", ref eType);
                if(Actions.ContainsKey(eType))
                {
                    LOG.Error("repeat state["+eType.ToString()+"] in same instruction[" + id + "]");
                }
                else
                {
                    Actions[eType] = pData;
                }
            }
        }
        public void FromPrefabs(CharacterEditorData dt)
        {
            this.id = dt.m_id;
            this.default_name = dt.m_defaultName;
            this.description = dt.description;
            this.resource_id = dt.m_resID;
            this.scale = dt.m_scale;
            this.walk_speed_x = dt.m_walkSpeedX;
            this.walk_speed_y = dt.m_walkSpeedY;
            this.Hatred = dt.m_hatred;
            this.low_fire_angle = dt.m_lowFireAngle;
            this.high_fire_angle = dt.m_higFireAngle;
            this.fire_range = dt.m_fireRange;
            this.WeaponPosition = dt.m_weaponPosition;
            this.be_attack_box_max_x = dt.m_beAttackBoxMaxX;
            this.be_attack_box_max_y = dt.m_beAttackBoxMaxY;
            this.be_attack_box_min_x = dt.m_beAttackBoxMinX;
            this.be_attack_box_min_y = dt.m_beAttackBoxMinY;
            foreach(CharacterEditorData.CharacterEditorStateData editStateDt in dt.m_lsStates)
            {
                CharacterStateData pData = DataBase.CreateInstance<CharacterStateData>();
                pData.FromPrefabs(editStateDt);
                this.Actions[editStateDt.m_stateType] = pData;
            }
        }
    }
    public class CharacterName : DataBase
    {
        public string Character_name;

        public override void FromXml(XmlElement xml)
        {
            XmlRead.Attr(xml, "name", ref Character_name);
        }
    }
    public class Chat_image:DataBase
    {
        public int image_id;
        public string image_source;
        public string image_name;
        public override void FromXml(XmlElement xml)
        {
            XmlRead.Attr(xml, "id", ref image_id);
            XmlRead.Attr(xml, "source_", ref image_source);
            XmlRead.Attr(xml, "text", ref image_name);

        }
    }
    public class Chat_text:DataBase
    {
        public int chat_id;
        public string chat_text;
        public override void FromXml(XmlElement xml)
        {
            XmlRead.Attr(xml,"id",ref chat_id);
            XmlRead.Attr(xml, "text", ref chat_text);
        }
    }

    public class ExpLevel : DataBase
    {
        public int level;
        public int needExp;
        public override void FromXml(XmlElement xml)
        {
            XmlRead.Attr(xml, "level", ref level);
            XmlRead.Attr(xml, "exp", ref needExp);
        }
    }

}