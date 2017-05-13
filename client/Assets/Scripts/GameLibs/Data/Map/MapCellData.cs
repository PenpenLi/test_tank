using System.Xml;
using TKBase;

namespace TKGame
{
    public class MapCellData : DataBase
    {
        public string resource;
        public int pos_x;
        public int pos_y;
        public bool digable;
	    public int Width;
	    public int Height;

        public override void FromXml(XmlElement xml)
        {
            XmlRead.Attr(xml, "resource", ref resource);
            XmlRead.Attr(xml, "pos_x", ref pos_x);
            XmlRead.Attr(xml, "pos_y", ref pos_y);
            XmlRead.Attr(xml, "digable", ref digable);
			XmlRead.Attr(xml,"width",ref Width);
			XmlRead.Attr(xml, "height", ref Height);
		}
    }
}
