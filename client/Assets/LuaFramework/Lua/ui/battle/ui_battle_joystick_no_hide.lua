local cls_ui_battle_joystick_no_hide = class("cls_ui_battle_joystick_no_hide",cls_ui_base)
cls_ui_battle_joystick_no_hide.s_ui_panel = 'UI/battle_ui/PanelJoyStickNoHide'
local l_instance = nill
local hasTimeTweener = false

function cls_ui_battle_joystick_no_hide:ctor()
    self.super.ctor(self)
end

function  cls_ui_battle_joystick_no_hide:SetSilder(slider, value)
    slider.value = value
end

function cls_ui_battle_joystick_no_hide:OnStart()
    LuaHelper.Play_Sound(-1,"sound/Music/battlebg_2");
    --LuaHelper.Set_Sound(1,"battle_bgm")
    self.m_transform = self.m_game_object.transform

    self.m_btn_message = self.m_transform:FindChild("PanelLeftMiddle/Message").gameObject
    self.m_btn_sound = self.m_transform:FindChild("PanelLeftMiddle/Sound").gameObject
    self.m_btn_face = self.m_transform:FindChild("PanelLeftMiddle/Face").gameObject

    self.m_cancel_panel = self.m_transform:FindChild("PanelLeftMiddle/cancel_panel").gameObject
    --以下表情  1000+
    self.m_emoji_bg = self.m_transform:FindChild("PanelLeftMiddle/EmojiBg").gameObject
    self.emoji = {}
    for i = 1,12 do
        self.emoji[i] = self.m_transform:FindChild("PanelLeftMiddle/EmojiBg/ScrollRect/ToggleGroup/emoji" .. tostring(i)).gameObject
        self.m_lua_behaviour:AddToggleClick(self.emoji[i], function (obj, isOn)
            --body
            if(LuaHelper.GetMode() == 4) then
                LuaHelper.Show_Emoji(i)
            else
                LuaHelper.Lua_set("Send_Chat_Id", 1000+i)
            end
            
            self.m_emoji_bg:SetActive(false)
            self.m_cancel_panel:SetActive(false)
        end);
    end 
    --以上表情
    --以下文字  2000+
    self.m_message_bg = self.m_transform:FindChild("PanelLeftMiddle/MessageBg").gameObject
    self.message = {}
    for i = 1,8 do
        self.message[i] = self.m_transform:FindChild("PanelLeftMiddle/MessageBg/ToggleGroup/message" .. tostring(i)).gameObject
        self.m_lua_behaviour:AddToggleClick(self.message[i], function (obj, isOn)
            --body
            if(LuaHelper.GetMode() == 4) then
                LuaHelper.Show_Message(i)
                LuaHelper.Play_Sound(2, "sound/girlsound/girl_0" .. tostring(i))
            else
                LuaHelper.Lua_set("Send_Chat_Id", 2000+i)
            end

            self.m_message_bg:SetActive(false)
            self.m_cancel_panel:SetActive(false)
        end);
    end 
    --以上文字
    self.m_lua_behaviour:AddClick(self.m_cancel_panel, function (obj)
        self.m_emoji_bg:SetActive(false)
        self.m_message_bg:SetActive(false)
        self.m_cancel_panel:SetActive(false)
    end);

    self.m_lua_behaviour:AddClick(self.m_btn_message, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        self.m_message_bg:SetActive(true)
        self.m_cancel_panel:SetActive(true)
    end);
     self.m_lua_behaviour:AddClick(self.m_btn_sound, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        require "ui/ui_message"
        ShowMessageUI("该功能暂未开放")
    end);
      self.m_lua_behaviour:AddClick(self.m_btn_face, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        self.m_emoji_bg:SetActive(true)
        self.m_cancel_panel:SetActive(true)
    end);

    self.m_time = self.m_transform:FindChild("PanelMiddleTop/PanelTime").gameObject
    self.m_time_ge = self.m_transform:FindChild("PanelMiddleTop/PanelTime/ge").gameObject
    self.m_time_shi = self.m_transform:FindChild("PanelMiddleTop/PanelTime/shi").gameObject
    self.m_wind_arrow = self.m_transform:FindChild("PanelMiddleTop/PanelWind/WindArrow").gameObject
    self.m_wind_arrow1 = self.m_transform:FindChild("PanelMiddleTop/PanelWind/WindArrow1").gameObject
    self.m_wind_arrow1:SetActive(false)
    obj = self.m_transform:FindChild("PanelMiddleTop/PanelWind/WindText").gameObject
    self.m_wind_text = obj:GetComponent('Text')

    self.m_smallmap_show = self.m_transform:FindChild("PanelRightUp/SmallMap/Show").gameObject
    self.m_smallmap_hide = self.m_transform:FindChild("PanelRightUp/SmallMap/Hide").gameObject
    self.m_turn_num = self.m_transform:FindChild("PanelRightUp/SmallMap/TurnNum"):GetComponent('Text')
    self.m_turn_num.text = tostring(LuaHelper.GetRound())
    self.m_smallmap_show:SetActive(false)
    self.m_lua_behaviour:AddClick(self.m_smallmap_show, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        self.m_smallmap_show:SetActive(false)
        self.m_smallmap_hide:SetActive(true)
    end);
    self.m_lua_behaviour:AddClick(self.m_smallmap_hide, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        self.m_smallmap_show:SetActive(true)
        self.m_smallmap_hide:SetActive(false)
    end);
    self.m_smallmap_friend1 = self.m_transform:FindChild("PanelRightUp/SmallMap/Hide/map/friend1").gameObject
    self.m_smallmap_friend2 = self.m_transform:FindChild("PanelRightUp/SmallMap/Hide/map/friend2").gameObject
    self.m_smallmap_friend3 = self.m_transform:FindChild("PanelRightUp/SmallMap/Hide/map/friend3").gameObject
    self.m_smallmap_enemy1 = self.m_transform:FindChild("PanelRightUp/SmallMap/Hide/map/enemy1").gameObject
    self.m_smallmap_enemy2 = self.m_transform:FindChild("PanelRightUp/SmallMap/Hide/map/enemy2").gameObject
    self.m_smallmap_enemy3 = self.m_transform:FindChild("PanelRightUp/SmallMap/Hide/map/enemy3").gameObject
    self.m_smallmap_friend1:SetActive(false)
    self.m_smallmap_friend2:SetActive(false)
    self.m_smallmap_friend3:SetActive(false)
    self.m_smallmap_enemy1:SetActive(false)
    self.m_smallmap_enemy2:SetActive(false)
    self.m_smallmap_enemy3:SetActive(false)
    self.now = LuaHelper.GetMode()
    if (self.now == 4) then
        self.m_smallmap_friend1:SetActive(true)
        self.m_smallmap_enemy1:SetActive(true)
    else 
        if (self.now >= 1) then
            self.m_smallmap_friend1:SetActive(true)
            self.m_smallmap_enemy1:SetActive(true)
        end
        if (self.now >= 2) then
            self.m_smallmap_friend2:SetActive(true)
            self.m_smallmap_enemy2:SetActive(true)
        end
        if (self.now >= 3) then
            self.m_smallmap_friend3:SetActive(true)
            self.m_smallmap_enemy3:SetActive(true)
        end
    end

    

    self:EnableUpdate()
    self:InitReturn()
