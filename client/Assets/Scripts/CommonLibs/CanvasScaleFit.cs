using UnityEngine;
using UnityEngine.UI;

public class CanvasScaleFit : MonoBehaviour {

	void Start() {
		CanvasScaler canvasScaler = transform.GetComponent<CanvasScaler>();

		if (!canvasScaler || canvasScaler.screenMatchMode != CanvasScaler.ScreenMatchMode.MatchWidthOrHeight) {
			return;
		}

		//Canvas宽高比 
		float canvasAspect = canvasScaler.referenceResolution.x / canvasScaler.referenceResolution.y;

		//屏幕宽高比
		float deviceAspect = Screen.width * 1.0f / Screen.height;

		//画布宽高比大于屏幕宽高比，即画布较宽，适应屏幕时应该根据宽度进行缩放 
		if (canvasAspect > deviceAspect) {
			canvasScaler.matchWidthOrHeight = 0;
			return;
		}

		//画布宽高比小于屏幕宽高比，即画布较高，适应屏幕时应该根据宽度进行缩放
		if (canvasAspect  < deviceAspect) {
			canvasScaler.matchWidthOrHeight = 1;
		}
	}
}
