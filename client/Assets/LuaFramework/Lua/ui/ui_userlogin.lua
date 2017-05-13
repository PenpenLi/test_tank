--[[
测试列表，有个按钮
]]

local cls_ui_userlogin = class("cls_ui_userlogin",cls_ui_base)
cls_ui_userlogin.s_ui_panel = 'oms_test/PanelUserlogin'
local l_instance = nil

function cls_ui_userlogin:ctor()
    self.super.ctor(self)
end

function cls_ui_userlogin:OnStart()
    LuaHelper.Lua_call("Net_init")
    self:EnableUpdate()
    self.m_transform = self.m_game_object.transform
    self.m_btn_login = self.m_transform:FindChild("PanelHide/login").gameObject
    self.m_btn_login1 = self.m_transform:FindChild("PanelHide/zhuce").gameObject
    self.m_btn_login2 = self.m_transform:FindChild("youke").gameObject
    self.m_input_name = self.m_transform:FindChild("PanelHide/userinput/Text"):GetComponent('Text')
    self.m_input_name_input = self.m_transform:FindChild("PanelHide/userinput"):GetComponent('InputField')
    self.m_input_pwd = self.m_transform:FindChild("PanelHide/pswinput/Text"):GetComponent('Text')
    self.m_input_pwd_input = self.m_transform:FindChild("PanelHide/pswinput"):GetComponent('InputField')
    self.m_input_name_input.text = LuaHelper.Get_Next_Name()
    self.m_input_pwd_input.text = LuaHelper.Get_Next_Pwd()

    self.m_PanelHide = self.m_transform:FindChild("PanelHide").gameObject
    self.m_fxxk = self.m_transform:FindChild("fxxk").gameObject

    self.wechatBtn = self.m_transform:FindChild("wechat").gameObject

    self.m_lua_behaviour:AddClick(self.wechatBtn, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        local temp = LuaHelper.wechatLogin()
        if (temp < 0) then
            self.now = -2
        elseif (temp == 0) then
            require "ui/ui_message"
            ShowMessageUI("目前只支持Android或IOS微信登录")
        end
    end);



    self.m_click = 0
    self.m_lua_behaviour:AddClick(self.m_fxxk, function (obj)
        self.m_click = self.m_click + 1
        if (self.m_click >= 5) then
            self.m_PanelHide:SetActive(true)
        end
    end);

    self.m_server_show = self.m_transform:FindChild("PanelHide/SmallMap/Show").gameObject
    self.m_server_hide = self.m_transform:FindChild("PanelHide/SmallMap/Hide").gameObject
    self.m_server_show:SetActive(false)
    self.m_lua_behaviour:AddClick(self.m_server_show, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        self.m_server_show:SetActive(false)
        self.m_server_hide:SetActive(true)
    end);
    self.m_lua_behaviour:AddClick(self.m_server_hide, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        self.m_server_show:SetActive(true)
        self.m_server_hide:SetActive(false)
    end);

    self.m_serverOuter = self.m_transform:FindChild("PanelHide/SmallMap/Hide/map/server1").gameObject
    self.m_serverInner = self.m_transform:FindChild("PanelHide/SmallMap/Hide/map/server2").gameObject

    --  innerNet = 1,
    --  outerNet = 2,
    self.m_lua_behaviour:AddToggleClick(self.m_serverInner, function (obj, isOn)
        if (self.m_serverInner:GetComponent('Toggle').isOn == true) then
            LuaHelper.setServer(1)
        end     
    end);
    self.m_lua_behaviour:AddToggleClick(self.m_serverOuter, function (obj, isOn)
        if (self.m_serverOuter:GetComponent('Toggle').isOn == true) then
            LuaHelper.setServer(2)
        end     
    end);


    self.m_btn_test = self.m_transform:FindChild("PanelHide/test").gameObject
    self.m_lua_behaviour:AddClick(self.m_btn_test, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        local name = self.m_input_name.text
        if (name == "") then
            math.randomseed(os.time()) 
            name = "游客" .. math.random(100)
        end
        LuaHelper.Send_Name(name)
        LuaHelper.Lua_call("AI_1v1")
        require "ui/ui_chooseMap"
        ShowchooseMapUI()
        --SendUIMessage("ENUM_SHOW_LOGIN")
        self:Close()
    end);

    self.now = -1   -- -1:初始值   -2:已经连接  -3：房间接收成功          
    self.m_lua_behaviour:AddClick(self.m_btn_login, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        if self.now ~= -1 then 
            return 
        end

        local name = self.m_input_name.text
        local pwd = self.m_input_pwd_input.text
        if (name == "" or pwd == "") then
            require "ui/ui_message"
            ShowMessageUI("账号或密码不能为空")
            return
        end

        local val2 = LuaHelper.CheckAccount(name, pwd, "1")
        if(val2 == 0) then
            require "ui/ui_message"
            ShowMessageUI("账号不存在或密码错误")
            return
        elseif(val2 == 1) then
            require "ui/ui_message"
            ShowMessageUI("输入不合法，只允许字母和数字")
            return
        end
        
        local val = LuaHelper.SocketConnect()
        if val == 0 then
            require "ui/ui_message"
            ShowMessageUI("网络连接失败")
            return
        end

        self.now = -2
    end);

    self.m_lua_behaviour:AddClick(self.m_btn_login1, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        if self.now ~= -1 then 
            return 
        end
        
        local name = self.m_input_name.text
        local pwd = self.m_input_pwd_input.text
        if (name == "" or pwd == "") then
            require "ui/ui_message"
            ShowMessageUI("账号或密码不能为空")
            return
        end

        local val2 = LuaHelper.CheckAccount(name, pwd, "0")
        if(val2 == 0) then
            require "ui/ui_message"
            ShowMessageUI("账号重复")
            return
        elseif(val2 == 1) then
            require "ui/ui_message"
            ShowMessageUI("输入不合法，只允许字母和数字")
            return
        else
            require "ui/ui_message"
            ShowMessageUI("系统消息：注册成功")
            return
        end

        
        local val = LuaHelper.SocketConnect()
        if val == 0 then
            require "ui/ui_message"
            ShowMessageUI("网络连接失败")
            return
        end

        self.now = -2
    end);

    
    self.m_lua_behaviour:AddClick(self.m_btn_login2, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
   --     if self.now ~= -1 then 
   --        return 
   --    end

        local val2 = LuaHelper.Lua_get("Send_Youke")
        if(val2 == 0)  then
            require "ui/ui_message"
            ShowMessageUI("游客账号获取失败")
            return 
        end

        local val = LuaHelper.SocketConnect()
        if val == 0 then
            
            require "ui/ui_message"
            ShowMessageUI("网络连接失败")
            return
        end
        self.now = -2
    end);
end

function cls_ui_userlogin:Update()
    local val = LuaHelper.GetStatus()
 --   LuaHelper.Log(val .. "", 3)

    if val == 3 and self.now == -2 then
        SendUIMessage("ENUM_SHOW_HOST")
        self:Close()
    elseif val == 4 and self.now == -2 then
        require "ui/ui_message"
        ShowMessageUI("登录失败(PHP)")
        self.now = -1
        return
    elseif val == 5 and self.now == -2 then
        SendUIMessage("ENUM_SHOW_ROLE")
        self:Close()
    end
end

function cls_ui_userlogin:OnDestroy()
    self.super.OnDestroy(self)
    l_instance = nil
end


function ShowUserloginUI()
    l_instance = cls_ui_userlogin:new()
end

function  HideUserloginUI()
    if l_instance ~= nil then
        l_instance:Close()
    end
end