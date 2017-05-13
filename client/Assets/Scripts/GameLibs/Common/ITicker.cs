using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public enum TickerFlag
{
    TICK_FLAG_NONE,
    TICK_FLAG_IN_BUFFER,
    TICK_FLAG_IN_LIST,
}
public interface ITicker
{
    TickerFlag IsInTickerListFlag
    {
        set;
        get;
    }
    void Tick(uint tickCount);
}