end

function cls_ui_battle_joystick_no_hide:InitReturn( ... )
    self.m_btn_return = self.m_transform:FindChild("PanelRightUp/ButtonReturn").gameObject;

    self.m_lua_behaviour:AddClick(self.m_btn_return, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        local val = LuaHelper.GetVersion()
        if(val == 0) then 
            LuaHelper.Lua_call("Send_Quit_Button")
            require "ui/battle/ui_battle_joystick"
            HideBattleJoyStickUI()
            SendUIMessage("ENUM_SHOW_LOGIN")
            self:Close()
        else 
            LuaHelper.Lua_call("Send_Quit_Button")
            require "ui/battle/ui_battle_joystick"
            HideBattleJoyStickUI()
            SendUIMessage("ENUM_SHOW_HOST")
            self:Close()
        end
    end);
end

function cls_ui_battle_joystick_no_hide:Update()
    self.m_turn_num.text = tostring(math.ceil(LuaHelper.GetRound()/2))
    tmp = math.floor(LuaHelper.GetRoundLeftTick() / 60)
    path = "Sprites/countDownNum"
    ge = 0 
    shi = 0
    if (tmp > 0) then
        ge = math.floor(tmp % 10)
        shi = math.floor(tmp / 10)
    end
    LuaHelper.LoadSprite(self.m_time_ge, path .. tostring(ge))
    LuaHelper.LoadSprite(self.m_time_shi, path .. tostring(shi))

    --10秒是预警临界值
    if(tmp < 6 and not hasTimeTweener) then
        LuaHelper.DoScaleLoop(self.m_time,1.2,0.5,-1)
        hasTimeTweener = true
    end   

    if(tmp > 6 and hasTimeTweener) then
        LuaHelper.KillAllTweeners(self.m_time)
        self.m_time.transform.localScale = Vector3.New(1,1,1)
        hasTimeTweener = false
    end

    if (self.now >= 1) then
        self.m_smallmap_friend1.transform.localPosition = Vector3.New(-109.5 + 219 * LuaHelper.GetCharacterPosition(1, 0, 1), -53 + 106 * LuaHelper.GetCharacterPosition(1, 0, 2), 0)
        self.m_smallmap_enemy1.transform.localPosition = Vector3.New(-109.5 + 219 * LuaHelper.GetCharacterPosition(2, 0, 1), -53 + 106 * LuaHelper.GetCharacterPosition(2, 0, 2), 0)
    end
    if (self.now >= 2 and self.now ~= 4) then
        self.m_smallmap_friend2.transform.localPosition = Vector3.New(-109.5 + 219 * LuaHelper.GetCharacterPosition(1, 1, 1), -53 + 106 * LuaHelper.GetCharacterPosition(1, 1, 2), 0)
        self.m_smallmap_enemy2.transform.localPosition = Vector3.New(-109.5 + 219 * LuaHelper.GetCharacterPosition(2, 1, 1), -53 + 106 * LuaHelper.GetCharacterPosition(2, 1, 2), 0)
    end
    if (self.now >= 3 and self.now ~= 4) then
        self.m_smallmap_friend3.transform.localPosition = Vector3.New(-109.5 + 219 * LuaHelper.GetCharacterPosition(1, 2, 1), -53 + 106 * LuaHelper.GetCharacterPosition(1, 2, 2), 0)
        self.m_smallmap_enemy3.transform.localPosition = Vector3.New(-109.5 + 219 * LuaHelper.GetCharacterPosition(2, 2, 1), -53 + 106 * LuaHelper.GetCharacterPosition(2, 2, 2), 0)
    end

