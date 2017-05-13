using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TKGame
{
    public class ChangeControllerEvent : EventArgs
    {
        public int m_iUniqueID;

        public ChangeControllerEvent(int uid)
        {
            m_iUniqueID = uid;
        }
    }
}
