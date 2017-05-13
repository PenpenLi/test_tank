using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.IO;

public class EditorMenus
{
    //[MenuItem("Assets/Game/SpiritState")]
    //static public void CreateSpiritState()
    //{
    //    EditorMenus.CreateAsset<SpiritState>("New SpiritState");
    //}
    //[MenuItem("Assets/Game/AttackParameter")]
    //static public void CreateAttackParameter()
    //{
    //    EditorMenus.CreateAsset<AttackParameter>("New AttackParameter");
    //}
    //[MenuItem("Assets/Game/SpiritBaseConfig")]
    //static public void CreateSpiritBaseConfig()
    //{
    //    EditorMenus.CreateAsset<SpiritBaseConfig>("New SpiritBaseConfig");
    //}
    //[MenuItem("Assets/Game/SpiritConfig")]
    //static public void CreateSpiritConfig()
    //{
    //    EditorMenus.CreateAsset<SpiritConfig>("New SpiritConfig");
    //}



    static private void CreateAsset<T>(String name) where T : ScriptableObject
    {
        var dir = "Assets/";
        var selected = Selection.activeObject;
        if (selected != null)
        {
            var assetDir = AssetDatabase.GetAssetPath(selected.GetInstanceID());
            if (assetDir.Length > 0 && Directory.Exists(assetDir))
            {
                dir = assetDir + "/";
            }
        }
        ScriptableObject asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, dir + name + ".asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    //[MenuItem("GameObject/Create Other/SpiritGroup")]
    //static public void CreateSpiritGroup()
    //{
    //    GameObject gameObject = new GameObject("New SpiritGroup", typeof(SpiritGroup));
    //    EditorUtility.FocusProjectWindow();
    //    Selection.activeObject = gameObject;
    //}
}

