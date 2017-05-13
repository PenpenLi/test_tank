using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TKGame;

namespace TKGameView
{
    public class SounderManager : MonoBehaviour
    {
        public void FunNet(int Bombid, bool kind, float x, float y)
        {
            StartCoroutine(DelayNet(Bombid, kind, x, y));
        }

        IEnumerator DelayNet(int Bombid, bool kind, float x, float y) //延迟发炮,delay_time 为延迟时间
        {
            while (GameGOW.Get().BombMgr.GetBombByUID(Bombid) == null)
            {
                yield return null;
            }
            GameGOW.Get().BombMgr.GetBombByUID(Bombid).Boom_pos = new Vector2(x, y);
            if (kind == false)
            {
                GameGOW.Get().BombMgr.GetBombByUID(Bombid).Net_Boom_Status = 1;
                //BaseBomb.Net_Boom_Status = 1;
            }
            else
            {
                GameGOW.Get().BombMgr.GetBombByUID(Bombid).Net_Boom_Status = 2;
                //BaseBomb.Net_Boom_Status = 2;
            }
        }

        private static SounderManager instance;
        public static SounderManager Get()
        {
            return instance;
        }
        public AudioSource EfxSource;
        public AudioSource MusicSource;
        public AudioSource SpeakSource;
        public AudioSource ElseSource; //非战斗音效 比如倒计时
        public float lowPitchRange = .95f;
        public float highPitchRange = 1.05f;
        public int yinyue = 1;
        public int yinxiao = 1;
        public int ttt = 0;
        public void Init()
        {
            SpeakSource.volume = 0.65f;
            MusicSource.volume = 0.5f; //背景音乐音量
            EfxSource.volume = 0.55f;   //音效音量
            ElseSource.volume = 0.55f;
            MusicSource.clip= GameGOW.Get().ResourceMgr.GetRes<AudioClip>("sound/Music/battlebg_4");
            MusicSource.Play();
        }
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(this);
        }
        public void PlaySound(int temp, string name)
        {
            if (temp == 1)
            {
                EfxSource.clip=GameGOW.Get().ResourceMgr.GetRes<AudioClip>(name);
                EfxSource.Play();
            }
            else if (temp == 0)
            {
                if(name=="music")
                {
                    MusicSource.Stop();
                }
                else
                {
                    EfxSource.Stop();
                }
            }
            else if(temp==-1)
            {
                MusicSource.Stop();
                MusicSource.clip= GameGOW.Get().ResourceMgr.GetRes<AudioClip>(name);
                MusicSource.Play();
            }
            else if(temp==2)
            {
                SpeakSource.clip = GameGOW.Get().ResourceMgr.GetRes<AudioClip>(name);
                SpeakSource.Play();
            }
            else if(temp==3)
            {
                ElseSource.clip = GameGOW.Get().ResourceMgr.GetRes<AudioClip>(name);
                ElseSource.Play();
            }
        }
        public void SetSound(int index, int type)
        {
            if (index == 0)
            {
                MusicSource.volume = type == 1 ? 0.5f : 0f;//index :0.背景音乐 1.音效 type:1开 0关 
                yinyue = type;
            }
            else
            {
                EfxSource.volume = type == 1 ? 0.55f : 0f;
                yinxiao = type;
            }
        }
        void Start()
        {
            Init();
        }
        private void OnDestroy()
        {

        }
    }
}
