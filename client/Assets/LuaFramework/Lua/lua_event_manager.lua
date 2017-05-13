---[[
local luaMessageTypeCallback = {
    ["EventChangeController"] = {file = "ui/battle/ui_battle_joystick_no_hide",     func = "OnChangeController(...)"},
    ["EventBattleStop"] = {{file = "ui/battle/ui_battle_joystick", func = "HideBattleJoyStickUI(...)"},
                            {file = "ui/battle/ui_battle_joystick_no_hide", func = "HideBattleJoyStickUI2(...)"},
                            {file = "ui/jiesuan/ui_jiesuan", func = "OnBattleStop(...)"},

                                },
    ["EventHideBattleJoyStickUI"] = {file = "ui/battle/ui_battle_joystick", func = "HideBattleJoyStickUI(...)"},
    ["EventShowBattleJoyStickUI"] = {file = "ui/battle/ui_battle_joystick", func = "ShowBattleJoyStickUI(...)"},
    ["EventShowMessageUI"] = {file = "ui/battle/ui_battle_joystick", func = "Message_Netoff(...)"},
    ["EventShowMessageUIwithString"] = {file = "ui/battle/ui_battle_joystick", func = "Message_withString(...)"},
    ["EventShowMessageUIwithRefuse"] = {file = "ui/battle/ui_battle_joystick", func = "Message_withString(...)"},
    ["EventShowMessageUIwithRefuseInvitation"] = {file = "ui/battle/ui_battle_joystick", func = "Message_withString(...)"},
    ["EventShowInfomationUI"] = {file = "ui/ui_information", func = "ShowInformationUI(...)"},
    ["EventUpdateFriendList"] = {file = "ui/ui_host", func = "UpdateFriendList(...)"},
    ["EventAddFriendNotify"] = {file = "ui/ui_addfriend", func = "ShowAddfriendUI(...)"},
    ["EventBackHost"] = {file = "ui/battle/ui_battle_joystick", func = "BackHost(...)"},
    ["EventShowPrepareUI"] = {file = "ui/ui_prepare", func = "ShowPrepareUI(...)"},
    ["EventInvitationFriendNotify"] = {file = "ui/ui_inviterec", func = "ShowInviteRecUI(...)"},
    ["EventInvitationFriendConfirm"] = {file = "ui/ui_invitesend", func = "ShowInviteSendUI(...)"},

    
}

function SendLuaMessage(luaMessageType, ...)
    ReceiveCPPMessage(luaMessageType, ...)
end


function ReciveCsharpMessage(luaMessageType,...)
    local handleEvents = luaMessageTypeCallback[luaMessageType]
    if handleEvents~=nil then
        local l_callbackFun = function (luaMessageType, events, ...)
            if events.file~=nil then
                require( events.file )
            end

            local func = loadstring( events.func )
            if func then
                func( luaMessageType, ... )
            end
        end
        if handleEvents.func == nil then
            for k,v in pairs(handleEvents) do
                l_callbackFun(luaMessageType, v, ...)
            end
        else
            l_callbackFun(luaMessageType, handleEvents, ...)
        end


    end
end
--]]
