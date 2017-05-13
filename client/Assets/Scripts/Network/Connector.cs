#define xue

// Network连接器
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

using UnityEngine;
using TKBase;
using TKGame;


public enum NetworkState
{
    None,
    Disconnected,
    ConnectFailed,
    ConnectSuccess,
}

public class ClientHeader
{
    public int m_iPackageLength;
    public int m_iUin;
    public byte m_cSHFlag;  //
    public short m_nOptionLength;        //更改为双字节长，方面以后扩展
    public byte[] m_szOption = new byte[128];  //随便填写的，之后和服务器的同步
    public short m_iMessageID;
    public short m_nPlayerID;
    public short m_nGroupID;
    public short m_nPlatformID;
    public int m_iSequenceID;			//默认不要参与头部的边界码
    //public ushort wdMagic;                    // 标记
    //public ushort wdLength;                   // 包长
    //public ushort wdCmdID;                    // 命令字
    //public uint         dwUserID;                   // 用户ID
    //public uint         dwDeviceID;                 // 设备ID
    //public char[]       szReserved = new char[4];   // 扩展预留
}

public enum CmdType : ushort
{
    MsgEcho = 1,
}

public class NetworkMsg
{
    public ClientHeader header;
    public byte[] data;
}

public partial class Connector
{

    public delegate void NetworkResponseMethod(NetworkMsg msg);

    Dictionary<MessageID, NetworkResponseMethod> ResponseMap = new Dictionary<MessageID, NetworkResponseMethod>();

    /************ c&d *************/
    public Connector() { }


    /************ Funcs **************/
    public void Connect(string ip, int port)
    {
        SetForceKickOff(false);

        //try
        //{
        LOG.Log("Time: " + System.DateTime.Now.Second.ToString() + " ip:" + ip + " port:" + port);

		/*
#if !(UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_WIN)
        bool ret = Security.PrefetchSocketPolicy(ip, port, 10000);
        LOG.Log("Time: " + System.DateTime.Now.Second.ToString() + " " + "Security.PrefetchSocketPolicy Ret: " + ret);
        if (ret == false)
        {
            LOG.Error("无法获取策略文件！");
            state = NetworkState.ConnectFailed;
            return;
        }
#else
#endif
*/

        try
        {
            String newServerIp = "";
            AddressFamily newAddressFamily = AddressFamily.InterNetwork;
            //cc  ipv6  getIPType(ip, port.ToString(), out newServerIp, out newAddressFamily);
            if (!string.IsNullOrEmpty(newServerIp)) { ip = newServerIp; }

            Debug.Log("Socket AddressFamily :" + newAddressFamily.ToString() + "ServerIp:" + ip);

            client = new TcpClient(newAddressFamily);

            client.Connect(ip, port);

            client.GetStream().BeginRead(readBuf, 0, READ_BUFFER_SIZE, new AsyncCallback(DoRead), null);

            state = NetworkState.ConnectSuccess;

            LOG.Log(state.ToString());
        }
        catch (Exception ex)
        {
            state = NetworkState.ConnectFailed;
            LOG.Log("Can not Connect!  " + ex + "  exception");
            //MessageBox.Instance.Show("服务器维护中");
        }
    }
    public NetworkState Disonnect()
    {
        if (client == null)
            return NetworkState.Disconnected;

        client.Close();
        LOG.Log(DateTime.Now + " Disconnect!");
        state = NetworkState.Disconnected;
        Network.NetworkMode = false;
        return NetworkState.Disconnected;
    }

