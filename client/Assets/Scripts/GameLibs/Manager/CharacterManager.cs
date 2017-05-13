using System.Collections.Generic;
using TKBase;

namespace TKGame
{
    public class CharacterManager: ManagerBase
    {
        public delegate void OnCharacterNewOrDelete(CharacterLogic value);
        public event OnCharacterNewOrDelete OnCharacterNew;
        public event OnCharacterNewOrDelete OnCharacterDelete;

        private Dictionary<int, CharacterLogic> m_dicCharacterLogic;

        public CharacterManager()
            :base(ManagerType.CharacterManager)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            m_dicCharacterLogic = new Dictionary<int, CharacterLogic>();
        }

        public override void UnInitialize()
        {
            base.UnInitialize();
        }

        public CharacterLogic GetCharacterByUid(int uid)
        {
            if(m_dicCharacterLogic.ContainsKey(uid))
            {
                return m_dicCharacterLogic[uid];
            }
            return null;
        }

        public CharacterLogic CreateCharacter(int uid, CharacterInstructionData pData)
        {
            if (m_dicCharacterLogic.ContainsKey(uid))
            {
                return m_dicCharacterLogic[uid];
            }
            else
            {
                CharacterLogic pRet = ObjectPools.CheckOut<CharacterLogic>((int)uid, pData);
                m_dicCharacterLogic[uid] = pRet;
                if (OnCharacterNew != null)
                {
                    OnCharacterNew(pRet);
                }
                return pRet;
            }
        }

        public List<CharacterLogic> GetCharacterByTeam(int team, bool isSameTeam, bool onlyAlive)
        {
            List<CharacterLogic> retList = new List<CharacterLogic>();
            foreach (var c in m_dicCharacterLogic)
            {
                if(onlyAlive && !c.Value.IsLiving)
                {
                    continue;
                }
                if(isSameTeam)
                {
                    if(team == c.Value.m_pInfo.m_iTeam)
                    {
                        retList.Add(c.Value);
                    }
                }
                else
                {
                    if (team != c.Value.m_pInfo.m_iTeam)
                    {
                        retList.Add(c.Value);
                    }
                }
            }
            return retList;
        }

        public void CheckIn(CharacterLogic pCharacter)
        {
            if (OnCharacterDelete != null) OnCharacterDelete(pCharacter);
            ObjectPools.CheckIn(pCharacter);
        }

        public void Clear()
        {
            foreach(var logic in m_dicCharacterLogic)
            {
                CheckIn(logic.Value);
            }
            m_dicCharacterLogic.Clear();
        }

        public void SetData(MapData pData)
        {

        }
    }

}