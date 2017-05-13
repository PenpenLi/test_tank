#define xue

using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using LuaInterface;
using System;
using TKGame;
using TKBase;
using TKGameView;
using DG.Tweening;
using UnityEngine.UI;
using Spine.Unity;
using Object = System.Object;

namespace LuaFramework {
	public static class LuaHelper {

		/// <summary>
		/// getType
		/// </summary>
		/// <param name="classname"></param>
		/// <returns></returns>
		public static System.Type GetType(string classname) {
			Assembly assb = Assembly.GetExecutingAssembly();  //.GetExecutingAssembly();
			System.Type t = null;
			t = assb.GetType(classname); ;
			if (t == null) {
				t = assb.GetType(classname);
			}
			return t;
		}

		/// <summary>
		/// 面板管理器
		/// </summary>
		public static PanelManager GetPanelManager() {
			return AppFacade.GetManager<PanelManager>();
		}

		/// <summary>
		/// 资源管理器
		/// </summary>
		public static ResourceManager GetResManager() {
			return AppFacade.GetManager<ResourceManager>();
		}

		public static void InputDirect(int type) {
			if (GameGOW.Get().BattleMgr.m_bIsInBattle == false) {
				return;
			}
			if (Network.gamemode == BattleType.AI_1v1) {
				int index = GameGOW.Get().BattleMgr.m_iCurrentRoundPlayerIndex;
				int id = GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[index].m_iPlayerUID;
				if (Network.playerid == id && Network.Moving == false && GameGOW.Get().BattleMgr.m_pCurrentPlayer.IsLiving == true) {
					if (Network.PreMove != type) {
						Network.MoveCnt = 0;
						Network.PreMove = type;
						if (type != 0) {
							var m_pCharacterLogic = GameGOW.Get().BattleMgr.m_pCurrentPlayer;
							m_pCharacterLogic.OnDirectionKeyChanged(type);
						} else {
							var m_pCharacterLogic = GameGOW.Get().BattleMgr.m_pCurrentPlayer;
							m_pCharacterLogic.OnDirectionKeyChanged(type);
						}

					}
				}
			} else if (Network.NetworkMode == true) {
				int index = GameGOW.Get().BattleMgr.m_iCurrentRoundPlayerIndex;
				int id = GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[index].m_iPlayerUID;
				if (Network.playerid == id && Network.Moving == false && GameGOW.Get().BattleMgr.m_pCurrentPlayer.IsLiving == true) {
					if (type != 0 && Network.PreMove == type) {
						//Network.MoveCnt = (Network.MoveCnt+1) % NetworkConfig.MoveSendCnt;
						//if(Network.MoveCnt == 0)
						//{
						//    var m_pCharacterLogic = GameGOW.Get().BattleMgr.m_pCurrentPlayer;
						//    Network.Send_Move(true, type, 1, m_pCharacterLogic.Position.x, m_pCharacterLogic.Position.y);
						//}
					} else if (Network.PreMove != type) {
						Network.MoveCnt = 0;
						Network.PreMove = type;
						if (type != 0) {
							var m_pCharacterLogic = GameGOW.Get().BattleMgr.m_pCurrentPlayer;
                            Network.Send_Move(true, type, 0, m_pCharacterLogic.Position.x, m_pCharacterLogic.Position.y);
                            m_pCharacterLogic.OnDirectionKeyChanged(type);
                        }
                        else {
							var m_pCharacterLogic = GameGOW.Get().BattleMgr.m_pCurrentPlayer;
                            Network.Send_Move(false, type, 0, m_pCharacterLogic.Position.x, m_pCharacterLogic.Position.y);
                            m_pCharacterLogic.OnDirectionKeyChanged(type);
                        }

					}
				}
			} else {
				var m_pCharacterLogic = GameGOW.Get().BattleMgr.m_pCurrentPlayer;
				m_pCharacterLogic.OnDirectionKeyChanged(type);
			}
		}

