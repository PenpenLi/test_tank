--[[
消息弹窗
]]

local cls_ui_addfriend = class("cls_ui_addfriend",cls_ui_base)
cls_ui_addfriend.s_ui_panel = 'oms_test/PanelAddfriend'
cls_ui_addfriend.s_ui_order = 100
local l_instance = nil

function cls_ui_addfriend:ctor(...)
    self.super.ctor(self)
    self.arr = {...}
end

function cls_ui_addfriend:OnStart()
    self.m_transform = self.m_game_object.transform
    self.m_cancelbtn = self.m_transform:FindChild("PanelRoot/cancel_panel").gameObject;
    self.m_agree = self.m_transform:FindChild("PanelRoot/Agree").gameObject
    self.m_refuse = self.m_transform:FindChild("PanelRoot/Refuse").gameObject
    self.m_tip = self.m_transform:FindChild("PanelRoot/PanelTip/TextTip"):GetComponent('Text')
    self.m_tip.text = self.arr[2] .. " 想添加你为好友"

    self.m_lua_behaviour:AddClick(self.m_cancelbtn, function (obj)
        self:Close()
    end);
    self.m_lua_behaviour:AddClick(self.m_agree, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        LuaHelper.Send_Add_Notify(self.arr[2], 1)
        self:Close()
    end);
    self.m_lua_behaviour:AddClick(self.m_refuse, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        LuaHelper.Send_Add_Notify(self.arr[2], 2)
        self:Close()
    end);
    --self.m_tip.text = 'hehe'
end

function cls_ui_addfriend:OnDestroy()
    self.super.OnDestroy(self)
    l_instance = nil
end

function ShowAddfriendUI(...)
    l_instance = cls_ui_addfriend:new(...)
end

function DestroyAddfriendUI()
   l_instance:Close()
end

function  HideAddfriendUI()
    if l_instance ~= nil then
        l_instance:Close()
    end
end
