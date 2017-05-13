namespace TKGame
{
    public class GameConfig
    {

        public const string VERSION = "0.1";


        public const int MODE_DEBUG = 0;
        public const int MODE_GM = 1;
        public const int MODE_RELEASE = 2;

        public static bool NoView = false;

        public static int mode = MODE_RELEASE;

        /**
         * 白日时间（秒） 
         */
        public static int DayTime = 420;

        /**
         * 黑夜时间 （秒）
         */
        public static int NightTime = 180;

    }
}