		public static void InputAngle(int type) {
			if (GameGOW.Get().BattleMgr.m_bIsInBattle == false) {
				return;
			}
			if (Network.gamemode == BattleType.AI_1v1) {
				int index = GameGOW.Get().BattleMgr.m_iCurrentRoundPlayerIndex;
				int id = GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[index].m_iPlayerUID;
				if (Network.playerid == id) {
					var m_pCharacterLogic = GameGOW.Get().BattleMgr.m_pCurrentPlayer;
					m_pCharacterLogic.OnAttackAngleChange(type);
				}
			} else if (Network.NetworkMode == true) {
				int index = GameGOW.Get().BattleMgr.m_iCurrentRoundPlayerIndex;
				int id = GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[index].m_iPlayerUID;
				if (Network.playerid == id) {
					var m_pCharacterLogic = GameGOW.Get().BattleMgr.m_pCurrentPlayer;
					m_pCharacterLogic.OnAttackAngleChange(type);
				}
			} else {
				var m_pCharacterLogic = GameGOW.Get().BattleMgr.m_pCurrentPlayer;
				m_pCharacterLogic.OnAttackAngleChange(type);
			}
		}

		public static void InputFunctionDown(int type) {
			if (GameGOW.Get().BattleMgr.m_bIsInBattle == false) {
				return;
			}
			if (Network.gamemode == BattleType.AI_1v1) {
				int index = GameGOW.Get().BattleMgr.m_iCurrentRoundPlayerIndex;
				int id = GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[index].m_iPlayerUID;
				if (Network.playerid == id && Network.Attacking == 0) {
					Network.BombEnergy = LuaHelper.GetSkillInfo2("ENERGY", 100 + SkillManager.CurrentSkillId);
					int Energy = LuaHelper.GetSetEnergy(-1);
					LuaHelper.GetSetEnergy(Energy - (int)Network.BombEnergy);
					Network.Moving = true;
					Network.Attacking = 1;
					var m_pCharacterLogic = GameGOW.Get().BattleMgr.m_pCurrentPlayer;
					m_pCharacterLogic.OnFunctionKeyDown(type);
				}
			} else if (Network.NetworkMode == true) {
				int index = GameGOW.Get().BattleMgr.m_iCurrentRoundPlayerIndex;
				int id = GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[index].m_iPlayerUID;
				if (Network.playerid == id && Network.Attacking == 0) {
					Network.BombEnergy = LuaHelper.GetSkillInfo2("ENERGY", 100 + SkillManager.CurrentSkillId);
					int Energy = LuaHelper.GetSetEnergy(-1);
					LuaHelper.GetSetEnergy(Energy - (int)Network.BombEnergy);
					Network.Moving = true;
					Network.Attacking = 1;
					var m_pCharacterLogic = GameGOW.Get().BattleMgr.m_pCurrentPlayer;
					m_pCharacterLogic.OnFunctionKeyDown(type);
				}
			} else {
				var m_pCharacterLogic = GameGOW.Get().BattleMgr.m_pCurrentPlayer;
				m_pCharacterLogic.OnFunctionKeyDown(type);
			}
		}

		public static void InputFunctionUp(int type) {
			if (GameGOW.Get().BattleMgr.m_bIsInBattle == false) {
				return;
			}
			if (Network.gamemode == BattleType.AI_1v1) {
				int index = GameGOW.Get().BattleMgr.m_iCurrentRoundPlayerIndex;
				int id = GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[index].m_iPlayerUID;
				if (Network.playerid == id && Network.Attacking == 1) {
					Network.Attacking = 2;
					var m_pCharacterLogic = GameGOW.Get().BattleMgr.m_pCurrentPlayer;
					m_pCharacterLogic.OnFunctionKeyUp(type);
				}
			} else if (Network.NetworkMode == true) {
				int index = GameGOW.Get().BattleMgr.m_iCurrentRoundPlayerIndex;
				int id = GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[index].m_iPlayerUID;
				if (Network.playerid == id && Network.Attacking == 1) {
					Network.Attacking = 2;
                    var m_pCharacterLogic = GameGOW.Get().BattleMgr.m_pCurrentPlayer;
                    m_pCharacterLogic.OnFunctionKeyUp(type);
                    Network.Send_Attach(false);
				}
			} else {
				var m_pCharacterLogic = GameGOW.Get().BattleMgr.m_pCurrentPlayer;
				m_pCharacterLogic.OnFunctionKeyUp(type);
			}
		}
		public static int GetJoyStickDate(string name, int type) {
			var info = GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo;
			if (name == "hp") {
				if (type == 0)
					return info.m_iHP;
				else
					return info.m_iMaxHP;
			} else if (name == "energy") {
				if (type == 0)
					return info.m_iMoveEnergy;
				else
					return info.m_iMaxMoveEnergy;
			} else if (name == "power") {
				if (type == 0)
					return info.m_iBombSpeed;
				else
					return info.m_pInstructionData.fire_range;
			}
			return -1;
		}
		public static float GetMyPlayerEnergy() {
			//var info = GameGOW.Get().MyPlayer.m_pInfo;
			var info = GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo;
			return info.m_iMoveEnergy * 1.0f / info.m_iMaxMoveEnergy;
		}
		public static int GetSetEnergy(int energy) {
			var info = GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo;
			if (energy > 0) {
				info.m_iMoveEnergy = energy;
			}
			return info.m_iMoveEnergy;
		}

