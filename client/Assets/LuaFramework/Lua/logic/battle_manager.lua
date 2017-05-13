module("battle_manager",package.seeall)

function StartBattle(map_id)
    --SendUIMessage("ENUM_SHOW_BATTLE_JOYSTICK_UI")
    --SendUIMessage("ENUM_SHOW_BATTLE_JOYSTICK_UI_NO_HIDE")
    
    LuaHelper.StartBattle(map_id)
    

    return true
end

function Net_StartBattle()

    LuaHelper.Net_StartBattle()
    SendUIMessage("ENUM_SHOW_BATTLE_JOYSTICK_UI")

    return true
end

local Time = Time
local count_down = 0
local function _TestTickUpdate()
    count_down = count_down - Time.deltaTime
    triggerGlobalEvent("ENUM_TEST_TICK_COUNT",count_down)
    if count_down <= -1 then
        UpdateBeat:Remove(_TestTickUpdate, "_TestTickUpdate")
    end
end

function TestTickCount(count_down_)
    if count_down > 0 then
        count_down = count_down_
        return
    end
    count_down = count_down_
    UpdateBeat:Add(_TestTickUpdate,"_TestTickUpdate")
end