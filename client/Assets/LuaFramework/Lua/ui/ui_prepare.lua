--[[
测试列表，有个按钮
]]

local cls_ui_prepare = class("cls_ui_prepare",cls_ui_base)
cls_ui_prepare.s_ui_panel = 'oms_test/PanelPrepare'
local l_instance = nil

function cls_ui_prepare:ctor()
    self.super.ctor(self)
end

function cls_ui_prepare:OnStart()
    self:EnableUpdate()
    self.m_transform = self.m_game_object.transform
    self.m_btn_prepare = self.m_transform:FindChild("bg/btm_prepare").gameObject
    self.m_head1 = self.m_transform:FindChild("bg/prepare/headbg1").gameObject
    self.m_head1_1 = self.m_transform:FindChild("bg/prepare/headbg1/none").gameObject
    self.m_head1_2 = self.m_transform:FindChild("bg/prepare/headbg1/head").gameObject
    self.m_head2 = self.m_transform:FindChild("bg/prepare/headbg2").gameObject
    self.m_head2_1 = self.m_transform:FindChild("bg/prepare/headbg2/none").gameObject
    self.m_head2_2 = self.m_transform:FindChild("bg/prepare/headbg2/head").gameObject
    self.m_head3 = self.m_transform:FindChild("bg/prepare/headbg3").gameObject
    self.m_head3_1 = self.m_transform:FindChild("bg/prepare/headbg3/none").gameObject
    self.m_head3_2 = self.m_transform:FindChild("bg/prepare/headbg3/head").gameObject
    self.m_head4 = self.m_transform:FindChild("bg/prepare/headbg4").gameObject
    self.m_head4_1 = self.m_transform:FindChild("bg/prepare/headbg4/none").gameObject
    self.m_head4_2 = self.m_transform:FindChild("bg/prepare/headbg4/head").gameObject
    self.m_head5 = self.m_transform:FindChild("bg/prepare/headbg5").gameObject
    self.m_head5_1 = self.m_transform:FindChild("bg/prepare/headbg5/none").gameObject
    self.m_head5_2 = self.m_transform:FindChild("bg/prepare/headbg5/head").gameObject
    self.m_head6 = self.m_transform:FindChild("bg/prepare/headbg6").gameObject
    self.m_head6_1 = self.m_transform:FindChild("bg/prepare/headbg6/none").gameObject
    self.m_head6_2 = self.m_transform:FindChild("bg/prepare/headbg6/head").gameObject
    self.m_show_num = self.m_transform:FindChild("bg/Text/number"):GetComponent('Text')
    self.m_show_time = self.m_transform:FindChild("bg/Text/time"):GetComponent('Text')


    --为了提前下载好图片
    self.tempHead  = self.m_transform:FindChild("bg/temp").gameObject
    for i = 1,6 do
        LuaHelper.setPlayerWxHead(self.tempHead, i)
    end

    self.m_head1_2:SetActive(false)
    self.m_head2_2:SetActive(false)
    self.m_head3_2:SetActive(false)
    self.m_head4_2:SetActive(false)
    self.m_head5_2:SetActive(false)
    self.m_head6_2:SetActive(false)

    self.m_show_num.text = "0/2"
    self.m_show_time.text = "30"
    self.now = -1   -- -1:初始值   -2:准备发送   -3:准备好等待开始
    self.begin = Time.time
    self.people = 0

    self.seat = {}

    self.mode = LuaHelper.GetMode()
    if (self.mode ~= 3) then
        self.m_head6:SetActive(false)
        self.m_head5:SetActive(false)
    end
     if (self.mode ~= 3 and self.mode ~= 2) then
        self.m_head3:SetActive(false)
        self.m_head4:SetActive(false)
    end

    self.m_lua_behaviour:AddClick(self.m_btn_prepare, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        self.m_btn_prepare:GetComponent('Button').interactable = false;
        if self.now == -1 then
            self.now = -2
            return 
        end
    end);
end
--[[
function  cls_ui_prepare:FixedUpdate()
    self.time = self.time - 1
    self.m_show_time.text = tostring(math.floor(self.time/60))
end]]

