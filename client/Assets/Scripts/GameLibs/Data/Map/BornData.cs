using UnityEngine;
using System;
using TKBase;

namespace TKGame
{
    public class BornData : DataBase
    {
        public int pos_x;
        public int pos_y;
        public int face;
        public BornData()
        {

        }

        override public void FromXml(System.Xml.XmlElement xml)
        {
            try
            {
                XmlRead.Attr(xml, "pos_x", ref pos_x);
                XmlRead.Attr(xml, "pos_y", ref pos_y);
                XmlRead.Attr(xml, "face", ref face);
            }
            catch(Exception e)
            {
                LOG.Error(e.Message);
            }
        }
    }
}