		public static float GetMyPlayerPower() {
			//var info = GameGOW.Get().MyPlayer.m_pInfo;
			var info = GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo;
			return info.m_iBombSpeed * 1.0f / info.m_pInstructionData.fire_range;
		}

		public static float GetMyPlayerWait() {
			var info = GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo;
			return info.m_iHP * 1.0f / info.m_iMaxHP;
			//return info.MoveEnergy * 100.0f / info.m_iMaxMoveEnergy;
			//var time = GameGOW.Get().BattleMgr.m_iCurrentRoundTime;
			//return time * 1.0f / BattleManager.ROUND_TIME;
		}
		public static string GetCharacterName(string ans) {
			if (ans == "wechat") {
				if (SdkManager.wxLogin == true)
					return SdkManager.wxName;
				else
					return null;
			}
			return GameGOW.Get().DataMgr.GetCharaterName(ans);
		}
		public static int GetRoundLeftTick() {
			return GameGOW.Get().BattleMgr.m_iCurrentRoundTime;
		}
		public static float GetMapWindPercent() {
			return GameGOW.Get().MapMgr.WindPercent;
		}
		public static void StartBattle(int iMapIndex) {
			GameGOW.Get().TestDemo(iMapIndex);
		}
		public static int GetWinnerUid() {
			return GameGOW.Get().Winner_Id;
		}

		public static int GameMode() {
			return GameConfig.mode;
		}

		public static string GetLogPath() {
			return Application.persistentDataPath + "/log/";
		}

		public static void Log(string msg, int eLogType = 3) {
			switch ((LogType)eLogType) {
				case LogType.Error:
					LOG.Error(msg);
					break;
				case LogType.Assert:
					LOG.KeyLog(msg);
					break;
				case LogType.Warning:
					LOG.Warning(msg);
					break;
				case LogType.Log:
					LOG.Log(msg);
					break;
			}
		}

		public static void ChangeRotation(GameObject obj) {
			obj.transform.Rotate(new Vector3(0, 180, 0));
		}
		public static void LoadSprite(GameObject obj, string path) {
			obj.GetComponent<Image>().sprite = Resources.Load<Sprite>(path);
		}
		public static void LoadRawImage(GameObject obj, string path) {
			obj.GetComponent<RawImage>().texture = Resources.Load<Texture>(path);
		}
		public static int SetSkill(int index, int skill) {
			if (skill != -1) {
				if (index == 1) Network.skill1 = skill;
				else if (index == 2) Network.skill2 = skill;
				else Network.default_skill = skill;
			}
			if (index == 1) return Network.skill1;
			else if (index == 2) return Network.skill2;
			else return Network.default_skill;
		}
		public static void ChangeColor(Transform trans, float r, float g, float b, float a) {
			a = Math.Max(0, a);
			if (r < 0 || g < 0 || b < 0 || a < 0)
				return;
			//var image = trans.GetComponent<Image>();
			var graphic = trans.GetComponent<Graphic>();

			Color _helpColor = new Color();
			if (graphic != null) {
				//LOG.Error(image.color.r + "+" + image.color.g);
				_helpColor.r = r / 255f;
				_helpColor.g = g / 255f;
				_helpColor.b = b / 255f;
				_helpColor.a = a / 255f;
				graphic.color = _helpColor;
			}
		}
		public static void ChangeTextColor(Transform trans, float r, float g, float b, float a) {
			if (r < 0 || g < 0 || b < 0 || a < 0)
				return;
			var text = trans.GetComponent<Text>();
			Color _help = new Color();
			if (text != null) {
				_help.r = r / 255f;
				_help.g = g / 255f;
				_help.b = b / 255f;
				_help.a = a / 255f;
				text.color = _help;
			}
			return;
		}


