--[[
消息弹窗
]]

local cls_ui_message2 = class("cls_ui_message2",cls_ui_base)
cls_ui_message2.s_ui_panel = 'oms_test/PanelMessage2'
cls_ui_message2.s_ui_order = 100
local l_instance = nil

function cls_ui_message2:ctor(...)
    self.super.ctor(self)
    local arr = {...}
    self.m_msg = table.concat( arr, "\n")
end

function cls_ui_message2:OnStart()
    self.m_transform = self.m_game_object.transform
    self.m_btn = self.m_transform:FindChild("PanelRoot/cancel_panel").gameObject;
    self.m_btn1 = self.m_transform:FindChild("PanelRoot/Button").gameObject
    self.m_transform:FindChild("PanelRoot/PanelTip/TextTip"):GetComponent('Text').text = tostring(self.m_msg)

    self.m_lua_behaviour:AddClick(self.m_btn1, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");

        require "ui/battle/ui_battle_joystick"
        HideAllUI()

        ShowUserloginUI()

        self:Close()
    end);
    self.m_lua_behaviour:AddClick(self.m_btn, function (obj)
        --LuaHelper.Play_Sound(1,"sound/yinxiao/03");

        require "ui/battle/ui_battle_joystick"
        HideAllUI()

        ShowUserloginUI()

        self:Close()
    end);
end

function cls_ui_message2:OnDestroy()
    self.super.OnDestroy(self)
    l_instance = nil
end

function ShowMessage2UI(...)
    l_instance = cls_ui_message2:new(...)
end

function DestroyMessage2UI()
   l_instance:Close()
end
