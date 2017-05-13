using UnityEngine;
using Spine.Unity;
using TKGame;
namespace TKGameView
{
    public class BombView : MonoBehaviour
    {
        enum BombState
        {
            FLY,
            BOMB
        }
        public static bool is_bomb_end = false;
        public int can_play_bomb = 0;
        public int id = 1;
        private BaseBomb m_stBombLogic = null;

        private SkeletonAnimation m_stAnimation;

        private BombState m_eState;
        public void SetBombLogic(BaseBomb logic)
        {
            m_stBombLogic = logic;
            GameObject pfb = Resources.Load("Spine/ZD" + logic.Config.m_iResourceID) as GameObject;
            GameObject aniObj = Instantiate<GameObject>(pfb);
            aniObj.transform.parent = this.transform;
            aniObj.transform.position = this.transform.position;
            m_stAnimation = aniObj.GetComponent<SkeletonAnimation>();
            m_stAnimation.AnimationName = SkillManager.cur_fly;
            m_stAnimation.loop = true;
            m_eState = BombState.FLY;
        }

        // Use this for initialization
        void Start()
        {


        }

        // Update is called once per frames
        void Update()
        {
            switch (m_eState)
            {
                case BombState.FLY:
                    OnFly();
                    break;
                case BombState.BOMB:
                    break;
            }
        }

        private void OnFly()
        {
            this.transform.localPosition = new Vector3(m_stBombLogic.Position.x / 100, m_stBombLogic.Position.y / 100, 5);
            if (m_stBombLogic.IsLiving == false && m_eState == BombState.FLY)
            {
                m_eState = BombState.BOMB;
                can_play_bomb = 1;
                m_stAnimation.loop = false;
                m_stAnimation.AnimationName = SkillManager.cur_boom;
                m_stAnimation.state.End += OnBombEnd;
                return;
            }

        }
        private void OnBombEnd(Spine.AnimationState state, int trackIndex)
        {
            m_stAnimation.state.End -= OnBombEnd;

            //xue delete
            //m_stBombLogic.Stop();
            m_stBombLogic.BombStatus++;
            Object.Destroy(this.gameObject);
        }
    }
}