		public static float GetPowerPercentmax() {
			var info = GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo;
			return Mathf.Abs(info.m_icanattackmax);
		}
		public static float GetPowerPercentmin() {
			var info = GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo;
			return Math.Abs(info.m_icanattackmin);
		}

		public static int SetMatchTime(int time) {
			if (time >= 0) GameGOW.Get().match_time = time;
			return GameGOW.Get().match_time;
		}

		public static int SocketConnect() {
			bool Status = Network.Instance.ConnectToServer();

			if (Status == false) return 0;
			Network.SequenceID = 0;
			Network.Instance.Send_Login();

			Network.gameState = Network.SGameState.Login;
			Network.Instance.Update();
			Network.NetworkMode = true;
			return 1;
		}

		public static int GetStatus() {
			return (int)Network.gameState;
		}

		public static int MAP_MATCH_result() {
			return Network.CMD_MAP_MATCH_result;
		}

		public static int GetVersion() {
			if (Network.NetworkMode == true) return 1;
			else return 0;
		}

		public static float GetAngle() {
			var m_pCharacterLogic = GameGOW.Get().BattleMgr.m_pCurrentPlayer;
			float angle = m_pCharacterLogic.GetFireAngle() + m_pCharacterLogic.CalcObjectAngle();
			if (m_pCharacterLogic.m_pInfo.m_iFacing == GameDefine.FACING_LEFT) angle *= -1;
			return angle;// Math.Abs(angle);
		}

		public static float GetCharacterPosition(int type, int index, int xy) {
			float pos = 0f, index_now = -1, index_my = -1;
			if (1 == Network.GetVersion()) {
				for (int i = 0; i < GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers.Count; i++) {
					if (GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[i].m_iPlayerUID == Network.playerid) {
						index_my = i;
						break;
					}
				}
				for (int i = 0; i < GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers.Count; i++) {
					if ((type == 1 && i % 2 == index_my % 2) || (type != 1 && i % 2 != index_my % 2)) {
						index_now++;
						if (index_now == index) {
							var m_pCharacterLogic = GameGOW.Get().CharacterMgr.GetCharacterByUid(GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[i].m_iPlayerUID);
							if (xy == 1) pos = m_pCharacterLogic.Position.x;
							else pos = m_pCharacterLogic.Position.y;
							break;
						}
					}
				}
			} else {
				if (type == 1) {
					//友方
					var characterList = GameGOW.Get().CharacterMgr.GetCharacterByTeam(GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iTeam, true, false);
					if (xy == 1) pos = characterList[index].Position.x;
					else pos = characterList[index].Position.y;
				} else {
					//敌方
					var characterList = GameGOW.Get().CharacterMgr.GetCharacterByTeam(GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iTeam, false, false);
					if (xy == 1) pos = characterList[index].Position.x;
					else pos = characterList[index].Position.y;
				}
			}
			if (xy == 1) return pos / GameGOW.Get().MapMgr.m_stBound.xMax;
			else return pos / GameGOW.Get().MapMgr.m_stBound.yMax;
		}

		public static void Lua_call(string name) {
			Network.LuaID id = (Network.LuaID)Enum.Parse(typeof(Network.LuaID), name);
			if (!Network.lua_fun.ContainsKey(id)) {
				Type type = typeof(Network);
				MethodInfo mi = type.GetMethod(name);
				if (mi == null) {
					LOG.Error("Can not find MSG RSP Function: " + name);
					return;
				}

				Network.Lua_fun dele = Delegate.CreateDelegate(typeof(Network.Lua_fun), null, mi) as Network.Lua_fun;

				Network.lua_fun.Add(id, dele);
				Network.lua_fun[id]();
			} else {
				Network.lua_fun[id]();
			}
		}

