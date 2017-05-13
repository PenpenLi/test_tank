
local cls_ui_gm = class("cls_ui_gm",cls_ui_base)
cls_ui_gm.s_ui_panel = 'UI/gm_ui/PanelGM'
cls_ui_gm.s_ui_order = 10000
local l_instance = nil

function cls_ui_gm:ctor(...)
    self.super.ctor(self)
end

function cls_ui_gm:OnStart()
    self.m_transform = self.m_game_object.transform

    self.m_btnClose = self.m_transform:FindChild("ButtonClose").gameObject;
    self.m_lua_behaviour:AddClick(self.m_btnClose, function (obj)
        self:Close()
    end);

    self.m_btnCopyLogs = self.m_transform:FindChild("ButtonCopyLogs").gameObject;
    self.m_lua_behaviour:AddClick(self.m_btnCopyLogs, function (obj)
        local logPath = LuaHelper.GetLogPath()
        local targetPath = "/mnt/sdcard/JYTank"

        local function execute(cmd)
            local ret_code = os.execute(cmd)
            if ret_code ~= 0 then
                LuaHelper.Log("cmd fail, code:"..ret_code.."\n"..cmd)
            end
            return ret_code
        end

        execute("mkdir -p "..targetPath)

        if not logPath then
            LuaHelper.Log("GetLogPath nil")
            return
        end

        local ret_code = execute("cp -rf "..logPath .. " ".. targetPath)
        if ret_code == 0 then
            LuaHelper.Log("copy file success\n dst:"..targetPath)
        end
        -- LuaHelper.CopyLogFilesToSDCard()
    end);

end



function cls_ui_gm:OnDestroy()
    self.super.OnDestroy(self)
    l_instance = nil
end

function ShowGMPanel()
    l_instance = cls_ui_gm:new()
end
