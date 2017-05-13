--[[
消息弹窗
]]

local cls_ui_invitesend = class("cls_ui_invitesend",cls_ui_base)
cls_ui_invitesend.s_ui_panel = 'oms_test/PanelInviteSend'
cls_ui_invitesend.s_ui_order = 100
local l_instance = nil

function cls_ui_invitesend:ctor(...)
    self.super.ctor(self)
    self.arr = {...}
    self.m_msg = self.arr[2]
end

function cls_ui_invitesend:OnStart()
    self.m_transform = self.m_game_object.transform
    self.m_confirm = self.m_transform:FindChild("PanelRoot/Confirm").gameObject
    self.m_cancel = self.m_transform:FindChild("PanelRoot/Cancel").gameObject
    self.m_tip = self.m_transform:FindChild("PanelRoot/PanelTip/TextTip"):GetComponent('Text')
    self.m_tip.text = "确定挑战" .. tostring(self.m_msg) .. "进行一场1V1对战吗？"

    self.m_lua_behaviour:AddClick(self.m_confirm, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        LuaHelper.Lua_call("Lua_Invitation_Friend")
        --body

        self:Close()
    end);
    self.m_lua_behaviour:AddClick(self.m_cancel, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        self:Close()
    end);
end

function cls_ui_invitesend:OnDestroy()
    self.super.OnDestroy(self)
    l_instance = nil
end

function ShowInviteSendUI(...)
    l_instance = cls_ui_invitesend:new(...)
end

function DestroyInviteSendUI()
   l_instance:Close()
end

function  HideInviteSendUI()
    if l_instance ~= nil then
        l_instance:Close()
    end
end