		public static int Lua_get(string name) {
			Network.LuaGetID id = (Network.LuaGetID)Enum.Parse(typeof(Network.LuaGetID), name);
			if (!Network.lua_get.ContainsKey(id)) {
				Type type = typeof(Network);
				MethodInfo mi = type.GetMethod(name);
				if (mi == null) {
					LOG.Error("Can not find MSG RSP Function: " + name);
					return -1;
				}

				Network.Lua_get dele = Delegate.CreateDelegate(typeof(Network.Lua_get), null, mi) as Network.Lua_get;

				Network.lua_get.Add(id, dele);
				return Network.lua_get[id]();
			} else {
				return Network.lua_get[id]();
			}
		}

		public static string Lua_get_string(string name) {
			Network.LuaGetStringID id = (Network.LuaGetStringID)Enum.Parse(typeof(Network.LuaGetStringID), name);
			if (!Network.lua_getstring.ContainsKey(id)) {
				Type type = typeof(Network);
				MethodInfo mi = type.GetMethod(name);
				if (mi == null) {
					LOG.Error("Can not find MSG RSP Function: " + name);
					return null;
				}

				Network.Lua_GetString dele = Delegate.CreateDelegate(typeof(Network.Lua_GetString), null, mi) as Network.Lua_GetString;

				Network.lua_getstring.Add(id, dele);
				return Network.lua_getstring[id]();
			} else {
				return Network.lua_getstring[id]();
			}
		}

		public static void Lua_set(string name, int val) {
			Network.LuaSetID id = (Network.LuaSetID)Enum.Parse(typeof(Network.LuaSetID), name);
			if (!Network.lua_set.ContainsKey(id)) {
				Type type = typeof(Network);
				MethodInfo mi = type.GetMethod(name);
				if (mi == null) {
					LOG.Error("Can not find MSG RSP Function: " + name);
				}

				Network.Lua_set dele = Delegate.CreateDelegate(typeof(Network.Lua_set), null, mi) as Network.Lua_set;

				Network.lua_set.Add(id, dele);
				Network.lua_set[id](val);
			} else {
				Network.lua_set[id](val);
			}
		}

		public static void Lua_set_string(string name, string val) {
			Network.LuaSetStringID id = (Network.LuaSetStringID)Enum.Parse(typeof(Network.LuaSetStringID), name);
			if (!Network.lua_setstring.ContainsKey(id)) {
				Type type = typeof(Network);
				MethodInfo mi = type.GetMethod(name);
				if (mi == null) {
					LOG.Error("Can not find MSG RSP Function: " + name);
				}

				Network.Lua_SetString dele = Delegate.CreateDelegate(typeof(Network.Lua_SetString), null, mi) as Network.Lua_SetString;

				Network.lua_setstring.Add(id, dele);
				Network.lua_setstring[id](val);
			} else {
				Network.lua_setstring[id](val);
			}
		}

		public static void Send_Name(string name) {
			name = name.Trim();
			Network.name = name;
		}

		public static int check_Name(string name) {
			Network.Start_xue();
			name = name.Trim();
			if (Network.Check_Name(name) == false) {
				return 0;
			}
			Network.name = name;
			return 1;
		}

		public static int Send_Sex(int sex) {
			if (sex != -1) {
				Network.sex = sex;
			}
			return Network.sex;
		}

		public static int Get_Sex(int index) {
			return Network.GameReady_pic[index - 1];
		}

