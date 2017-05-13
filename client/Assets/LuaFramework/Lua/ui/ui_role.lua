  local cls_ui_role = class("cls_ui_role", cls_ui_base)
cls_ui_role.s_ui_panel = 'oms_test/PanelRole'
local l_instance = nill
function cls_ui_role:ctor()
	self.super.ctor(self)
end

function cls_ui_role:OnStart()
	self:EnableUpdate()
	self.m_transform = self.m_game_object.transform

	self.m_sex = self.m_transform:FindChild("Sex").gameObject
	--self.m_girl = self.m_transform:FindChild("Sex/GirlBG/Girl").gameObject
	self.m_girl_body = self.m_transform:FindChild("Sex/GirlBG/Girl").gameObject
	--self.m_boy = self.m_transform:FindChild("Sex/BoyBG/Boy").gameObject
	self.m_boy_body = self.m_transform:FindChild("Sex/BoyBG/Boy").gameObject
	self.m_enter1 = self.m_transform:FindChild("Sex/Enter").gameObject
	self.m_girl_guang1 = self.m_transform:FindChild("Sex/GirlBG/guang1").gameObject
	self.m_girl_guang2 = self.m_transform:FindChild("Sex/GirlBG/guang2").gameObject
	self.m_boy_guang1 = self.m_transform:FindChild("Sex/BoyBG/guang1").gameObject
	self.m_boy_guang2 = self.m_transform:FindChild("Sex/BoyBG/guang2").gameObject

	self.m_name = self.m_transform:FindChild("Name").gameObject
	self.m_dice = self.m_transform:FindChild("Name/Bg/Dice").gameObject
	self.m_input_name = self.m_transform:FindChild("Name/Bg/Input"):GetComponent('InputField')
	self.m_check = self.m_transform:FindChild("Name/Bg/Check").gameObject
	self.m_enter2 = self.m_transform:FindChild("Name/Enter2").gameObject

	self.m_boy_guang1:SetActive(false)
	self.m_boy_guang2:SetActive(false)
	self.m_girl_guang1:SetActive(false)
	self.m_girl_guang2:SetActive(false)
	LuaHelper.ChangeColor(self.m_boy_body.transform, 150, 150, 150, 255)
	LuaHelper.ChangeColor(self.m_girl_body.transform, 150, 150, 150, 255)
	self.now = 0
	self.m_enter1:GetComponent('Button').interactable = false
	self.m_name:SetActive(false)
	--0.尚未选择 1.选过男生 2.选过女生 
	self.m_lua_behaviour:AddClick(self.m_girl_body, function(obj)
		LuaHelper.Play_Sound(1,"sound/yinxiao/03");
		if (self.now == 2) then
			self.m_girl_body.transform.localScale = Vector3.New(1, 1, 1)
			self.m_girl_guang1:SetActive(false)
			self.m_girl_guang2:SetActive(false)
			LuaHelper.ChangeColor(self.m_girl_body.transform, 150, 150, 150, 255)
			self.now = 0
			self.m_enter1:GetComponent('Button').interactable = false
		elseif (self.now == 1) then
			self.m_boy_body.transform.localScale = Vector3.New(1, 1, 1)
			self.m_boy_guang1:SetActive(false)
			self.m_boy_guang2:SetActive(false)
			LuaHelper.ChangeColor(self.m_boy_body.transform, 150, 150, 150, 255)
			self.m_girl_body.transform.localScale = Vector3.New(1.2, 1.2, 1.2)
			self.m_girl_guang1:SetActive(true)
			self.m_girl_guang2:SetActive(true)
			LuaHelper.ChangeColor(self.m_girl_body.transform, 255, 255, 255, 255)
			self.now = 2
			self.m_enter1:GetComponent('Button').interactable = true
		else
			self.m_girl_body.transform.localScale = Vector3.New(1.2, 1.2, 1.2)
			self.m_girl_guang1:SetActive(true)
			self.m_girl_guang2:SetActive(true)
			LuaHelper.ChangeColor(self.m_girl_body.transform, 255, 255, 255, 255)
			self.now = 2
			self.m_enter1:GetComponent('Button').interactable = true
		end
	end)
	self.m_lua_behaviour:AddClick(self.m_boy_body, function(obj)
		LuaHelper.Play_Sound(1,"sound/yinxiao/03");
		if (self.now == 1) then
			self.m_boy_body.transform.localScale = Vector3.New(1, 1, 1)
			self.m_boy_guang1:SetActive(false)
			self.m_boy_guang2:SetActive(false)
			LuaHelper.ChangeColor(self.m_boy_body.transform, 150, 150, 150, 255)
			self.now = 0
			self.m_enter1:GetComponent('Button').interactable = false
		elseif (self.now == 2) then
			self.m_girl_body.transform.localScale = Vector3.New(1, 1, 1)
			self.m_girl_guang1:SetActive(false)
			self.m_girl_guang2:SetActive(false)
			LuaHelper.ChangeColor(self.m_girl_body.transform, 150, 150, 150, 255)
			self.m_boy_body.transform.localScale = Vector3.New(1.2, 1.2, 1.2)
			self.m_boy_guang1:SetActive(true)
			self.m_boy_guang2:SetActive(true)
			LuaHelper.ChangeColor(self.m_boy_body.transform, 255, 255, 255, 255)
			self.now = 1
			self.m_enter1:GetComponent('Button').interactable = true
		else 
			self.m_boy_body.transform.localScale = Vector3.New(1.2, 1.2, 1.2)
			self.m_boy_guang1:SetActive(true)
			self.m_boy_guang2:SetActive(true)
			LuaHelper.ChangeColor(self.m_boy_body.transform, 255, 255, 255, 255)
			self.now = 1
			self.m_enter1:GetComponent('Button').interactable = true
		end
	end)
	self.m_lua_behaviour:AddClick(self.m_enter1, function(obj)
		LuaHelper.Play_Sound(1,"sound/yinxiao/03");
		if (self.now == 2) then
			self.m_name:SetActive(true)
			self.m_sex:SetActive(false)
		elseif (self.now == 1) then
			self.m_name:SetActive(true)
			self.m_sex:SetActive(false)
		else 
			--body
		end
	end)

	math.randomseed(os.time())
	local name=LuaHelper.GetCharacterName("first_name")..LuaHelper.GetCharacterName("second_name")..LuaHelper.GetCharacterName("third_name")
	local name2 = LuaHelper.GetCharacterName("wechat")
	if name2 ~= nil then
		name = name2
	end
	self.m_input_name.text = name
	self.m_lua_behaviour:AddClick(self.m_dice, function(obj)
		--body
		LuaHelper.Play_Sound(1,"sound/yinxiao/03");
		math.randomseed(os.time())
		
		local name=LuaHelper.GetCharacterName("first_name")..LuaHelper.GetCharacterName("second_name")..LuaHelper.GetCharacterName("third_name")
	
		self.m_input_name.text = name
	end)

	self.m_lua_behaviour:AddClick(self.m_check, function(obj)
		LuaHelper.Play_Sound(1,"sound/yinxiao/03");
		if(self.xue ~= -1) then
			return
		end
		name = self.m_input_name.text
		if(name == "") then
			require "ui/ui_message"
        	ShowMessageUI("名字不允许为空")
        	return
        end

		local val = LuaHelper.check_Name(name)
		if(val == 0) then
			require "ui/ui_message"
        	ShowMessageUI("名字输入不合法")
        	return
        end
		LuaHelper.Send_Sex(self.now)

		LuaHelper.Lua_call("Send_Check")
		self.xue = -2
	end)

	self.m_lua_behaviour:AddClick(self.m_enter2, function(obj)
		LuaHelper.Play_Sound(1,"sound/yinxiao/03");
		if(self.xue ~= -1) then
			return
		end

		name = self.m_input_name.text
		if(name == "") then
			require "ui/ui_message"
            ShowMessageUI("名字不允许为空")
        	return
        end
		
		local val = LuaHelper.check_Name(name)
		if(val == 0) then
			require "ui/ui_message"
        	ShowMessageUI("名字输入不合法")
        	return
        end
		LuaHelper.Send_Sex(self.now)

		LuaHelper.Lua_call("Send_Create")
		self.xue = -2
	end)

	self.xue = -1  --  -1:初始值  -2: 发起创建角色  0：创建成功
				--	1411：查询失败   1412：名字可用   1413：更新失败  1421：已经有重名  
				--  1422：名字太长   1423：不能为空   1424：名字中有非法字符
