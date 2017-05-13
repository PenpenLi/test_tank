using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;
#endif
using System.IO;

public static class XCodePostProcess
{

#if UNITY_EDITOR
	[PostProcessBuild(999)]
	public static void OnPostProcessBuild( BuildTarget target, string pathToBuiltProject )
	{
#if UNITY_5
		if (target != BuildTarget.iOS) {
#else
        if (target != BuildTarget.iPhone) {
#endif
			Debug.LogWarning("Target is not iPhone. XCodePostProcess will not run");
			return;
		}

		// Create a new project object from build target
		XCProject project = new XCProject( pathToBuiltProject );

		// Find and run through all projmods files to patch the project.
		// Please pay attention that ALL projmods files in your project folder will be excuted!
		string path = Path.GetFullPath(pathToBuiltProject);
		File.Copy(Application.dataPath + "/../../tools/ci/WeChatSDK.a", path + "/Classes/WeChatSDK.a", true);
		string[] files = Directory.GetFiles( Application.dataPath, "*.projmods", SearchOption.AllDirectories );
		foreach( string file in files ) {
			UnityEngine.Debug.Log("ProjMod File: "+file);
			project.ApplyMod( file );
		}

		//TODO disable the bitcode for iOS 9
		project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "Release");
		project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "Debug");

		project.overwriteBuildSetting("GCC_OPTIMIZATION_LEVEL", "0", "Release");

		//TODO implement generic settings as a module option
//		project.overwriteBuildSetting("CODE_SIGN_IDENTITY[sdk=iphoneos*]", "iPhone Distribution", "Release");


		EditPlist(path);
		//EditFiles(path);
		File.Copy(Application.dataPath + "/../../tools/ci/UnityAppController.mm", path + "/Classes/UnityAppController.mm", true);
		File.Copy(Application.dataPath + "/../../tools/ci/UnityAppController.h", path + "/Classes/UnityAppController.h", true);

        //project.overwriteBuildSetting("URL_TYPE", "");
        // Finally save the xcode project
        project.Save();

	}
#endif

	private static void EditPlist(string filePath)
	{
		string PlistAdd = @"
	<key>CFBundleURLTypes</key>
	<array>
		<dict>
			<key>CFBundleTypeRole</key>
			<string>Editor</string>
			<key>CFBundleURLName</key>
			<string>weixin</string>
			<key>CFBundleURLSchemes</key>
			<array>
				<string>wx1d4d05028c3c4910</string>
			</array>
		</dict>
		<dict>
			<key>CFBundleTypeRole</key>
			<string>Editor</string>
			<key>CFBundleURLName</key>
			<string>com.joyyou.tank</string>
			<key>CFBundleURLSchemes</key>
			<array>
				<string>tank</string>
			</array>
		</dict>
	</array>
    <key>LSApplicationQueriesSchemes</key>
    <array>
        <string>weixin</string>
        <string>wechat</string>
    </array>
	";
		XClass list = new XClass(filePath + "/Info.plist");
		list.WriteBelow ("<dict>", PlistAdd);
	}


	public static void Log(string message)
	{
		UnityEngine.Debug.Log("PostProcess: "+message);
	}
}
