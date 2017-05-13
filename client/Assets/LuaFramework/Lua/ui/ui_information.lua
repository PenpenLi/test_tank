--[[
消息弹窗
]]

local cls_ui_information = class("cls_ui_information",cls_ui_base)
cls_ui_information.s_ui_panel = 'oms_test/PanelInformation'
cls_ui_information.s_ui_order = 100
local l_instance = nil

-- 2name  3sex  4exp  5cnt  6win  77relation
function cls_ui_information:ctor(...)
    self.super.ctor(self)
    self.arr = {...}
    self.m_msg = table.concat( self.arr, "\n")
end

function cls_ui_information:OnStart()
    self.m_transform = self.m_game_object.transform
    
    self.m_head = self.m_transform:FindChild("PanelRoot/headbg/head").gameObject
    self.m_sex = self.m_transform:FindChild("PanelRoot/sexbg/sex").gameObject
    self.m_level = self.m_transform:FindChild("PanelRoot/levelbg/level"):GetComponent('Text')
    self.m_name = self.m_transform:FindChild("PanelRoot/name"):GetComponent('Text')
    self.m_sum = self.m_transform:FindChild("PanelRoot/ratebg/gamevalue"):GetComponent('Text')
    self.m_win = self.m_transform:FindChild("PanelRoot/ratebg/ratevalue"):GetComponent('Text')
    
    self.m_cancelbtn = self.m_transform:FindChild("PanelRoot/cancel_panel").gameObject;
    self.m_addbtn = self.m_transform:FindChild("PanelRoot/AddButton").gameObject
    self.m_delbtn = self.m_transform:FindChild("PanelRoot/DelButton").gameObject
    --self.m_transform:FindChild("PanelRoot/PanelTip/TextTip"):GetComponent('Text').text = tostring(self.m_msg)

    --init
    self.m_name.text = self.arr[2]
    path = "Sprites/head" .. tostring(self.arr[3])
    LuaHelper.LoadSprite(self.m_head, path)
                --微信头像

    path = "Sprites/sex" .. tostring(self.arr[3])
    LuaHelper.LoadSprite(self.m_sex, path)

    self.m_level.text = LuaHelper.transExp(tostring(self.arr[4]), 2)
    self.m_sum.text = tostring(self.arr[5]) .. "场"
    if(self.arr[5] == 0) then
        self.m_win.text = "0%"
    else
        self.m_win.text = math.floor(self.arr[6] / self.arr[5] * 100) .. "%"
    end


    relation = self.arr[7] -- 0自己 1好友 2非好友
    if (relation == 0) then
        LuaHelper.setWXhead(self.m_head)  
        self.m_addbtn:SetActive(false)
        self.m_delbtn:SetActive(false)
    elseif (relation == 1) then
        if(self.arr[8] == 1) then
            LuaHelper.setFriendHead(self.m_head, self.arr[9])  
        end
        self.m_addbtn:SetActive(false)
    else
        if(self.arr[8] == 1) then
            LuaHelper.setFriendHead(self.m_head, self.arr[9])  
        end
        self.m_delbtn:SetActive(false)
    end

    self.m_lua_behaviour:AddClick(self.m_cancelbtn, function (obj)
        self:Close()
    end);
    self.m_lua_behaviour:AddClick(self.m_addbtn, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        LuaHelper.Lua_set_string("Lua_Add_Friend", self.m_name.text)
        self:Close()
    end);
    self.m_lua_behaviour:AddClick(self.m_delbtn, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        LuaHelper.Lua_set_string("Lua_Del_Friend", self.m_name.text)
        self:Close()
    end);

    self.description = self.m_transform:FindChild("PanelRoot/description").gameObject
    self.descrip_text = self.m_transform:FindChild("PanelRoot/description/descrip"):GetComponent('Text')

    self.food_description = self.m_transform:FindChild("PanelRoot/food_description").gameObject
    self.food_description_text = self.m_transform:FindChild("PanelRoot/food_description/text"):GetComponent('Text')
    self.food_description_image = self.m_transform:FindChild("PanelRoot/food_description/image").gameObject
    self.food = {}
    for i = 1, 4 do
        if (i > 2) then
            self.food[i] = self.m_transform:FindChild("PanelRoot/foodbg/ScrollRect/ToggleGroup/food" .. tostring(i)).gameObject;
            self.m_lua_behaviour:AddTouchHandle(self.food[i], function (obj, value)
                if value == 1 then
                    self.food_description:SetActive(true)
                    self.food_description.transform.localPosition = Vector3.New(self.food[i].transform.localPosition.x - 150, 170, 0)
                    self.food_description_text.text = LuaHelper.GetTankInfo("DECRIP", i * 100000 + 1)
                    Skill = tonumber(LuaHelper.GetTankInfo("SKILL", i * 100000 + 1))
                    log("shabia=" .. Skill)
                    path = "Sprites/ch_sk" .. tostring(Skill - 100)
                    LuaHelper. LoadSprite(self.food_description_image, path)
                elseif value == 3 then
                    self.food_description:SetActive(false)
                end
            end);
        end
        -- self.m_lua_behaviour:AddToggleHandle(self.food[i], function (obj,touch_type)
                -- if touch_type == 1 then
                --     self.description:SetActive(true)
                --     self.descrip_text.text = LuaHelper.GetTankInfo("NAME", i * 100000 + 1)
                -- else
                --     self.description:SetActive(false)
                -- end
        -- end);
    end
    self.toggle_group = self.m_transform:FindChild("PanelRoot/skillbg/ScrollRect/ToggleGroup")
    self.skill = {}
    for i = 1, 7 do
        if (i ~= 4 and i ~= 2 and i ~= 5) then
            self.skill[i] = self.m_transform:FindChild("PanelRoot/skillbg/ScrollRect/ToggleGroup/skill" .. tostring(i)).gameObject;
            self.m_lua_behaviour:AddTouchHandle(self.skill[i], function (obj, value)
                if value == 1 then
                    self.description:SetActive(true)
                    self.description.transform.localPosition = Vector3.New(self.skill[i].transform.localPosition.x - 150, -50, 0)
                    self.descrip_text.text = LuaHelper.GetSkillInfo("DESCRIPTION", i + 100)
                elseif value == 3 then
                    self.description:SetActive(false)
                end
            end);
            -- self.m_lua_behaviour:AddToggleHandle(self.skill[i], function (obj, touch_type)
            --     if touch_type == 1 then
            --         self.description:SetActive(true)
            --         self.descrip_text.text = LuaHelper.GetSkillInfo("DESCRIPTION", i + 100)
            --     else
            --         self.description:SetActive(false)
            --     end
            -- end);
        end
    end

end

function cls_ui_information:OnDestroy()
    self.super.OnDestroy(self)
    l_instance = nil
end

function ShowInformationUI(...)
    l_instance = cls_ui_information:new(...)
end

function DestroyInformationUI()
   l_instance:Close()
end

function  HideInformationUI()
    if l_instance ~= nil then
        l_instance:Close()
    end
end