using UnityEngine;
using DG.Tweening;
using TKGame;
using Spine.Unity;

// ReSharper disable once CheckNamespace
namespace TKGameView {
	public class CharacterLogicView : MonoBehaviour {

		public float AttackAngle;

		public int Hp = 50;

		public int MoveEnergy = 1000;

		public int AttackEnergy = 0;

		private CharacterLogic _character;
		private SkeletonAnimation _bodyView;

		public Transform Body;
		public Transform BackGround;
		public Transform FireAngle;
		public Transform ControllerArrows;
		public ProgressBar HpProgress;
		public TextMesh NickName;
		public TextMesh SkillPrompt;
		public Transform Emoji;
		public Transform Message;
		public Transform MessageBg;

		public Transform Damage;
		public Transform Damage1;
		public Transform Damage2;
		public Transform Damage3;
		public Transform Damage4;
		public Transform Damage5;

		private float _countdown = 0f;
		private float _preHp = 0;

		private float _emojiCd = 0f;
		private float _messageCd = 0f;

		private Tweener _emojiScaleTweener;

		private Tweener _skillPromptMoveTweener;

		public void SetCharacterLogic(CharacterLogic logic) {
			_character = logic;
		}
		// Use this for initialization
		void Start() {
			GameObject aniObj = GameGOW.Get().ResourceMgr.GetRes("Spine/TK" + _character.m_pInfo.m_pInstructionData.resource_id);
			aniObj.transform.parent = Body;
			aniObj.transform.position = this.transform.position;
			aniObj.transform.localScale *= 0.8f;
			_bodyView = aniObj.GetComponent<SkeletonAnimation>();
			_bodyView.loop = true;
			BoxCollider box = this.gameObject.AddComponent<BoxCollider>();

			box.size.Set((_character.m_pInfo.m_pInstructionData.be_attack_box_max_x - _character.m_pInfo.m_pInstructionData.be_attack_box_min_x) / 100,
						(_character.m_pInfo.m_pInstructionData.be_attack_box_max_y - _character.m_pInfo.m_pInstructionData.be_attack_box_min_y) / 100,
						1f);
			box.center = new Vector3(0, box.size.y / 2, 1);

			Vector2 pos = _character.m_pInfo.m_pInstructionData.WeaponPosition;
			BackGround.localPosition = new Vector3(pos.x / 100, pos.y / 100, GameViewDefine.Z_ZSORT_LAYER + 0.01f);

			var attackbg = BackGround.GetChild(0);

			var pAttackAngleBg = attackbg.GetComponent<SectorProgressBar>();
			pAttackAngleBg.startAngleDegree = _character.m_pInfo.m_pInstructionData.low_fire_angle;
			pAttackAngleBg.angleDegree = _character.m_pInfo.m_pInstructionData.high_fire_angle - _character.m_pInfo.m_pInstructionData.low_fire_angle + 3;

			ControllerArrows.localPosition = new Vector3(0, _character.m_pInfo.m_pInstructionData.be_attack_box_max_y / 100.0f + 1f, 0);

			ControllerArrows.DOLocalMoveY(ControllerArrows.localPosition.y + 0.3f, 1).SetLoops(-1, LoopType.Yoyo);
			ControllerArrows.DOLocalRotate(Vector3.up * 360, 2f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);

			SkillPrompt.transform.localPosition = new Vector3(0, _character.m_pInfo.m_pInstructionData.be_attack_box_max_y / 100f + 0.8f, 0);
			_skillPromptMoveTweener = SkillPrompt.transform.DOLocalMoveY(2.8f, 0.9f).SetAutoKill(false).Pause().OnComplete(() => {
				SkillPrompt.gameObject.SetActive(false);
			});

			if (Network.gamemode == BattleType.AI_1v1) {
				if (_character.ID != Network.playerid) {
					BackGround.gameObject.SetActive(false);
					HpProgress.Color = 255f;
					NickName.color = Color.red;
				} else {
					NickName.color = Color.green;
					SkillManager.SkillChangedHandle += ShowSkillPrompt;
				}
			} else if (Network.NetworkMode == true) {
				if (_character.ID == Network.playerid) {
					NickName.color = Color.green;
					SkillManager.SkillChangedHandle += ShowSkillPrompt;
				} else {
					BackGround.gameObject.SetActive(false);
					int index = 0, index2 = 0;
					for (int i = 0; i < GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers.Count; i++) {
						if (GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[i].m_iPlayerUID == _character.ID) {
							index = i;
							break;
						}
					}

					for (int i = 0; i < GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers.Count; i++) {
						if (GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[i].m_iPlayerUID == Network.playerid) {
							index2 = i;
							break;
						}
					}

					if (index % 2 == index2 % 2) {
						NickName.color = Color.blue;
					} else {
						HpProgress.Color = 255f;
						NickName.color = Color.red;
					}
				}

			} else {
				//
			}

			if (Network.NetworkMode == true) {
				for (int i = 0; i < GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers.Count; i++) {
					if (GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[i].m_iPlayerUID == _character.ID) {
						NickName.text = GameGOW.Get().BattleMgr.m_stBattleInfo.m_listPlayers[i].m_strName;
						break;
					}
				}
			} else {
				NickName.text = _character.m_pInfo.m_pInstructionData.default_name;
			}

			_preHp = _character.m_pInfo.m_iHP;
			_countdown = 0f;
			Damage1.GetComponent<SpriteRenderer>().sprite = null;
			Damage2.GetComponent<SpriteRenderer>().sprite = null;
			Damage3.GetComponent<SpriteRenderer>().sprite = null;
			Damage4.GetComponent<SpriteRenderer>().sprite = null;
			Damage5.GetComponent<SpriteRenderer>().sprite = null;

			_emojiScaleTweener = Emoji.DOScale(Emoji.transform.localScale * 1.2f, 0.5f).SetLoops(-1, LoopType.Yoyo);
			_emojiCd = 0f;

			Message = MessageBg.GetChild(1);
			_messageCd = 0f;
		}

