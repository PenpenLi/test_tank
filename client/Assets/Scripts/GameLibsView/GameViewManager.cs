using UnityEngine;
using System.Collections.Generic;
using System;
using TKBase;
using TKGame;
using UnityEngine.Assertions.Must;

namespace TKGameView {
	public class GameViewDefine {
		public const float Z_BACKGROUND = 100;
		public const float Z_ZSORT_LAYER = 10;
	}
	public class GameViewManager : MonoBehaviour {
		public Transform m_pBackGroundLayer;
		public Transform m_pTerrainLayer;
		public Transform m_pZSort;
		public CameraFollow m_pCamera;

		private Dictionary<int, CharacterLogicView> m_dicCharacterView;
		private Dictionary<int, BombView> m_dicBombView;

		public GameViewManager() {
			m_dicCharacterView = new Dictionary<int, CharacterLogicView>();
			m_dicBombView = new Dictionary<int, BombView>();
		}
		// Use this for initialization
		void Start() {
			GameGOW.Get().CharacterMgr.OnCharacterNew += AddCharacter;
			GameGOW.Get().CharacterMgr.OnCharacterDelete += DeleteCharacter;
			GameGOW.Get().BombMgr.OnBombNew += AddBomb;
			GameGOW.Get().BombMgr.OnBombDelete += DeleteBomb;
			GameGOW.Get().MapMgr.OnTerrainCellNew += AddTerrainCell;
			AddEvents();

		}

		// Update is called once per frame
		void Update() {

		}

		void OnDistory() {
			RemoveEvents();
		}

		public void AddEvents() {
			EventDispatcher.AddListener("EventChangeController", OnChangeController);
			EventDispatcher.AddListener("EventBattleStart", OnBattleStart);
			EventDispatcher.AddListener("EventBattleStop", OnBattleStop);
			EventDispatcher.AddListener("EventBeforeSwitchMap", BeforeSwitchMap);
			EventDispatcher.AddListener("EventAfterSwitchMap", AfterSwitchMap);
			EventDispatcher.AddListener("EventHideBattleJoyStickUI", OnHideBattleJoyStickUI);
			EventDispatcher.AddListener("EventShowBattleJoyStickUI", OnShowBattleJoyStickUI);
			EventDispatcher.AddListener("EventShowMessageUI", OnShowMessageUI);
			EventDispatcher.AddListener("EventShowMessageUIwithString", OnShowMessageUIwithString);
			EventDispatcher.AddListener("EventShowMessageUIwithRefuse", OnShowMessageUIwithRefuse);
			EventDispatcher.AddListener("EventShowInfomationUI", OnShowInfomationUI);
			EventDispatcher.AddListener("EventUpdateFriendList", OnUpdateFriendList);
			EventDispatcher.AddListener("EventAddFriendNotify", OnAddFriendNotify);
			EventDispatcher.AddListener("EventBackHost", OnBackHost);
			EventDispatcher.AddListener("EventShowPrepareUI", OnShowPrepareUI);


			//invitation
			EventDispatcher.AddListener("EventInvitationFriendConfirm", OnInvitationFriendConfirm);
			EventDispatcher.AddListener("EventInvitationFriendNotify", OnInvitationFriendNotify);
			EventDispatcher.AddListener("EventShowMessageUIwithRefuseInvitation", OnShowMessageUIwithRefuseInvitation);
		}

		public void RemoveEvents() {
			EventDispatcher.RemoveListener("EventChangeController", OnChangeController);
			EventDispatcher.RemoveListener("EventBattleStart", OnBattleStart);
			EventDispatcher.RemoveListener("EventBattleStop", OnBattleStop);
			EventDispatcher.RemoveListener("EventBeforeSwitchMap", BeforeSwitchMap);
			EventDispatcher.RemoveListener("EventAfterSwitchMap", AfterSwitchMap);
			EventDispatcher.RemoveListener("EventHideBattleJoyStickUI", OnHideBattleJoyStickUI);
			EventDispatcher.RemoveListener("EventShowBattleJoyStickUI", OnShowBattleJoyStickUI);
			EventDispatcher.RemoveListener("EventShowMessageUI", OnShowMessageUI);
			EventDispatcher.RemoveListener("EventShowMessageUIwithString", OnShowMessageUIwithString);
			EventDispatcher.RemoveListener("EventShowMessageUIwithRefuse", OnShowMessageUIwithRefuse);
			EventDispatcher.RemoveListener("EventShowInfomationUI", OnShowInfomationUI);
			EventDispatcher.RemoveListener("EventUpdateFriendList", OnUpdateFriendList);
			EventDispatcher.RemoveListener("EventAddFriendNotify", OnAddFriendNotify);
			EventDispatcher.RemoveListener("EventBackHost", OnBackHost);
			EventDispatcher.RemoveListener("EventShowPrepareUI", OnShowPrepareUI);
			EventDispatcher.RemoveListener("EventInvitationFriendConfirm", OnInvitationFriendConfirm);

			//invitation
			EventDispatcher.RemoveListener("EventInvitationFriendNotify", OnInvitationFriendNotify);
			EventDispatcher.RemoveListener("EventShowMessageUIwithRefuseInvitation", OnShowMessageUIwithRefuseInvitation);
		}

