local cls_ui_battle_joystick = class("cls_ui_battle_joystick",cls_ui_base)
cls_ui_battle_joystick.s_ui_panel = 'UI/battle_ui/PanelJoyStick'
local l_instance = nill
local yourturn = true

function cls_ui_battle_joystick:ctor()
    self.super.ctor(self)
end

function  cls_ui_battle_joystick:SetSilder(slider, value)
    slider.value = value
end

function cls_ui_battle_joystick:OnStart()
    LuaHelper.Play_Sound(3,"sound/yinxiao/my_turn")
    self.m_transform = self.m_game_object.transform
    self.m_middle_down = self.m_transform:FindChild("PanelMiddleDown").gameObject
    self.m_left_down = self.m_transform:FindChild("PanelLeftDown").gameObject
    self.m_right_down = self.m_transform:FindChild("PanelRightDown").gameObject    
    self.m_left_up = self.m_transform:FindChild("PanelLeftUp").gameObject

    self.m_button_up = self.m_transform:FindChild("PanelLeftDown/ButtonUp").gameObject
    self.m_button_down = self.m_transform:FindChild("PanelLeftDown/ButtonDown").gameObject
    self.m_button_right = self.m_transform:FindChild("PanelLeftDown/ButtonRight").gameObject
    self.m_button_left = self.m_transform:FindChild("PanelLeftDown/ButtonLeft").gameObject
    obj = self.m_transform:FindChild("PanelLeftDown/Angle").gameObject
    self.m_angle_text = obj:GetComponent('Text')

    self.m_button_attack = self.m_transform:FindChild("PanelRightDown/ButtonAttack").gameObject
    self.m_button_skill1 = self.m_transform:FindChild("PanelRightDown/skill1").gameObject
    self.m_button_skill2 = self.m_transform:FindChild("PanelRightDown/skill2").gameObject
    self.m_button_skill3 = self.m_transform:FindChild("PanelRightDown/skill3").gameObject
    self.m_button_cancel1 = self.m_transform:FindChild("PanelRightDown/skill1/cancel1").gameObject
    self.m_button_cancel2 = self.m_transform:FindChild("PanelRightDown/skill2/cancel2").gameObject
    self.m_button_cancel3 = self.m_transform:FindChild("PanelRightDown/skill3/cancel3").gameObject
    self.skill1_time_text = self.m_transform:FindChild("PanelRightDown/skill1/text"):GetComponent('Text')
    self.skill2_time_text = self.m_transform:FindChild("PanelRightDown/skill2/text"):GetComponent('Text')
    self.skill3_time_text = self.m_transform:FindChild("PanelRightDown/skill3/text"):GetComponent('Text')
    self.skill_tip = self.m_transform:FindChild("PanelRightDown/Tips")
    self.confirm_skill1 = LuaHelper.SetSkill(1, -1)
    self.confirm_skill2 = LuaHelper.SetSkill(2, -1)
    self.confirm_skill3 = LuaHelper.SetSkill(3, -1)
    self.skill1_time = 0--LuaHelper.Get_CD(self.confirm_skill1)
    self.skill2_time = 0--LuaHelper.Get_CD(self.confirm_skill2)
    self.skill3_time = 0--LuaHelper.Get_CD(self.confirm_skill3)
    
    path1 = "Sprites/ch_sk" .. tostring(self.confirm_skill1)
    LuaHelper. LoadSprite(self.m_button_skill1, path1)
    path2 = "Sprites/ch_sk" .. tostring(self.confirm_skill2)
    LuaHelper. LoadSprite(self.m_button_skill2, path2)
    path3 = "Sprites/ch_sk" .. tostring(self.confirm_skill3)
    LuaHelper. LoadSprite(self.m_button_skill3, path3)
    
    -- if (self.skill1_time > 0) then
    --     self.m_button_skill1:GetComponent('Button').interactable = false
    --     self.skill1_time_text.text = tostring(self.skill1_time)
    -- else
    --     self.skill1_time_obj:SetActive(false)
    -- end

    -- if (self.skill2_time > 0) then
    --     self.m_button_skill2:GetComponent('Button').interactable = false
    --     self.skill2_time_text.text = tostring(self.skill2_time)
    -- else
    --     self.skill2_time_obj:SetActive(false)
    -- end
    
    -- if (self.skill3_time > 0) then
    --     self.m_button_skill3:GetComponent('Button').interactable = false
    --     self.skill3_time_text.text = tostring(self.skill3_time)
    -- else
    --     self.skill3_time_obj:SetActive(false)
    -- end
    
    self.skill = 0 --未选技能
    self.attack_flag = 0   --标记是否攻击过
    self.aa = 0--技能提示透明度
    Energy = LuaHelper.GetSetEnergy(-1)
    energy1 = LuaHelper.GetSkillInfo2("ENERGY", self.confirm_skill1 + 100)
    energy2 = LuaHelper.GetSkillInfo2("ENERGY", self.confirm_skill2 + 100)
    energy3 = LuaHelper.GetSkillInfo2("ENERGY", self.confirm_skill3 + 100)
    self.skill1_time_text.text = tostring(math.floor(energy1))
    self.skill2_time_text.text = tostring(math.floor(energy2))
    self.skill3_time_text.text = tostring(math.floor(energy3))
    --log("Energy=" .. Energy .. "energy1=".. energy1 .. "energy2=" .. energy2 .. "energy3=" .. energy3)
    -- self.m_lua_behaviour:AddClick(self.m_button_skill1, function (obj)
        
    -- end);
    -- self.m_lua_behaviour:AddClick(self.m_button_skill2, function (obj)
        
    -- end);
    -- self.m_lua_behaviour:AddClick(self.m_button_skill3, function (obj)
        
    -- end);
    self.skill_description_text =  self.m_transform:FindChild("PanelRightDown/skill_description/descrip").gameObject:GetComponent('Text')
    self.skill_description =  self.m_transform:FindChild("PanelRightDown/skill_description").gameObject
    self.m_lua_behaviour:AddTouchHandle(self.m_button_skill1, function (obj,touch_type)
        if touch_type == 1 then
            self.IsDown1 = 1
            self.delay = Time.time
            self.skill_description_text.text = LuaHelper.GetSkillInfo("DESCRIPTION", self.confirm_skill1 + 100)
--释放技能
            if(self.skill1_time ~= 0 or self.attack_flag == 1) then
            return
            end
            if (energy1 > Energy) then
                LuaHelper.ChangeColor(self.skill_tip, 255,255,255,255)
                self.aa = 255
                return
            end
            if (self.skill == 0) then
                self.skill = 1
                LuaHelper.Run_Skill(self.confirm_skill1)
                --LuaHelper. LoadSprite(self.m_button_skill1, "Sprites/cancel")
                self.m_button_cancel1:SetActive(true)
                LuaHelper. LoadSprite(self.m_button_attack, path1)
                --self.m_button_skill1.transform.localScale = Vector3.New(1.2, 1.2, 1.2)
                --LuaHelper.ChangeColor(self.m_button_skill1.transform, 255, 255, 255, 255)
            elseif (self.skill == 1) then
                self.skill = 0
                LuaHelper.Run_Skill(0)
                --LuaHelper. LoadSprite(self.m_button_skill1, path1)
                self.m_button_cancel1:SetActive(false)
                LuaHelper. LoadSprite(self.m_button_attack, "Sprites/fireDown")
                --self.m_button_skill1.transform.localScale = Vector3.New(1, 1, 1)
                --LuaHelper.ChangeColor(self.m_button_skill1.transform, 200, 200, 200, 200)
            elseif (self.skill == 2) then
                self.skill = 1
                LuaHelper.Run_Skill(self.confirm_skill1)
                --LuaHelper. LoadSprite(self.m_button_skill1, "Sprites/cancel")
                --LuaHelper. LoadSprite(self.m_button_skill2, path2)
                self.m_button_cancel1:SetActive(true)
                self.m_button_cancel2:SetActive(false)
                LuaHelper. LoadSprite(self.m_button_attack, path1)
                --self.m_button_skill1.transform.localScale = Vector3.New(1.2, 1.2, 1.2)
                --LuaHelper.ChangeColor(self.m_button_skill1.transform, 255, 255, 255, 255)
                --self.m_button_skill2.transform.localScale = Vector3.New(1, 1, 1)
                --LuaHelper.ChangeColor(self.m_button_skill2.transform, 200, 200, 200, 200)
            elseif (self.skill == 3) then
                self.skill = 1
                LuaHelper.Run_Skill(self.confirm_skill1)
                --LuaHelper. LoadSprite(self.m_button_skill1, "Sprites/cancel")
                --LuaHelper. LoadSprite(self.m_button_skill3, path3)
                self.m_button_cancel1:SetActive(true)
                self.m_button_cancel3:SetActive(false)
                LuaHelper. LoadSprite(self.m_button_attack, path1)
                --self.m_button_skill1.transform.localScale = Vector3.New(1.2, 1.2, 1.2)
                --LuaHelper.ChangeColor(self.m_button_skill1.transform, 255, 255, 255, 255)
                --self.m_button_skill3.transform.localScale = Vector3.New(1, 1, 1)
                --LuaHelper.ChangeColor(self.m_button_skill3.transform, 200, 200, 200, 200)
            end
--结束
        else
            self.IsDown1 = 0
            self.skill_description:SetActive(false)
        end
    end);
    self.m_lua_behaviour:AddTouchHandle(self.m_button_skill2, function (obj,touch_type)
        if touch_type == 1 then
            self.IsDown2 = 1
            self.delay = Time.time
            self.skill_description_text.text = LuaHelper.GetSkillInfo("DESCRIPTION", self.confirm_skill2 + 100)
--释放技能
            if(self.skill2_time ~= 0 or self.attack_flag == 1) then
            return
        end
        if (energy2 > Energy) then
            LuaHelper.ChangeColor(self.skill_tip, 255,255,255,255)
            self.aa = 255
            return
        end
        if (self.skill == 0) then
            self.skill = 2
            LuaHelper.Run_Skill(self.confirm_skill2)
            --LuaHelper. LoadSprite(self.m_button_skill2, "Sprites/cancel")
            self.m_button_cancel2:SetActive(true)
            LuaHelper. LoadSprite(self.m_button_attack, path2)
            --self.m_button_skill2.transform.localScale = Vector3.New(1.2, 1.2, 1.2)
            --LuaHelper.ChangeColor(self.m_button_skill2.transform, 255, 255, 255, 255)
        elseif (self.skill == 1) then
            self.skill = 2
            LuaHelper.Run_Skill(self.confirm_skill2)
            --LuaHelper. LoadSprite(self.m_button_skill2, "Sprites/cancel")
            --LuaHelper. LoadSprite(self.m_button_skill1, path1)
            self.m_button_cancel2:SetActive(true)
            self.m_button_cancel1:SetActive(false)
            LuaHelper. LoadSprite(self.m_button_attack, path2)
            --self.m_button_skill2.transform.localScale = Vector3.New(1.2, 1.2, 1.2)
            --LuaHelper.ChangeColor(self.m_button_skill2.transform, 255, 255, 255, 255)
            --self.m_button_skill1.transform.localScale = Vector3.New(1, 1, 1)
            --LuaHelper.ChangeColor(self.m_button_skill1.transform, 200, 200, 200, 200)
        elseif (self.skill == 2) then
            self.skill = 0
            LuaHelper.Run_Skill(0)
            --LuaHelper. LoadSprite(self.m_button_skill2, path2)
            self.m_button_cancel2:SetActive(false)
            LuaHelper. LoadSprite(self.m_button_attack, "Sprites/fireDown")
            --self.m_button_skill2.transform.localScale = Vector3.New(1, 1, 1)
            --LuaHelper.ChangeColor(self.m_button_skill2.transform, 200, 200, 200, 200)
        elseif (self.skill == 3) then
            self.skill = 2
            LuaHelper.Run_Skill(self.confirm_skill2)
            --LuaHelper. LoadSprite(self.m_button_skill2, "Sprites/cancel")
            --LuaHelper. LoadSprite(self.m_button_skill3, path3)
            self.m_button_cancel2:SetActive(true)
            self.m_button_cancel3:SetActive(false)
            LuaHelper. LoadSprite(self.m_button_attack, path2)
            --self.m_button_skill2.transform.localScale = Vector3.New(1.2, 1.2, 1.2)
            --LuaHelper.ChangeColor(self.m_button_skill2.transform, 255, 255, 255, 255)
            --self.m_button_skill3.transform.localScale = Vector3.New(1, 1, 1)
            --LuaHelper.ChangeColor(self.m_button_skill3.transform, 200, 200, 200, 200)
        end
--结束
        else
            self.IsDown2 = 0
            self.skill_description:SetActive(false)
        end
    end);
    self.m_lua_behaviour:AddTouchHandle(self.m_button_skill3, function (obj,touch_type)
        if touch_type == 1 then
            self.IsDown3 = 1
            self.delay = Time.time
            self.skill_description_text.text = LuaHelper.GetSkillInfo("DESCRIPTION", self.confirm_skill3 + 100)
--释放技能
            if(self.skill3_time ~= 0 or self.attack_flag == 1) then
            return
        end
        if (energy3 > Energy) then
            LuaHelper.ChangeColor(self.skill_tip, 255,255,255,255)
            self.aa = 255
            return
        end
        if (self.skill == 0) then
            self.skill = 3
            LuaHelper.Run_Skill(self.confirm_skill3)
            --LuaHelper. LoadSprite(self.m_button_skill3, "Sprites/cancel")
            self.m_button_cancel3:SetActive(true)
            LuaHelper. LoadSprite(self.m_button_attack, path3)
            --self.m_button_skill3.transform.localScale = Vector3.New(1.2, 1.2, 1.2)
            --LuaHelper.ChangeColor(self.m_button_skill3.transform, 255, 255, 255, 255)
        elseif (self.skill == 1) then
            self.skill = 3
            LuaHelper.Run_Skill(self.confirm_skill3)
            --LuaHelper. LoadSprite(self.m_button_skill3, "Sprites/cancel")
            --LuaHelper. LoadSprite(self.m_button_skill1, path1)
            self.m_button_cancel3:SetActive(true)
            self.m_button_cancel1:SetActive(false)
            LuaHelper. LoadSprite(self.m_button_attack, path3)
            --self.m_button_skill3.transform.localScale = Vector3.New(1.2, 1.2, 1.2)
            --LuaHelper.ChangeColor(self.m_button_skill3.transform, 255, 255, 255, 255)
            --self.m_button_skill1.transform.localScale = Vector3.New(1, 1, 1)
            --LuaHelper.ChangeColor(self.m_button_skill1.transform, 200, 200, 200, 200)
        elseif (self.skill == 2) then
            self.skill = 3
            LuaHelper.Run_Skill(self.confirm_skill3)
            --LuaHelper. LoadSprite(self.m_button_skill3, "Sprites/cancel")
            --LuaHelper. LoadSprite(self.m_button_skill2, path2)
            self.m_button_cancel3:SetActive(true)
            self.m_button_cancel2:SetActive(false)
            LuaHelper. LoadSprite(self.m_button_attack, path3)
            --self.m_button_skill3.transform.localScale = Vector3.New(1.2, 1.2, 1.2)
            --LuaHelper.ChangeColor(self.m_button_skill3.transform, 255, 255, 255, 255)
            --self.m_button_skill2.transform.localScale = Vector3.New(1, 1, 1)
            --LuaHelper.ChangeColor(self.m_button_skill2.transform, 200, 200, 200, 200)
        elseif (self.skill == 3) then
            self.skill = 0
            LuaHelper.Run_Skill(0)
            --LuaHelper. LoadSprite(self.m_button_skill3, path3)
            self.m_button_cancel3:SetActive(false)
            LuaHelper. LoadSprite(self.m_button_attack, "Sprites/fireDown")
            --self.m_button_skill3.transform.localScale = Vector3.New(1, 1, 1)
            --LuaHelper.ChangeColor(self.m_button_skill3.transform, 200, 200, 200, 200)
        end
--结束
        else
            self.IsDown3 = 0
            self.skill_description:SetActive(false)
        end
    end);

    local obj = nil
    obj = self.m_transform:FindChild("PanelMiddleDown/Panel/SliderEnergy").gameObject;
    self.m_slider_energy = obj:GetComponent('Slider')
    self.m_energy_value = self.m_transform:FindChild("PanelMiddleDown/Panel/SliderEnergy/EnergyValue"):GetComponent('Text')
    obj = self.m_transform:FindChild("PanelMiddleDown/Panel/SliderPower").gameObject;
    self.m_slider_power = obj:GetComponent('Slider')   
    self.m_record_power = self.m_transform:FindChild("PanelMiddleDown/Panel/SliderPower/Handle Slide Area/Handle")
    self.m_suit_power = self.m_transform:FindChild("PanelMiddleDown/Panel/SliderPower/suitablerange").gameObject
    obj = self.m_transform:FindChild("PanelMiddleDown/Panel/SliderWait").gameObject;
    self.m_slider_wait = obj:GetComponent('Slider')
    self.m_hp_value = self.m_transform:FindChild("PanelMiddleDown/Panel/SliderWait/BloodValue"):GetComponent('Text')
    self.m_tip = self.m_transform:FindChild("PanelMiddleDown/Tip")
    LuaHelper.ChangeColor(self.m_tip, 255,255,255,255)
    self.m_record_power.localPosition = Vector3.New(LuaHelper.GetMyPlayerPower() * 500 - 250, 0, 0)
    self.a = 255--回合提示透明度

    if (LuaHelper.GetMode() >= 1 and LuaHelper.GetMode() <= 3) then
        self.m_suit_power:SetActive(false)
    else
        self.m_suit_power:SetActive(true)
    end

    self.m_button_skip = self.m_transform:FindChild("PanelLeftUp/Skip").gameObject

    self.m_lua_behaviour:AddClick(self.m_button_skip, function (obj)
        LuaHelper.Lua_call("Send_Skip")
    end);


    self.m_lua_behaviour:AddTouchHandle(self.m_button_up, function (obj,touch_type)
        if touch_type == 1 and self.m_input_direct == 0 then
            self.m_input_angle = 0x1
        else
            self.m_input_angle = 0
        end
    end);

    self.m_lua_behaviour:AddTouchHandle(self.m_button_down, function (obj,touch_type)
        if touch_type == 1 and self.m_input_direct == 0 then
            self.m_input_angle = 0x2
        else
            self.m_input_angle = 0
        end
    end);

    self.m_lua_behaviour:AddTouchHandle(self.m_button_right, function (obj,touch_type)
        if touch_type == 1 and  self.m_input_function_down == nil and self.m_input_angle == 0 then
            self.m_input_direct = 0x8
        else
            self.m_input_direct = 0
        end
    end);

    self.m_lua_behaviour:AddTouchHandle(self.m_button_left, function (obj,touch_type)
        if touch_type == 1 and  self.m_input_function_down == nil and self.m_input_angle == 0 then
            self.m_input_direct = 0x4
        else
            self.m_input_direct = 0
        end
    end);

    self.m_lua_behaviour:AddTouchHandle(self.m_button_attack, function (obj,touch_type)
        if touch_type == 1 and self.m_input_angle == 0 and self.m_input_direct == 0 then
            if (self.skill == 1) then
                tmp = self.confirm_skill1
            elseif (self.skill == 2) then
                tmp = self.confirm_skill2
            elseif (self.skill == 3) then
                tmp = self.confirm_skill3
            else
                tmp = 0
            end
            energy = LuaHelper.GetSkillInfo2("ENERGY", 100 + tmp)
            if (Energy - energy < 0) then
                LuaHelper.ChangeColor(self.skill_tip, 255,255,255,255)
                self.aa = 255
            else
                self.m_input_function_down = 0x11
                self.attack_flag = 1
                --LuaHelper.GetSetEnergy(Energy - energy)
            end
        else
            self.m_input_function_down = nil
            LuaHelper.InputFunctionUp(0x11)
            --self.m_last_power.localPosition = Vector3.New(LuaHelper.GetMyPlayerPower()*560,0,0)
        end
    end);

    self.m_input_direct = 0
    self.m_input_angle = 0
    self.m_input_function_down = nil
    self.round = 1
    self.xue = 0
    self:EnableUpdate()
