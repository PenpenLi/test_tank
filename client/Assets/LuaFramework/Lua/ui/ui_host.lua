--[[
测试列表，有个按钮
]]

local cls_ui_host = class("cls_ui_host",cls_ui_base)
cls_ui_host.s_ui_panel = 'oms_test/PanelHost'
local l_instance = nil

function cls_ui_host:ctor()
    self.super.ctor(self)
end

function  cls_ui_host:SetSilder(slider, value)
    slider.value = value
end

function cls_ui_host:OnStart()
    LuaHelper.Lua_call("Lua_xue")
    self:EnableUpdate()
    self.m_transform = self.m_game_object.transform
    self.m_btn_renji = self.m_transform:FindChild("renji").gameObject
    self.m_btn_paiwei = self.m_transform:FindChild("paiwei").gameObject
    self.m_btn_duizhan = self.m_transform:FindChild("duizhan").gameObject

    self.m_characterinfo = self.m_transform:FindChild("CharacterInfo/headbg/head").gameObject
    self.m_btn_addgold = self.m_transform:FindChild("CharacterInfo/coin/coin_add").gameObject
    self.m_btn_addjewel = self.m_transform:FindChild("CharacterInfo/jewel/jewel_add").gameObject
    self.m_btn_friend = self.m_transform:FindChild("CharacterInfo/users").gameObject
    self.m_btn_mail = self.m_transform:FindChild("CharacterInfo/mail").gameObject
    self.m_btn_setting = self.m_transform:FindChild("CharacterInfo/setting").gameObject

    self.m_InnerPanel = self.m_transform:FindChild("InnerPanel").gameObject
    self.m_content  = self.m_transform:FindChild("InnerPanel/Content").gameObject
    self.m_Panel_setting = self.m_transform:FindChild("InnerPanel/Content/Panel_setting").gameObject
    self.m_Panel_setting1 = self.m_transform:FindChild("InnerPanel/Panel_setting1").gameObject
    self.m_Panel_setting1_button = self.m_Panel_setting1:GetComponent('Button')
    self.m_setting_yinyue = self.m_transform:FindChild("InnerPanel/Content/Panel_setting/yinyue").gameObject
    self.m_setting_yinxiao = self.m_transform:FindChild("InnerPanel/Content/Panel_setting/yinxiao").gameObject
    self.m_setting_quit = self.m_transform:FindChild("InnerPanel/Content/Button").gameObject

    self.m_InnerPanel:SetActive(false)

    --以下好友
    self.m_Panel_users1 = self.m_transform:FindChild("UsersBg/cancel_panel").gameObject;
    self.m_Panel_users = self.m_transform:FindChild("UsersBg").gameObject
    self.m_scroll_rect = self.m_transform:FindChild("UsersBg/ScrollRect").gameObject
    self.m_toggle_group = self.m_transform:FindChild("UsersBg/ScrollRect/ToggleGroup").gameObject
    self.m_friend_btn = self.m_transform:FindChild("UsersBg/friendbtn").gameObject
    self.m_addfriend_btn = self.m_transform:FindChild("UsersBg/addfriendbtn").gameObject
    self.m_userinput = self.m_transform:FindChild("UsersBg/userinput").gameObject
    self.m_users_find = self.m_transform:FindChild("UsersBg/userinput/find").gameObject
    self.m_find_text = self.m_transform:FindChild("UsersBg/userinput/Text"):GetComponent('Text')
    self.m_Panel_users:SetActive(false)
    self.m_userinput:SetActive(false)
    self.statu = 1 --1为好友状态 2为添加状态
    self.prefriend = 0
    
    self.m_lua_behaviour:AddClick(self.m_friend_btn, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        if (self.statu == 2) then
            self.statu = 1
            self.m_scroll_rect:SetActive(true)
            self.m_userinput:SetActive(false)
            path = "Sprites/friendChoose"
            LuaHelper. LoadSprite(self.m_friend_btn, path)
            path = "Sprites/addfriend"
            LuaHelper.LoadSprite(self.m_addfriend_btn, path)
        end
    end)
    self.m_lua_behaviour:AddClick(self.m_addfriend_btn, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        if (self.statu == 1) then
            self.statu = 2
            self.m_scroll_rect:SetActive(false)
            self.m_userinput:SetActive(true)
            path = "Sprites/friend"
            LuaHelper. LoadSprite(self.m_friend_btn, path)
            path = "Sprites/addfriendChoose"
            LuaHelper.LoadSprite(self.m_addfriend_btn, path)
        end
    end)

    self.m_lua_behaviour:AddClick(self.m_users_find, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        name = self.m_find_text.text

        LuaHelper.Lua_set_string("Lua_Find_Friend", name)
        -- /****  zyx
        -- if (name == '') then
        --     --未输入
        -- elseif (name == "baoge") then
        --     require "ui/ui_message"
        --     ShowMessageUI("用户名不存在")
        -- else
        --     require "ui/ui_information"
        --     ShowInformationUI("开放了你妹")
        -- end
        -- ***/

        -- path = 'oms_test/friend'
        -- LuaHelper.CreatePrefab(self.m_toggle_group.transform, path)
    end)

    --为什么1个prefab不置顶？
    for i = 1,20 do
        -- path = 'oms_test/friend'
        -- name = 'friend' .. tostring(i) 
        -- LuaHelper.CreatePrefab(self.m_toggle_group.transform, path, name)
    end
    

    -- self.m_test_add = self.m_transform:FindChild("UsersBg/add1").gameObject
    -- self.m_test_del = self.m_transform:FindChild("UsersBg/del1").gameObject
    -- x = 0
    -- self.m_lua_behaviour:AddClick(self.m_test_add, function (obj)
    --     for i = 1,3 do
    --         path = 'oms_test/friend'
    --         name = 'friend' .. tostring(x + i)
    --         LuaHelper.CreatePrefab(self.m_toggle_group.transform, path, name)
    --         obj_name = self.m_transform:FindChild("UsersBg/ScrollRect/ToggleGroup/friend" .. tostring(x + i) .. "/name"):GetComponent('Text')
    --         obj_name.text = tostring(x + i)
    --     end
    --     x = x + 3
    -- end)
    -- self.m_lua_behaviour:AddClick(self.m_test_del, function (obj)
    --     for i = 0,(x-1) do
    --         go = self.m_transform:FindChild("UsersBg/ScrollRect/ToggleGroup/friend" .. tostring(x - i)).gameObject
    --         LuaHelper.DestroyObj(go)
    --     end
    --     x = 0
    -- end)
    --以上好友
    


    self.m_lua_behaviour:AddClick(self.m_btn_setting, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        if (self.m_InnerPanel.activeSelf == true) then
            self.m_Panel_setting1:SetActive(false)
            LuaHelper.DoLocalMoveY(self.m_content,800,0.3,function ()
                self.m_InnerPanel:SetActive(false)
            end)
            return
        end
        self.m_Panel_setting1_button.enabled = false
        self.m_Panel_setting1:SetActive(true)
        self.m_InnerPanel:SetActive(true)
        LuaHelper.DoLocalMoveY(self.m_content,0,0.3,function ()
            self.m_Panel_setting1_button.enabled = true
        end)
    end)
    self.m_lua_behaviour:AddClick(self.m_Panel_setting1, function (obj)
        if (self.m_InnerPanel.activeSelf == true) then
            self.m_Panel_setting1:SetActive(false)
            LuaHelper.DoLocalMoveY(self.m_content,800,0.3,function ()
                self.m_InnerPanel:SetActive(false)
            end)
        end
    end)

    if (LuaHelper.SetSound(0, -1) == 1) then 
            self.m_setting_yinyue:GetComponent('Toggle').isOn = true
    else    
            self.m_setting_yinyue:GetComponent('Toggle').isOn = false
    end
    if (LuaHelper.SetSound(1, -1) == 1) then 
            self.m_setting_yinxiao:GetComponent('Toggle').isOn = true
    else    
            self.m_setting_yinxiao:GetComponent('Toggle').isOn = false
    end

    self.m_lua_behaviour:AddToggleClick(self.m_setting_yinyue, function (obj, isOn)
        if (self.m_setting_yinyue:GetComponent('Toggle').isOn == true) then
            LuaHelper.SetSound(0, 1)
        else 
            LuaHelper.SetSound(0, 0)
        end
    end);

    self.m_lua_behaviour:AddToggleClick(self.m_setting_yinxiao, function (obj, isOn)
        if (self.m_setting_yinxiao:GetComponent('Toggle').isOn == true) then
            LuaHelper.SetSound(1, 1)
        else 
            LuaHelper.SetSound(1, 0)
        end
    end);
    
    --以下我的个人信息
    self.m_lua_behaviour:AddClick(self.m_characterinfo, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        LuaHelper.Lua_set_string("Lua_Find_Friend", LuaHelper.Get_playerInfoName())
        --require "ui/ui_information"
        --ShowInformationUI("开放了你妹")
    end);
    --以上我的个人信息
    self.m_lua_behaviour:AddClick(self.m_btn_addgold, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        require "ui/ui_message"
        ShowMessageUI("该功能暂未开放")
    end);
    self.m_lua_behaviour:AddClick(self.m_btn_addjewel, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        require "ui/ui_message"
        ShowMessageUI("该功能暂未开放")
    end);

    --好友部分
    self.m_cancel_user_btn = self.m_Panel_users1:GetComponent('Button') 
    self.m_lua_behaviour:AddClick(self.m_Panel_users1, function (obj)
   --     LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        self.m_cancel_user_btn.enabled = false
        LuaHelper.DoLocalMoveX(self.m_Panel_users,800,0.1,function ()
            self.m_Panel_users:SetActive(false)
        end)
    end);

    self.m_lua_behaviour:AddClick(self.m_btn_friend, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        if (self.m_Panel_users.activeSelf == true) then
            self.m_Panel_users:SetActive(false)
        else
            UpdateFriendList()
            self.m_cancel_user_btn.enabled = false
            self.m_Panel_users:SetActive(true)
            LuaHelper.DoLocalMoveX(self.m_Panel_users,408,0.1,function ()
                self.m_cancel_user_btn.enabled = true
            end)
        end
    end);

    --
    self.m_lua_behaviour:AddClick(self.m_btn_mail, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        require "ui/ui_message"
        ShowMessageUI("该功能暂未开放")
    end);

    self.m_head = self.m_transform:FindChild("CharacterInfo/headbg/head").gameObject
    self.m_name_text = self.m_transform:FindChild("CharacterInfo/username"):GetComponent('Text')
    self.m_gold_text = self.m_transform:FindChild("CharacterInfo/coin/value"):GetComponent('Text')
    self.m_diamond_text =self.m_transform:FindChild("CharacterInfo/jewel/value"):GetComponent('Text')
    self.Now_exp = self.m_transform:FindChild("CharacterInfo/levelexp/exp"):GetComponent('Text')
    self.Now_level = self.m_transform:FindChild("CharacterInfo/levelexp/level/value"):GetComponent('Text')
    obj = self.m_transform:FindChild("CharacterInfo/levelexp").gameObject
    self.exp_slider = obj:GetComponent('Slider')

    local val2 = LuaHelper.GetVersion()
    if(val2 == 1) then
        self.m_name_text.text = LuaHelper.Get_playerInfoName()
        -- gold = 1,
        -- diamond = 2,
        -- exp = 3,
        -- addgold = 4,
        -- adddiamond = 5,
        -- addexp = 6,
        -- sex = 7,
        self.info = {}
        for i = 1,7 do
            self.info[i] = LuaHelper.Get_playerInfo(i)
        end
        self.m_gold_text.text = tostring(self.info[1])
        self.m_diamond_text.text = tostring(self.info[2])
        path = "Sprites/head" .. tostring(self.info[7])
        LuaHelper.LoadSprite(self.m_head, path)
    
        LuaHelper.setWXhead(self.m_head)

        --type 1.level2exp 2.exp2level 3.resexp2level
        level = LuaHelper.transExp(self.info[3], 2)
        res = LuaHelper.transExp(self.info[3], 3)
        exp = LuaHelper.transExp(self.info[3], 1)
        self.Now_exp.text = res .. "/" .. tostring(exp)
        self.Now_level.text = tostring(level)
        self:SetSilder(self.exp_slider, res/exp)
    end 

    self.m_lua_behaviour:AddClick(self.m_btn_renji, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        -- require "ui/ui_message"
        -- ShowMessageUI("该模式暂未开放")
        LuaHelper.Lua_call("AI_1v1")
        SendUIMessage("ENUM_SHOW_CHOOSEMAP")
        self:Close()
    end);
    self.m_lua_behaviour:AddClick(self.m_btn_paiwei, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        require "ui/ui_message"
        ShowMessageUI("该模式暂未开放")
        --self:Close()
    end);
    self.m_lua_behaviour:AddClick(self.m_btn_duizhan, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        SendUIMessage("ENUM_SHOW_Duizhan")
        self:Close()
    end);

    self.m_lua_behaviour:AddClick(self.m_setting_quit, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        LuaHelper.Lua_call("Off_Net")
        SendUIMessage("ENUM_SHOW_Userlogin")
        self:Close()
    end);

    
