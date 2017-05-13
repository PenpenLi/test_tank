--[[
消息弹窗
]]

local cls_ui_inviterec = class("cls_ui_inviterec",cls_ui_base)
cls_ui_inviterec.s_ui_panel = 'oms_test/PanelInviteRec'
cls_ui_inviterec.s_ui_order = 100
local l_instance = nil

function cls_ui_inviterec:ctor(...)
    self.super.ctor(self)
    self.arr = {...}
    self.m_msg = self.arr[2]
end

function cls_ui_inviterec:OnStart()
    self.m_transform = self.m_game_object.transform
    self.m_agree = self.m_transform:FindChild("PanelRoot/Agree").gameObject
    self.m_refuse = self.m_transform:FindChild("PanelRoot/Refuse").gameObject
    self.m_tip = self.m_transform:FindChild("PanelRoot/PanelTip/TextTip"):GetComponent('Text')
    self.m_tip.text = tostring(self.m_msg) .. "向你发起了一场挑战"

    self.m_lua_behaviour:AddClick(self.m_agree, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        LuaHelper.Lua_set("Send_Invitation", 1)
        --body
        self:Close()
    end);
    self.m_lua_behaviour:AddClick(self.m_refuse, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        LuaHelper.Lua_set("Send_Invitation", 0)
        --body
        self:Close()
    end);
end

function cls_ui_inviterec:OnDestroy()
    self.super.OnDestroy(self)
    l_instance = nil
end

function ShowInviteRecUI(...)
    l_instance = cls_ui_inviterec:new(...)
end

function DestroyInviteRecUI()
   l_instance:Close()
end

function  HideInviteRecUI()
    if l_instance ~= nil then
        l_instance:Close()
    end
end