--[[
测试列表，有个按钮
]]

local cls_ui_match = class("cls_ui_match",cls_ui_base)
cls_ui_match.s_ui_panel = 'oms_test/PanelMatch'
local l_instance = nil

function cls_ui_match:ctor(...)
    self.super.ctor(self)
    local arr = {...}
    self.m_msg = table.concat( arr, "\n")
end

function cls_ui_match:OnStart()
    self:EnableUpdate()
    self.m_transform = self.m_game_object.transform
    self.m_btn_quit = self.m_transform:FindChild("quit").gameObject
    self.m_module1 = self.m_transform:FindChild("LeftBg/ModTitle/1v1").gameObject
    self.m_module2 = self.m_transform:FindChild("LeftBg/ModTitle/2v2").gameObject
    self.m_module3 = self.m_transform:FindChild("LeftBg/ModTitle/3v3").gameObject

    self.m_head1 = self.m_transform:FindChild("LeftBg/Head/headbg1").gameObject
    self.m_head1_1 = self.m_transform:FindChild("LeftBg/Head/headbg1/none").gameObject
    self.m_head1_2 = self.m_transform:FindChild("LeftBg/Head/headbg1/head").gameObject
    self.m_head2 = self.m_transform:FindChild("LeftBg/Head/headbg2").gameObject
    self.m_head2_1 = self.m_transform:FindChild("LeftBg/Head/headbg2/none").gameObject
    self.m_head2_2 = self.m_transform:FindChild("LeftBg/Head/headbg2/head").gameObject
    self.m_head3 = self.m_transform:FindChild("LeftBg/Head/headbg3").gameObject
    self.m_head3_1 = self.m_transform:FindChild("LeftBg/Head/headbg3/none").gameObject
    self.m_head3_2 = self.m_transform:FindChild("LeftBg/Head/headbg3/head").gameObject

    self.m_gold_text = self.m_transform:FindChild("CharacterInfo/coin/value"):GetComponent('Text')
    self.m_diamond_text =self.m_transform:FindChild("CharacterInfo/jewel/value"):GetComponent('Text')
    self.m_gold_text.text = LuaHelper.Get_playerInfo(1)
    self.m_diamond_text.text = LuaHelper.Get_playerInfo(2)

    self.m_matching = self.m_transform:FindChild("LeftBg/MatchShow").gameObject
    self.m_people = self.m_transform:FindChild("LeftBg/MatchShow/peopleNum"):GetComponent('Text')
    self.m_time = self.m_transform:FindChild("LeftBg/MatchShow/timeNum"):GetComponent('Text')

    self.m_btn_match = self.m_transform:FindChild("LeftBg/Match").gameObject
    self.m_btn_cancel = self.m_transform:FindChild("LeftBg/CancelMatch").gameObject

    --self.m_btn_find = self.m_transform:FindChild("RightBg/userinput/find").gameObject
    self.m_btn_message = self.m_transform:FindChild("Message/send").gameObject

    self.begin =Time.time


    --      (UI上面部分的各种按钮 及回调函数)
    self.m_btn_addgold = self.m_transform:FindChild("CharacterInfo/coin/coin_add").gameObject
    self.m_btn_addjewel = self.m_transform:FindChild("CharacterInfo/jewel/jewel_add").gameObject
    self.m_btn_friend = self.m_transform:FindChild("CharacterInfo/users").gameObject
    self.m_btn_mail = self.m_transform:FindChild("CharacterInfo/mail").gameObject
    self.m_btn_setting = self.m_transform:FindChild("CharacterInfo/setting").gameObject

    self.m_Panel_setting = self.m_transform:FindChild("InnerPanel/Panel_setting").gameObject
    self.m_Panel_setting1 = self.m_transform:FindChild("InnerPanel/Panel_setting1").gameObject
    self.m_setting_yinyue = self.m_transform:FindChild("InnerPanel/Panel_setting/yinyue").gameObject
    self.m_setting_yinxiao = self.m_transform:FindChild("InnerPanel/Panel_setting/yinxiao").gameObject
    self.m_setting_quit = self.m_transform:FindChild("InnerPanel/Panel_setting/quit").gameObject
    self.m_Panel_setting:SetActive(false)
    self.m_Panel_setting1:SetActive(false)
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

    self.m_lua_behaviour:AddClick(self.m_btn_setting, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        if (self.m_Panel_setting.activeSelf == true) then
            self.m_Panel_setting:SetActive(false)
            self.m_Panel_setting1:SetActive(false)
            return
        end
        self.m_Panel_setting:SetActive(true)
        self.m_Panel_setting1:SetActive(true)
    end)
    self.m_lua_behaviour:AddClick(self.m_Panel_setting1, function (obj)
        if (self.m_Panel_setting1.activeSelf == true) then
            self.m_Panel_setting:SetActive(false)
            self.m_Panel_setting1:SetActive(false)
        end
    end)
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
    self.m_lua_behaviour:AddClick(self.m_Panel_users1, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        self.m_Panel_users:SetActive(false)
    end)
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
    end)

    --为什么1个prefab不置顶？
    for i = 1,20 do
        -- path = 'oms_test/friend'
        -- name = 'friend' .. tostring(i) 
        -- LuaHelper.CreatePrefab(self.m_toggle_group.transform, path, name)
    end
    

    self.m_lua_behaviour:AddClick(self.m_btn_friend, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        if (self.m_Panel_users.activeSelf == true) then
            self.m_Panel_users:SetActive(false)
        else
            self.m_Panel_users:SetActive(true)
        end
    end);
    --以上好友
    
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
    self.m_lua_behaviour:AddClick(self.m_btn_mail, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        require "ui/ui_message"
        ShowMessageUI("该功能暂未开放")
    end);

    -- self.m_lua_behaviour:AddClick(self.m_btn_find, function (obj)
    --     LuaHelper.Play_Sound(1,"sound/yinxiao/03");
    --     require "ui/ui_message"
    --     ShowMessageUI("该功能暂未开放")
    -- end);
    self.m_lua_behaviour:AddClick(self.m_btn_message, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        require "ui/ui_message"
        ShowMessageUI("该功能暂未开放")
    end);