end

function cls_ui_host:OnDestroy()
    self.super.OnDestroy(self)
    l_instance = nil
end


function ShowHostUI()
    l_instance = cls_ui_host:new()
end

function  HideHostUI()
    if l_instance ~= nil then
        l_instance:Close()
    end
end

function cls_ui_host:OnFriendChange()
    if (l_instance ~= nil and self.prefriend ~= nil) then

        tmp = self.prefriend - 1
        for i = tmp, 0,-1 do
            go = self.m_transform:FindChild("UsersBg/ScrollRect/ToggleGroup/friend" .. tostring(i)).gameObject
            LuaHelper.DestroyObj(go)
        end
        
        -- local arr = {...}
        -- self.prefriend = arr[2]
        -- log("[[[[[[[[[[[[[[" .. arr[2])
        self.prefriend = LuaHelper.Get_Friend_Cnt()
        object = self.m_transform:FindChild("UsersBg/ScrollRect/ToggleGroup").gameObject
        tmp = self.prefriend - 1
        path = 'oms_test/friend'
        
        
        for i = 0, tmp do
            name = 'friend' .. tostring(i)
            LuaHelper.CreatePrefab(object.transform, path, name)
            -- local obj_toggle = self.m_transform:FindChild("UsersBg/ScrollRect/ToggleGroup/friend" .. tostring(i) .. "/headbg").gameObject
            -- local obj_head = self.m_transform:FindChild("UsersBg/ScrollRect/ToggleGroup/friend" .. tostring(i) .. "/headbg/head").gameObject
            -- local obj_name = self.m_transform:FindChild("UsersBg/ScrollRect/ToggleGroup/friend" .. tostring(i) .. "/name"):GetComponent('Text')
            -- local obj_statu = self.m_transform:FindChild("UsersBg/ScrollRect/ToggleGroup/friend" .. tostring(i) .. "/status"):GetComponent('Text')

            -- self.m_lua_behaviour:AddToggleClick(obj_toggle, function (obj, isOn)
            --     LuaHelper.Play_Sound(1,"anniu_0");
            --     LuaHelper.Lua_set_string("Lua_Find_Friend", LuaHelper.Get_Friend_Name(i))
            -- end);

            -- obj_name.text = LuaHelper.Get_Friend_Name(i)
            -- log("caonimeiaaaaaa" .. tostring(arr[2]) .. obj_name.text)
            -- self.m_transform:FindChild("UsersBg/ScrollRect/ToggleGroup/friend" .. tostring(i) .. "/name"):GetComponent('Text').text = LuaHelper.Get_Friend_Name(i)
            -- if (LuaHelper.Get_Friend_Status(i) == 1) then
            --     obj_statu.text = "在线"
            -- else
            --     obj_statu.text = "离线"
            -- end
            -- headpath = "Sprites/head" .. tostring(LuaHelper.Get_Friend_Sex(i))
            -- LuaHelper.LoadSprite(obj_head, headpath)
        end
    end
end

function UpdateFriendList()
    if (l_instance ~= nil) then
        l_instance:OnFriendChange()
    end
end