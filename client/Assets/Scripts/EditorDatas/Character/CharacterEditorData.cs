using UnityEngine;
using System.Collections;
using TKGame;
using System.Collections.Generic;
using System;

//人物编辑器数据结构
[ExecuteInEditMode]
public class CharacterEditorData : MonoBehaviour
{
    public int m_id = 100001;
    public int m_resID = 100001;
    public string m_defaultName;
    public string description;
    public float m_scale;

    public int m_walkSpeedX = 2;
    public int m_walkSpeedY = 8;
    public float m_hatred = 0; //仇恨值， 越高越吸引攻击
    public float m_lowFireAngle = 25;
    public float m_higFireAngle = 45;
    public int m_fireRange = 300;//射程

    public Vector2 m_weaponPosition;

    public int m_beAttackBoxMinX = 0;
    public int m_beAttackBoxMinY = 0;
    public int m_beAttackBoxMaxX = 0;
    public int m_beAttackBoxMaxY = 0;

    [System.Serializable]
    public class CharacterEditorAttackData
    {
        public CharacterAttackType m_attackType;
        public int m_iFrame;
        public static CharacterEditorAttackData CreateData(CharacterAttackType attackType) {
            switch (attackType){
                case CharacterAttackType.BOMB:
                    return new CharacterEditorBombAttackData();
                case CharacterAttackType.NORMAL:
                    return new CharacterEditorNormalAttackData();
            }
            return null;
        }

    }

    [System.Serializable]
    public class CharacterEditorBombAttackData : CharacterEditorAttackData
    {
        public int m_bombCofigID;
        public int m_damage;
        public int m_centerDamage;
    }

    [System.Serializable]
    public class CharacterEditorNormalAttackData : CharacterEditorAttackData
    {
        public int m_iDamage;
    }

    [System.Serializable]
    public class CharacterEditorStateData : ISerializationCallbackReceiver
    {
        public string m_animationName;
        public int m_totFrame;
        public CharacterStateType m_stateType;
        [HideInInspector]
        public CharacterStateType m_oldStateType;
        [HideInInspector][NonSerialized]
        public List<CharacterEditorAttackData> m_attackDatas = new List<CharacterEditorAttackData>();
        [HideInInspector][SerializeField]
        public List<CharacterEditorBombAttackData> m_attackBmDatas = new List<CharacterEditorBombAttackData>();
        [HideInInspector][SerializeField]
        public List<CharacterEditorNormalAttackData> m_attackNmDatas = new List<CharacterEditorNormalAttackData>();
        public CharacterEditorAttackData IsFrameDataExist(int frame)
        {
            foreach (CharacterEditorAttackData dt in m_attackDatas)
            {
                if (frame == dt.m_iFrame)
                    return dt;
            }
            return null;
        }

        public bool AddFrameData(int newFrame)
        {
            CharacterEditorAttackData dt = CharacterEditorAttackData.CreateData(CharacterAttackType.BOMB);
            if (dt == null)
                return false;
            dt.m_iFrame = newFrame;
            dt.m_attackType = CharacterAttackType.BOMB;
            this.m_attackDatas.Add(dt);
            return true;
        }

        public bool RemoveFrameData(int oldFrame)
        {
            CharacterEditorAttackData dt = this.IsFrameDataExist(oldFrame);
            if (dt == null)
                return false;
            this.m_attackDatas.Remove(dt);
            return true;
        }

        public void ChangeFrameData(int index , CharacterAttackType attType)
        {
            CharacterEditorAttackData dt = this.m_attackDatas[index];
            int iFrame = dt.m_iFrame;
            if (attType != dt.m_attackType)
            {
                dt = CharacterEditorAttackData.CreateData(attType);
                dt.m_iFrame = iFrame;
                dt.m_attackType = attType;
                this.m_attackDatas[index] = dt;
            }
        }

