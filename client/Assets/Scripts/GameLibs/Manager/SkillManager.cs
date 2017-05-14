using System.Collections.Generic;
using System.Collections;
using System;
using TKBase;
using UnityEngine;
/*
 * 俩发炮
 * 三发炮
 * 散射炮
 * 治疗炮
 * 瞬移炮
 * 机器人
 * 
 */
namespace TKGame {
	public class SkillManager : MonoBehaviour {
		public static bool DO_bomb_Action = true;    /* 是否挖图产生攻击 true/false */
		public static float kill_bomb = 1.0f;          /* 攻击弹(+)or治疗弹(-)*/
		public static int thunder_bomb_num = 0;        /*轰天雷个数-1*/
		public static int bomb_num = 0;              /*技能炸弹个数*/
		public static string cur_fly;
		public static string cur_boom;

		public enum SkillType {
			Normal,   //单炮                    0
			DoubleBomb,      //俩发炮                  101
			TripleBomb,    //仨发炮 （散炮）         102
			ThunderBomb,  //轰天雷 （天上下炸弹）   103
			Robot,         //机器人 （行走炸弹）     104
			Rocket,     //火箭炮  (跟踪弹)        105
			Cure,     //治疗弹                  106
			Teleport,   //瞬移   （纸飞机）       107
		}
		
		void Start() {
			GameGOW.Get().OnSkillPlay += Throw_Bomb;
		}
		private void OnDestroy() {
			GameGOW.Get().OnSkillPlay -= Throw_Bomb;
		}
		void Update() {
			/* */
		}
		private static int _currentSkillId = 0;
		public static int EnemyID = 0;
		public static float PreAngle = 0f;

		public static int CurrentSkillId
		{
			get
			{
				return _currentSkillId;
			}

			set
			{
				_currentSkillId = value;
				SkillChangedHandle((SkillType)value);
			}
		}

		public static Action<SkillType> SkillChangedHandle = delegate (SkillType type) { };

		public static void Init_Round() {
			EnemyID = 0;
			kill_bomb = 1;
		}

