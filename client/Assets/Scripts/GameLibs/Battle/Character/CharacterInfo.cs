using UnityEngine;
using System.Collections;

namespace TKGame
{
    public class CharacterInfo
    {
        public bool m_bIsInRound;//是否在行动回合
        #region(战斗参数)
        public int m_iHP;
        public int m_iMaxHP;
        public float m_iFireAngle;//攻击角度
        public float m_iCurrentAngle;//当前角色角度
        public int m_iCurrentBombID;
        public bool m_bInBombSpeedUp;
        public int m_iBombSpeed;
        public int m_iTeam; //队伍
        public float m_icanattackmin;
        public float m_icanattackmax;
        #endregion(战斗参数)

        #region(角色动作)
        public CharacterStateData m_pCurrentStateData;
        public CharacterInstructionData m_pInstructionData;

        public CharacterStateType m_eCurrentStateType;
        public int m_iCurrentFrame;

        public int EmojiID;
        public int MessageID;
        #endregion(角色动作)

        public int m_iFacing = GameDefine.FACING_RIGHT;
        public int m_iDirectionKeys;
        public int m_iMoveEnergy;
        public int m_iAddEnergy;
        public int m_iMaxMoveEnergy;

        public bool m_bIsHidden;

        public void UnInitialize()
        {
            m_iHP = 0;
            m_iFireAngle = 0;
            m_iCurrentAngle = 0;

            m_pCurrentStateData = null;
            m_pInstructionData = null;
            m_eCurrentStateType = CharacterStateType.UNDEFINE;
            m_iCurrentFrame = 0;

            m_iFacing = GameDefine.FACING_RIGHT;
            m_iDirectionKeys = 0;
            m_iMoveEnergy = 0;
            m_iAddEnergy = 0;

            m_icanattackmin = 0;
            m_icanattackmax = 0;

            EmojiID = 0;
            MessageID = 0;
        }
    }
}