    DateTime disconnectTime;
    private void DoRead(IAsyncResult ar)
    {
        int sizeRead;
        try
        {
            // Finish asynchronous read into readBuf and return number of bytes read.
            if (!client.Connected)
                return;

            sizeRead = client.GetStream().EndRead(ar);
            if (NetworkConfig.isNetworkEncrypt)
            {
                // 解密协议
                decryptRingBuf.Put(readBuf, 0, sizeRead);
                while (decryptRingBuf.Size > 4)
                {
                    // 加密包长度
                    byte[] lengthByte = new byte[2];
                    decryptRingBuf.CopyTo(2, lengthByte, 0, 2);
                    ushort length = NetworkToHostOrder(BitConverter.ToUInt16(lengthByte, 0));
                    if (decryptRingBuf.Size < length)
                    {
                        break;  // 不完整
                    }

                    // 提取加密块
                    decryptRingBuf.Get(4);
                    byte[] block = new byte[length - 4];
                    decryptRingBuf.Get(block);

                    // 解密并处理
                    ringBuf.Put(Decrypt(encryptKey, block));
                }
            }
            else
            {
                ringBuf.Put(readBuf, 0, sizeRead);
            }

            if (sizeRead < 1)
            {
                state = NetworkState.Disconnected;
                client.Close();

                disconnectTime = DateTime.Now;
                //LOG.Error(DateTime.Now + " Disconnect!" + "  sizeRead < 1");

                PromptDisconnect();

                return;
            }
            else
            {
                client.GetStream().BeginRead(readBuf, 0, READ_BUFFER_SIZE, new AsyncCallback(DoRead), null);
            }
        }
        catch (System.Exception ex)
        {
            state = NetworkState.Disconnected;
            client.Close();

            disconnectTime = DateTime.Now;
            //LOG.Error(DateTime.Now + " Disconnect!!" + "  exception: " + ex);
        }
    }

    public static uint NetworkToHostOrder(uint val)
    {
        byte[] array = BitConverter.GetBytes(val);
        Array.Reverse(array);
        return BitConverter.ToUInt32(array, 0);
    }
    public static int NetworkToHostOrder(int val)
    {
        byte[] array = BitConverter.GetBytes(val);
        Array.Reverse(array);
        return BitConverter.ToInt32(array, 0);
    }
    public static ushort NetworkToHostOrder(ushort val)
    {
        byte[] array = BitConverter.GetBytes(val);
        Array.Reverse(array);
        return BitConverter.ToUInt16(array, 0);
    }
    public static short NetworkToHostOrder(short val)
    {
        byte[] array = BitConverter.GetBytes(val);
        Array.Reverse(array);
        return BitConverter.ToInt16(array, 0);
    }



    public void SetAccount(string name)
    {
        accountName = name;

        uint id;
        if (uint.TryParse(accountName, out id))
        {
            uid = id;
        }
        else
        {
            uid = (uint)accountName.GetHashCode();
        }

        Debug.Log(uid);

    }

    public void SetDeviceID(string deviceID)
    {
        deviceIDHash = (uint)deviceID.GetHashCode();
    }

    /// <summary>
    /// 发送协议
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="cmd"></param>
    /// <param name="fCoolDown">连续2次协议的冷却时间，注意不要冷却的要设置为0</param>
    /// <returns></returns>
    public bool SendMsg(ClientHeader header, byte [] msgBytes, float fCoolDown = 0.4f)
    {
        return SendMsg(header, msgBytes, true, fCoolDown);
    }

    public class cLagMsg
    {
        public cLagMsg(System.Object msg, byte [] msgBytes, bool showLog)
        {
            this.msg = msg;
            //this.cmdSend = cmdSend;
            this.showLog = showLog;
        }
        public System.Object msg;
        public MessageID cmdSend = MessageID.CMD_INVALID;
        public bool showLog;
    }

    float sendTime = -1f;  // last successfully send time

    System.Object msg;
    MessageID cmdSend = MessageID.CMD_INVALID;
    bool showLog;
    List<cLagMsg> listMsg = new List<cLagMsg>();

