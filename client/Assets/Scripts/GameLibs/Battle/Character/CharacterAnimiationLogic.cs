using System.Collections.Generic;


namespace TKGame
{
    class AnimationAction
    {
        public CharacterStateType m_eType;
        public bool m_bStopAtEnd;
        public bool m_bRepeat;
        public bool m_bReplaceSame;
        public AnimationAction(CharacterStateType eType, bool replaceSame, bool repeat, bool stopAtEnd)
        {
            m_eType = eType;
            m_bReplaceSame = replaceSame;
            m_bRepeat = repeat;
            m_bStopAtEnd = stopAtEnd;
        }

        virtual public bool CanReplace(AnimationAction action)
        {
            return action.m_eType != this.m_eType || m_bReplaceSame;
        }
    }

    public class CharacterAnimiationLogic
    {
        
        private static readonly Dictionary<CharacterStateType, AnimationAction> m_dicActions = new Dictionary<CharacterStateType, AnimationAction>()
        {
            { CharacterStateType.UNDEFINE, new AnimationAction(CharacterStateType.UNDEFINE,false,true,false)},
            { CharacterStateType.IDLE, new AnimationAction(CharacterStateType.IDLE,false,true,false)},
            { CharacterStateType.IDLE1, new AnimationAction(CharacterStateType.IDLE1,false,true,false)},
            { CharacterStateType.IDLE2, new AnimationAction(CharacterStateType.IDLE2,false,true,false)},
            { CharacterStateType.WALK, new AnimationAction(CharacterStateType.WALK,false,false,false)},
            { CharacterStateType.ATTACK, new AnimationAction(CharacterStateType.ATTACK,true,false,true)},
            { CharacterStateType.BEATTACK, new AnimationAction(CharacterStateType.BEATTACK,false,false,false)},
            { CharacterStateType.DEAD, new AnimationAction(CharacterStateType.DEAD,false,false,true)}
        };

        private bool m_bIsPlaying = false;
        private CharacterInfo m_pInfo;
        private AnimationAction m_pCurrentAction;
        private CharacterStateType m_pDefaultActionType;

        public CharacterAnimiationLogic(CharacterInfo info)
        {
            m_pInfo = info;
            m_pCurrentAction = m_dicActions[CharacterStateType.UNDEFINE];
            m_pDefaultActionType = CharacterStateType.IDLE;
        }

        public void UnInitialize()
        {
            m_pCurrentAction = m_dicActions[CharacterStateType.UNDEFINE];
            m_bIsPlaying = false;
        }
        
        public void DoAction(CharacterStateType eType)
        {
            AnimationAction pAction = m_dicActions[eType];
            if (m_pCurrentAction.CanReplace(pAction))
            {
                m_pCurrentAction = pAction;
                m_pInfo.m_pCurrentStateData = m_pInfo.m_pInstructionData.Actions[eType];
                m_pInfo.m_eCurrentStateType = eType;
                m_pInfo.m_iCurrentFrame = 0;
            }
            m_bIsPlaying = true;
        }

        public void Tick(uint tickCount)
        {
            if (m_bIsPlaying)
            {
                if (m_pInfo.m_iCurrentFrame < m_pInfo.m_pCurrentStateData.total_frame)
                {
                    m_pInfo.m_iCurrentFrame++;
                }
                else
                {
                    if (m_pCurrentAction.m_bRepeat)
                    {
                        m_pInfo.m_iCurrentFrame = 0;
                    }
                    else
                    {
                        if(m_pCurrentAction.m_bStopAtEnd)
                        {
                            m_bIsPlaying = false;
                        }
                        else
                        {
                            DoAction(m_pDefaultActionType);
                        }
                    }
                }
            }
        }
    }
}