end


function cls_ui_battle_joystick:Update()
    LuaHelper.InputDirect(self.m_input_direct)
    LuaHelper.InputAngle(self.m_input_angle)
    if self.m_input_function_down then
        LuaHelper.InputFunctionDown(self.m_input_function_down)
    end
    self:SetSilder(self.m_slider_energy, LuaHelper.GetMyPlayerEnergy())
    self:SetSilder(self.m_slider_power, LuaHelper.GetMyPlayerPower())
    self:SetSilder(self.m_slider_wait, LuaHelper.GetMyPlayerWait())
    self.m_angle_text.text = math.floor(LuaHelper.GetAngle()) .. "°"

    --self.m_min_power.localPosition = Vector3.New(560 * LuaHelper.GetPowerPercentmin(), 0, 0);
    --self.m_max_power.localPosition = Vector3.New(560 * LuaHelper.GetPowerPercentmax(), 0, 0);
    local maxpower = math.min(500*LuaHelper.GetPowerPercentmax(), 500*0.98)
    local minpower = math.max(500*LuaHelper.GetPowerPercentmin(), 500*0.02)  
    self.m_suit_power.transform.sizeDelta = Vector2.New(maxpower - minpower, 23)
    self.m_suit_power.transform.localPosition = Vector3.New((maxpower + minpower) / 2, -6.4, 0)

    text = tostring(LuaHelper.GetJoyStickDate("hp",0)) .. '/' .. tostring(LuaHelper.GetJoyStickDate("hp",1))
    self.m_hp_value.text = text
    text = tostring(LuaHelper.GetJoyStickDate("energy",0)) .. '/' .. tostring(LuaHelper.GetJoyStickDate("energy",1))
    self.m_energy_value.text = text
    if ((self.m_input_direct ~= 0) and (LuaHelper.GetJoyStickDate("energy",0) <= 0)) then
        LuaHelper.ChangeColor(self.skill_tip, 255,255,255,255)
        if (self.aa <= 0) then
            self.aa = 255
        end
    end

    if (self.m_tip.localPosition.x > 0) then
        self.m_tip.localPosition = Vector3.New(self.m_tip.localPosition.x - Time.deltaTime * 500, self.m_tip.localPosition.y, self.m_tip.localPosition.z)
    else
        self.a = self.a - Time.deltaTime * 100
        LuaHelper.ChangeColor(self.m_tip, 255, 255, 255, self.a)
    end
    if (self.aa > 0) then
        self.aa = self.aa - Time.deltaTime * 100
        LuaHelper.ChangeColor(self.skill_tip, 255,255,255, self.aa)
    end

    Energy = LuaHelper.GetSetEnergy(-1)
    if (self.skill ~= 0) then
        if (self.skill == 1) then
            tmp = self.confirm_skill1
            --self.m_button_cancel1:SetActive(false)
        elseif (self.skill == 2) then
            tmp = self.confirm_skill2
            --self.m_button_cancel2:SetActive(false)
        else
            tmp = self.confirm_skill3
            --self.m_button_cancel3:SetActive(false)
        end
        energy = LuaHelper.GetSkillInfo2("ENERGY", 100 + tmp)
        if (Energy - energy + LuaHelper.Get_BombEnergy()< 0) then
            LuaHelper. LoadSprite(self.m_button_attack, "Sprites/fireDown")
            self.m_button_cancel1:SetActive(false)
            self.m_button_cancel2:SetActive(false)
            self.m_button_cancel3:SetActive(false)
            LuaHelper.Run_Skill(0)
            self.skill = 0
        end
    end

    if (self.IsDown1 == 1) then
        if (Time.time - self.delay >= 0.7) then
            self.skill_description:SetActive(true)
        end
    else
        --
    end

    if (self.IsDown2 == 1) then
        if (Time.time - self.delay >= 0.7) then
            self.skill_description:SetActive(true)
        end
    else
        --
    end

    if (self.IsDown3 == 1) then
        if (Time.time - self.delay >= 0.7) then
            self.skill_description:SetActive(true)
        end
    else
        --
    end
