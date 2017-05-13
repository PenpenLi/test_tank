--[[
测试列表，有个按钮
]]

local cls_ui_duizhan = class("cls_ui_duizhan",cls_ui_base)
cls_ui_duizhan.s_ui_panel = 'oms_test/PanelDuizhan'
local l_instance = nil

function cls_ui_duizhan:ctor()
    self.super.ctor(self)
end

function cls_ui_duizhan:OnStart()
    LuaHelper.Lua_call("Net_init")
    self.m_transform = self.m_game_object.transform
    self.m_btn_match_one = self.m_transform:FindChild("one").gameObject
    self.m_btn_match_two = self.m_transform:FindChild("two").gameObject
    self.m_btn_match_three = self.m_transform:FindChild("three").gameObject
    self.m_btn_quit = self.m_transform:FindChild("PanelMiddleTop/Return").gameObject

    --以下规则提示
    self.m_btn_rule = self.m_transform:FindChild("PanelMiddleTop/Rule").gameObject
    self.m_rule_tip = self.m_transform:FindChild("PanelRoot/all").gameObject
    self.m_close_rule = self.m_transform:FindChild("PanelRoot/all/Button").gameObject
    self.m_close_rule_btn = self.m_close_rule:GetComponent('Button')
    self.m_cancel_panel = self.m_transform:FindChild("PanelRoot/cancel_panel").gameObject
    self.m_cancel_panel_btn = self.m_cancel_panel:GetComponent('Button')
    
    self.m_lua_behaviour:AddClick(self.m_cancel_panel, function (obj)
        self.m_cancel_panel:SetActive(false)
        self.m_close_rule_btn.enabled = false
        LuaHelper.DoLocalMoveY(self.m_rule_tip,800,0.3,function ()
            self.m_rule_tip:SetActive(false)
        end)
    end);
    self.m_lua_behaviour:AddClick(self.m_close_rule, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        self.m_cancel_panel:SetActive(false)
        self.m_close_rule_btn.enabled = false
        LuaHelper.DoLocalMoveY(self.m_rule_tip,800,0.3,function ()
            self.m_rule_tip:SetActive(false)
        end)
    end);
    self.m_lua_behaviour:AddClick(self.m_btn_rule, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");    
        self.m_cancel_panel_btn.enabled = false
        self.m_close_rule_btn.enabled = false
        self.m_cancel_panel:SetActive(true)
        self.m_rule_tip:SetActive(true)
        LuaHelper.DoLocalMoveY(self.m_rule_tip,4,0.3,function ()
            self.m_close_rule_btn.enabled = true
            self.m_cancel_panel_btn.enabled = true
        end)
    end);
    --以上规则提示

    self.m_lua_behaviour:AddClick(self.m_btn_match_one, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        require "ui/ui_match"
        ShowMatchUI("1")
        self:Close()
        --LuaHelper.Lua_call("NORMAL_PVP")
        --LuaHelper.Lua_call("Send_MapMatch")
    end);

    self.m_lua_behaviour:AddClick(self.m_btn_match_two, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        require "ui/ui_match"
        ShowMatchUI("2")
        self:Close()
        --LuaHelper.Lua_call("PVP_2V2")
        --LuaHelper.Lua_call("Send_MapMatch")
    end);

     self.m_lua_behaviour:AddClick(self.m_btn_match_three, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        require "ui/ui_match"
        ShowMatchUI("3")
        self:Close()
        --LuaHelper.Lua_call("PVP_2V2")
        --LuaHelper.Lua_call("Send_MapMatch")
    end);

    self.m_lua_behaviour:AddClick(self.m_btn_quit, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        SendUIMessage("ENUM_SHOW_HOST")
        self:Close()
    end);

end

function cls_ui_duizhan:OnDestroy()
    self.super.OnDestroy(self)
    l_instance = nil
end


function ShowduizhanUI()
    l_instance = cls_ui_duizhan:new()
end

function  HideduizhanUI()
    if l_instance ~= nil then
        l_instance:Close()
    end
end