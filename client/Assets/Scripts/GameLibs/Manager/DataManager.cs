using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using TKBase;

namespace TKGame
{
    public class DataManager:ManagerBase
    {
        private Dictionary<int, MapData> m_dicMapData;
        private Dictionary<int, CharacterInstructionData> m_dicCharacterData;
        private Dictionary<int, SkillINstructionData> m_dicSkillData;
        private Dictionary<int, BombData> m_dicBombData;
        private Dictionary<int, string> m_dicFirstNameDate;
        private Dictionary<int, string> m_dicSecondNameDate;
        private Dictionary<int, string> m_dicThirdNameDate;
        private Dictionary<int, Chat_image> m_dicImageDate;
        private Dictionary<int, Chat_text> m_dicChatDate;
        

        //private Dictionary<int, int> m_dicExpToLevel;           //经验换等级
        private Dictionary<int, int> m_Level2Exp;
        private Dictionary<int, int> m_Exp2Level;
        private Dictionary<int, int> m_resExp2Level;
        private int maxExp = 0;
        private int maxLevel = 0;

        public DataManager()
            :base(ManagerType.DataManager)
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();
            m_dicMapData = new Dictionary<int, MapData>();
            m_dicCharacterData = new Dictionary<int, CharacterInstructionData>();
            m_dicSkillData = new Dictionary<int, SkillINstructionData>();
            m_dicBombData = new Dictionary<int, BombData>();
            m_dicFirstNameDate = new Dictionary<int, string>();
            m_dicSecondNameDate = new Dictionary<int, string>();
            m_dicThirdNameDate = new Dictionary<int, string>();
            m_dicImageDate = new Dictionary<int, Chat_image>();
            m_dicChatDate = new Dictionary<int, Chat_text>();
            //m_dicExpToLevel = new Dictionary<int, int>();
            m_Level2Exp = new Dictionary<int, int>();
            m_Exp2Level = new Dictionary<int, int>();
            m_resExp2Level = new Dictionary<int, int>();

        }
        public override void UnInitialize()
        {
            base.UnInitialize();

            m_dicMapData.Clear();
            m_dicMapData = null;

            m_dicCharacterData.Clear();
            m_dicCharacterData = null;

            m_dicBombData.Clear();
            m_dicBombData = null;

            m_dicFirstNameDate.Clear();
            m_dicFirstNameDate = null;

            m_dicSecondNameDate.Clear();
            m_dicSecondNameDate = null;

            m_dicThirdNameDate.Clear();
            m_dicThirdNameDate = null;

            m_Level2Exp.Clear();
            m_Level2Exp = null;

            m_Exp2Level.Clear();
            m_Exp2Level = null;

            m_resExp2Level.Clear();
            m_resExp2Level = null;

            m_dicSkillData.Clear();
            m_dicSkillData = null;

            m_dicImageDate.Clear();
            m_dicImageDate = null;

            m_dicChatDate.Clear();
            m_dicImageDate = null;
        }

        private MapData LoadMapListData(int iMapID)
        {
            string path = Application.dataPath;
            Object obj = Resources.Load("Config/" + iMapID);
            string f = obj.ToString();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(f);
            XmlElement root =  doc.DocumentElement;
            MapData data = new MapData();
            data.FromXml(root);
            m_dicMapData[iMapID] = data;
            return data;
        }

        private void LoadCharacterData()
        {
            string path = Application.dataPath;
            Object obj = Resources.Load("Config/character_data");
            string f = obj.ToString();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(f);
            XmlElement root = doc.DocumentElement;

            XmlNodeList xmlList = root.GetElementsByTagName("instruction");
            foreach (XmlNode node in xmlList)
            {
                XmlElement ele = node as XmlElement;
                CharacterInstructionData pData = DataBase.CreateInstance<CharacterInstructionData>();
                pData.FromXml(ele);
                m_dicCharacterData[pData.id] = pData;
            }
            /*GameObject obj = (GameObject)Resources.Load("Prefab/EditorPrefab/CharacterEditorData");
            GameObject gm = Object.Instantiate(obj);
            CharacterEditorData[] chaEditorDts = gm.GetComponents<CharacterEditorData>();
            foreach(CharacterEditorData dt in chaEditorDts)
            {
                CharacterInstructionData pData = DataBase.CreateInstance<CharacterInstructionData>();
                pData.FromPrefabs(dt);
                m_dicCharacterData[pData.id] = pData;
            }*/
        }
        private void LoadSkillData()
        {
            string path = Application.dataPath;
            Object obj = Resources.Load("Config/skill_data");
            string f = obj.ToString();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(f);
            XmlElement root = doc.DocumentElement;

            XmlNodeList xmlList = root.GetElementsByTagName("instruction");
            foreach (XmlNode node in xmlList)
            {
                XmlElement ele = node as XmlElement;
                SkillINstructionData pData = DataBase.CreateInstance<SkillINstructionData>();
                pData.FromXml(ele);
                m_dicSkillData[pData.id] = pData;
            }
        }
        private void LoadCharacterNameData(string name_data)
        {
            string path = Application.dataPath;
            Object obj = Resources.Load("Config/name_data");
            string f = obj.ToString();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(f);
            XmlElement root = doc.DocumentElement;
            
            XmlNodeList xmlList = root.GetElementsByTagName(name_data);
            int t = 0;
            foreach (XmlNode node in xmlList)
            {
                XmlElement ele = node as XmlElement;
                CharacterName pData = DataBase.CreateInstance<CharacterName>();
                pData.FromXml(ele);
                if(name_data=="first_name")
                m_dicFirstNameDate[t++] = pData.Character_name;
                if (name_data == "second_name")
                m_dicSecondNameDate[t++] = pData.Character_name;
                if (name_data == "third_name")
                m_dicThirdNameDate[t++] = pData.Character_name;
            }
        }
        private void LoadImageData()
        {
            string path = Application.dataPath;
            Object obj = Resources.Load("Config/image_data");
            string f = obj.ToString();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(f);
            XmlElement root = doc.DocumentElement;

            XmlNodeList xmlList = root.GetElementsByTagName("source");
          
            foreach (XmlNode node in xmlList)
            {
                XmlElement ele = node as XmlElement;
                Chat_image pData = DataBase.CreateInstance<Chat_image>();
                pData.FromXml(ele);
                m_dicImageDate[pData.image_id] = pData;
            }


             xmlList = root.GetElementsByTagName("chat");
            foreach (XmlNode node in xmlList)
            {
                XmlElement ele = node as XmlElement;
                Chat_text pData = DataBase.CreateInstance<Chat_text>();
                pData.FromXml(ele);
                m_dicChatDate[pData.chat_id] = pData;
            }
        }