		private void BeforeSwitchMap(object sender, EventArgs e) {
			Clear();
		}

		private void AfterSwitchMap(object sender, EventArgs e) {
			SetData(GameGOW.Get().CurrentMapData);
		}

		private void OnBattleStart(object sender, EventArgs e) {
			LuaFramework.Util.SendMessageToLua("EventBattleStart");
			ChangeControllerEvent chgEvent = e as ChangeControllerEvent;
			if (chgEvent != null && m_dicCharacterView.ContainsKey(chgEvent.m_iUniqueID)) {
				CharacterLogicView view = m_dicCharacterView[chgEvent.m_iUniqueID];
				m_pCamera.SetFocus(view.transform);
			}
			GameGOW.Get().PlayWind(1, 0.0f, true);
			GameGOW.Get().MapMgr.Wind = 0;
			//BackgroundParallax.Get().SetCamera();
		}

		private void OnBattleStop(object sender, EventArgs e) {
			LuaFramework.Util.SendMessageToLua("EventBattleStop");
		}
		private void OnHideBattleJoyStickUI(object sender, EventArgs e) {
			LuaFramework.Util.SendMessageToLua("EventHideBattleJoyStickUI");
		}
		private void OnShowBattleJoyStickUI(object sender, EventArgs e) {
			LuaFramework.Util.SendMessageToLua("EventShowBattleJoyStickUI");
		}
		private void OnShowMessageUI(object sender, EventArgs e) {
			LuaFramework.Util.SendMessageToLua("EventShowMessageUI");
		}
		private void OnShowMessageUIwithString(object sender, EventArgs e) {
			LuaFramework.Util.SendMessageToLua("EventShowMessageUIwithString", "error");
		}
		private void OnShowMessageUIwithRefuse(object sender, EventArgs e) {
			LuaFramework.Util.SendMessageToLua("EventShowMessageUIwithRefuse", "refuse");
		}
		private void OnShowInfomationUI(object sender, EventArgs e) {
			LuaFramework.Util.SendMessageToLua("EventShowInfomationUI", Network.Find_Friend.name, Network.Find_Friend.sex, Network.Find_Friend.exp, Network.Find_Friend.game_cnt, Network.Find_Friend.game_win, Network.Find_Friend.relation, Network.Find_Friend.wxLogin ? 1 : 0, Network.Find_Friend.wxHead);
		}
		private void OnUpdateFriendList(object sender, EventArgs e) {
			LuaFramework.Util.SendMessageToLua("EventUpdateFriendList", Network.Friend_cnt);
		}
		private void OnAddFriendNotify(object sender, EventArgs e) {
			LuaFramework.Util.SendMessageToLua("EventAddFriendNotify", Network.Add_Friend_Name);
		}
		private void OnBackHost(object sender, EventArgs e) {
			LuaFramework.Util.SendMessageToLua("EventBackHost");
		}
		private void OnShowPrepareUI(object sender, EventArgs e) {
			LuaFramework.Util.SendMessageToLua("EventShowPrepareUI");
		}

		//invitation
		private void OnInvitationFriendConfirm(object sender, EventArgs e) {
			LuaFramework.Util.SendMessageToLua("EventInvitationFriendConfirm", Network.Invitation_name);
		}
		private void OnInvitationFriendNotify(object sender, EventArgs e) {
			LuaFramework.Util.SendMessageToLua("EventInvitationFriendNotify", Network.Invitation_name);
		}
		private void OnShowMessageUIwithRefuseInvitation(object sender, EventArgs e) {
			LuaFramework.Util.SendMessageToLua("EventShowMessageUIwithRefuseInvitation", "refuse2");
		}


		private void OnChangeController(object sender, EventArgs e) {
			LuaFramework.Util.SendMessageToLua("EventChangeController", e);
			ChangeControllerEvent chgEvent = e as ChangeControllerEvent;
			if (chgEvent != null && m_dicCharacterView.ContainsKey(chgEvent.m_iUniqueID)) {
				CharacterLogicView view = m_dicCharacterView[chgEvent.m_iUniqueID];
				m_pCamera.SetFocus(view.transform);
			}
		}

