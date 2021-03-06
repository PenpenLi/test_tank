--[[
lua里约定

1. 类名小写下划线风格，加`cls_`前缀，其中ui类前缀为`cls_ui_`
2. 类属性名小写下划线风格，私有的前缀`_`，共有的前缀`m_`，属于类的前缀`s_`
3. 类方法名驼峰风格，事件响应型的前缀`On`，特殊方法`ctor,new`等再考虑
4. 文件名小写下划线风格，ui文件前缀`ui_`，其中定义的ui类的后缀和文件名一致

1. logic使用事件通知ui

]]




-- 这个文件会在c#里首先加载执行
require "common/define"
require "common/functions"
require "common/misc"
require "common/event_control"
require "ui/ui_base"
require "ui/ui_event_control"
require "lua_event_manager"


-- LuaManager初始化好了
function Main()
    LuaHelper.Log("LuaManager初始化好了",LogType.Key)
end

-- GameManager初始化好了，游戏正式开始。
function OnInitOK()
    --SendUIMessage("ENUM_SHOW_LOGIN")
    SendUIMessage("ENUM_SHOW_Userlogin")
    if LuaHelper.GameMode() ~= GameMode.MODE_RELEASE then
        SendUIMessage("ENUM_SHOW_GM_BUTTON")
    end
    LuaHelper.Log("GameMode:"..LuaHelper.GameMode(),LogType.Key)
    LuaHelper.Log("GameManager初始化好了，游戏正式开始。",LogType.Key)
end

--场景切换通知
function OnLevelWasLoaded(level)
    Time.timeSinceLevelLoad = 0
end