		private void DrawMesh() {

		}

		// Update is called once per frame
		void Update() {
			if (_character == null) {
				Start();
			}
			_bodyView.AnimationName = _character.m_pInfo.m_pCurrentStateData.animation_name;
			_bodyView.Frame = _character.m_pInfo.m_iCurrentFrame;
			this.transform.localPosition = new Vector3(_character.Position.x / 100, _character.Position.y / 100, GameViewDefine.Z_ZSORT_LAYER);
			Body.localScale = new Vector3(_character.m_pInfo.m_iFacing, 1, 1);
			float characterAngle = _character.CalcObjectAngle();
			Body.eulerAngles = new Vector3(0, 0, characterAngle);
			AttackAngle = _character.GetFireAngle() + characterAngle;
			FireAngle.eulerAngles = new Vector3(0, 0, AttackAngle);
			Hp = _character.m_pInfo.m_iHP;
			MoveEnergy = _character.m_pInfo.m_iMoveEnergy;
			AttackEnergy = _character.m_pInfo.m_iBombSpeed;
			HpProgress.Value = _character.m_pInfo.m_iHP * 1.0f / _character.m_pInfo.m_iMaxHP;
			ControllerArrows.gameObject.SetActive(_character.m_pInfo.m_bIsInRound & (_emojiCd <= 0) & (_messageCd <= 0));

			if (Mathf.Abs(Hp - _preHp) > 1f) {
				int diff = (int)(_preHp - Hp);
				_preHp = Hp;
				_countdown = 1f;
				Damage.gameObject.SetActive(true);
				ShowDamage(diff);
				Damage.localPosition = new Vector3(0, _character.m_pInfo.m_pInstructionData.be_attack_box_max_y / 100.0f + 1f, 0);
			}
			if (_countdown <= 0f) {
				Damage.gameObject.SetActive(false);
			} else {
				_countdown -= Time.deltaTime;
				Damage.localPosition += new Vector3(0f, 2f, 0f) * Time.deltaTime;
			}

			if (_character.m_pInfo.EmojiID != 0) {
				ShowEmoji(_character.m_pInfo.EmojiID);
				_character.m_pInfo.EmojiID = 0;
			}
			if (_emojiCd <= 0f) {
				Emoji.gameObject.SetActive(false);
			} else {
				_emojiCd -= Time.deltaTime;
			}
			if (_character.m_pInfo.MessageID != 0) {
				ShowMessage(_character.m_pInfo.MessageID);
				_character.m_pInfo.MessageID = 0;
			}
			if (_messageCd <= 0f) {
				MessageBg.gameObject.SetActive(false);
			} else {
				_messageCd -= Time.deltaTime;
			}
		}

