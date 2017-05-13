--[[
测试列表，有个按钮
]]

local cls_ui_login = class("cls_ui_login",cls_ui_base)
cls_ui_login.s_ui_panel = 'oms_test/PanelLogin'
local l_instance = nil

function cls_ui_login:ctor()
    self.super.ctor(self)
end

function cls_ui_login:OnStart()
    self.m_transform = self.m_game_object.transform
    self.m_btn_login = self.m_transform:FindChild("ButtonLogin").gameObject;
    self.m_input_name = self.m_transform:FindChild("InputFieldName/Text"):GetComponent('Text')
    self.m_input_place_holder = self.m_transform:FindChild("InputFieldName/Placeholder"):GetComponent('Text')

    -- self.obj = self.m_transform:FindChild("obj").gameObject
    -- LuaHelper.SetAni(1, self.obj)
    self.m_btn_quit = self.m_transform:FindChild("quit").gameObject

    self.m_lua_behaviour:AddClick(self.m_btn_login, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        local name = self.m_input_name.text
        if name == "" then
            name = self.m_input_place_holder.text
        end
        
        local id = tonumber(name)
           if id and id  >= 1 and id <= 6 and math.ceil(id)==id then
        
        elseif id and id >= 201 and id <= 203 and math.ceil(id) then

        elseif id >= 301 and id <= 303 then

        elseif id == 100001 then
            SendUIMessage("ENUM_SHOW_ADDFRIEND")
            return
        else
            id=1
            require "ui/ui_message"
            ShowMessageUI("请输入数字1 - 5,选择地图")
            return
        end
        
        self:Close()

        require "logic/battle_manager"
        battle_manager.StartBattle(id)

        require "ui/ui_loading"
        ShowLoadingUI()

    end);

    self.m_lua_behaviour:AddClick(self.m_btn_quit, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        local val = LuaHelper.GetVersion()
        if(val == 1) then
            SendUIMessage("ENUM_SHOW_HOST")
        else
            SendUIMessage("ENUM_SHOW_Userlogin")
        end

        self:Close()
    end);

end

function cls_ui_login:OnDestroy()
    self.super.OnDestroy(self)
    l_instance = nil
end


function ShowLoginUI()
    l_instance = cls_ui_login:new()
end

function  HideLoginUI()
    if l_instance ~= nil then
        l_instance:Close()
    end
end