end

function  cls_ui_battle_joystick_no_hide:OnWindChange()
    local windPercent = LuaHelper.GetMapWindPercent()
    --self.m_wind_arrow.localPosition = Vector3.New(windPercent*194,0,0)
    self.m_wind_text.text = string.format("%0.1f",math.abs(windPercent)* 12.0)
    if windPercent > 0 then
        self.m_wind_arrow:SetActive(false)
        self.m_wind_arrow1:SetActive(true)
    elseif windPercent <= 0 then
        self.m_wind_arrow:SetActive(true)
        self.m_wind_arrow1:SetActive(false)
    else
        --self.m_wind_arrow.localScale = Vector3.New(0,1,1)
    end
end

function cls_ui_battle_joystick_no_hide:OnDestroy()
     LuaHelper.Play_Sound(-1,"sound/Music/battlebg_4");
     --LuaHelper.Set_Sound(1,"bgm")
    self.super.OnDestroy(self)
    l_instance = nil
end

function ShowBattleJoyStickUI2()
    l_instance = cls_ui_battle_joystick_no_hide:new()
end

function HideBattleJoyStickUI2()
    if l_instance ~= nil then
        l_instance:Close()
    end
end

function OnChangeController()
    if l_instance ~= nil then
        l_instance:OnWindChange()
    end
end