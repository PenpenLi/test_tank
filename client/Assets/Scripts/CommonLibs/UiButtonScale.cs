using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

// ReSharper disable once CheckNamespace
namespace TKBase {

	public class UiButtonScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
		public float ScaleRatio = 0.97f;
		public float Duration = 0.07f;

		private Vector3 _initScale;

		void Start() {
			_initScale = transform.localScale;
		}

		public void OnPointerDown(PointerEventData eventData) {
			transform.DOScale(_initScale * ScaleRatio, Duration);
		}

		public void OnPointerUp(PointerEventData eventData) {
			transform.DOScale(_initScale, Duration);
		}
	}

}