    public bool SendMsg(ClientHeader header, byte [] msgBytes, bool showLog, float fCoolDown = 0.4f)
    {
#if cc
        if ((MessageID)header.m_iMessageID == this.cmdSend && Mathf.Abs(Time.realtimeSinceStartup - this.sendTime) < fCoolDown)
        {
            Debug.Log("Skip repeated message in short interval");
            return false;
        }
#endif 
        this.cmdSend = (MessageID)header.m_iMessageID;

        if (NetworkConfig.single)
        {
            return false;
        }

        if (client == null || !client.Connected)
        {
            PromptDisconnect();
            return false;
        }

        if (showLog)
        {
            Network.PrintSendMsgProperties(msg);
        }

        if (msgBytes == null)
            return false;

        byte[] packageBytes = null;
        using (MemoryStream stream = new MemoryStream())
        {
            BinaryWriter bw = new BinaryWriter(stream);

            bw.Write((int)IPAddress.HostToNetworkOrder((int)header.m_iPackageLength));
            bw.Write((int)IPAddress.HostToNetworkOrder((int)header.m_iUin));
            bw.Write((byte)IPAddress.HostToNetworkOrder((byte)header.m_cSHFlag));
            bw.Write((short)IPAddress.HostToNetworkOrder((short)header.m_nOptionLength));
            if (header.m_nOptionLength > 0)
            {
                bw.Write(header.m_szOption);
            }

            bw.Write((short)IPAddress.HostToNetworkOrder((short)header.m_iMessageID));
            bw.Write((short)IPAddress.HostToNetworkOrder((short)header.m_nPlayerID));
            bw.Write((short)IPAddress.HostToNetworkOrder((short)header.m_nGroupID));
            bw.Write((short)IPAddress.HostToNetworkOrder((short)header.m_nPlatformID));
            bw.Write((int)IPAddress.HostToNetworkOrder((int)header.m_iSequenceID));
            bw.Write(msgBytes);

            packageBytes = stream.ToArray();

        }

        if (NetworkConfig.isNetworkEncrypt)
        {
            byte[] key; ushort ksn;
            lock (encryptMutex)
            {
                key = encryptKey;
                ksn = encryptKSN;
            }
            // 加密协议 2字节加密块长度 + 加密正文
            packageBytes = Encrypt(key, packageBytes);
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    ushort magic = 0xBEAD;
                    bw.Write((ushort)IPAddress.HostToNetworkOrder((short)magic));
                    bw.Write((ushort)IPAddress.HostToNetworkOrder((short)(packageBytes.Length + 6)));
                    bw.Write((ushort)IPAddress.HostToNetworkOrder((short)ksn));
                    bw.Write(packageBytes);
                }
                packageBytes = ms.ToArray();
            }
        }

        try
        {
            // recheck disconnect
            if (client == null || !client.Connected)
            {
                PromptDisconnect();
                return false;
            }
            BinaryWriter writer = new BinaryWriter(client.GetStream());

            writer.Write(packageBytes);

            writer.Flush();

            string cmdstr2 = Enum.GetName(typeof(MessageID), header.m_iMessageID);
            //Debug.Log("xue:send success:--cmd" + cmdstr2 + "-----" + Network.xue);
            Network.xue++;

            sendTime = Time.realtimeSinceStartup;
        }
        catch (System.Exception ex)
        {
            state = NetworkState.Disconnected;
            client.Close();
            disconnectTime = DateTime.Now;
            //LOG.Error(DateTime.Now + " Disconnect!" + "  exception: " + ex);
        }
        return true;
    }

    public void Send(int _curIndex)
    {
        /* cc
        if (_curIndex < 0 || _curIndex > listMsg.Count - 1)
        {
            return;
        }

        msg = listMsg[_curIndex].msg;
        cmdSend = listMsg[_curIndex].cmdSend;
        showLog = listMsg[_curIndex].showLog;

        Send();
        **/
    }


    public delegate void OnMSGDeserialized(MessageID cmd);
    public OnMSGDeserialized onMSGDeserialized;
    GameObject GoTemp = null;
    public MessageID cmdRcv = MessageID.CMD_INVALID;
    //public ResultID err = ResultID.RET_SUCCESS;
    private void ScanMSGs()
    {
       // Debug.LogError("ScanMSGs()");
        while (ringBuf.Size != 0)
        {

            //Console.Log("ScanMSGs: " + sending);

            //Waiting.Instance.Hide();

            byte[] lengthByte = new byte[sizeof(int)];
            //ringBuf.CopyTo(0, lengthByste, 0, lengthByte.Length);
            //ushort wdMagic = BitConverter.ToUInt16(lengthByte, 0);

            ringBuf.CopyTo(0, lengthByte, 0, lengthByte.Length);
            
 
			int msgLength = BitConverter.ToInt32(lengthByte, 0);
			msgLength = NetworkToHostOrder(msgLength);

            if (ringBuf.Size >= msgLength && msgLength >= 6)
            {
                byte[] msgdata = new byte[msgLength];
                ringBuf.Get(msgdata);
                //int realLength = ringBuf.Get(msgdata);
                //Console.Assert(realLength == msgLength);

                using (MemoryStream stream = new MemoryStream(msgdata))
                {
                    BinaryReader br = new BinaryReader(stream);
                    ClientHeader header = new ClientHeader();

                    header.m_iPackageLength = NetworkToHostOrder(br.ReadInt32());
                    header.m_iUin = NetworkToHostOrder(br.ReadInt32());
                    header.m_iMessageID = NetworkToHostOrder(br.ReadInt16());
                    header.m_iSequenceID = NetworkToHostOrder(br.ReadInt32());

                    int body_len = msgLength - 14;
                    byte[] data = br.ReadBytes(body_len);

                    NetworkMsg msg = new NetworkMsg();
                    msg.header = header;
                    msg.data = data;


                    //msgList.Add(msg);

                    cmdRcv = (MessageID)msg.header.m_iMessageID;

                    string cmdstr = Enum.GetName(typeof(MessageID), cmdRcv);

                    if (string.IsNullOrEmpty(cmdstr))
                    {
                        LOG.Error("客户端版本错误！！ " + cmdRcv);
                        LuaCheckErr(cmdRcv);
                        return;
                    }

                    // 心跳包不显示
                    if (cmdRcv != MessageID.CMD_HEART_BEAT_RSP)
                    {
                        LOG.Log("Receive MSG: " + msg + "  " + cmdstr + "\n");
                    }


                    Type type = typeof(Network);

                    if (!ResponseMap.ContainsKey(cmdRcv))
                    {
                        MethodInfo mi = type.GetMethod(cmdstr);
                        if (mi == null)
                        {
                            LOG.Error("Can not find MSG RSP Function: " + cmdstr);
                            continue;
                        }

                        NetworkResponseMethod dele = Delegate.CreateDelegate(typeof(NetworkResponseMethod), null, mi) as NetworkResponseMethod;

                        ResponseMap.Add(cmdRcv, dele);
                    }

                    try
                    {
                        if (ResponseMap.ContainsKey(cmdRcv) && ResponseMap[cmdRcv] != null)
                        {
#if xue
#else
                         //   Debug.LogError("lua send: " + luaCmdSend +" val: "+(int)luaCmdSend+ " cmdRcv: " + cmdRcv+" val: "+(int)cmdRcv);
                            if ((int)cmdRcv == (int)luaCmdSend + 1)
                            {
                                LuaRcvMsg(cmdRcv, msg);
                            }
#endif
                            //if (!HotfixManager.Instance.TryFixNet(HotfixMode.BEFORE, (uint)cmdRcv, msg))
                            {
                                ResponseMap[cmdRcv](msg);
                                //HotfixManager.Instance.TryFixNet(HotfixMode.AFTER, (uint)cmdRcv, msg);
                            }
                        }
                        if (onMSGDeserialized != null)
                        {
                            onMSGDeserialized(cmdRcv);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        LOG.Error("RSP MSG ERR : " + ex);
                    }
                }
            }
            else
            {
                break;
            }
        }
    }

    float fCheckTimer = 0;
    public void Update()
    {
        ScanMSGs();

        if (state == NetworkState.Disconnected)
        {
            //MessageBox.Instance.Show("已经断开连接，请刷新页面重新链接！", OnDisconnect);
#if xue
            if (NetworkConfig.Click_Exit_Button == true)
            {
                NetworkConfig.Click_Exit_Button = false;
            }
            else
            {
                GameGOW.Get().BattleMgr.m_bIsInBattle = false;
                EventDispatcher.DispatchEvent("EventShowMessageUI", null, null);
            }
#endif
            state = NetworkState.None;
        }

        if (client != null && client.Connected)
        {
            if (Time.realtimeSinceStartup - lastBeatSendTime >= heartBeatInterval)
            {
                //Network.Send_Heart();  //发送心跳包
            }

            if (lastBeatRecvTimeOut != 0 && Time.realtimeSinceStartup >= lastBeatRecvTimeOut)
            {
                PromptDisconnect();
                return;
            }
        }

        fCheckTimer += Time.deltaTime;
        if (fCheckTimer > 3f)
        {
            fCheckTimer = 0f;

            //Debug.LogError("Check one time");
            if (client == null || !client.Connected)
            {
                PromptDisconnect();
                return;
            }
        }
    }

    /// <summary>
    /// 强迫立刻检测断线
    /// </summary>
    public void ForceCheckNetNow()
    {
        fCheckTimer = 3f;
    }

    //private void            OnDisconnect(System.Object param)
    //{
    //    GameStateManager.instance.ChangeState(GameStateType.Start);

    //    Network.Rest();

    //}

    void OnOKButClick(System.Object data)
    {
        //Network.Rest();

        Application.Quit();
    }

    // 标记成被强制下线，不做掉线提示
    public void SetForceKickOff(bool flag)
    {
        force_kick_off_flag = flag;
    }

    void PromptDisconnect()
    {
        if (!force_kick_off_flag)
        {
            // 断线重连界面
            //if ((!Reconnect.Instance.IsVisible) && (Network.loginRsp != null))
            //{
            //    Reconnect.Instance.Show();
            //}
        }
    }

    byte[] Encrypt(byte[] key, byte[] plainText)
    {
        // Check arguments. 
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException("plainText");

        // Create an Rijndael object 
        // with the specified key and IV. 
        using (Rijndael rijAlg = Rijndael.Create())
        {
            rijAlg.Key = key;
            rijAlg.IV = ENCRYPT_IV;
            //rijAlg.Mode = CipherMode.CBC;
            //rijAlg.Padding = PaddingMode.PKCS7;

            // Create a decrytor to perform the stream transform.
            ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

            // Create the streams used for encryption. 
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (BinaryWriter bw = new BinaryWriter(cs))
                    {
                        bw.Write(plainText);
                        if (++encryptPSN == 0)
                            ++encryptPSN;
                        bw.Write(BitConverter.GetBytes((ushort)IPAddress.HostToNetworkOrder((short)encryptPSN)));
                        ushort checksum = 0xDEED;
                        bw.Write(BitConverter.GetBytes((ushort)IPAddress.HostToNetworkOrder((short)checksum)));
                    }
                }
                return ms.ToArray();
            }
        }
    }

    byte[] Decrypt(byte[] key, byte[] cipherText)
    {
        // Check arguments. 
        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException("cipherText");

        // Create an Rijndael object 
        // with the specified key and IV. 
        using (Rijndael rijAlg = Rijndael.Create())
        {
            rijAlg.Key = key;
            rijAlg.IV = ENCRYPT_IV;
            //rijAlg.Mode = CipherMode.CBC;
            //rijAlg.Padding = PaddingMode.PKCS7;

            // Create a decrytor to perform the stream transform.
            ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

            // Create the streams used for decryption. 
            using (MemoryStream ms = new MemoryStream(cipherText))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (BinaryReader br = new BinaryReader(cs))
                    {
                        using (MemoryStream _ms = new MemoryStream())
                        {
                            byte[] buffer = new byte[READ_BUFFER_SIZE];
                            int count;
                            while ((count = br.Read(buffer, 0, buffer.Length)) > 0)
                                _ms.Write(buffer, 0, count);
                            byte[] plainText = _ms.ToArray();
                            // 解出密钥序号
                            ushort newKSN = NetworkToHostOrder(BitConverter.ToUInt16(plainText, plainText.Length - 8));
                            uint shortKey = BitConverter.ToUInt32(plainText, plainText.Length - 6);
                            // 解出短密钥并生成实际密钥
                            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                            byte[] newKey = md5.ComputeHash(BitConverter.GetBytes(shortKey));
                            // 返回有效载荷
                            byte[] returnText = new byte[plainText.Length - 8];
                            for (int i = 0; i < plainText.Length - 8; returnText[i] = plainText[i], ++i) ;

                            lock (encryptMutex)
                            {
                                encryptKey = newKey;
                                encryptKSN = newKSN;
                            }
                            return returnText;
                        }
                    }
                }
            }

        }
    }

    /************** Members ***************/
    private TcpClient client;

    private string accountName;
    private uint uid;
    private uint deviceIDHash;

    const int RING_BUFFER_SIZE = 65536;   // 64k
    private CircularBuffer<byte> ringBuf = new CircularBuffer<byte>(RING_BUFFER_SIZE, true);

    const int DECRYPT_RING_BUFFER_SIZE = 65536;   // 64k
    private CircularBuffer<byte> decryptRingBuf = new CircularBuffer<byte>(DECRYPT_RING_BUFFER_SIZE, true);

    const int READ_BUFFER_SIZE = 4096;    // 4k
    private byte[] readBuf = new byte[READ_BUFFER_SIZE];

    public List<NetworkMsg> msgList = new List<NetworkMsg>();

    public NetworkState state;

    public float heartBeatInterval = 10f;
    public float heartBeatTimeOut = 6f;
    public float lastBeatSendTime = 0.0f;
    public float lastBeatRecvTimeOut = 0.0f;

    private bool force_kick_off_flag = false;

    // 协议加解密
    class EncryptMutex {}
    private EncryptMutex encryptMutex = new EncryptMutex();

    private ushort encryptPSN = 0;
    private ushort encryptKSN = 0;
    private byte[] encryptKey = System.Text.Encoding.Default.GetBytes("s'SEln94bjr5l{DL");
    private static readonly byte[] ENCRYPT_IV = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

