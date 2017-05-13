
local cls_ui_gmButton = class("cls_ui_gm_button",cls_ui_base)
cls_ui_gmButton.s_ui_panel = 'UI/gm_ui/PanelGMButton'
cls_ui_gmButton.s_ui_order = 10000
local l_instance = nil

function cls_ui_gmButton:ctor(...)
    self.super.ctor(self)
end

function cls_ui_gmButton:OnStart()
    self.m_transform = self.m_game_object.transform
    self.m_btn = self.m_transform:FindChild("GMButton").gameObject;
    self.m_lua_behaviour:AddClick(self.m_btn, function (obj)
        require "ui/gm/ui_gm"
        ShowGMPanel()
    end);
end


function cls_ui_gmButton:OnDestroy()
    self.super.OnDestroy(self)
    l_instance = nil
end

function ShowGMButton()
    if l_instance == nil then
        l_instance = cls_ui_gmButton:new()
    else
        l_instance:Close()
    end
end
