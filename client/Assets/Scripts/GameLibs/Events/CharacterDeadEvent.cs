using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TKGame
{
    public class CharacterDeadEvent : EventArgs
    {
        public int m_iUniqueID;
        public CharacterDeadEvent(int uid)
        {
            m_iUniqueID = uid;
        }
    }
}