--[[
    self.m_friend1 = self.m_transform:FindChild("RightBg/ScrollRect/ToggleGroup/Image").gameObject
    self.m_lua_behaviour:AddToggleClick(self.m_friend1, function (obj, isOn)
        log("hahaha" .. tostring(isOn))
    end);
    self.mm = self.m_transform:FindChild("RightBg/ScrollRect/ToggleGroup/Image"):GetComponent('Toggle')
    log("ooooooooooooo" .. tostring(self.mm.isOn))
]]
    self.m_time.text = "0"
    self.m_matching:SetActive(false)
    self.m_btn_cancel:SetActive(false)

    if (self.m_msg == "1") then
        self.m_module2:SetActive(false)
        self.m_module3:SetActive(false)
        self.m_head1_1:SetActive(false)
        self.m_head2:SetActive(false)
        self.m_head3:SetActive(false)
        self.m_people.text = "1/1"
        self.m_head1.transform.localPosition = Vector3.New(0, 0, 0)
    elseif (self.m_msg == "2" ) then
        self.m_module1:SetActive(false)
        self.m_module3:SetActive(false)
        self.m_head1_1:SetActive(false)
        self.m_head2_2:SetActive(false)
        self.m_head3:SetActive(false)
        self.m_people.text = "1/2"
        self.m_head1.transform.localPosition = Vector3.New(-100, 0, 0)
        self.m_head2.transform.localPosition = Vector3.New(100, 0, 0)
    else
        self.m_module1:SetActive(false)
        self.m_module2:SetActive(false)
        self.m_head1_1:SetActive(false)
        self.m_head2_2:SetActive(false)
        self.m_head3_2:SetActive(false)
        self.m_people.text = "1/3"
    end
    path = "Sprites/head" .. tostring(LuaHelper.Get_playerInfo(7))
    LuaHelper. LoadSprite(self.m_head1_2, path)
    
    LuaHelper.setWXhead(self.m_head1_2)
    
    self.now = -1   -- -1:初始值   -2:发起匹配  -3:发起撤销  1402:发起匹配成功  1403:发起撤销成功
                    -- -4:发起准备，即将进入游戏

    self.m_lua_behaviour:AddClick(self.m_btn_match, function (obj)
        if self.now == -3 then
--          self.m_show_text.text = "撤销匹配处理中"
            return 
        end
        if self.now == -2 then
--          self.m_show_text.text = "已经发起过匹配请求，请勿重新点击"
            return 
        end

        if self.now == 1402 then
--           self.m_show_text.text = "已经在匹配队列中"
            return 
        end
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        self.now = -2
        if (self.m_msg == "1" ) then
            LuaHelper.Lua_call("NORMAL_PVP")
        elseif (self.m_msg == "2" ) then
            LuaHelper.Lua_call("PVP_2V2")
        elseif (self.m_msg == "3" ) then
            LuaHelper.Lua_call("PVP_3V3")
        end
        LuaHelper.Lua_call("Send_MapMatch")
    end);
 
    self.m_lua_behaviour:AddClick(self.m_btn_cancel, function (obj)
        if self.now ~= 1402 then
--          self.m_show_text.text = "没在匹配队列中"
            return 
        end
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        self.now = -3
        LuaHelper.Lua_call("Send_MapMatch_Cancel")
    end);

    self.m_lua_behaviour:AddClick(self.m_btn_quit, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        --在匹配队列里先发送取消匹配
        if self.now == 1402 then
            self.now = -1
            LuaHelper.Lua_call("Send_MapMatch_Cancel")
        end
        SendUIMessage("ENUM_SHOW_Duizhan")
        self:Close()
    end);

