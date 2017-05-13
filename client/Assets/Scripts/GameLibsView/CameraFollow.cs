using UnityEngine;
using System.Collections;
using TKBase;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace TKGameView {
	public class CameraFollow : MonoBehaviour {
		enum MouseType { LEFT = 0, RIGHT };

		public float xMargin = 1.0f; //到焦点的X轴距离
		public float yMargin = 1.0f; //到焦点的Y轴距离
		public float xSmooth = 8.0f; //水平方向平滑移动参数
		public float ySmooth = 8.0f; //垂直方向平滑移动参数
		public Vector2 maxXAndY = new Vector2(1000, 30);//摄像机可移动到的右上角位置
		public Vector2 minXAndY;//摄像机可移动到的左下角位置

		public Transform focus;//焦点
		public float DragSpeed = 10f;

		private bool m_bIsDrag;
		private Vector3 m_originDragPos;

		private Transform _thisTransform;
		private float _prePostionX;

		public Transform BgTransform;
		public float BgMoveSpeed;
		[NonSerialized]
		public float BgWidth;
		[NonSerialized]
		public float TerrainWidth;

		private Vector3 _bgInitPostion;
		private Vector3 _cameraInitPostion;

		bool CheckXMargin() {
			return Mathf.Abs(transform.position.x - focus.position.x) > xMargin;
		}
		bool CheckYMargin() {
			return Mathf.Abs(transform.position.y - focus.position.y - 0.145f) > yMargin;
		}

		void FixedUpdate() {

		}

		void FollowFocus() {
			float targetX = focus.position.x;
			float targetY = focus.position.y + 0.145f;
			if (CheckXMargin()) {
				targetX = Mathf.Lerp(_thisTransform.position.x, targetX, xSmooth * Time.deltaTime);
			}
			if (CheckYMargin()) {
				targetY = Mathf.Lerp(_thisTransform.position.y, targetY, ySmooth * Time.deltaTime);
			}

			targetX = Mathf.Clamp(targetX, minXAndY.x, maxXAndY.x);
			targetY = Mathf.Clamp(targetY, minXAndY.y, maxXAndY.y);
			_thisTransform.position = new Vector3(targetX, targetY, _thisTransform.position.z);

		}
		// Use this for initialization
		void Start() {
			m_bIsDrag = false;
			_thisTransform = transform;
			_prePostionX = _thisTransform.position.x;
			_bgInitPostion = BgTransform.position;
			_cameraInitPostion = _thisTransform.position;
		}

		void OnDestroy() {
		}

		void DragCamera() {
			DragSpeed = 10f;
			//  Debug.Log("WWWQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQ"+DragSpeed);
			Vector3 nowMousePos = Input.mousePosition;
			Vector3 move = nowMousePos - m_originDragPos;
			move = Camera.main.ScreenToViewportPoint(move) * DragSpeed * -1;
			//平移没有差值运算
			_thisTransform.Translate(move);
			float x = Mathf.Clamp(_thisTransform.position.x, minXAndY.x, maxXAndY.x);
			float y = Mathf.Clamp(_thisTransform.position.y, minXAndY.y, maxXAndY.y);
			Vector3 pos = new Vector3(x, y, _thisTransform.position.z);
			_thisTransform.position = pos;
			m_originDragPos = nowMousePos;
			if (Vector3.Dot(move, move) > 0)
				focus = null;
		}

		// Update is called once per frame
		void Update() {
			int mouse = (int)MouseType.LEFT;
			//记录某一帧时按下的状态(之后的持续按下都返回false，知道下次释放在按下返回true)
			if (Input.GetMouseButtonDown(mouse)) {
				//不能是UI层
				PointerEventData pointerData = new PointerEventData(EventSystem.current);
				pointerData.position = Input.mousePosition;
				List<RaycastResult> results = new List<RaycastResult>();
				EventSystem.current.RaycastAll(pointerData, results);

				if (results.Count > 0) {
					if (results[0].gameObject.layer == LayerMask.NameToLayer("UI")) {
						return;
					}
				}
				m_bIsDrag = true;
				//屏幕坐标系
				m_originDragPos = Input.mousePosition;
				return;
			}
			//表示当前的释放
			if (!Input.GetMouseButton(mouse)) {
				m_bIsDrag = false;
				return;
			}
		}

		void LateUpdate() {
			if (m_bIsDrag) {
				DragCamera();
			}

			if (focus != null) {
				FollowFocus();
			}

			if (!Mathf.Approximately(_thisTransform.position.x, _prePostionX)) {
				MoveBg();
			}
		}

		private void MoveBg() {
			var xOffset = (_prePostionX - _thisTransform.position.x) * (BgWidth - TerrainWidth) / (TerrainWidth - 1400);
			BgTransform.position = new Vector3(BgTransform.position.x + xOffset, BgTransform.position.y, BgTransform.position.z);
			_prePostionX = _thisTransform.position.x;
		}

		public void SetFocus(Transform target) {
			if (target == null) {
				LOG.Warning("[CameraFollow][SetFocus] the focus is null");
				return;
			}
			focus = target;
			BgTransform.position = _bgInitPostion;
			_thisTransform.position = _cameraInitPostion;
			_prePostionX = _thisTransform.position.x;
		}

		public void DebugTrans(Transform target) {
			if (target == focus) {
				//Debug.Log(" x: " + target.position.x + " y: " + target.position.y);
				// Debug.Log(" oox: " + focus.position.x + " ooy: " + focus.position.y);
			}
		}
	}
}