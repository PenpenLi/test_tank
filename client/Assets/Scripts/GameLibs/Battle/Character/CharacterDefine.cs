using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TKGame
{
    public class CharacterDefine
    {
        public const int STEP_X = 1;
        public const int STEP_Y = 3;
        public const float ANGLE_STEP = 0.4f;
    }

    public enum CharacterStateType
    {
        UNDEFINE,
        ENTER_SCENE,
        IDLE,
        IDLE1,
        IDLE2,
        WALK,
        RUN,
        ATTACK,
        BEATTACK,
        DEAD,
    }

    public enum CharacterAttackType
    {
        NONE = 0,
        BOMB = 1,
        NORMAL = 2,

    }

}