		public void AddCharacter(CharacterLogic logic) {
			GameObject obj = GameGOW.Get().ResourceMgr.GetRes("Prefab/CharacterLogic");
			CharacterLogicView view = obj.GetComponent<CharacterLogicView>();
			view.SetCharacterLogic(logic);
			obj.transform.parent = m_pZSort;
			m_dicCharacterView[logic.ID] = view;
		}

		public void DeleteCharacter(CharacterLogic logic) {
			if (m_dicCharacterView.ContainsKey(logic.ID)) {
				GameObject.Destroy(m_dicCharacterView[logic.ID].gameObject);
				m_dicCharacterView.Remove(logic.ID);
			}
		}

		public void AddBomb(BaseBomb logic, bool is_foucs) {
			GameObject bombView = GameGOW.Get().ResourceMgr.GetRes("Prefab/BaseBomb");
			bombView.transform.position = new Vector3(logic.Position.x, logic.Position.y, 5);
			BombView view = bombView.GetComponent<BombView>();
			view.SetBombLogic(logic);
			view.transform.parent = m_pZSort;
			m_dicBombView[logic.ID] = view;

			if (is_foucs) {
				m_pCamera.SetFocus(bombView.transform);
			}

		}

		public void DeleteBomb(BaseBomb logic, bool test_) {
			if (m_dicBombView.ContainsKey(logic.ID)) {
				if (m_dicBombView[logic.ID] != null) {
					GameObject.Destroy(m_dicBombView[logic.ID].gameObject);
				}
				m_dicBombView.Remove(logic.ID);
			}
		}

		public void AddTerrainCell(Texture2D texture, MapCellData pData) {
			GameObject obj = GameGOW.Get().ResourceMgr.GetRes("Prefab/MapCell");
			SpriteRenderer spr = obj.GetComponent<SpriteRenderer>();
			Rect rect = new Rect(0, 0, texture.width, texture.height);
			Sprite sp = Sprite.Create(texture, rect, new Vector2(0, 0));
			spr.sprite = sp;
			obj.transform.parent = m_pTerrainLayer;
			obj.transform.localPosition = new Vector3(pData.pos_x, pData.pos_y, m_pBackGroundLayer.position.z - m_pTerrainLayer.childCount * 0.01f);
		}

		public void Clear() {
			for (int i = m_pBackGroundLayer.childCount - 1; i >= 0; --i) {
				GameObject.DestroyObject(m_pBackGroundLayer.GetChild(i).gameObject);
			}
			for (int i = m_pTerrainLayer.childCount - 1; i >= 0; --i) {
				GameObject.DestroyObject(m_pTerrainLayer.GetChild(i).gameObject);
			}
			for (int i = m_pZSort.childCount - 1; i >= 0; --i) {
				GameObject.DestroyObject(m_pZSort.GetChild(i).gameObject);
			}
		}

		public void SetData(MapData pData) {
			m_pCamera.xMargin = pData.camera_margin_x;
			m_pCamera.yMargin = pData.camera_margin_y;
			m_pCamera.xSmooth = pData.camera_smooth_x;
			m_pCamera.ySmooth = pData.camera_smooth_y;
			m_pCamera.minXAndY = new Vector2(pData.camera_min_x, pData.camera_min_y);
			m_pCamera.maxXAndY = new Vector2(pData.camera_max_x, pData.camera_max_y);
			m_pCamera.BgWidth = pData.m_listBG[0].Width;
			m_pCamera.TerrainWidth = pData.m_listTerrain[0].Width;
			ResourceManager mgr = GameGOW.Get().ResourceMgr;

			for (int i = 0; i < pData.m_listBG.Count; ++i) {
				MapCellData cellData = pData.m_listBG[i];
				GameObject obj = mgr.GetRes("Prefab/MapCell");
				SpriteRenderer spr = obj.GetComponent<SpriteRenderer>();
				Texture2D texture = mgr.GetRes<Texture2D>(cellData.resource);
				//因为背景图很大，需要压缩，而压缩后长宽会改变，所以需要Bilinear将长宽设置回原图的值，又因为压缩格式的图片不能重新设置尺寸，所以需要重新创建一张图
				//创建一张图片
				var newTexture = new Texture2D(texture.width, texture.height);
				//设置像素
				newTexture.SetPixels(texture.GetPixels());
				//应用更改
				newTexture.Apply();
				//设置图片尺寸
				TextureScale.Bilinear(newTexture, cellData.Width, cellData.Height);
				//创建Sprite
				Rect rect = new Rect(0, 0, newTexture.width, newTexture.height);
				Sprite sp = Sprite.Create(newTexture, rect, new Vector2(0, 0));
				spr.sprite = sp;
				obj.transform.parent = m_pBackGroundLayer;
				obj.transform.localPosition = new Vector3(cellData.pos_x, cellData.pos_y, m_pBackGroundLayer.position.z - i * 0.01f);
			}
		}
	}
}