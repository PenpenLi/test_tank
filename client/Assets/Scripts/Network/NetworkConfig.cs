
class NetworkConfig
{
    public static bool isNetworkEncrypt = false;  //是否加密
    public static bool single = false;

    private static bool isInstabilityNetwork = false;  //网络是否不稳定
    public static bool IsInstabilityNetwork
    {
        get
        {
#if (UNITY_ANDROID || UNITY_IPHONE)
            return false;
#else
            return isInstabilityNetwork;
#endif
        }
        set
        {
            isInstabilityNetwork = value;
        }
    }

    public static float netWorkLag = 0;

    public static bool IsDebugMode = false;

    public static int MoveSendCnt = 2;

    public static bool Click_Exit_Button = false;  //记录是否点击退出按钮
}

