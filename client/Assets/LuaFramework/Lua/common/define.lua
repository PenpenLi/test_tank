Util = LuaFramework.Util;
LuaHelper = LuaFramework.LuaHelper;

resMgr = LuaHelper.GetResManager();
panelMgr = LuaHelper.GetPanelManager();

WWW = UnityEngine.WWW;
GameObject = UnityEngine.GameObject;

Input = UnityEngine.Input;
Resources = UnityEngine.Resources

GameMode =
{
    MODE_DEBUG = 0,
    MODE_GM = 1,
    MODE_RELEASE = 2,
}

LogType = {
    Error = 0,
    Key = 1,
    Warning = 2,
    Log = 3,
    Exception = 4,
}