		void OnDestroy() {
			// ReSharper disable once DelegateSubtraction
			SkillManager.SkillChangedHandle -= ShowSkillPrompt;
		}

		private void ShowSkillPrompt(SkillManager.SkillType skillType) {
			if (GameGOW.Get().BattleMgr.m_pCurrentPlayer != _character) {
				return;
			}

			if (skillType == SkillManager.SkillType.Normal) {
				return;
			}

			if (_skillPromptMoveTweener != null) {
				SkillPrompt.text = GameGOW.Get().DataMgr.GetSkillINstructionDataByID(100 + (int)skillType).default_name;
				SkillPrompt.gameObject.SetActive(true);
				_skillPromptMoveTweener.Restart();
			}
		}

		private void ShowEmoji(int id) {
			string path = GameGOW.Get().DataMgr.GetImageByID(id).image_source;
			Emoji.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(path);
			Emoji.gameObject.SetActive(true);
			if (_emojiScaleTweener != null) {
				_emojiScaleTweener.Restart(false);
			}
			_emojiCd = 2f;
		}
		private void ShowMessage(int id) {
			string path = GameGOW.Get().DataMgr.GetChatByID(id).chat_text;
			Message.GetComponent<TextMesh>().text = path;
			MessageBg.gameObject.SetActive(true);
			_messageCd = 2f;
		}

		private void ShowDamage(int diff) {
			int pdiff = diff < 0 ? (-diff) : (diff);//取正
			int pdiff2 = pdiff; //判断位数
			int last = 1;
			string path = "Sprites/damageNum";
			if (pdiff2 >= 0)//个位数
			{
				//Debug.Log("trytry" + diff + "trytry" + (diff % 10).ToString());
				Damage1.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(path + (pdiff % 10).ToString());
				pdiff /= 10;
				last = 2;
			} else Damage1.GetComponent<SpriteRenderer>().sprite = null;
			if (pdiff2 >= 10)//十位数
			{
				Damage2.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(path + (pdiff % 10).ToString());
				pdiff /= 10;
				last = 3;
			} else Damage2.GetComponent<SpriteRenderer>().sprite = null;
			if (pdiff2 >= 100)//百位数
			{
				Damage3.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(path + (pdiff % 10).ToString());
				pdiff /= 10;
				last = 4;
			} else Damage3.GetComponent<SpriteRenderer>().sprite = null;
			if (pdiff2 >= 1000)//千位数
			{
				Damage4.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(path + (pdiff % 10).ToString());
				pdiff /= 10;
				last = 5;
			} else Damage4.GetComponent<SpriteRenderer>().sprite = null;
			if (pdiff2 >= 10000)//万位数
			{
				Debug.LogError("damage too large!");
			} else Damage5.GetComponent<SpriteRenderer>().sprite = null;
			if (diff >= 0) {
				Transform trans = Damage1;
				if (last == 2) trans = Damage2;
				if (last == 3) trans = Damage3;
				if (last == 4) trans = Damage4;
				if (last == 5) trans = Damage5;
				trans.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/damageSign");
			} else {
				Transform trans = Damage1;
				if (last == 2) trans = Damage2;
				if (last == 3) trans = Damage3;
				if (last == 4) trans = Damage4;
				if (last == 5) trans = Damage5;
				trans.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/damageSign2");
			}
		}
	}
}