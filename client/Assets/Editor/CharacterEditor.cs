using UnityEngine;
using UnityEditor;
using System.Collections;
using TKGame;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(CharacterEditorData))]
public class CharacterEditor : Editor {

    enum AddState { NONE, NEWSTATE, FULLSTATE };
    public const string TAG = "[CharacterEditor]";
    public CharacterEditorData m_chaEditData = null;
    private AddState m_addState;
    public void OnEnable(){
        m_chaEditData = target as CharacterEditorData;
        if (m_chaEditData == null)
            return;
        m_addState = AddState.NONE;
    }

    public override void OnInspectorGUI()
    { 
        if (m_chaEditData == null)
        {
            PrintLog("the chaEditorData is null");
            return;
        }
        m_chaEditData.m_id = EditorGUILayout.IntField("PlayerID: ", m_chaEditData.m_id);
        m_chaEditData.m_resID = EditorGUILayout.IntField("PlayerResourceID:", m_chaEditData.m_resID);
        m_chaEditData.m_defaultName = EditorGUILayout.TextField("PlayerDefaultName: ", m_chaEditData.m_defaultName);
        m_chaEditData.m_scale = EditorGUILayout.FloatField("PlayerScale:", m_chaEditData.m_scale);
        m_chaEditData.m_walkSpeedX = EditorGUILayout.IntField("PlayerXSpeed: ", m_chaEditData.m_walkSpeedX);
        m_chaEditData.m_walkSpeedY = EditorGUILayout.IntField("PlayerYSpeed:", m_chaEditData.m_walkSpeedY);
        m_chaEditData.m_hatred = EditorGUILayout.FloatField("PlayerHatred:", m_chaEditData.m_hatred);
        m_chaEditData.m_lowFireAngle = EditorGUILayout.FloatField("PlayerLowFireAngle:", m_chaEditData.m_lowFireAngle);
        m_chaEditData.m_higFireAngle = EditorGUILayout.FloatField("PlayerHigFireAngle:", m_chaEditData.m_higFireAngle);
        m_chaEditData.m_fireRange = EditorGUILayout.IntField("PlayerFireRange:", m_chaEditData.m_fireRange);
        m_chaEditData.m_weaponPosition = EditorGUILayout.Vector2Field("PlayerWeaponPos", m_chaEditData.m_weaponPosition);
        m_chaEditData.m_beAttackBoxMinX = EditorGUILayout.IntField("PlayerBAtkBoxMinX:", m_chaEditData.m_beAttackBoxMinX);
        m_chaEditData.m_beAttackBoxMinY = EditorGUILayout.IntField("PlayerBAtkBoxMinY:", m_chaEditData.m_beAttackBoxMinY);
        m_chaEditData.m_beAttackBoxMaxX = EditorGUILayout.IntField("PlayerBAtkBoxMaxX:", m_chaEditData.m_beAttackBoxMaxX);
        m_chaEditData.m_beAttackBoxMaxY = EditorGUILayout.IntField("PlayerBAtkBoxMaxY:", m_chaEditData.m_beAttackBoxMaxY);
        if (GUILayout.Button("Add New State"))
        {
            if (m_chaEditData.IsAllStateExist())
                m_addState = AddState.FULLSTATE;
            else
                m_addState = AddState.NEWSTATE;
        }
        EditorGUILayout.Space();
        if (m_addState == AddState.FULLSTATE)
            EditorGUILayout.LabelField("all states is used");
        else if (m_addState == AddState.NEWSTATE)
        {
            CharacterStateType newestState = m_chaEditData.GetNewestState();
            m_chaEditData.AddNewState(newestState);
            m_addState = AddState.NONE;
        }

        EditorGUILayout.Space();
        ///Debug.Log("yes");
        for (int index = 0; index < m_chaEditData.m_lsStates.Count; index++)
        {
            CharacterEditorData.CharacterEditorStateData chaState = m_chaEditData.m_lsStates[index];
            //Debug.Log(EditorGUILayout.EnumPopup("state:", chaState.m_newState));
            CharacterStateType state = (CharacterStateType)EditorGUILayout.EnumPopup("state:", chaState.m_stateType);
            m_chaEditData.ChangeByState(chaState, state);
            chaState.m_animationName = EditorGUILayout.TextField("AnimationName:", chaState.m_animationName);
            int totFrame = EditorGUILayout.IntField("TotalFrame:", chaState.m_totFrame);
            if (totFrame != chaState.m_totFrame)
            {
                chaState.m_totFrame = totFrame;
                chaState.UpdateFramesSz();
            }
            if (chaState.m_stateType == CharacterStateType.ATTACK)
            {
                if (GUILayout.Button("Add Frame Data", GUILayout.MaxWidth(130), GUILayout.MaxHeight(20)))
                {
                    int newFrame = chaState.GetNewFrame();
                    //Debug.Log(newFrame);
                    if (newFrame != -1)
                    {
                        chaState.AddFrameData(newFrame);
                    }
                }
                EditorGUILayout.Space();
                for (int i = 0; i < chaState.m_attackDatas.Count; i++)
                {
                    CharacterEditorData.CharacterEditorAttackData frameData = chaState.m_attackDatas[i];
                    int frame = EditorGUILayout.IntField("Frame:", frameData.m_iFrame);
                    if (chaState.IsLegalFrame(frame))
                    {
                        //Debug.Log(frame);
                        frameData.m_iFrame = frame;
                    }
                    CharacterAttackType attackType = (CharacterAttackType)EditorGUILayout.EnumPopup("AttackType:", frameData.m_attackType);
                    chaState.ChangeFrameData(i , attackType);
                    EditorGUILayout.Space();
                    if (frameData.m_attackType == CharacterAttackType.BOMB)
                    {
                        CharacterEditorData.CharacterEditorBombAttackData bomb = (CharacterEditorData.CharacterEditorBombAttackData)frameData;
                        bomb.m_bombCofigID = EditorGUILayout.IntField("BombConfigID:", bomb.m_bombCofigID);
                        bomb.m_damage = EditorGUILayout.IntField("Damge:", bomb.m_damage);
                        bomb.m_centerDamage = EditorGUILayout.IntField("CenterDamage:", bomb.m_centerDamage);
                    }
                    if (GUILayout.Button("Remove this Frame"))
                    {
                        chaState.RemoveFrameData(frameData.m_iFrame);
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                }
            }
            if (GUILayout.Button("remove this state"))
            {
                m_chaEditData.RemoveOldState(index);
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }
        EditorUtility.SetDirty(m_chaEditData);
    }

    private void PrintLog(string str)
    {
        Debug.Log(TAG+" "+ str);
    }
}
