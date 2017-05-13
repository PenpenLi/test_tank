using System;
using System.Xml;
using System.Collections.Generic;


namespace TKBase
{
    public static class XmlRead
    {

        //这个针对枚举类型
        public static void AttrEnum<T>(XmlElement xml, string name, ref T value)
        {
            LOG.Assert(typeof(T).IsEnum, "Error XmlRead " + typeof(T).Name + "is Not Enum ");

            int temp;
            var sValue = xml.GetAttribute(name);
            if (int.TryParse(sValue, out temp))
            {
                value = (T)Enum.ToObject(typeof(T), temp);
            }
        }

        public static void Attr(XmlElement xml, string name, ref bool value)
        {
            bool temp;
            var sValue = xml.GetAttribute(name);
            if (bool.TryParse(sValue, out temp))
            {
                value = temp;
            }
        }

        public static void Attr(XmlElement xml, string name, ref int value)
        {
            int temp;
            var sValue = xml.GetAttribute(name);
            if (int.TryParse(sValue, out temp))
            {
                value = temp;
            }
        }

        public static void Attr(XmlElement xml, string name, ref float value)
        {
            float temp;
            var sValue = xml.GetAttribute(name);
            if (float.TryParse(sValue, out temp))
            {
                value = temp;
            }
        }

        public static void Attr(XmlElement xml, string name, ref double value)
        {
            double temp;
            var sValue = xml.GetAttribute(name);
            if (double.TryParse(sValue, out temp))
            {
                value = temp;
            }
        }

        public static void Attr(XmlElement xml, string name, ref string value)
        {
            var sValue = xml.GetAttribute(name);
            if (sValue != "") value = sValue;
        }

        public static XmlNodeList GetList(XmlElement xml, string name)
        {
            XmlNodeList list = xml.GetElementsByTagName(name);
            if(list.Count > 0)
            {
                return (list[0] as XmlElement).GetElementsByTagName("item");
            }
            return null;
        }
    }

    public static class XmlWrite
    {
        private static XmlDocument doc = new XmlDocument();

        public static void InitDoc(XmlDocument stDoc)
        {
            doc = stDoc;
        }

        //这个针对枚举类型
        public static void AttrEnum<T>(XmlElement xml, string name, ref T value)
        {
            LOG.Assert(typeof(T).IsEnum, "Error XmlWrite " + typeof(T).Name + "is Not Enum ");

            int temp = Convert.ToInt32(value);
            xml.SetAttribute(name, temp.ToString());
        }

        public static void Attr(XmlElement xml, string name, ref bool value)
        {
            xml.SetAttribute(name, value.ToString());
        }

        public static void Attr(XmlElement xml, string name, ref int value)
        {
            xml.SetAttribute(name, value.ToString());
        }

        public static void Attr(XmlElement xml, string name, ref double value)
        {
            xml.SetAttribute(name, value.ToString());
        }

        public static void Attr(XmlElement xml, string name, ref string value)
        {
            xml.SetAttribute(name, value);
        }
    }
}