/*** cc code  ipv6
    [DllImport("__Internal")]
	private static extern string getIPv6(string mHost, string mPort);  

//"192.168.1.1&&ipv4"
  public static string GetIPv6(string mHost, string mPort)
  {
#if UNITY_IPHONE && !UNITY_EDITOR
		string mIPv6 = getIPv6(mHost, mPort);
		return mIPv6;
#else
    return mHost + "&&ipv4";
#endif
  }

    

void getIPType(String serverIp, String serverPorts, out String newServerIp, out AddressFamily  mIPType)
    {
       mIPType = AddressFamily.InterNetwork;
       newServerIp = serverIp;
      try
      {
        string mIPv6 = GetIPv6(serverIp, serverPorts);
        if (!string.IsNullOrEmpty(mIPv6))
        {
          string[] m_StrTemp = System.Text.RegularExpressions.Regex.Split(mIPv6, "&&");
          if (m_StrTemp != null && m_StrTemp.Length >= 2)
          {
            string IPType = m_StrTemp[1];
            if (IPType == "ipv6")
            {
              newServerIp = m_StrTemp[0];
              mIPType = AddressFamily.InterNetworkV6;
            }
          }
        }
      }
      catch (Exception e)
      {
        Debug.Log("GetIPv6 error:" + e);
      }
      
    }
    ****/
}