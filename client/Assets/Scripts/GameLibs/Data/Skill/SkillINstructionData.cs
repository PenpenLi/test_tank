using System.Collections.Generic;
using System;
using UnityEngine;
using System.Xml;
using TKBase;
using TKGameView;
namespace TKGame
{
    public class SkillINstructionData : DataBase
    {
        /* 技能使用说明
         * 
         */
        public int     id = 100001;
        public string  default_name;
        public string  description;
        public int     damage;
        public int     center_damage;
        public int     cd;
        public int     index;
        public int     energy;
        public string  image_resource;
        public string  fly_voice_resource;
        public string  bomb_voice_resource;
        public override void FromXml(XmlElement xml)
        {
            XmlRead.Attr(xml, "id", ref id);
            XmlRead.Attr(xml,"default_name",ref default_name);
            XmlRead.Attr(xml,"description",ref description);
            XmlRead.Attr(xml,"damage",ref damage);
            XmlRead.Attr(xml,"center_damage",ref center_damage);
            XmlRead.Attr(xml,"cd",ref cd);
            XmlRead.Attr(xml, "energy",ref energy);
            XmlRead.Attr(xml,"index",ref index);
            XmlRead.Attr(xml,"image_resource",ref image_resource);
            XmlRead.Attr(xml,"fly_voice_resource",ref fly_voice_resource);
            XmlRead.Attr(xml,"bomb_voice_resource",ref bomb_voice_resource);
        }
    }
}