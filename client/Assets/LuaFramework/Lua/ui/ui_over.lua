local cls_ui_over = class("cls_ui_over", cls_ui_base)
cls_ui_over.s_ui_panel = 'oms_test/PanelOver'

function cls_ui_over:ctor()
	self.super.ctor(self)
end

function  cls_ui_over:SetSilder(slider, value)
    slider.value = value
end

function cls_ui_over:OnStart()
	self.m_transform = self.m_game_object.transform
	self.m_btn_restart = self.m_transform:FindChild("bg/ButtonOver").gameObject
--	self.m_text = self.m_transform:FindChild("WinId"):GetComponent('Text')
--	self.m_text.text = "Winner id: " .. tostring(LuaHelper.GetWinnerUid())
	self.m_win = self.m_transform:FindChild("bg/win").gameObject
	self.m_lose = self.m_transform:FindChild("bg/lose").gameObject
	
	self.m_name_text = self.m_transform:FindChild("bg/CharacterInfo/username"):GetComponent('Text')
	self.battleres = self.m_transform:FindChild("bg/battleres").gameObject
	self.exp = self.m_transform:FindChild("bg/battleres/exp").gameObject
	self.coin = self.m_transform:FindChild("bg/battleres/coin").gameObject
	self.jewel = self.m_transform:FindChild("bg/battleres/jewel").gameObject
	self.Get_exp = self.m_transform:FindChild("bg/battleres/exp/value"):GetComponent('Text')
	self.Get_coin = self.m_transform:FindChild("bg/battleres/coin/value"):GetComponent('Text')
	self.Get_jewel = self.m_transform:FindChild("bg/battleres/jewel/value"):GetComponent('Text')
	self.Now_coin = self.m_transform:FindChild("bg/CharacterInfo/weekycoin"):GetComponent('Text')
	self.Now_jewel = self.m_transform:FindChild("bg/CharacterInfo/weekydiamond"):GetComponent('Text')
	self.Now_exp = self.m_transform:FindChild("bg/CharacterInfo/levelexp/exp"):GetComponent('Text')
	obj = self.m_transform:FindChild("bg/CharacterInfo/levelexp").gameObject
	self.exp_slider = obj:GetComponent('Slider')
	self.m_head = self.m_transform:FindChild("bg/CharacterInfo/headbg/head").gameObject
    local val2 = LuaHelper.GetVersion()
    if(val2 == 1) then
    	self.info = {}
    	self.m_name_text.text = LuaHelper.Get_playerInfoName()
    	
    	for i = 1,7 do
    		self.info[i] = LuaHelper.Get_playerInfo(i)
    	end
    	-- gold = 1,
        -- diamond = 2,
        -- exp = 3,
        -- addgold = 4,
        -- adddiamond = 5,
        -- addexp = 6,
        -- sex = 7,
        self.Now_coin.text = "金币: " .. tostring(self.info[1])
        self.Now_jewel.text = "钻石: " .. tostring(self.info[2])
      
        LuaHelper.DoLocalMoveX(self.battleres,0,0.2,function ()        	
        	self.exp:SetActive(true)
        	LuaHelper.DoToInt(0,self.info[6],0.3,function (value)
        	self.Get_exp.text = tostring(value)
       	 	end,function ()
       	 		self.coin:SetActive(true)
        		LuaHelper.DoToInt(0,self.info[4],0.3,function (value)
        		self.Get_coin.text = tostring(value)
        		end,function ()
        			self.jewel:SetActive(true)
        			LuaHelper.DoToInt(0,self.info[5],0.3,function (value)
        			self.Get_jewel.text = tostring(value)
        			end,nil)
        		end)
       	 	end)
        end)

    	path = "Sprites/head" .. tostring(self.info[7])
		LuaHelper. LoadSprite(self.m_head, path)
		LuaHelper.setWXhead(self.m_head)              --微信头像
    	
    	--type 1.level2exp 2.exp2level 3.resexp2level
		level = LuaHelper.transExp(self.info[3], 2)
		res = LuaHelper.transExp(self.info[3], 3)
		exp = LuaHelper.transExp(self.info[3], 1)
		self.Now_exp.text = res .. "/" .. tostring(exp)
    	self:SetSilder(self.exp_slider, res/exp)
    end

	local win_id = LuaHelper.GetWinnerUid()
	if (win_id == 1) then
		self.m_win:SetActive(false)
	else
		self.m_lose:SetActive(false)
	end
	
	self.m_lua_behaviour:AddClick(self.m_btn_restart, function(obj)
		LuaHelper.Play_Sound(1,"sound/yinxiao/03");
		local val = LuaHelper.GetVersion()
		if(val == 0) then
			require "ui/ui_login"
			ShowLoginUI()
			self:Close()
		else 
			SendUIMessage("ENUM_SHOW_HOST")
        	self:Close()
        end
	end)

end

function cls_ui_over:OnDestroy()
	self.super.OnDestroy(self)
	l_instance = nil
end

function ShowOverUI()
	l_instance = cls_ui_over:new()
end

function  HideOverUI()
    if l_instance ~= nil then
        l_instance:Close()
    end
end