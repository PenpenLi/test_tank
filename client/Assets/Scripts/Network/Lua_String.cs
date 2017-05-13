#define xue

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
using TKBase;
using ProtoTest;
using UnityEngine;
using TKGame;

public partial class Network : Singleton<Network>
{
    public enum LuaGetStringID
    {

    }
    public delegate string Lua_GetString();
    public static Dictionary<LuaGetStringID, Lua_GetString> lua_getstring = new Dictionary<LuaGetStringID, Lua_GetString>();



    public enum LuaSetStringID
    {
        Lua_Find_Friend,
        Lua_Add_Friend,
        Lua_Del_Friend,
    }
    public delegate void Lua_SetString(string str);
    public static Dictionary<LuaSetStringID, Lua_SetString> lua_setstring = new Dictionary<LuaSetStringID, Lua_SetString>();

    public static void Lua_Del_Friend(string name)
    {
        Network.Send_Del_Friend(name);
    }

    public static void Lua_Add_Friend(string name)
    {
        Network.Send_Add_Friend(name);
    }

    public static void Lua_Find_Friend(string name)
    {
        Network.Send_Find_Friend(name);
    }
}