function cls_ui_prepare:Update()
    local tmp = LuaHelper.Lua_get("Get_Seat")
    if (tmp ~= self.people) then
        self.people = tmp
        -- if (tmp >= 6) then
        --     self.m_head6_1:SetActive(false)
        --     self.m_head6_2:SetActive(true)
        -- end
        -- if (tmp >= 5) then
        --     self.m_head5_1:SetActive(false)
        --     self.m_head5_2:SetActive(true)
        -- end
        -- if (tmp >= 4) then
        --     self.m_head4_1:SetActive(false)
        --     self.m_head4_2:SetActive(true)
        -- end
        -- if (tmp >= 3) then
        --     self.m_head3_1:SetActive(false)
        --     self.m_head3_2:SetActive(true)
        -- end
        -- if (tmp >= 2) then
        --     self.m_head2_1:SetActive(false)
        --     self.m_head2_2:SetActive(true)
        -- end
        -- if (tmp >= 1) then
        --     self.m_head1_1:SetActive(false)
        --     self.m_head1_2:SetActive(true)
        -- end
        Seat = LuaHelper.Lua_get("Get_Seat_Binary")
        --log("aiiiiiiiii" .. Seat)
        for i = 1,6 do
            self.seat[i] = Seat % 2
            Seat = math.floor(Seat / 2)
        end
        path = "Sprites/head"
        if (self.mode == 3) then
            if (self.seat[1] == 1) then
                self.m_head5_1:SetActive(false)
                self.m_head5_2:SetActive(true)
                LuaHelper.LoadSprite(self.m_head5_2, path .. tostring(LuaHelper.Get_Sex(1)))
                LuaHelper.setPlayerWxHead(self.m_head5_2, 1)
            end 
            if (self.seat[2] == 1) then
                self.m_head1_1:SetActive(false)
                self.m_head1_2:SetActive(true)
                LuaHelper.LoadSprite(self.m_head1_2, path .. tostring(LuaHelper.Get_Sex(2)))
                LuaHelper.setPlayerWxHead(self.m_head1_2, 2)
            end
            if (self.seat[3] == 1) then
                self.m_head3_1:SetActive(false)
                self.m_head3_2:SetActive(true)
                LuaHelper.LoadSprite(self.m_head3_2, path .. tostring(LuaHelper.Get_Sex(3)))
                LuaHelper.setPlayerWxHead(self.m_head3_2, 3)
            end
            if (self.seat[4] == 1) then
                self.m_head4_1:SetActive(false)
                self.m_head4_2:SetActive(true)
                LuaHelper.LoadSprite(self.m_head4_2, path .. tostring(LuaHelper.Get_Sex(4)))
                LuaHelper.setPlayerWxHead(self.tempHead, 4)
            end
            if (self.seat[5] == 1) then
                self.m_head2_1:SetActive(false)
                self.m_head2_2:SetActive(true)
                LuaHelper.LoadSprite(self.m_head2_2, path .. tostring(LuaHelper.Get_Sex(5)))
                LuaHelper.setPlayerWxHead(self.m_head2_2, 5)
            end
            if (self.seat[6] == 1) then
                self.m_head6_1:SetActive(false)
                self.m_head6_2:SetActive(true)
                LuaHelper.LoadSprite(self.m_head6_2, path .. tostring(LuaHelper.Get_Sex(6)))
                LuaHelper.setPlayerWxHead(self.m_head6_2, 6)
            end
        elseif (self.mode == 2) then
            if (self.seat[1] == 1) then
                self.m_head1_1:SetActive(false)
                self.m_head1_2:SetActive(true)
                LuaHelper.LoadSprite(self.m_head1_2, path .. tostring(LuaHelper.Get_Sex(1)))
                LuaHelper.setPlayerWxHead(self.m_head1_2, 1)
            end
            if (self.seat[2] == 1) then
                self.m_head3_1:SetActive(false)
                self.m_head3_2:SetActive(true)
                LuaHelper.LoadSprite(self.m_head3_2, path .. tostring(LuaHelper.Get_Sex(2)))
                LuaHelper.setPlayerWxHead(self.m_head3_2, 2)
            end
            if (self.seat[3] == 1) then
                self.m_head4_1:SetActive(false)
                self.m_head4_2:SetActive(true)
                LuaHelper.LoadSprite(self.m_head4_2, path .. tostring(LuaHelper.Get_Sex(3)))
                LuaHelper.setPlayerWxHead(self.m_head4_2, 3)
            end
            if (self.seat[4] == 1) then
                self.m_head2_1:SetActive(false)
                self.m_head2_2:SetActive(true)
                LuaHelper.LoadSprite(self.m_head2_2, path .. tostring(LuaHelper.Get_Sex(4)))
                LuaHelper.setPlayerWxHead(self.m_head2_2, 4)
            end
        else
            if (self.seat[1] == 1) then
                self.m_head1_1:SetActive(false)
                self.m_head1_2:SetActive(true)
                LuaHelper.LoadSprite(self.m_head1_2, path .. tostring(LuaHelper.Get_Sex(1)))
                LuaHelper.setPlayerWxHead(self.m_head1_2, 1)
            end
            if (self.seat[2] == 1) then
                self.m_head2_1:SetActive(false)
                self.m_head2_2:SetActive(true)
                LuaHelper.LoadSprite(self.m_head2_2, path .. tostring(LuaHelper.Get_Sex(2)))
                LuaHelper.setPlayerWxHead(self.m_head2_2, 2)
            end
        end

        self.m_show_num.text = tmp .. "/" .. LuaHelper.GetMode() * 2
    end

    local tmp = LuaHelper.MAP_MATCH_result()
    self.time = Time.time - self.begin
    if(self.time > 30) then 
        self.time = 30
    end
    self.m_show_time.text = tostring(math.floor(30 - self.time))

    if ((self.now == -1 and self.time >= 30) or self.now == -2) then
        LuaHelper.Lua_call("Send_GameReady")
        self.now = -3
        return
    end

    if (tmp == 10000) then
--[[
        require "logic/battle_manager"
        battle_manager.StartBattle(1)

        require "ui/ui_loading"
        ShowLoadingUI()
--]]
        
        require "ui/battle/ui_battle_joystick"
        HideAllUI()
        SendUIMessage("ENUM_SHOW_CHOOSE")
    end
end

function cls_ui_prepare:OnDestroy()
    self.super.OnDestroy(self)
    l_instance = nil
end


function ShowPrepareUI()
    l_instance = cls_ui_prepare:new()
end

function  HidePrepareUI()
    if l_instance ~= nil then
        l_instance:Close()
    end
end