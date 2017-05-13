local cls_ui_loding = class("cls_ui_loding",cls_ui_base)
cls_ui_loding.s_ui_panel = 'oms_test/PanelLoading'
local l_instance = nill

function cls_ui_loding:ctor()
    self.super.ctor(self)
end

function  cls_ui_loding:SetSilder(slider, value)
    slider.value = value
end

function cls_ui_loding:OnStart()
    self.m_transform = self.m_game_object.transform
    self.m_smallmap = self.m_transform:FindChild("smallmap/map").gameObject
    self.m_mapName = self.m_transform:FindChild("smallmap/map/name").gameObject:GetComponent('Text')
    self.mapindex = LuaHelper.GetMap()
    path = "Map/background_" .. tostring(self.mapindex)
    LuaHelper.LoadRawImage(self.m_smallmap, path)
    mpNames = {"蛋糕王国","迷雾森林","极光幻境","蛋糕王国(大)","迷雾森林(大)","极光幻境(大)"}
    if (self.mapindex <= 6) then
        self.m_mapName.text = mpNames[self.mapindex]
    end
    self.info_loading = {{}, {}, {}, {}, {}, {}}
    for i = 1, 6 do
        obj = self.m_transform:FindChild("prepare1v1/p" .. i .. "/loading").gameObject;
        self.info_loading[i].slider = obj:GetComponent('Slider')

        self.info_loading[i].name = self.m_transform:FindChild("prepare1v1/p" .. i .. "/name").gameObject:GetComponent('Text')
    end

    for i = 1, 6 do
        self.info_loading[i].head = self.m_transform:FindChild("prepare1v1/p" .. i .. "/pic").gameObject
        self.info_loading[i].level = self.m_transform:FindChild("prepare1v1/p" .. i .. "/level/value").gameObject:GetComponent('Text')
        tmp = LuaHelper.Choose(i) - 100
        path = "Sprites/tank" .. tostring(tmp)
        LuaHelper.LoadSprite(self.info_loading[i].head, path)
        self.info_loading[i].level.text = LuaHelper.GetLevel(i)
        if (i % 2 == 1) then
            --self.info_loading[i].head.transform.rotation.eulerAngles = new Vector3(0,180,0);
            LuaHelper.ChangeRotation(self.info_loading[i].head)
        end
        if (LuaHelper.GetMode() == 4) then
            tmp = LuaHelper.Get_Tankid(i)--LuaHelper.Choose(i) - 100
            path = "Sprites/tank" .. tostring(tmp)
            LuaHelper.LoadSprite(self.info_loading[i].head, path)
        end
    end

    if (LuaHelper.GetMode() == 4) then
        self.info_loading[1].name.text = LuaHelper.Get_playerInfoName()
        self.info_loading[2].name.text = "心是六月的情"
    else
        self.info_loading[1].name.text = LuaHelper.GetName(1)
        self.info_loading[2].name.text = LuaHelper.GetName(2)
    end

    local val = LuaHelper.GetMode()
    if (val == 4) then
        self.m_transform:FindChild("prepare1v1/p3").gameObject:SetActive(false)
        self.m_transform:FindChild("prepare1v1/p4").gameObject:SetActive(false)
        self.m_transform:FindChild("prepare1v1/p5").gameObject:SetActive(false)
        self.m_transform:FindChild("prepare1v1/p6").gameObject:SetActive(false)
    else
        if(val <= 1) then
            self.m_transform:FindChild("prepare1v1/p3").gameObject:SetActive(false)
            self.m_transform:FindChild("prepare1v1/p4").gameObject:SetActive(false)
        else
            self.info_loading[3].name.text = LuaHelper.GetName(3)
            self.info_loading[4].name.text = LuaHelper.GetName(4)
        end
        if(val <= 2) then
            self.m_transform:FindChild("prepare1v1/p5").gameObject:SetActive(false)
            self.m_transform:FindChild("prepare1v1/p6").gameObject:SetActive(false)
        else
            self.info_loading[5].name.text = LuaHelper.GetName(5)
            self.info_loading[6].name.text = LuaHelper.GetName(6)
        end
    end
    self.xue = 0
    self:EnableUpdate()
    self:InitReturn()
end

function cls_ui_loding:InitReturn( ... )
    -- self.m_btn_return = self.m_transform:FindChild("PanelRightUp/ButtonReturn").gameObject;

    -- self.m_lua_behaviour:AddClick(self.m_btn_return, function (obj)
    -- end);
end

function cls_ui_loding:Update()
    local val = LuaHelper.Lua_get("GetLoading")

    if(val == 10) then
        LuaHelper.Lua_call("Start_Game")
        self:Close()
        SendUIMessage("ENUM_SHOW_BATTLE_JOYSTICK_UI")
        SendUIMessage("ENUM_SHOW_BATTLE_JOYSTICK_UI_NO_HIDE")
        LuaHelper.Lua_call("Init_Battle_UI")
    end

    val = LuaHelper.Get_Loading(1)
    self:SetSilder(self.info_loading[1].slider, val / 100)

    local ver = LuaHelper.GetMode()
    if (ver == 4) then
        self:SetSilder(self.info_loading[2].slider, 1)
    else 
        if(ver >= 1) then
            local val2 = LuaHelper.Get_Loading(2)
            self:SetSilder(self.info_loading[2].slider, val2 / 100)
        end
        if(ver >= 2) then
            local val2 = LuaHelper.Get_Loading(3)
            self:SetSilder(self.info_loading[3].slider, val2 / 100)
            val2 = LuaHelper.Get_Loading(4)
            self:SetSilder(self.info_loading[4].slider, val2 / 100)
        end
        if(ver >= 3) then
            local val2 = LuaHelper.Get_Loading(5)
            self:SetSilder(self.info_loading[5].slider, val2 / 100)
            val2 = LuaHelper.Get_Loading(6)
            self:SetSilder(self.info_loading[6].slider, val2 / 100)
        end
    end
end

function cls_ui_loding:OnDestroy()
    self.super.OnDestroy(self)
    l_instance = nil
end


function ShowLoadingUI()
    l_instance = cls_ui_loding:new()
end

function HideLoadingUI()
    if l_instance ~= nil then
        l_instance:Close()
    end
end
