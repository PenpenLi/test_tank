using UnityEngine;
using System.Collections;

namespace TKGame
{
    public enum ManagerType
    {
        TickManager = 0,
        AIManager,
        CharacterManager,
        ResourceManager,
        TerrainManager,
        BattleManager,
        SounderManager,
        BombManager,
        DataManager,
        MAX_MANAGER_COUNT,
    }

    public class ManagerBase
    {
        private ManagerType m_eType;
        public ManagerType Type
        {
            get { return m_eType; }
        }
        public ManagerBase(ManagerType eType)
        {
            m_eType = eType;
        }

        virtual public void Initialize()
        {

        }

        virtual public void UnInitialize()
        {

        }
    }
}
