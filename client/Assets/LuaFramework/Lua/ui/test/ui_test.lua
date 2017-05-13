--[[
消息弹窗,测试用
]]

local cls_ui_test = class("cls_ui_test",cls_ui_base)
cls_ui_test.s_ui_panel = 'oms_test/PanelMessage'
cls_ui_test.s_ui_order = 1000
local l_instance = nil

function cls_ui_test:ctor(...)
    self.super.ctor(self)
end

function cls_ui_test:OnStart()
    self.m_transform = self.m_game_object.transform
    self.m_btn = self.m_transform:FindChild("PanelRoot/Button").gameObject;
    self.m_btn.transform:FindChild("Text"):GetComponent('Text').text = "重置"

    self.m_TextTip = self.m_transform:FindChild("PanelRoot/TextTip"):GetComponent('Text')

    require "logic/battle_manager"
    self.m_lua_behaviour:AddClick(self.m_btn, function (obj)

        battle_manager.TestTickCount(10)
        -- self:Close()
    end);
    battle_manager.TestTickCount(10)
    registerGlobalEvent("ENUM_TEST_TICK_COUNT", self, self.OnTickCount)
end

function cls_ui_test:OnTickCount(_,_,count_down)
    self.m_TextTip.text = string.format("%.1f",math.max(0,count_down)) .. "秒后关闭"
    if count_down <= 0 then
        self:Close()
    end
    if count_down <= 0 then
        log("count_down:"..count_down)
    end
end

function cls_ui_test:OnDestroy()
    self.super.OnDestroy(self)
    l_instance = nil
    log("cls_ui_test:OnDestroy")
end

function ShowTestUI(...)
    l_instance = cls_ui_test:new(...)
end