        private void LoadCharacterLevelInfo()                              //level->exp       XML
        {
            string path = Application.dataPath;
            Object obj = Resources.Load("Config/level_data");
            string f = obj.ToString();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(f);
            XmlElement root = doc.DocumentElement;

            XmlNodeList xmlList = root.GetElementsByTagName("level_info");

            maxExp = 0;
            foreach (XmlNode node in xmlList)
            {
                XmlElement ele = node as XmlElement;
                ExpLevel pData = DataBase.CreateInstance<ExpLevel>();
                pData.FromXml(ele);
                //m_dicExpToLevel[pData.level] = pData.needExp;
                
                m_Level2Exp[(int)pData.level] = pData.needExp - maxExp;
                for (int i = maxExp; i < pData.needExp; i ++)
                {
                    m_Exp2Level[i] = pData.level;
                    m_resExp2Level[i] = i - maxExp;
                }
                maxExp = pData.needExp;
                maxLevel = pData.level;
            }
            //debug
            //for (int i = 0; i <= 637; i ++)
            //{
            //    int level = m_Exp2Level[i];
            //    int res = m_resExp2Level[i];
            //    int sumlevel = m_Level2Exp[level];
            //    Debug.Log(level + "zzzz" + res + "zzzz" + sumlevel);
            //}
        }

        private void LoadBombData()
        {
            string path = Application.dataPath;
            Object obj = Resources.Load("Config/bomb_data");
            string f = obj.ToString();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(f);
            XmlElement root = doc.DocumentElement;
            XmlNodeList xmlList = root.GetElementsByTagName("bomb");
            foreach (XmlNode node in xmlList)
            {
                XmlElement ele = node as XmlElement;

                int id = 0;
                XmlRead.Attr(ele, "", ref id);

                BombData pData = DataBase.CreateInstance<BombData>();
                pData.FromXml(ele);
                m_dicBombData[pData.config_id] = pData;
            }
        }

        public void LoadAllData()
        {
            LoadCharacterData();
            LoadBombData();
            LoadSkillData();
            LoadCharacterNameData("first_name");
            LoadCharacterNameData("second_name");
            LoadCharacterNameData("third_name");
            LoadCharacterLevelInfo();
            LoadImageData();
        }

        public MapData GetMapDataByID(int iMapID)
        {
            if(m_dicMapData.ContainsKey(iMapID))
            {
                return m_dicMapData[iMapID];
            }
            else
            {
                return LoadMapListData(iMapID);
            }
        }

        public CharacterInstructionData GetCharacterInstructionByID(int id)
        {
            if(m_dicCharacterData.ContainsKey(id))
            {
                return m_dicCharacterData[id];
            }
            else
            {
                return null;
            }
        }
        public SkillINstructionData GetSkillINstructionDataByID(int id)
        {
            if(m_dicSkillData.ContainsKey(id))
            {
                return m_dicSkillData[id];
            }
            else
            {
                return null;
            }
        }
        public Chat_image GetImageByID(int id)
        {
            if(m_dicImageDate.ContainsKey(id))
            {
                return m_dicImageDate[id];
            }
            else

            {
                return null;
            }
        }
        public Chat_text GetChatByID(int id)
        {
            if(m_dicChatDate.ContainsKey(id))
            {
                return m_dicChatDate[id];
            }
            else
            {
                return null;
            }
        }
        public int ExpToLevel(int exp, int type)
        {
            //type 1.level2exp 2.exp2level 3.resexp2level
            if (exp >= maxExp)
            {
                if (type == 1)
                {
                    return m_Level2Exp[maxLevel];
                }
                else if (type == 2)
                {
                    return maxLevel;
                }
                else
                {
                    return maxLevel;
                }
            }
            int level = m_Exp2Level[exp];
            if (type == 1)
            {
                return m_Level2Exp[level];
            }
            else if (type == 2)
            {
                return level;
            }
            else
            {
                return m_resExp2Level[exp];
            }
        }

        public string GetCharaterName(string _name)
        {
            if (_name=="first_name")
            {
                return m_dicFirstNameDate[GOWRandom.GetRandom()%(m_dicFirstNameDate.Count)];
            }
            else if(_name=="second_name")
            {
                return m_dicSecondNameDate[GOWRandom.GetRandom() % m_dicSecondNameDate.Count];
            }
            else
            {
                return m_dicThirdNameDate[GOWRandom.GetRandom() % m_dicThirdNameDate.Count];
            }
        }

        public BombData GetBombDataByID(int configID)
        {
            if(m_dicBombData.ContainsKey(configID))
            {
                return m_dicBombData[configID];
            }
            else
            {
                return null;
            }
        }
    }
}
