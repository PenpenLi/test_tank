using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public enum MessageID
{
    CMD_INVALID,
    
    CMD_LOGIN = 0x2001,
    CMD_ENTERROOM = 0x2101,
    CMD_HEART_BEAT_RSP = 0x2105, //心跳包
    CMD_MAP_MATCH = 0x2106,   //随机地图匹配
    CMD_ROLE = 0x2107,          //角色信息
    CMD_SITDOWN = 0x2201,    //进入table并已经坐下
    CMD_PLAYGAME = 0x2205,  // 双向,GameData
    CMD_PUSHGAMEDATA = 0x2207,  // S->C 服务器向客户端推送游戏数据
    CMD_LOGIC_FRIEND = 0x2108,   //双向  好友系统通信协议

    MSG_FRIEND_ADD = 0x2111,
    MSG_FRIEND_DEL = 0x2112,
    MSG_FRIEND_ARRAY = 0x2113,
    MSG_FRIEND_ADD_NOTIFY = 0x2114,
    MSG_FRIEND_DEL_NOTIFY = 0x2117,

    MSG_FRIEND_OPT_ADD = 0x2115,
    MSG_FRIEND_GETINFO = 0x2116,

    MSG_FRIEND_STATUS =  0x2118,   //上线 离线更新

    MSG_FRIEND_INVITATION = 0x2121,
    MSG_FRIEND_INV_NOTIFY = 0x2122,
    MSG_FRIEND_OPT_INV = 0x2123,

    MSG_PVE_OVER = 0x2130,   //人机结束
}

public enum ResultID
{
    result_id_success = 0,
    result_id_not_free = 1400,    //不是空闲状态    (匹配中或游戏中, 不能开始匹配，点击匹配按钮没反应)
    result_id_not_merging = 1401,     //不在匹配状态   (取消匹配失败，客户端不用作出任何反应）
    result_id_start_merging = 1402,     //开始匹配      （弹出"正在匹配"，附带取消按钮）
    result_id_cacel_ok = 1403,     //取消匹配成功   ("正在匹配"消失)
    result_id_not_at_room = 1404,     //玩家不在房间   (不能开始匹配，点击匹配按钮没反应)
    result_id_not_found_player = 1405,     //匹配队列中未找到当前玩家 (取消匹配失败，客户端不用作出任何反应）
    result_id_matching_full = 1406,     //匹配队列已经满了   (不能开始匹配，点击匹配按钮没反应)
    result_id_not_online = 1407,


    //DB
    result_id_query_fail = 1411,     //查询失败
    result_id_query_null = 1412,     //查询结果为空
    result_id_update_fail = 1413,     //更新失败

    //创角相关
    result_id_have_same_name = 1421,     //已经有相同的名字
    result_id_too_long = 1422,     //名字太长
    result_id_name_can_not_null = 1423,     //名字不能为空
    result_id_illegal_char = 1424,     //名字中含有非法字符

    result_id_at_playing = 1432,     //想加的人正在游戏中


    result_enter_game = 10000,
    result_choose_done = 20000,
}

public enum GameID
{
    Ready = 0,
    Ready_Notify = 1,
    Choose,
    Choose_Notify,
    Load,

    Attach,
    Boom,
    Move,
    
    Round,
    Die,
    Quit,
    Skip = 11,
    Disconnect = 12,
    Chat = 13,
    Damage = 14,

    Over = 15,
}

public enum ControlID
{
    Up_I = 0x1,
    Down_I = 0x2,
    Left_I = 0x4,
    Right_I = 0x8,
    Attach_I = 0x11,
    Attach_O = 0x11,
}

public enum GameResID
{
    game_result_win = 0,   // 胜利
    game_result_fail = 1,   // 失败
    game_result_tied = 2,   // 平局
    game_result_exception = 3,   //异常
}

public enum FriendID
{
    Add = 1,
    Del = 2,
    List = 3,
    Add_notify = 4,
    Res_addnotify = 5,
    Get_Info = 6,
    Del_notify = 7,
}
