local cls_ui_choose = class("cls_ui_choose", cls_ui_base)
cls_ui_choose.s_ui_panel = "oms_test/PanelChoose"
local l_instance = nil

function  cls_ui_choose:ctor()
	self.super.ctor(self)
end

function cls_ui_choose:OnStart()
	self:EnableUpdate()
	self.m_transform = self.m_game_object.transform



	self.m_btn_confirm = self.m_transform:FindChild("RightBg/confirm").gameObject
	self.m_show_time = self.m_transform:FindChild("bottomBg/clocktime"):GetComponent('Text')
	self.begin = Time.time

	self.m_info = {{}, {}, {}}
	self.m_info[1].bg = self.m_transform:FindChild("RightBg/chooseBg/p1Bg").gameObject
	self.m_info[1].ok = self.m_transform:FindChild("RightBg/chooseBg/p1Bg/current").gameObject
	self.m_info[1].head_head = self.m_transform:FindChild("RightBg/chooseBg/p1Bg/headbg1/head").gameObject
	self.m_info[1].head_none = self.m_transform:FindChild("RightBg/chooseBg/p1Bg/headbg1/none").gameObject
	self.m_info[1].name = self.m_transform:FindChild("RightBg/chooseBg/p1Bg/name").gameObject:GetComponent('Text')

	self.m_info2bg = self.m_transform:FindChild("RightBg/chooseBg/p2Bg").gameObject
	self.m_info[2].ok = self.m_transform:FindChild("RightBg/chooseBg/p2Bg/current").gameObject
	self.m_info[2].head_head = self.m_transform:FindChild("RightBg/chooseBg/p2Bg/headbg2/head").gameObject
	self.m_info[2].head_none = self.m_transform:FindChild("RightBg/chooseBg/p2Bg/headbg2/none").gameObject
	self.m_info[2].name = self.m_transform:FindChild("RightBg/chooseBg/p2Bg/name").gameObject:GetComponent('Text')

	self.m_info3bg = self.m_transform:FindChild("RightBg/chooseBg/p3Bg").gameObject
	self.m_info[3].ok = self.m_transform:FindChild("RightBg/chooseBg/p3Bg/current").gameObject
	self.m_info[3].head_head = self.m_transform:FindChild("RightBg/chooseBg/p3Bg/headbg3/head").gameObject
	self.m_info[3].head_none = self.m_transform:FindChild("RightBg/chooseBg/p3Bg/headbg3/none").gameObject
	self.m_info[3].name = self.m_transform:FindChild("RightBg/chooseBg/p3Bg/name").gameObject:GetComponent('Text')

	self.centBG = self.m_transform:FindChild("CentBg")
	self.tank = self.m_transform:FindChild("CentBg/tank").gameObject
	self.tankskill = self.m_transform:FindChild("CentBg/tankskill").gameObject
	self.tankname = self.m_transform:FindChild("CentBg/tankname").gameObject:GetComponent('Text')
	self.tankdcp = self.m_transform:FindChild("CentBg/ch_sk_bg/description").gameObject:GetComponent('Text')
	self.config = {}
	self.config[1] = self.m_transform:FindChild("CentBg/config1/bloodv").gameObject
	self.config[2] = self.m_transform:FindChild("CentBg/config1/attackv").gameObject
	self.config[3] = self.m_transform:FindChild("CentBg/config1/energyv").gameObject
	self.config[4] = self.m_transform:FindChild("CentBg/config2/rangev").gameObject
	self.config[5] = self.m_transform:FindChild("CentBg/config2/speedv").gameObject
	self.config[6] = self.m_transform:FindChild("CentBg/config2/anglev").gameObject

	self.confirm_tankskill = 7
	self.m_btn_skill = {}
	self.m_skillBG = {}
	
	for i = 1,7 do
		self.m_btn_skill[i] = self.m_transform:FindChild("CentBg/ch_sk_bg/skill" .. tostring(i)).gameObject
		self.m_skillBG[i] = self.m_transform:FindChild("CentBg/ch_sk_bg/skill" .. tostring(i).."bg").gameObject
	end
	--self.m_btn_skill[2] = self.m_transform:FindChild("CentBg/ch_sk_bg/skill2").gameObject
	self.m_btn_skill1 = self.m_transform:FindChild("CentBg/ch_skill1").gameObject
	self.m_btn_skill2 = self.m_transform:FindChild("CentBg/ch_skill2").gameObject
	self.m_change_skill1 = self.m_transform:FindChild("CentBg/ch_skill1/text1").gameObject
	self.m_change_skill2 = self.m_transform:FindChild("CentBg/ch_skill2/text2").gameObject	
	self.skillbg = self.m_transform:FindChild("CentBg/ch_sk_bg").gameObject
	self.skillconfirm = self.m_transform:FindChild("CentBg/ch_sk_bg/skill_confirm").gameObject
	self.status = -1 --当前状态 -1:隐藏面板 1:选技能1 2:选技能2
	self.skillbg:SetActive(false)
	self.skill1 = 1
	self.confirm_skill1 = 1
	self.skill2 = 4
	self.confirm_skill2 = 4

	--选中技能的框
	self.m_skillBG[self.skill1]:SetActive(true)
	--self.m_skillBG[self.skill2]:SetActive(true)

	self.m_index = LuaHelper.Lua_get("My_Index")

	self.mode = LuaHelper.GetMode()
	if (self.mode < 3) then
		self.m_info3bg:SetActive(false)
	end
	if (self.mode < 2) then
		self.m_info2bg:SetActive(false)
	end

	for i = 1, 6 do
		if ((i - self.m_index) % 2 == 0) then
			t = math.floor((i + 1) / 2)
			self.m_info[t].head_head:SetActive(false)
			self.m_info[t].name.text = LuaHelper.GetName(i)
		end
	end
		
	array = {}
	array[1] = self.m_transform:FindChild("LeftBg/ScrollRect/ToggleGroup/tank1").gameObject
	array[2] = self.m_transform:FindChild("LeftBg/ScrollRect/ToggleGroup/tank1").gameObject
	array[3] = self.m_transform:FindChild("LeftBg/ScrollRect/ToggleGroup/tank1").gameObject
	array[4] = self.m_transform:FindChild("LeftBg/ScrollRect/ToggleGroup/tank1").gameObject
	array[101] = self.m_transform:FindChild("LeftBg/ScrollRect/ToggleGroup/tank1/tank11").gameObject
	array[102] = self.m_transform:FindChild("LeftBg/ScrollRect/ToggleGroup/tank2/tank22").gameObject
	array[103] = self.m_transform:FindChild("LeftBg/ScrollRect/ToggleGroup/tank3/tank33").gameObject
	array[104] = self.m_transform:FindChild("LeftBg/ScrollRect/ToggleGroup/tank4/tank44").gameObject

	self.choose = -1  --可选择状态  0：不可选择则状态
	self.now = -1 --我的选择状态 -1未选择  1-n选择某坦克 100+(1-n)确认选择
	self.tank_demo = -1 -- -1没有demo 0有demo
	self.people = 0 --已选坦克人数
	self.record = {}
	self.pre = {}
	for i = 1,6 do
		self.record[i] = 0  --1表示已经确认准备
	end
	for i = 1,6 do
		self.pre[i] = 0  --记录之前状态
	end
	for i = 1,6 do
		self.pre[i] = 0  --记录之前状态
	end

	self.m_btn_confirm:GetComponent('Button').interactable = false
	self.m_lua_behaviour:AddClick(self.m_btn_confirm, function (obj)
		if (self.status ~= -1) then
			return
		end
        if (self.choose == -1 and (self.now == -1 or self.now > 100)) then
        	return
        end
        LuaHelper.Play_Sound(1,"sound/yinxiao/03");
        self.now = self.now + 100
        self.m_btn = self.m_btn_confirm:GetComponent('Button')
        self.m_btn.interactable = false 
        --send sure self.now
        LuaHelper.Lua_set("Send_Choose", self.now)
    end);

    for i = 1,4 do
    		-- self.m_lua_behaviour:AddToggleClick(array[i], function (obj, isOn)
    		-- 	if (self.choose == -1 and (self.now < 100 and self.now ~= i)) then
	    	-- 		if (self.now ~= i) then
	     --    			if (self.status ~= -1) then
						-- 	return
						-- end
	     --    			self.now = i
	     --    			self.m_btn_confirm:GetComponent('Button').interactable = true
	     --    			--send choose i
	     --    			LuaHelper.Lua_call("Choosing")
	     --    			LuaHelper.Lua_set("Send_Choose", self.now)
	        			
	     --    		end
      --   		end
      --   	end);
        	self.m_lua_behaviour:AddToggleClick(array[100 + i], function (obj, isOn)
        		if (self.choose == -1 and (self.now < 100 and self.now ~= i)) then
	        		if (self.now ~= i) then
	        			if (self.status ~= -1) then
							return
						end
	        			self.now = i
	        			self.m_btn_confirm:GetComponent('Button').interactable = true
	        			--send choose i
	        			LuaHelper.Lua_call("Choosing")
	        			LuaHelper.Lua_set("Send_Choose", self.now)
	        			
	        		end
	        	end
        	end);
    end
 	

	self.m_lua_behaviour:AddClick(self.m_change_skill1, function (obj)
		if (self.status == 1) then
			return
		end
		LuaHelper.Play_Sound(1,"sound/yinxiao/03");
		self.status = 1
		self.skillbg:SetActive(true)
    end);
    self.m_lua_behaviour:AddClick(self.m_change_skill2, function (obj)
		if (self.status == 2) then
			return
		end
		LuaHelper.Play_Sound(1,"sound/yinxiao/03");
		self.status = 2
		self.skillbg:SetActive(true)
    end);
    for i = 1,7 do
    	self.m_lua_behaviour:AddClick(self.m_btn_skill[i], function (obj)
    		LuaHelper.Play_Sound(1,"sound/yinxiao/03");

    		self.tankdcp.text = LuaHelper.GetSkillInfo("DESCRIPTION", i + 100)

			if (self.status == -1) then
				--body
			elseif (self.status == 1) then
				if (i ~= self.skill2) then
					self.m_skillBG[self.skill1]:SetActive(false)
					self.skill1 = i
					self.m_skillBG[self.skill1]:SetActive(true)
				end
				
			elseif (self.status == 2) then
				if (i ~= self.skill1) then
					self.m_skillBG[self.skill2]:SetActive(false)
					self.skill2 = i
					self.m_skillBG[self.skill2]:SetActive(true)
				end
				
			end
   		end);
    end

    self.m_lua_behaviour:AddClick(self.skillconfirm, function (obj)
		self.status = -1
		if (self.skill1 == self.confirm_skill2) then
			self.confirm_skill2 = self.confirm_skill1
			self.skill2 = self.confirm_skill1
			self.confirm_skill1 = self.skill1
		elseif (self.skill2 == self.confirm_skill1) then
			self.confirm_skill1 = self.confirm_skill2
			self.skill1 = self.confirm_skill2
			self.confirm_skill2 = self.skill2
		else
			self.confirm_skill1 = self.skill1
			self.confirm_skill2 = self.skill2
		end
		path = "Sprites/ch_sk" .. tostring(self.confirm_skill1)
		LuaHelper. LoadSprite(self.m_btn_skill1, path)
		path = "Sprites/ch_sk" .. tostring(self.confirm_skill2)
		LuaHelper. LoadSprite(self.m_btn_skill2, path)
		--send choose 2 skill
		self.skillbg:SetActive(false)
    end);

    self.skill_description_text =  self.m_transform:FindChild("CentBg/skill_description/descrip").gameObject:GetComponent('Text')
    self.skill_description =  self.m_transform:FindChild("CentBg/skill_description").gameObject
    self.m_lua_behaviour:AddTouchHandle(self.m_btn_skill1, function (obj,touch_type)
        if touch_type == 1 then
            self.IsDown1 = 1
            self.delay = Time.time
            self.skill_description_text.text = LuaHelper.GetSkillInfo("DESCRIPTION", self.confirm_skill1 + 100)
        else
            self.IsDown1 = 0
            self.skill_description:SetActive(false)
        end
    end);
    self.m_lua_behaviour:AddTouchHandle(self.m_btn_skill2, function (obj,touch_type)
        if touch_type == 1 then
            self.IsDown2 = 1
            self.delay = Time.time
            self.skill_description_text.text = LuaHelper.GetSkillInfo("DESCRIPTION", self.confirm_skill2 + 100)
        else
            self.IsDown2 = 0
            self.skill_description:SetActive(false)
        end
    end);
    self.m_lua_behaviour:AddTouchHandle(self.tankskill, function (obj,touch_type)
        if touch_type == 1 then
            self.IsDown0 = 1
            self.delay = Time.time
            self.skill_description_text.text = LuaHelper.GetSkillInfo("DESCRIPTION", self.confirm_tankskill + 100)
        else
            self.IsDown0 = 0
            self.skill_description:SetActive(false)
        end
    end);
