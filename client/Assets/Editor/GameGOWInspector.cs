using UnityEngine;
using System.Collections;
using UnityEditor;
using TKGame;

[CustomEditor(typeof(GameGOW))]
public class GameGOWInspector : Editor {
    void OnEnable()
    {
    }

    protected virtual void gui()
    {
        GameGOW compoment = (GameGOW)target;

        EditorGUI.BeginChangeCheck();

		EditorGUILayout.Space ();

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        gui();
    }
}
