using UnityEngine;
namespace TKGame
{
    public class GameDefine
    {
        public const int INFINITE = 0x7FFFFFFF;//infinite
        public static Quaternion LeftRotation = Quaternion.Euler(0, 180, 0);
        public static Quaternion RightRotation = Quaternion.identity;
        public const int FACING_LEFT = -1;
        public const int FACING_RIGHT = 1;
        public const float FRAME_TIME_INTERVAL = 0.0166f;
    }

    public enum Team
    {
        NORMAL_ENEMY = 0,
        PLAYER = 1
    }
}