end

function cls_ui_match:Update()
    local tmp = LuaHelper.MAP_MATCH_result()
    
    if tmp == 1402 then  
        if (self.m_btn_cancel.activeSelf == true) then
            local time = Time.time - self.begin
            self.m_time.text = tostring(math.floor(time))
        else
            
        end
    end

    if tmp == 1402 and self.now == -2 then
        self.begin = Time.time
        self.m_time.text = tostring(0)
        
        self.m_matching:SetActive(true)
        self.m_btn_cancel:SetActive(true)
        self.m_btn_match:SetActive(false)
        self.now = 1402
        return 
    elseif tmp == 1403 and self.now == -3 then
        self.now = -1
        self.m_matching:SetActive(false)
        self.m_btn_cancel:SetActive(false)
        self.m_btn_match:SetActive(true)
        --self.m_show_text.text = "取消成功"
        return
    elseif tmp == 0 and (self.now == 1402 or self.now == -2) then
        --self.m_show_text.text = "匹配成功"
        --SendUIMessage("ENUM_SHOW_PREPARE")
        self.now = -5    
        return
    elseif (tmp == 1400 or tmp == 1401 or tmp == 1404 or tmp == 1405 or tmp == 1406) and self.now ~= -1 then
        self.now = -1
        require "ui/ui_message"
        ShowMessageUI("匹配失败，请重新匹配" .. tmp)
        return
    elseif tmp == 10000 and self.now == -4 then
 --       require "logic/battle_manager"
 --       battle_manager.StartBattle(1)
 --       self:Close()
    end
end

function cls_ui_match:OnDestroy()
    self.super.OnDestroy(self)
    l_instance = nil
end


function ShowMatchUI(...)
    l_instance = cls_ui_match:new(...)
end

function  HideMatchUI()
    if l_instance ~= nil then
        l_instance:Close()
    end
end