local scaleStatus = {
    ["ENUM_BIGER"] = 1,
    ["ENUM_NORMAL"] = 2
}

local cls_ui_chooseMap = class("cls_ui_chooseMap",cls_ui_base)
cls_ui_chooseMap.s_ui_panel = 'oms_test/PanelChooseMap'
local l_instance = nil

function cls_ui_chooseMap:ctor()
    self.super.ctor(self)
end

function cls_ui_chooseMap:OnStart()
    self.m_transform = self.m_game_object.transform
    self.mapWhole = {}
    self.mapBtn = {}
    self.scaleVec = {Vector3.New(1.2, 1.2, 1.2), Vector3.New(1, 1, 1)}
    
    self.enterBtn = self.m_transform:FindChild("Btn").gameObject             --进入游戏按钮
    self.enterBtn1 = self.enterBtn:GetComponent('Button')
    self.btnActive = {true, false}

    self.localScale = scaleStatus["ENUM_NORMAL"]         --当前尺寸
    self.enterBtn1.interactable  = false         --正常状态不可点击,变大可点击
    self.preClick = -1                                   --上一次点击的图片
    self.clickOn = -1                                    --当前点击图片

    --核心逻辑 6张图片的按钮
    for i = 1,6 do
        self.mapWhole[i] = self.m_transform:FindChild("allMap/smallmap"..tostring(i-1)).gameObject      --整体图片框
        self.mapBtn[i] = self.m_transform:FindChild("allMap/smallmap"..tostring(i-1).."/map" .. tostring(i - 1)).gameObject --按钮位置
        self.m_lua_behaviour:AddClick(self.mapBtn[i], function (obj)
            LuaHelper.Play_Sound(1,"sound/yinxiao/03");
            self.preClick = self.clickOn
            self.clickOn = i

            if i == self.preClick or self.preClick == -1 then      --点的是同一张图片 或者 没有点击过 
                self.localScale = 3 - self.localScale
                self.mapWhole[i].transform.localScale = self.scaleVec[self.localScale]
                self.enterBtn1.interactable = self.btnActive[self.localScale]
            elseif self.preClick ~= -1 then                                   --点击的不是同一张图片
                if self.localScale == scaleStatus["ENUM_NORMAL"] then
                    self.localScale = 3-self.localScale
                end
                self.mapWhole[i].transform.localScale = self.scaleVec[self.localScale]
                self.enterBtn1.interactable = self.btnActive[self.localScale]
                self.mapWhole[self.preClick].transform.localScale = self.scaleVec[3-self.localScale]
            end
        end)
    end

    --进入游戏按钮
    self.m_lua_behaviour:AddClick(self.enterBtn, function (obj)
            LuaHelper.Play_Sound(1,"sound/yinxiao/03");
            self:Close()
            require "logic/battle_manager"
            battle_manager.StartBattle(self.clickOn)
            require "ui/ui_loading"
            ShowLoadingUI()
            
        end)

    --返回大厅按钮
    self.m_btn_quit = self.m_transform:FindChild("PanelMiddleTop/Return").gameObject
    self.m_lua_behaviour:AddClick(self.m_btn_quit, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        SendUIMessage("ENUM_SHOW_HOST")
        self:Close()
    end);

    -- 规则按钮
    self.m_btn_rule = self.m_transform:FindChild("PanelMiddleTop/Rule").gameObject
    self.m_rule_tip = self.m_transform:FindChild("PanelRoot/all").gameObject
    self.m_close_rule = self.m_transform:FindChild("PanelRoot/all/Button").gameObject
    self.m_close_rule_btn = self.m_close_rule:GetComponent('Button')
    self.m_cancel_panel = self.m_transform:FindChild("PanelRoot/cancel_panel").gameObject
    self.m_cancel_panel_btn = self.m_cancel_panel:GetComponent('Button')

    self.m_lua_behaviour:AddClick(self.m_cancel_panel, function (obj)
        self.m_cancel_panel:SetActive(false)
        self.m_close_rule_btn.enabled = false
        LuaHelper.DoLocalMoveY(self.m_rule_tip,800,0.3,function ()
            self.m_rule_tip:SetActive(false)
        end)
    end);
    self.m_lua_behaviour:AddClick(self.m_close_rule, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        self.m_cancel_panel:SetActive(false)
        self.m_close_rule_btn.enabled = false
        LuaHelper.DoLocalMoveY(self.m_rule_tip,800,0.3,function ()
            self.m_rule_tip:SetActive(false)
        end)
    end);
    self.m_lua_behaviour:AddClick(self.m_btn_rule, function (obj)
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");    
        self.m_cancel_panel_btn.enabled = false
        self.m_close_rule_btn.enabled = false
        self.m_cancel_panel:SetActive(true)
        self.m_rule_tip:SetActive(true)
        LuaHelper.DoLocalMoveY(self.m_rule_tip,4,0.3,function ()
            self.m_close_rule_btn.enabled = true
            self.m_cancel_panel_btn.enabled = true
        end)
    end);
end

function cls_ui_chooseMap:OnDestroy()
    self.super.OnDestroy(self)
    l_instance = nil
end


function ShowchooseMapUI()
    l_instance = cls_ui_chooseMap:new()
end

function  HidechooseMapUI()
    if l_instance ~= nil then
        l_instance:Close()
    end
end