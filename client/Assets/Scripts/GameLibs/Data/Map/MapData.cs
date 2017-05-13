using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using TKBase;

namespace TKGame
{
    public class MapData : DataBase
    {
        public int bound_left;
        public int bound_top;
        public int bound_right;
        public int bound_bottom;

        public float gravity;
        public float air_resistance;
        public float wind_range;
        public float wind_step;

        public float camera_margin_x;
        public float camera_margin_y;
        public float camera_smooth_x;
        public float camera_smooth_y;

        public float camera_min_x;
        public float camera_min_y;
        public float camera_max_x;
        public float camera_max_y;

        private Dictionary<int, List<BornData>> m_dicBornData;

        public List<MapCellData> m_listBG;
        public List<MapCellData> m_listTerrain;

        public MapData()
        {
            m_dicBornData = new Dictionary<int, List<BornData>>();
            m_listBG = new List<MapCellData>();
            m_listTerrain = new List<MapCellData>();
        }

        override public void FromXml(XmlElement xml)
        {
            XmlRead.Attr(xml, "bound_left", ref bound_left);
            XmlRead.Attr(xml, "bound_top", ref bound_top);
            XmlRead.Attr(xml, "bound_right", ref bound_right);
            XmlRead.Attr(xml, "bound_bottom", ref bound_bottom);

            XmlRead.Attr(xml, "gravity", ref gravity);
            XmlRead.Attr(xml, "air_resistance", ref air_resistance);
            XmlRead.Attr(xml, "wind_range", ref wind_range);
            XmlRead.Attr(xml, "wind_step", ref wind_step);

            XmlRead.Attr(xml, "camera_margin_x", ref camera_margin_x);
            XmlRead.Attr(xml, "camera_margin_y", ref camera_margin_y);
            XmlRead.Attr(xml, "camera_smooth_x", ref camera_smooth_x);
            XmlRead.Attr(xml, "camera_smooth_y", ref camera_smooth_y);

            XmlRead.Attr(xml, "camera_min_x", ref camera_min_x);
            XmlRead.Attr(xml, "camera_min_y", ref camera_min_y);
            XmlRead.Attr(xml, "camera_max_x", ref camera_max_x);
            XmlRead.Attr(xml, "camera_max_y", ref camera_max_y);

            ReadBorn(xml);
            ReadMapCell(xml, "background", m_listBG);
            ReadMapCell(xml, "terrain", m_listTerrain);
        }

        private void ReadBorn(XmlElement xml)
        {
            XmlNodeList bornList = XmlRead.GetList(xml, "born_pos");
            m_dicBornData.Clear();
            foreach (XmlNode node in bornList)
            {
                XmlElement ele = node as XmlElement;
                BornData pBorn = DataBase.CreateInstance<BornData>();
                pBorn.FromXml(ele);
                int team = 0;
                XmlRead.Attr(ele, "team", ref team);
                if (m_dicBornData.ContainsKey(team))
                {
                    m_dicBornData[team].Add(pBorn);
                }
                else
                {
                    m_dicBornData[team] = new List<BornData>() { pBorn };
                }
            }
        }

        private void ReadMapCell(XmlElement xml, string name, List<MapCellData> list)
        {
            XmlNodeList xmlList = XmlRead.GetList(xml, name);
            list.Clear();
            foreach(XmlNode node in xmlList)
            {
                XmlElement ele = node as XmlElement;
                MapCellData pData = DataBase.CreateInstance<MapCellData>();
                pData.FromXml(ele);
                list.Add(pData);
            }
        }

        public BornData GetBornData(int team, int index)
        {
            if(m_dicBornData.ContainsKey(team))
            {
                if(m_dicBornData[team].Count > index)
                {
                    return m_dicBornData[team][index];
                }
                return m_dicBornData[team][0];
            }
            return m_dicBornData[0][0];
        }
    }
}