end

function  cls_ui_choose:Update()
	self.time = Time.time - self.begin
	if(self.time >= 60) then
		self.time = 60
	end
	self.m_show_time.text = tostring(math.floor(60 - self.time))
	
	self.choose = LuaHelper.Lua_get("Choose_Status")

	--receive
	for i = 1, 6 do
		tmp = LuaHelper.Choose(i)

		if ((i - self.m_index) % 2 == 0 and self.pre[i] ~= tmp) then
			self.pre[i] = tmp
			t = math.floor((i + 1) / 2)
			if (tmp >= 1) then
				if (tmp > 100) then
					self.m_info[t].ok:SetActive(false)
					tmp = tmp - 100
				end
				self.m_info[t].head_none:SetActive(false)
				self.m_info[t].head_head:SetActive(true)
				path = "Sprites/tank" .. tostring(tmp)
				LuaHelper. LoadSprite(self.m_info[t].head_head, path)

				if (i == self.m_index) then
					--修改展现配置
					--LuaHelper. LoadSprite(self.tank, path)
					--LuaHelper.ChangeColor(self.tank.transform, 255,255,255,255)
					if (self.tank_demo ~= -1) then
						go = self.m_transform:FindChild("CentBg/tank_demo").gameObject
            			LuaHelper.DestroyObj(go)
					end
					self.tank_demo = 0
					LuaHelper.CreatePrefab(self.centBG, "Prefab/tank" .. tostring(tmp), "tank_demo")
					LuaHelper.ChangeColor(self.tankskill.transform, 255,255,255,255)
					Skill = tonumber(LuaHelper.GetTankInfo("SKILL", tmp * 100000 + 1))
					path = "Sprites/ch_sk" .. tostring(Skill - 100)
					self.confirm_tankskill = Skill - 100
					LuaHelper. LoadSprite(self.tankskill, path)
					self.tankname.text = LuaHelper.GetTankInfo("NAME", tmp * 100000 + 1)
				end

			end
			-- if (tmp > 100) then
			-- 	self.m_info[tmp - 100].ok:SetActive(false)
			-- end
		end
		if (self.record[i] == 0 and tmp > 100) then
			self.record[i] = 1
			self.people = self.people + 1
		end
	end

	if (self.time >= 60 and self.now < 100) then
		if (self.now == -1) then
			math.randomseed(os.time())
			self.now = math.random(4)
			if (self.now < 3) then
				self.now = self.now + 2
			end
		end
		self.m_btn = self.m_btn_confirm:GetComponent('Button')
        self.m_btn.interactable = false

        --send sure self.now
        LuaHelper.Lua_set("Send_Choose", self.now + 100)
        self.now = self.now + 100
	end

	if (LuaHelper.MAP_MATCH_result() == 20000) then

		--确认技能
		LuaHelper.SetSkill(3, self.confirm_tankskill)
		LuaHelper.SetSkill(1, self.confirm_skill1)
		LuaHelper.SetSkill(2, self.confirm_skill2)

		require "logic/battle_manager"
        battle_manager.StartBattle(1)

        SendUIMessage("ENUM_SHOW_LOADING")
        self:Close()
	end

	if (self.IsDown1 == 1) then
		if (Time.time - self.delay >= 0) then
			self.skill_description:SetActive(true)
		end
	else
		--
	end

	if (self.IsDown2 == 1) then
		if (Time.time - self.delay >= 0) then
			self.skill_description:SetActive(true)
		end
	else
		--
	end

	if (self.IsDown0 == 1) then
		if (Time.time - self.delay >= 0) then
			self.skill_description:SetActive(true)
		end
	else
		--
	end
end

function cls_ui_choose:OnDestroy()
    self.super.OnDestroy(self)
    l_instance = nil
end


function ShowChooseUI()
    l_instance = cls_ui_choose:new()
end

function  HideChooseUI()
    if l_instance ~= nil then
        l_instance:Close()
    end
end