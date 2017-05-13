using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class EncrypInt
{
    private int m_iIndex;
    private int m_iEncrypValue;
    public int Value
    {
        get 
        { 
            int val = m_iEncrypValue;
            val ^= (1 << (16 + m_iIndex % 9));
            val ^= 52942;
            val ^= (1 << m_iIndex);
            m_iIndex = 5 + GOWRandom.GetRandom() % 11;
            Value = val;
            return val;
        }

        set 
        {
            value ^= (1 << (16 + m_iIndex % 9));
            //		value ^= (1<<8);
            //		value ^= (1<<5);
            //		value ^= (1<<4);
            //		value ^= (1<<2);
            value ^= 52942;
            value ^= (1 << m_iIndex);
            m_iEncrypValue = value;
			
        }
    }

    public EncrypInt()
    {
        m_iIndex = 5 + GOWRandom.GetRandom() % 11;
        Value = 0;
    }
}

