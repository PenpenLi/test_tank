using UnityEngine;
using System.Collections;
namespace TKGameView
{
    public class BackgroundParallax : MonoBehaviour
    {
        private static BackgroundParallax m_stInst;
        public static BackgroundParallax Get()
        {
            return m_stInst;
        }
        public Transform[] backgrounds; //背景元素
        public float parallaxScale = 0.5f; //移动幅度
        public float parallaxReductionFactor = 0; //关联因素
        public float smoothing = 8.0f;//移动平滑程度

        private Transform _camera; //相机运动位置
        private Vector3 _preCameraPos; //相机之前的位置

        public void SetCamera()
        {
            _camera = Camera.main.transform;
            _preCameraPos = _camera.position;
            return;
        }

        void Awake()
        {
            m_stInst = this;
            _camera = Camera.main.transform;
        }
        // Use this for initialization
        void Start()
        {
            _preCameraPos = _camera.position;
        }

        // Update is called once per frame
        void Update()
        {
            float parallax = (_preCameraPos.x - _camera.position.x) * parallaxScale;
            for (int i = 0; i < backgrounds.Length; ++i)
            {
                float bgTargetX = backgrounds[i].position.x + parallax * (i * parallaxReductionFactor + 1);
                Vector3 bgTargetPos = new Vector3(bgTargetX, backgrounds[i].position.y, backgrounds[i].position.z);
                backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, bgTargetPos, smoothing * Time.deltaTime);
            }

            _preCameraPos = _camera.position;
        }
    }
}