local luaTypeCallback = {

    -- common message
    ["ENUM_SHOW_MESSAGE"] = { file = "ui/ui_message", func = "ShowMessageUI(...)"},
    ["ENUM_SHOW_MESSAGE2"] = { file = "ui/ui_message2", func = "ShowMessage2UI(...)"},
    ["ENUM_SHOW_INVITEREC"] = { file = "ui/ui_inviterec", func = "ShowInviteRecUI(...)"},
    ["ENUM_SHOW_INVITESEND"] = { file = "ui/ui_invitesend", func = "ShowInviteSendUI(...)"},
    ["ENUM_SHOW_ADDFRIEND"] = { file = "ui/ui_addfriend", func = "ShowAddfriendUI(...)"},
    ["ENUM_SHOW_INFORMATION"] = { file = "ui/ui_information", func = "ShowInformationUI(...)"},
    ["ENUM_SHOW_LOGIN"] = { file = "ui/ui_login", func = "ShowLoginUI(...)"},
    ["ENUM_SHOW_BATTLE_JOYSTICK_UI"] = { file = "ui/battle/ui_battle_joystick", func = "ShowBattleJoyStickUI(...)"},
    ["ENUM_SHOW_BATTLE_JOYSTICK_UI_NO_HIDE"] = { file = "ui/battle/ui_battle_joystick_no_hide", func = "ShowBattleJoyStickUI2(...)"},
    ["ENUM_SHOW_OVER"] = {file = "ui/ui_over", func = "ShowOverUI(...)"},
    ["ENUM_SHOW_OVERTIP"] = {file = "ui/ui_overtip", func = "ShowOverTipUI(...)"},
    ["ENUM_SHOW_ROLE"] = {file = "ui/ui_role", func = "ShowRoleUI(...)"},
    ["ENUM_SHOW_Userlogin"] = {file = "ui/ui_userlogin", func = "ShowUserloginUI(...)"},
    ["ENUM_SHOW_GM_BUTTON"] = {file = "ui/gm/ui_gm_button", func = "ShowGMButton(...)"},
    ["ENUM_SHOW_HOST"] = {file = "ui/ui_host", func = "ShowHostUI(...)"},
    ["ENUM_SHOW_Duizhan"] = {file = "ui/ui_duizhan", func = "ShowduizhanUI(...)"},
    ["ENUM_SHOW_MATCH"] = {file = "ui/ui_match", func = "ShowMatchUI(...)"},
    ["ENUM_SHOW_PREPARE"] = {file = "ui/ui_prepare", func = "ShowPrepareUI(...)"},
    ["ENUM_SHOW_CHOOSE"] = {file = "ui/ui_choose", func = "ShowChooseUI(...)"},
    ["ENUM_SHOW_CHOOSEMAP"] = {file = "ui/ui_chooseMap", func = "ShowchooseMapUI(...)"},
    ["ENUM_SHOW_LOADING"] = {file = "ui/ui_loading", func = "ShowLoadingUI(...)"}
}

local function ReceiveUIMessage(message,... )
    local handleEvents = luaTypeCallback[message]
    if handleEvents then
        if handleEvents.file~=nil then
            require( handleEvents.file )
        end

        local func = handleEvents.func
        if type(func) == "string" then
            func = loadstring(func)
        end

        if func then
            func( luaType, ... )
        end
        func = nil
    end
end

function SendUIMessage( ... )
    ReceiveUIMessage(...)
end