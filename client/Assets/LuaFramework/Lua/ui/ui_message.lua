--[[
消息弹窗
]]

local cls_ui_message = class("cls_ui_message",cls_ui_base)
cls_ui_message.s_ui_panel = 'oms_test/PanelMessage'
cls_ui_message.s_ui_order = 100
local l_instance = nil

function cls_ui_message:ctor(...)
    self.super.ctor(self)
    local arr = {...}
    if(arr[2] == "error") then
        self.m_msg = "用户不存在或者输入不合法"
    elseif(arr[2] == "refuse") then
        self.m_msg = "对方拒绝添加你为好友"
    elseif(arr[2] == "refuse2") then
        self.m_msg = "对方拒绝好友1v1比赛"
    else
        self.m_msg = table.concat( arr, "\n")
    end
end

function cls_ui_message:OnStart()
    self.m_transform = self.m_game_object.transform
    self.m_btn = self.m_transform:FindChild("PanelRoot/cancel_panel").gameObject;
    self.m_btn1 = self.m_transform:FindChild("PanelRoot/Button").gameObject
    self.m_transform:FindChild("PanelRoot/PanelTip/TextTip"):GetComponent('Text').text = tostring(self.m_msg)

    self.m_lua_behaviour:AddClick(self.m_btn1, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        self:Close()
    end);
    self.m_lua_behaviour:AddClick(self.m_btn, function (obj)
        --LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        self:Close()
    end);
end

function cls_ui_message:OnDestroy()
    self.super.OnDestroy(self)
    l_instance = nil
end

function ShowMessageUI(...)
    l_instance = cls_ui_message:new(...)
end

function DestroyMessageUI()
   l_instance:Close()
end

function  HideMessageUI()
    if l_instance ~= nil then
        l_instance:Close()
    end
end