end

function cls_ui_battle_joystick:OnDestroy()
    self.super.OnDestroy(self)
    l_instance = nil
end


function ShowBattleJoyStickUI()
    l_instance = cls_ui_battle_joystick:new()
end

function HideBattleJoyStickUI()
    if l_instance ~= nil then
        l_instance:Close()
    end
end

function cls_ui_battle_joystick:ChangeTurn(flag)
    l_instance.m_middle_down:SetActive(flag)
    l_instance.m_left_down:SetActive(flag)
    l_instance.m_right_down:SetActive(flag)
    l_instance.m_left_up:SetActive(flag)
end

function ShowYourTurnUI()
    if l_instance ~= nil then
       l_instance:ChangeTurn(true)
 --[[       l_instance.m_middle_down:SetActive(true)
        l_instance.m_left_down:SetActive(true)
        l_instance.m_right_down:SetActive(true)
        l_instance.m_left_up:SetActive(true)]]
    end
end

function HideYourTurnUI()
    if l_instance ~= nil then
       l_instance:ChangeTurn(false)
--[[        l_instance.m_middle_down:SetActive(false)
        l_instance.m_left_down:SetActive(false)
        l_instance.m_right_down:SetActive(false)
        l_instance.m_left_up:SetActive(false)]]
    end
