using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net.Sockets;
using TKGame;
using TKBase;
using ProtoTest;

public partial class Network : Singleton<Network>
{
    enum ServerType : byte
    {
        innerNet = 1,
        outerNet = 2,
    };
    public static int m_iServerType;

    public delegate void OnLoginInSuccess(bool bReconnect = false);
    public static event OnLoginInSuccess onLoginSrvSuccess;

    public delegate void OnConnected(NetworkState state);
    public static event OnConnected onConnected;

    public static bool ConnectToServer(string ip, string port)
    {
        LOG.Log("ConnectToServer");

        if (connector != null)
        {
            if (connector.state == NetworkState.ConnectSuccess)
            {
                connector.Disonnect();
            }

            connector = null;
        }

        connector = new Connector();

        connector.Connect(ip, int.Parse(port));

        if (onConnected != null)
        {
            onConnected(connector.state);
        }

        if (connector.state == NetworkState.ConnectSuccess)
        {
            return true;
        }

        return false;
    }

    public bool ConnectToServer()
    {
        switch (m_iServerType)
        {
            case (int)ServerType.innerNet:
                return ConnectToServer("10.0.128.183", "8001");
            case (int)ServerType.outerNet:
                return ConnectToServer("42.62.22.60", "8001");
            default:
                return ConnectToServer("42.62.22.60", "8001");
        }
    }

    public void Disonnect()
    {
        if (connector != null)
        {
            connector.Disonnect();
        }
    }

    /**
	public bool SendMsg(System.Object msg, MessageID cmd, float fCoolDown = 0.4f) {
		if (connector != null)
		{
			return connector.SendMsg(msg, cmd);
		}
		return false;
	}

        **/

    public void Update()
    {
        if (connector != null)
        {
            connector.Update();
        }
        //        if (Input.GetKeyDown(KeyCode.T))
        //        {
        //            MessageBoxOK.Instance.Show("测试逻辑掉线", GameStateManager.Instance.OnButternChangeStateToAccountLoginState, null);
        //        }
#if UNITY_WEBPLAYER
       if (Input.GetKeyDown(KeyCode.C))
       {
           ResourceManager.Instance.UnloadUnUsedModels();
       }
#endif
    }

    void OnApplicationPause(bool pause)
    {
        if (connector == null)
            return;

        if (pause == true)
        {
            //KeepAliveReq();//协议
        }
        else
        {
            //HeartBeat();//协议
        }

    }

    /********* Members **********/
    public enum SGameState : int
    {
        Offline,
        Waiting,
        Login,
        LoginOK,
        LoginFail,
        CreateCharacter,
        CreateCharacterFail,
        CreateCharacterOK,
        JoiningScene,
        JoinSceneOK,
        LeavingScene,
        LeaveSceneOK,
        InScene,
        Success = 10000,
    }
    public static SGameState gameState = SGameState.Offline;
    public static Connector connector;

    public IEnumerator LagSend(int _curIndex)
    {
        yield return new WaitForSeconds(NetworkConfig.IsInstabilityNetwork ? Random.Range(0, NetworkConfig.netWorkLag) : NetworkConfig.netWorkLag);
        if (connector != null)
        {
            connector.Send(_curIndex);
        }
        yield return null;
    }


}