		enum playerInfoType : byte {
			gold = 1,
			diamond = 2,
			exp = 3,
			addgold = 4,
			adddiamond = 5,
			addexp = 6,
			sex = 7,
		};
		public static int Get_playerInfo(int type) {
			switch (type) {
				case ((int)playerInfoType.exp):
					return Network.exp;
				case ((int)playerInfoType.gold):
					return Network.gold;
				case ((int)playerInfoType.diamond):
					return Network.diamond;
				case ((int)playerInfoType.addgold):
					return Network.addgold;
				case ((int)playerInfoType.adddiamond):
					return Network.adddiamond;
				case ((int)playerInfoType.addexp):
					return Network.addexp;
				case ((int)playerInfoType.sex):
					return Network.sex; //1.boy 2.girl
				default:
					return -1;
			}
		}
		public static int transExp(int Exp, int type) {
			//type 1.level2exp 2.exp2level 3.resexp2level
			return GameGOW.Get().DataMgr.ExpToLevel(Exp, type);
		}
		public static string Get_playerInfoName() {
			return Network.name;
		}

		public static int GetMode() {
			if (Network.NetworkMode == false) return 1;
			if (Network.gamemode == BattleType.NORMAL_PVP) return 1;
			else if (Network.gamemode == BattleType.PVP_2V2) return 2;
			else if (Network.gamemode == BattleType.PVP_3V3) return 3;
			else return 4;
		}

		public static int CheckAccount(string name, string pwd, string kind) {
			name = name.Trim();
			pwd = pwd.Trim();
			if (Network.Check_Str(name) == false || Network.Check_Str(pwd) == false) {
				return 1;
			}

			if (kind == "0") {
				return Network.Register(name, pwd);
			} else {
				return Network.Login(name, pwd);
			}
		}

		public static string Get_Next_Name() {
			if (PlayerPrefs.GetString("AccountPre") != null && PlayerPrefs.GetString("AccountPre") != "") {
				return PlayerPrefs.GetString("AccountPre");
			} else {
				return "";
			}
		}

		public static string Get_Next_Pwd() {
			if (PlayerPrefs.GetString("AccountPrePass") != null && PlayerPrefs.GetString("AccountPrePass") != "") {
				return PlayerPrefs.GetString("AccountPrePass");
			} else {
				return "";
			}
		}

		public static List<string> Get_Next_Info() {
			List<string> x = new List<string>();
			if (PlayerPrefs.GetString("AccountPre") != null && PlayerPrefs.GetString("AccountPre") != "") {
				x.Add(PlayerPrefs.GetString("AccountPre"));
				x.Add(PlayerPrefs.GetString("AccountPrePass"));
			} else {
				x.Add("");
			}
			return x;
		}

		public static int Get_Loading(int num) {
			return Network.GameLoading_num[num];
		}

		public static string GetName(int id) {
			if (id > Network.battleinfo.m_listPlayers.Count) {
				return null;
			}
			return Network.battleinfo.m_listPlayers[id - 1].m_strName;
		}
		public static int GetLevel(int id) {
			return Network.InfoLevel[id];
		}

		public static int Choose(int id) {
			return Network.GameChoose_num[id];
		}

		public static void Run_Skill(int id) {
			SkillManager.CurrentSkillId = id;
		}
		public static int Get_CD(int id) {
			return Network.Skill_CD[id];
		}
		public static void Play_Sound(int index, string audio) {
			SounderManager.Get().PlaySound(index, audio);
		}
		public static int GetMap() {
			int xml = Network.battleinfo.m_iMapIndex;

			//add by 翔
			if (Network.gamemode != BattleType.AI_1v1 && Network.gamemode != BattleType.NORMAL_PVP) xml += 3;

			return xml;
		}
		public static int SetSound(int index, int type) {
			if (type != -1) {
				SounderManager.Get().SetSound(index, type);
			}
			if (index == 0) {
				return SounderManager.Get().yinyue;
			} else {
				return SounderManager.Get().yinxiao;
			}
		}

		public static int wechatLogin() {
			return SdkManager.wechatLogin();
		}

		public static void setPlayerWxHead(GameObject obj, int num) {
			num -= 1;
			if (Network.Battle_wxLogin[num]) {
				string url = Network.Battle_wxHead[num];
				AsyncImageDownload.GetInstance().SetAsyncImage(url, obj.GetComponent<Image>());
			}
		}
		public static void setFriendHead(GameObject obj, string url) {
			AsyncImageDownload.GetInstance().SetAsyncImage(url, obj.GetComponent<Image>());
		}

