using System.Xml;
using UnityEngine;
using TKBase;

namespace TKGame
{
    public class BombData : DataBase
    {
        public int config_id;
        public int m_iResourceID = 300001;
        public string m_strShapePath = "Common/shape";
        public string m_strBorderPath = "Common/border";
        public float m_fMass = 10.0f;
        public float m_fGFactor = 100.0f;
        public float m_fWindFactor = 1.0f;
        public float m_fAirResitFactor = 1.0f;
        public Rect m_pAttackRect = new Rect(-3, -3, 9, 9);
        public float m_fBombRange = 100;

        public BombData()
        {

        }

        public override void FromXml(XmlElement xml)
        {
            XmlRead.Attr(xml, "config_id", ref config_id);
            XmlRead.Attr(xml, "resource_id", ref m_iResourceID);
            XmlRead.Attr(xml, "shape_path", ref m_strShapePath);
            XmlRead.Attr(xml, "border_path", ref m_strBorderPath);
            XmlRead.Attr(xml, "mass", ref m_fMass);
            XmlRead.Attr(xml, "g_factor", ref m_fGFactor);
            XmlRead.Attr(xml, "wind_factor", ref m_fWindFactor);
            XmlRead.Attr(xml, "air_resit_factor", ref m_fAirResitFactor);

            float attack_box_min_x = 0;
            XmlRead.Attr(xml, "attack_box_min_x", ref attack_box_min_x);
            m_pAttackRect.xMin = attack_box_min_x;

            float attack_box_min_y = 0;
            XmlRead.Attr(xml, "attack_box_min_y", ref attack_box_min_y);
            m_pAttackRect.yMin = attack_box_min_y;

            float attack_box_max_x = 0;
            XmlRead.Attr(xml, "attack_box_max_x", ref attack_box_max_x);
            m_pAttackRect.xMax = attack_box_max_x;

            float attack_box_max_y = 0;
            XmlRead.Attr(xml, "attack_box_max_y", ref attack_box_max_y);
            m_pAttackRect.yMax = attack_box_max_y;

            XmlRead.Attr(xml, "bomb_range", ref m_fBombRange);
        }
    }
}