end

function Message_Netoff()
    require "ui/ui_message2"
    ShowMessage2UI("网络连接中断")
end

function Message_withString(...)
    require "ui/ui_message"
    ShowMessageUI(...)
end

function BackHost(...)
    require "ui/ui_choose"
    require "ui/ui_prepare"
    require "ui/ui_match"
    require "ui/battle/ui_battle_joystick"
    require "ui/battle/ui_battle_joystick_no_hide"
    HidePrepareUI()
    HideChooseUI()
    HideMatchUI()
    HideBattleJoyStickUI()
    HideBattleJoyStickUI2()
    HideBattleJoyStickUI()
    HideBattleJoyStickUI2()
    require "ui/ui_host"
    ShowHostUI()
    require "ui/ui_message"
    ShowMessageUI("因有玩家逃跑，游戏已解散")
end

function HideAllUI(...)
    require "ui/battle/ui_battle_joystick"
    require "ui/battle/ui_battle_joystick_no_hide"
    require "ui/ui_choose"
    require "ui/ui_duizhan"
    require "ui/ui_host"
    require "ui/ui_information"
    require "ui/ui_inviterec"
    require "ui/ui_invitesend"
    require "ui/ui_loading"
    require "ui/ui_login"
    require "ui/ui_match"
    require "ui/ui_message"
    require "ui/ui_over"
    require "ui/ui_overtip"
    require "ui/ui_prepare"
    require "ui/ui_role"
    require "ui/ui_userlogin"
    require "ui/ui_addfriend"
    
    
    HideBattleJoyStickUI()
    HideBattleJoyStickUI2()
    HideUserloginUI()
    HideRoleUI()
    HidePrepareUI()
    HideOverTipUI()
    HideOverUI()
    HideMessageUI()
    HideMatchUI()
    HideLoginUI()
    HideLoadingUI()
    HideInviteSendUI()
    HideInviteRecUI()
    HideInformationUI()
    HideHostUI()
    HideduizhanUI()
    HideChooseUI()
    HideAddfriendUI()
    HideBattleJoyStickUI()
    HideBattleJoyStickUI2()
end