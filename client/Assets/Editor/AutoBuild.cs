using UnityEditor;
using UnityEngine;

class AutoBuild
{
    public static string m_strKeyStore = "Assets/Plugins/Android/tank.keystore";
    public static string m_strKeyAlias = "android.keystore";
    public static string m_strKeyStorePass = "joyyou";
    public static string m_strKeyAliasPass = "joyyou";
    private static string m_strExportPath = "";
	//解析unity3d的build选项
	//例如: build_out_path-$SCRIPT_DIR/../../game/xcode
	private static void AnalysisParameters ()
	{
		foreach (string arg in System.Environment.GetCommandLineArgs()) {
			if (arg.StartsWith ("build_out_path")) {
				string[] args = arg.Split ('=');
				if (args.Length > 1) {
					m_strExportPath = args [1];
				}
				return;
			}
		}
		m_strExportPath = "";
	}

	static void BuildIOS ()
	{
        ToLuaMenu.CopyLuaFilesToRes();
        SetKeystore();

        AnalysisParameters ();
		if (m_strExportPath == "")
			m_strExportPath = "xcode";
		BuildPipeline.BuildPlayer (new[] { "Assets/tank_demo.unity" }, m_strExportPath, BuildTarget.iOS, BuildOptions.None);
	}

	static void BuildAndroid()
	{
        ToLuaMenu.CopyLuaFilesToRes();
        SetKeystore();

        AnalysisParameters();
		if (m_strExportPath == "")
			m_strExportPath = "x.apk";
        BuildPipeline.BuildPlayer(new[] { "Assets/tank_demo.unity" }, m_strExportPath, BuildTarget.Android, BuildOptions.None);
    }

    [MenuItem("坦克" + "/填写设置" + "/安卓keystore", false, 10001)]
    private static void SetKeystore()
    {
        PlayerSettings.Android.keystoreName = m_strKeyStore;
        PlayerSettings.Android.keyaliasName = m_strKeyAlias;
        PlayerSettings.keystorePass = m_strKeyStorePass;
        PlayerSettings.keyaliasPass = m_strKeyAliasPass;
    }


    [MenuItem("CustomBuild/Build Windows 64位")]
    static void PerformAndroidWindows64()
    {
        ToLuaMenu.CopyLuaFilesToRes();
        m_strExportPath = Application.dataPath + "/../../release/win64/demo.exe";
        BuildPipeline.BuildPlayer(new[] { "Assets/tank_demo.unity" }, m_strExportPath, BuildTarget.StandaloneWindows64, BuildOptions.None);
    }

    [MenuItem("CustomBuild/Build Windows 32位")]
    static void PerformAndroidWindows32()
    {
        ToLuaMenu.CopyLuaFilesToRes();
        m_strExportPath = Application.dataPath + "/../../release/win32/demo.exe";
        BuildPipeline.BuildPlayer(new[] { "Assets/tank_demo.unity" }, m_strExportPath, BuildTarget.StandaloneWindows, BuildOptions.None);
    }

    [MenuItem("CustomBuild/Build Android")]
    static void PerformAndroidBuild()
    {
        ToLuaMenu.CopyLuaFilesToRes();
        m_strExportPath = Application.dataPath + "/../../release/demo.apk";
        BuildPipeline.BuildPlayer(new[] { "Assets/tank_demo.unity" }, m_strExportPath, BuildTarget.Android, BuildOptions.None);
    }
}