end

function cls_ui_role:Update()
    local val = LuaHelper.Lua_get("Get_Role")          
    if val == 0 and self.xue == -2 then
        SendUIMessage("ENUM_SHOW_HOST")
        self:Close()
        self.xue = -1
        LuaHelper.Lua_call("Init_Role_Res")
        return
    elseif val == 1412 and self.xue == -2 then
    	require "ui/ui_message"
        ShowMessageUI("名字可用")
        self.xue = -1
        LuaHelper.Lua_call("Init_Role_Res")
        return
    elseif val == 1421 and self.xue == -2 then
    	require "ui/ui_message"
        ShowMessageUI("名字重复")
        self.xue = -1
        LuaHelper.Lua_call("Init_Role_Res")
        return
    elseif val == 1422 and self.xue == -2 then
    	require "ui/ui_message"
        ShowMessageUI("名字过长或过短")
        self.xue = -1
        LuaHelper.Lua_call("Init_Role_Res")
        return
	elseif val == 1423 and self.xue == -2 then
    	require "ui/ui_message"
        ShowMessageUI("不能为空")
        self.xue = -1
        LuaHelper.Lua_call("Init_Role_Res")
        return
    elseif val == 1424 and self.xue == -2 then
    	require "ui/ui_message"
        ShowMessageUI("名字中有非法字符")
        self.xue = -1
        LuaHelper.Lua_call("Init_Role_Res")
        return
	elseif self.xue == -2 and (val == 1411  or val == 1413) then
		require "ui/ui_message"
        --ShowMessageUI("输入不合法" .. val)
        ShowMessageUI("服务器异常")
        self.xue = -1
        LuaHelper.Lua_call("Init_Role_Res")
        return
    end
end

function cls_ui_role:OnDestroy()
	self.super.OnDestroy(self)
	l_instance = nil
end

function ShowRoleUI()
	l_instance = cls_ui_role:new()
end

function  HideRoleUI()
    if l_instance ~= nil then
        l_instance:Close()
    end
end