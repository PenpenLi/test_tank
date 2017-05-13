//
// LuaConnector.cs
// Created by alexpeng on 2016/04/21 03:40:39
//

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using LuaInterface;
using UnityEngine;
using TKBase;

public partial class Connector
{

    public delegate void LuaNetError(string msg);

    byte[] luaMsgBytes;
    MessageID luaCmdSend;
    public NetworkResponseMethod luaOnFinish;
    public LuaNetError luaError;

    public void LuaSendMsg(byte[] msg, int cmd, LuaFunction onFinish, LuaFunction onError)
    {
        LuaInternalSendMsg(msg, cmd, (mg) =>
            {
                onFinish.Call(mg);
            },
            (err) =>
            {
                onError.Call(err);
            });
    }


    public bool LuaInternalSendMsg(byte[] msg, int cmd, NetworkResponseMethod onFinish, LuaNetError onError)
    {
        if (cmd == (int)this.luaCmdSend &&
            Mathf.Abs(Time.realtimeSinceStartup - this.sendTime) < 0.4f)
        {
            Debug.Log("Skip repeated message in short interval");
            return false;
        }
        this.luaMsgBytes = msg;
        this.luaCmdSend = (MessageID)cmd;
        this.luaOnFinish = onFinish;
        this.luaError = onError;
        LuaSend();
        return true;
    }


    public void LuaSend()
    {

        if (NetworkConfig.single) return;
        //if (luaMsgBytes == null || luaMsgBytes.Length <= 0) return;

        if (client == null || !client.Connected)
        {
            PromptDisconnect();
            return;
        }
        ClientHeader header = new ClientHeader();
        
        header.m_iUin = 0;
        header.m_cSHFlag = 0;
        header.m_nOptionLength = 0;
        //header.m_szOption = null;
        header.m_iMessageID = 0;
        header.m_nPlayerID = 0;
        header.m_nGroupID = 0;
        header.m_nPlatformID = 0;
        header.m_iSequenceID = 0;
        header.m_iPackageLength = 23+header.m_nOptionLength + luaMsgBytes.Length;

        byte[] packageBytes = null;
        using (MemoryStream stream = new MemoryStream())
        {
            BinaryWriter bw = new BinaryWriter(stream);

            bw.Write((int)IPAddress.HostToNetworkOrder((int)header.m_iPackageLength));
            bw.Write((short)IPAddress.HostToNetworkOrder((short)header.m_iUin));
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
            bw.Write(luaMsgBytes);

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
            if (client == null || !client.Connected)
            {
                PromptDisconnect();
                return;
            }
            BinaryWriter writer = new BinaryWriter(client.GetStream());
            writer.Write(packageBytes);
            writer.Flush();
            sendTime = Time.realtimeSinceStartup;
        }
        catch (System.Exception ex)
        {
            state = NetworkState.Disconnected;
            client.Close();
#if xue
                EventDispatcher.DispatchEvent("EventShowMessageUI", null, null);
#endif
            disconnectTime = DateTime.Now;
            LOG.Error(DateTime.Now + " Disconnect!" + "  exception: " + ex);
        }
    }


    private void LuaRcvMsg(MessageID cmd, NetworkMsg netmsg)
    {
        if (cmd == luaCmdSend + 1 && luaOnFinish != null)
        {
            luaOnFinish(netmsg);
        }
    }


    private void LuaCheckErr(MessageID cmd)
    {
        if (cmd == luaCmdSend + 1 && luaError != null)
        {
            luaError("net error" + cmd);
        }
    }

}
