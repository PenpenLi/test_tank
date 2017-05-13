using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GOWRandom
{
    private static Random m_stRandom = new Random();
    public static int GetRandom()
    {
        return m_stRandom.Next()%1000;
    }

}