		public static void setWXhead(GameObject obj)                        //放上网上下载的微信头像
		{
			//  string url = "http://wx.qlogo.cn/mmopen/g3MonUZtNHkdmzicIlibx6iaFqAc56vxLSUfpb6n5WKSYVY0ChQKkiaJSgQ1dZuTOgvLLrhJbERQQ4eMsv84eavHiaiceqxibJxCfHe/64";
			//  AsyncImageDownload.GetInstance().SetAsyncImage(url, obj.GetComponent<Image>());
			if (SdkManager.wxLogin == false)
				return;
			string url = SdkManager.wxHeadUrl;
			AsyncImageDownload.GetInstance().SetAsyncImage(url, obj.GetComponent<Image>());
		}

		public static void setServer(int serverType)       //设置服务器
		{
			Network.m_iServerType = serverType;
		}

		private static SkeletonAnimation test;
		public static void SetAni(int id, GameObject anitest) {
			anitest.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
			anitest = GameGOW.Get().ResourceMgr.GetRes("Spine/TK100001");
			test = anitest.GetComponent<SkeletonAnimation>();
			test.loop = true;
			test.AnimationName = "IDLE1";

		}

		public static string GetTankInfo(string type, int id) {
			if (type == "NAME") {
				return GameGOW.Get().DataMgr.GetCharacterInstructionByID(id).default_name;
			}
            if (type == "SKILL"){
                return GameGOW.Get().DataMgr.GetCharacterInstructionByID(id).self_skill.ToString();
            }
            if (type == "DECRIP"){
                return GameGOW.Get().DataMgr.GetCharacterInstructionByID(id).description;
            }
			return "????";
		}
		public static string GetSkillInfo(string type, int id) {
			if (type == "NAME") {
				return GameGOW.Get().DataMgr.GetSkillINstructionDataByID(id).default_name;
			}
			if (type == "DESCRIPTION") {
				return GameGOW.Get().DataMgr.GetSkillINstructionDataByID(id).description;
			}

			return "???";
		}
		public static float GetSkillInfo2(string type, int id) {
			if (id == 100) {
				id = 0;
			}
			if (type == "ENERGY") {
				return GameGOW.Get().DataMgr.GetSkillINstructionDataByID(id).energy;
			}

			return 0.0f;
		}
		public static int GetRound() {
			return Network.Round;
		}

		public static void DestroyObj(GameObject obj) {
			if (obj != null) {
				GameObject.Destroy(obj);
			}
		}
		public static GameObject CreatePrefab(Transform parent, string path, string name) {
			try {
				int num = 0;
				for (int i = 6; i < name.Length; i++) {
					num = num * 10 + name[i] - '0';
				}
				GameObject template = AppFacade.ResManager.GetRes<GameObject>(path);
				GameObject go = GameObject.Instantiate(template) as GameObject;
				go.name = name;
				go.transform.SetParent(parent, false);
				go.transform.GetChild(1).GetComponent<Text>().text = Network.Friend_list[num].name;

				Toggle tgl = go.transform.GetChild(0).GetComponent<Toggle>();
				tgl.onValueChanged.AddListener(
					delegate (bool isOn) {
						Lua_set_string("Lua_Find_Friend", Get_Friend_Name(num));
					}
				);
				if (LuaHelper.Get_Friend_Status(num) == 1) {
					go.transform.GetChild(2).GetComponent<Text>().text = "在线";
					ChangeTextColor(go.transform.GetChild(2), 0f, 255f, 119f, 255f);
				} else {
					go.transform.GetChild(2).GetComponent<Text>().text = "离线";
					ChangeTextColor(go.transform.GetChild(2), 51f, 68f, 119f, 255f);
				}

				string headpath = "Sprites/head" + (LuaHelper.Get_Friend_Sex(num)).ToString();
				LoadSprite(go.transform.GetChild(0).GetChild(0).gameObject, headpath);

				//好友列表微信头像
				if (Network.Friend_list[num].wxLogin) {
					setFriendHead(go.transform.GetChild(0).GetChild(0).gameObject, Network.Friend_list[num].wxHead);
				}

				GameObject invite_btn = go.transform.GetChild(3).gameObject;
				if (LuaHelper.Get_Friend_Status(num) != 1) {
					invite_btn.SetActive(false);
				}
				//invite_btn.SetActive(false);//出包暂时隐藏
				Button invite = invite_btn.GetComponent<Button>();
				invite.onClick.AddListener(
					delegate () {
						Network.Invitation_name = Get_Friend_Name(num);
						EventDispatcher.DispatchEvent("EventInvitationFriendConfirm", null, null);
						//Network.Send_Invitation_Friend(Get_Friend_Name(num));
						LOG.Log("heheda");
					}
				);

				return go;
			} catch (Exception e) {
				LOG.Log(e.Message);
				LOG.Log(e.StackTrace);
			}
			return null;
		}