		IEnumerator DelayBomb(CharacterLogic A, AttackBaseData attack, float delay_time) //延迟发炮,delay_time 为延迟时间
		{
			yield return new WaitForSeconds(delay_time);
			GameGOW.Get().BombMgr.ThrowBomb(A, attack as AttackBombData, false);
		}
		public void Rock_Bomb() {
			CurrentSkillId = (int)SkillType.Rocket;
			int mi = 0x1f1f1f1f;
			for (int i = (GameGOW.Get().BattleMgr.m_iCurrentRoundPlayerIndex + 1) % 2; i < Network.battleinfo.m_listPlayers.Count; i += 2) {
				int HP = GameGOW.Get().CharacterMgr.GetCharacterByUid(Network.battleinfo.m_listPlayers[i].m_iPlayerUID).m_pInfo.m_iHP;
				if (HP < mi && GameGOW.Get().CharacterMgr.GetCharacterByUid(Network.battleinfo.m_listPlayers[i].m_iPlayerUID).IsLiving == true) {
					mi = HP;
					EnemyID = i;
				}
			}


			var Enemy = GameGOW.Get().CharacterMgr.GetCharacterByUid(Network.battleinfo.m_listPlayers[EnemyID].m_iPlayerUID);
			var m_pCharacterLogic = GameGOW.Get().BattleMgr.m_pCurrentPlayer;
			EnemyID = Network.battleinfo.m_listPlayers[EnemyID].m_iPlayerUID;

			PreAngle = GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iFireAngle;
			float xx = Enemy.Position.x - m_pCharacterLogic.GetWeaponPosition().x;
			float yy = Enemy.Position.y - m_pCharacterLogic.GetWeaponPosition().y;
            
			if (Math.Abs(xx - 0.0f) <= 1e-5) {
				if (yy > 0) {
					GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iFireAngle = 90;
				} else {
					GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iFireAngle = -90;
                }
			} else {
				float angle = Mathf.Atan(yy / xx);
				angle = angle * 180 / Mathf.PI;
				GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iFireAngle = angle;
				//Debug.Log("xue<<<<<<<<<<<<<<<<" + GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iFireAngle + ":::" + Network.xue++);
				//Debug.Log("xue^^^^^^^^^^^^(" + xx + ", " + yy + ")    " + Network.xue++ + "          " + GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iFacing);
				if (GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iFacing == GameDefine.FACING_RIGHT) {
					if (xx < 0) {
						if (yy > 0 && angle != 0) angle = 180 + angle;
						if (yy < 0 && angle != 0) angle = -(180 - angle);
					}
				} else {
					if (xx > 0) {
						if (yy > 0 && angle != 0) angle = (180 - angle);
						if (yy < 0 && angle != 0) angle = -(180 + angle);
					}
					if (xx < 0) {
						angle = -angle;
					}
				}
				GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iFireAngle = angle;
				GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iBombSpeed = 750;
			}
			kill_bomb = 0.5f;
		}
		public void set_animation(int x) {
			cur_boom = "BOOM-1";
			if (x < 2) {
				cur_fly = "FLY-2";
			} else if (x < 7) {
				cur_fly = "FLY-3";
				if (x == 6) {
					cur_boom = "BOOM-2";
				}
			} else if (x == 7) {

				if (GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iFacing == GameDefine.FACING_RIGHT) {
					cur_boom = cur_fly = "FLY-4";
				} else {
					cur_boom = cur_fly = "FLY-5";
				}
			}
		}
		public void Throw_Bomb(SkillType x, CharacterLogic A, AttackBaseData AA) {
			Init_Round();
			set_animation((int)x);
			if (A.go_down == true) {
				return;
			}
			thunder_bomb_num = 0;
			kill_bomb = 1.0f;
			DO_bomb_Action = true;
			bomb_num = 1;

			switch (x) {
				case SkillType.Normal:
					Throw_one_Bomb(A, AA);
					break;

				case SkillType.Cure:
					DO_bomb_Action = false;
					Throw_cure_bomb(A, AA);
					break;

				case SkillType.DoubleBomb:
					bomb_num = 2;
					Throw_two_Bomb(A, AA);
					break;

				case SkillType.TripleBomb:
					bomb_num = 3;
					Throw_three_Bomb(A, AA);
					break;

				case SkillType.Teleport:
					DO_bomb_Action = false;
					Throw_one_Bomb(A, AA);
					break;

				case SkillType.Robot:
					break;

				case SkillType.ThunderBomb:
					thunder_bomb_num = 6;
					bomb_num = 5;
					Throw_thunder_bomb(A, AA);
					break;
				case SkillType.Rocket:
					Rock_Bomb();
					Throw_one_Bomb(A, AA);
					break;
			}
		}
		public void Throw_cure_bomb(CharacterLogic A, AttackBaseData AA) {
			kill_bomb = -1.0f;
			Throw_one_Bomb(A, AA);
		}
		public void Throw_one_Bomb(CharacterLogic A, AttackBaseData AA) {
			GameGOW.Get().BombMgr.ThrowBomb(A, AA as AttackBombData, true);
		}
		public void Throw_two_Bomb(CharacterLogic A, AttackBaseData AA) {
			GameGOW.Get().BombMgr.ThrowBomb(A, AA as AttackBombData, true);
			StartCoroutine(DelayBomb(A, AA, 0.2f));
           
        }
		public void Throw_three_Bomb(CharacterLogic A, AttackBaseData AA) {
			int temp = GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iBombSpeed;

			GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iBombSpeed -= 100;
			GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iCurrentAngle -= 3;
			GameGOW.Get().BombMgr.ThrowBomb(A, AA as AttackBombData, false);

			GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iBombSpeed += 100;
			GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iCurrentAngle += 3;
			GameGOW.Get().BombMgr.ThrowBomb(A, AA as AttackBombData, true);

			GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iBombSpeed += 100;
			GameGOW.Get().BattleMgr.m_pCurrentPlayer.m_pInfo.m_iCurrentAngle += 6;
			GameGOW.Get().BombMgr.ThrowBomb(A, AA as AttackBombData, false);

		}
		public void Throw_thunder_bomb(CharacterLogic A, AttackBaseData AA) {
			while (--thunder_bomb_num != 0)
				GameGOW.Get().BombMgr.ThrowBomb(A, AA as AttackBombData, false);
		}
	}

}