        public int GetNewFrame()
        {
            if (m_attackDatas.Count == 0)
                return 0;
            int frame = -1;
            foreach (CharacterEditorAttackData dt in m_attackDatas)
            {
                if (frame <= dt.m_iFrame)
                    frame = dt.m_iFrame;
            }
            if (frame == this.m_totFrame)
                return -1;
            return frame + 1;
        }

        public bool IsLegalFrame(int frame)
        {
            if (IsFrameDataExist(frame) != null || frame < 0 || frame > m_totFrame)
                return false;
            return true;
        }

        public bool UpdateFramesSz()
        {
            int count = m_attackDatas.Count;
            for (int i = count - 1; i > m_totFrame - 1; i--)
            {
                this.m_attackDatas.RemoveAt(i);
            }
            return true;
        }

        public void Init(CharacterStateType state)
        {
            m_animationName = "11111";
            m_totFrame = 12;
            m_stateType = state;
            m_oldStateType = state;
            m_attackDatas.Clear();
            m_attackBmDatas.Clear();
            m_attackNmDatas.Clear();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (m_attackBmDatas == null || m_attackNmDatas == null || m_attackDatas == null)
                return;
            m_attackBmDatas.Clear();
            m_attackNmDatas.Clear();
            foreach(CharacterEditorAttackData item in m_attackDatas)
            {
                switch(item.m_attackType)
                {
                    case CharacterAttackType.BOMB:m_attackBmDatas.Add((CharacterEditorBombAttackData)item); break;
                    case CharacterAttackType.NORMAL: m_attackNmDatas.Add((CharacterEditorNormalAttackData)item);break;
                }
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (m_attackBmDatas == null || m_attackNmDatas == null || m_attackDatas == null)
                return;
            m_attackDatas.Clear();
            foreach (CharacterEditorAttackData item in m_attackBmDatas)
            {
                m_attackDatas.Add(item);
            }
            foreach (CharacterEditorAttackData item in m_attackNmDatas)
            {
                m_attackDatas.Add(item);
            }
        }
    }

    [SerializeField]
    public List<CharacterEditorStateData> m_lsStates = new List<CharacterEditorStateData>();

    public void AddNewState(CharacterStateType state)
    {
        if (IsStateExist(state))
            return;
        CharacterEditorStateData dt = new CharacterEditorStateData();
        dt.Init(state);
        m_lsStates.Add(dt);
    }

    public void RemoveOldState(int index)
    {
        if (m_lsStates.Count <= index)
            return;
        m_lsStates.RemoveAt(index);
    }

    public bool IsStateExist(CharacterStateType stateType)
    {
        foreach (CharacterEditorStateData stateData in m_lsStates)
        {
            if (stateData.m_stateType == stateType)
                return true;
        }
        return false;
    }

    public int GetIndexByState(CharacterStateType stateType)
    {
        for (int index = 0; index < m_lsStates.Count; ++index)
        {
            if (m_lsStates[index].m_oldStateType == stateType)
                return index;
        }
        return -1; 
    }

    public bool IsAllStateExist()
    {
        foreach (CharacterStateType item in Enum.GetValues(typeof(CharacterStateType)))
        {
            if (item != CharacterStateType.UNDEFINE && item != CharacterStateType.ENTER_SCENE && !IsStateExist(item))
                return false;
        }
        return true;
    }

    public CharacterStateType GetNewestState()
    {
        foreach (CharacterStateType item in Enum.GetValues(typeof(CharacterStateType)))
        {
            if (item != CharacterStateType.UNDEFINE && item != CharacterStateType.ENTER_SCENE && !IsStateExist(item))
                return item;
        }
        return CharacterStateType.UNDEFINE;
    }

    public CharacterStateType ChangeByState(CharacterEditorStateData dt, CharacterStateType state)
    {
        if (IsStateExist(state) || state == CharacterStateType.ENTER_SCENE || state == CharacterStateType.UNDEFINE)
        {
            //Debug.Log("error state or exist state");
            return dt.m_oldStateType;
        }
        dt.Init(state);
        return state;
    }

    public CharacterEditorStateData GetStateDataByIndex(int index){
        return m_lsStates[index];
    }

    void Update()
    {

    }
}