		//获取好友列表
		public static string Get_Friend_Name(int id) {
			return Network.Friend_list[id].name;
		}
		public static int Get_Friend_Sex(int id) {
			return Network.Friend_list[id].sex;
		}
		public static int Get_Friend_Status(int id) {
			return Network.Friend_list[id].status;
		}
		public static int Get_Friend_Cnt() {
			return Network.Friend_cnt;
		}

		public static void Send_Add_Notify(string name, int val) {
			Network.Send_Add_Notify_Friend(Network.name, name, val == 1 ? true : false);
		}

		public static void Show_Emoji(int id) {
			var info = GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo;
			if (Network.NetworkMode == true) {
				info = GameGOW.Get().CharacterMgr.GetCharacterByUid(Network.playerid).m_pInfo;
			}
			info.EmojiID = id + 1000;
		}
		public static void Show_Message(int id) {
			var info = GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo;
			if (Network.NetworkMode == true) {
				info = GameGOW.Get().CharacterMgr.GetCharacterByUid(Network.playerid).m_pInfo;
			}
			info.MessageID = id + 2000;
		}
		public static int Get_Tankid(int id) {
			if (id == 1) return Network.Player_tank;
			else return Network.Ai_tank;
		}

		public static float Get_BombEnergy() {
			return Network.BombEnergy;
		}

		#region DOTween

		public static void DoLocalMoveY(GameObject go, float endY, float duration, LuaFunction luaFunction) {
			go.transform.DOLocalMoveY(endY, duration).OnComplete(() => {
				if (luaFunction != null) {
					luaFunction.Call();
				}
			});
		}

		public static void DoLocalMoveX(GameObject go, float endX, float duration, LuaFunction endFunction) {
			go.transform.DOLocalMoveX(endX, duration).OnComplete(() => {
				if (endFunction != null) {
					endFunction.Call();
				}
			});
		}

		public static void DoScaleLoop(GameObject go, float endX, float endY, float endZ, float duration, int times = -1) {
			go.transform.DOScale(new Vector3(endX, endY, endZ), duration).SetLoops(times, LoopType.Yoyo);
		}

		public static void DoScaleLoop(GameObject go, float endValue, float duration, int times = -1) {
			DoScaleLoop(go, endValue, endValue, endValue, duration, times);
		}

		/// <summary>
		/// 浮点型数字渐变，如2秒内由1.0f渐变到10.0f
		/// </summary>
		public static void DoToFloat(float startValue, float endValue, float duration, LuaFunction luaFunction, LuaFunction endFunction) {
			DOTween.To(() => startValue, value => {
				if (luaFunction != null) {
					luaFunction.Call(value);
				}
			}, endValue, duration).OnComplete(() => {
				if (endFunction != null) {
					endFunction.Call();
				}
			});
		}

		/// <summary>
		/// 整型数字渐变，如2秒内由1渐变到10
		/// </summary>
		public static void DoToInt(int startValue, int endValue, float duration, LuaFunction luaFunction, LuaFunction endFunction) {
			DOTween.To(() => startValue, value => {
				if (luaFunction != null) {
					luaFunction.Call(value);
				}
			}, endValue, duration).OnComplete(() => {
				if (endFunction != null) {
					endFunction.Call();
				}
			});
		}

		public static void KillAllTweeners(GameObject go) {
			go.transform.DOKill();
		}

		#endregion
	}
}