local cls_ui_overtip = class("cls_ui_overtip", cls_ui_base)
cls_ui_overtip.s_ui_panel = 'oms_test/PanelOverTip'

function cls_ui_overtip:ctor()
	self.super.ctor(self)
end

function cls_ui_overtip:OnStart()
	   LuaHelper.Play_Sound(-1,"sound/Music/battlebg_4");
--	   LuaHelper.Set_Sound(1,"bgm")
	self:EnableUpdate()
	self.m_transform = self.m_game_object.transform
	self.m_win = self.m_transform:FindChild("win").gameObject
	self.m_lose = self.m_transform:FindChild("lose").gameObject

	local win_id = LuaHelper.GetWinnerUid()
	if (win_id == 1) then
		self.m_win:SetActive(false)
	else
		self.m_lose:SetActive(false)
	end
	require "ui/battle/ui_battle_joystick"
    require "ui/battle/ui_battle_joystick_no_hide"
    HideBattleJoyStickUI()
    HideBattleJoyStickUI2()

end
function cls_ui_overtip:Update()
	if (Input.GetMouseButtonDown(0)) then
		SendUIMessage("ENUM_SHOW_OVER")
        self:Close()
	end

	if (self.m_win.transform.localScale.x < 1) then
        self.m_win.transform.localScale = Vector3.New(self.m_win.transform.localScale.x + Time.deltaTime, self.m_win.transform.localScale.y + Time.deltaTime, self.m_win.transform.localScale.z)
    end
    if (self.m_lose.transform.localScale.x < 1) then
        self.m_lose.transform.localScale = Vector3.New(self.m_lose.transform.localScale.x + Time.deltaTime, self.m_lose.transform.localScale.y + Time.deltaTime, self.m_lose.transform.localScale.z)
    end
end

function cls_ui_overtip:OnDestroy()
	self.super.OnDestroy(self)
	l_instance = nil
end

function ShowOverTipUI()
	l_instance = cls_ui_overtip:new()
end

function  HideOverTipUI()
    if l_instance ~= nil then
        l_instance:Close()
    end
end