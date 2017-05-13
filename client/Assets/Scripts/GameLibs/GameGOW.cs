#define xue

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TKBase;

namespace TKGame
{
    public class GameGOW : MonoBehaviour
    {
        private static GameGOW m_stInst;
        public static GameGOW Get()
        {
            return m_stInst;
        }

        //public delegate void SoundPlay(int method, string soundtype);
        //public event SoundPlay OnSoundPlay;

        //public void PlaySound(int method, string soundtype)
        //{
        //    if (OnSoundPlay != null) OnSoundPlay(method, soundtype);
        //}

        //public delegate void SetSound(int index, int typ);
        //public event SetSound OnSoundSet;
        //public void SoundSet(int index, int type)
        //{
        //    if (OnSoundSet != null)
        //    {
        //        OnSoundSet(index, type);
        //    }
        //}

        /*
         *method        修改类型 ： 1 有无风，2 改角度（可以理解为力度）
         * is_live      有无风   ： true 有， false 无
         * A            风的角度  （风力的表现）  
         */
        public delegate void WindPlay(int method, float A, bool is_live);
        public event WindPlay OnWindPlay;

        public void PlayWind(int method, float A, bool is_live)
        {
            if (OnWindPlay != null) OnWindPlay(method, A, is_live);
        }
        public delegate void SkillPlay(SkillManager.SkillType temp, CharacterLogic A, AttackBaseData AA);
        public event SkillPlay OnSkillPlay;
        /*
         * temp：one_bomb two_bomb three_bomb save_bomb paper_plane thunder_bomb
         *       常规炮     双炮    散炮         疗伤炮   纸飞机       轰天雷
         */
        public void PlaySkill(SkillManager.SkillType temp, CharacterLogic A, AttackBaseData AA)
        {
            if (OnSkillPlay != null) OnSkillPlay(temp, A, AA);
        }
        private int winner_id = -1;
        public int Winner_Id
        {
            get { return winner_id; }
            set { winner_id = value; }
        }

        private int time = 0;
        public int match_time
        {
            get { return time; }
            set { time = value; }
        }


        Dictionary<ManagerType, ManagerBase> m_dicMgrs;
        private TickerManager m_pTickerMgr;
        public TickerManager TickerMgr
        {
            get { return m_pTickerMgr; }
        }



        private ResourceManager m_pResourceMgr;
        public ResourceManager ResourceMgr
        {
            get { return m_pResourceMgr; }
        }
        private AIManager m_pAIMgr;
        public AIManager AIMgr
        {
            get { return m_pAIMgr; }
        }
        private CharacterManager m_pCharacterMgr;
        public CharacterManager CharacterMgr
        {
            get { return m_pCharacterMgr; }
        }
        private MapManager m_pMapMgr;
        public MapManager MapMgr
        {
            get { return m_pMapMgr; }
        }
        private BombManager m_pBombMgr;
        public BombManager BombMgr
        {
            get { return m_pBombMgr; }
        }

        private DataManager m_pDataMgr;
        public DataManager DataMgr
        {
            get { return m_pDataMgr; }
        }

        private BattleManager m_pBattleMgr;
        public BattleManager BattleMgr
        {
            get { return m_pBattleMgr; }
        }
        private MapData m_pCurrentMapData;
        public MapData CurrentMapData
        {
            get { return m_pCurrentMapData; }
        }
        private SkillManager m_pSkillMgr;
        public SkillManager SkillMgr
        {
            get { return m_pSkillMgr; }
        }


        void Awake()
        {
            m_stInst = this;
            GameConfig.mode = GameConfig.MODE_DEBUG;

            m_dicMgrs = new Dictionary<ManagerType, ManagerBase>();

            ManagerBase pMgr = null;
            pMgr = m_pTickerMgr = new TickerManager();
            m_dicMgrs[pMgr.Type] = pMgr;

            pMgr = m_pResourceMgr = new ResourceManager();
            m_dicMgrs[pMgr.Type] = pMgr;



            pMgr = m_pAIMgr = new AIManager();
            m_dicMgrs[pMgr.Type] = pMgr;

            pMgr = m_pCharacterMgr = new CharacterManager();
            m_dicMgrs[pMgr.Type] = pMgr;

            pMgr = m_pMapMgr = new MapManager();
            m_dicMgrs[pMgr.Type] = pMgr;

            pMgr = m_pBombMgr = new BombManager();
            m_dicMgrs[pMgr.Type] = pMgr;


            pMgr = m_pDataMgr = new DataManager();
            m_dicMgrs[pMgr.Type] = pMgr;

            pMgr = m_pBattleMgr = new BattleManager();
            m_dicMgrs[pMgr.Type] = pMgr;


            int count = (int)ManagerType.MAX_MANAGER_COUNT;
            for (int i = 0; i < count; ++i)
            {
                ManagerType eType = (ManagerType)i;
                if (m_dicMgrs.ContainsKey(eType))
                {
                    m_dicMgrs[eType].Initialize();
                }
            }

            m_iMapUniqueID = 0;

        }

        void OnDestroy()
        {
            int count = (int)ManagerType.MAX_MANAGER_COUNT;
            for (int i = count - 1; i >= 0; --i)
            {
                ManagerType eType = (ManagerType)i;
                if (m_dicMgrs.ContainsKey(eType))
                {
                    m_dicMgrs[eType].UnInitialize();
                }
            }
            Network.Instance.Disonnect();
        }

        private int m_iMapUniqueID;
        public int GetNextUid()
        {
            return ++m_iMapUniqueID;
        }

        public void SetNextUiqueID()
        {
            m_iMapUniqueID = 0;
        }

        public ManagerBase GetMgr(ManagerType eType)
        {
            return m_dicMgrs[eType];
        }

        // Use this for initialization
        void Start()
        {
            LOG.m_bIsHideLog = true;
            Application.runInBackground = true;
            m_pTickerMgr.Start();
            if (GameConfig.mode == GameConfig.MODE_DEBUG)
            {
                //gameObject.AddComponent<TestController>();
                //gameObject.AddComponent<GameTest>();
            }

            m_pDataMgr.LoadAllData();
            Network.Start_xue();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            m_pTickerMgr.Signal(Time.deltaTime);
            Network.Instance.Update();
        }

        public void SaveGameInfo()
        {
        }

        private void Update()
        {
            CheckLoading();
            if (Network.NetworkMode == true)
            {
                Network.Heart = Network.Heart + Time.deltaTime;
                if (Network.Heart > 30)
                {
                    Network.Send_Heart();
                    Network.Heart = 0;
                }
            }

        }

        public static BattleInfo battleinfo = new BattleInfo();
        public void CheckLoading()
        {
            Network.GameLoading_Cnt = 1000;
            if (Network.GameLoading == 0) return;
            if (Network.GameLoading >= 1)
            {
                Network.GameLoading_Cnt++;
            }

            if (Network.GameLoading == 1 && Network.GameLoading_Cnt > 120)
            {
                Network.GameLoading_Cnt = 120;
                m_pTickerMgr.Clear();
                m_pMapMgr.Clear();
                m_pCharacterMgr.Clear();
                m_pBattleMgr.Clear();

                EventDispatcher.DispatchEvent("EventBeforeSwitchMap", null, null);

                m_pCurrentMapData = m_pDataMgr.GetMapDataByID(battleinfo.m_iMapIndex);   //读取配置文件

                if (Network.gamemode == BattleType.AI_1v1)
                {
                    Network.GameLoading_num[1] = UnityEngine.Random.Range((Network.GameLoading + 1) * 10, (Network.GameLoading + 3) * 10);
                    Network.GameLoading = 3;
                }
                else if (Network.NetworkMode == true)
                {
                    Network.Send_GameLoading(UnityEngine.Random.Range((Network.GameLoading + 1) * 10, (Network.GameLoading + 3) * 10));
                    Network.GameLoading++;
                }
                else
                {
                    Network.GameLoading_num[1] = UnityEngine.Random.Range((Network.GameLoading + 1) * 10, (Network.GameLoading + 3) * 10);
                    Network.GameLoading = 3;
                }
            }
            else if (Network.GameLoading == 3 && Network.GameLoading_Cnt > 160)
            {
                Network.GameLoading_Cnt = 160;
                m_pMapMgr.SetData(m_pCurrentMapData);          //加载地图资源  

                if (Network.gamemode == BattleType.AI_1v1)
                {
                    Network.GameLoading_num[1] = UnityEngine.Random.Range((Network.GameLoading + 1) * 10, (Network.GameLoading + 3) * 10);
                    Network.GameLoading = 5;
                }
                else if (Network.NetworkMode == true)
                {
                    Network.Send_GameLoading(UnityEngine.Random.Range((Network.GameLoading + 1) * 10, (Network.GameLoading + 3) * 10));
                    Network.GameLoading++;
                }
                else
                {
                    Network.GameLoading_num[1] = UnityEngine.Random.Range((Network.GameLoading + 1) * 10, (Network.GameLoading + 3) * 10);
                    Network.GameLoading = 5;
                }
            }
            else if (Network.GameLoading == 5 && Network.GameLoading_Cnt > 190)
            {
                Network.GameLoading_Cnt = 190;
                m_pCharacterMgr.SetData(m_pCurrentMapData);    //加载坦克资源

                if (Network.gamemode == BattleType.AI_1v1)
                {
                    Network.GameLoading_num[1] = UnityEngine.Random.Range((Network.GameLoading + 1) * 10, (Network.GameLoading + 3) * 10);
                    Network.GameLoading = 7;
                }
                else if (Network.NetworkMode == true)
                {
                    Network.Send_GameLoading(UnityEngine.Random.Range((Network.GameLoading + 1) * 10, (Network.GameLoading + 3) * 10));
                    Network.GameLoading++;
                }
                else
                {
                    Network.GameLoading_num[1] = UnityEngine.Random.Range((Network.GameLoading + 1) * 10, (Network.GameLoading + 3) * 10);
                    Network.GameLoading = 7;
                }
            }
            else if (Network.GameLoading == 7 && Network.GameLoading_Cnt > 260)
            {
                Network.GameLoading_Cnt = 260;
                EventDispatcher.DispatchEvent("EventAfterSwitchMap", null, null);    //加载背景

                if (Network.gamemode == BattleType.AI_1v1)
                {
                    Network.GameLoading_num[1] = 100;
                    Network.GameLoading = 9;
                }
                else if (Network.NetworkMode == true)
                {
                    Network.Send_GameLoading(100);
                    Network.GameLoading++;
                }
                else
                {
                    Network.GameLoading_num[1] = 100;
                    Network.GameLoading = 9;
                }
            }
            else if (Network.GameLoading == 9 && Network.GameLoading_Cnt > 340)
            {
                Network.GameLoading_Cnt = 340;
                //m_pBattleMgr.StartBattle(battleinfo, m_pCurrentMapData);
                //m_pMyPlayer = m_pBattleMgr.m_pCurrentPlayer;
                if (Network.gamemode == BattleType.AI_1v1)
                {
                    Network.GameLoading = 10;
                }
                else if (Network.NetworkMode == true)
                {

                }
                else
                {
                    Network.GameLoading = 10;
                }
            }
        }

        public void Start_Game()
        {
            SetNextUiqueID();
            m_pBattleMgr.StartBattle(battleinfo, m_pCurrentMapData);
            m_pMyPlayer = m_pBattleMgr.m_pCurrentPlayer;
            Network.GameLoading = 0;
        }

        public void SwitchMap(BattleInfo info)
        {
            //m_pTickerMgr.Clear();
            //m_pMapMgr.Clear();
            //m_pCharacterMgr.Clear();
            //m_pBattleMgr.Clear();

            //EventDispatcher.DispatchEvent("EventBeforeSwitchMap", null, null);

            //m_pCurrentMapData = m_pDataMgr.GetMapDataByID(info.m_iMapIndex);
            //m_pMapMgr.SetData(m_pCurrentMapData);
            //m_pCharacterMgr.SetData(m_pCurrentMapData);

            //EventDispatcher.DispatchEvent("EventAfterSwitchMap", null, null);
            //m_pBattleMgr.StartBattle(info, m_pCurrentMapData);
            //m_pMyPlayer = m_pBattleMgr.m_pCurrentPlayer;
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
            m_pTickerMgr.Pause();
        }

        public void ResumeGame()
        {
            m_pTickerMgr.Resume();
        }

        public void StartGame()
        {
            Time.timeScale = 1;
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        private CharacterLogic m_pMyPlayer;
        public CharacterLogic MyPlayer
        {
            get { return m_pMyPlayer; }
        }

        #region(测试)
        public void TestDemo(int iMapIndex)
        {
            Network.InitRound();
            BattleInfo info = new BattleInfo();
            info.m_iMapIndex = iMapIndex;
            info.m_eBattleType = BattleType.NORMAL_PVP;
            info.m_iRoundLimit = 10;
            info.m_iTimeLimit = GameDefine.INFINITE;

            PlayerInfo p1 = new PlayerInfo();

            p1.m_iPlayerUID = 1;
            p1.m_iTeam = 1;
            p1.m_strName = "P1";
            p1.m_iInstructionID = 300001;
            p1.m_iHP = GameGOW.Get().DataMgr.GetCharacterInstructionByID(p1.m_iInstructionID).m_iHP;
            p1.m_iMoveEnergy = GameGOW.Get().DataMgr.GetCharacterInstructionByID(p1.m_iInstructionID).m_iMoveEnergy;
            p1.m_iAddEnergy = GameGOW.Get().DataMgr.GetCharacterInstructionByID(p1.m_iInstructionID).m_iAddEnergy;

            if (Network.gamemode == BattleType.AI_1v1)
            {
                p1.m_strName = Network.name;
                p1.m_iPlayerUID = Network.playerid;
                p1.m_iInstructionID = Network.Player_tank * 100000+1;
                p1.m_iHP = GameGOW.Get().DataMgr.GetCharacterInstructionByID(p1.m_iInstructionID).m_iHP;
                p1.m_iMoveEnergy = GameGOW.Get().DataMgr.GetCharacterInstructionByID(p1.m_iInstructionID).m_iMoveEnergy;
                p1.m_iAddEnergy = GameGOW.Get().DataMgr.GetCharacterInstructionByID(p1.m_iInstructionID).m_iAddEnergy;
                if(Network.Player_tank == 3)
                {
                    Network.default_skill = (int)SkillManager.SkillType.Rocket;
                }
                else
                {
                    Network.default_skill = (int)SkillManager.SkillType.TripleBomb;
                }

                //while ((Network.skill1 = UnityEngine.Random.Range(1, 7)) == 4) ;
                while (true)
                {
                    Network.skill1 = UnityEngine.Random.Range(1, 8);
                    if (Network.skill1!=4  && Network.skill1!= (int)SkillManager.SkillType.Rocket && Network.skill1 != (int)SkillManager.SkillType.TripleBomb)
                    {
                        break;
                    }
                }
            }
            else if (Network.NetworkMode == true)
            {
                info.m_iMapIndex = Network.battleinfo.m_iMapIndex;

                p1.m_iPlayerUID = Network.battleinfo.m_listPlayers[0].m_iPlayerUID;
                p1.m_iInstructionID = Network.battleinfo.m_listPlayers[0].m_iInstructionID;
                p1.m_iTeam = Network.battleinfo.m_listPlayers[0].m_iTeam;
                p1.m_strName = Network.battleinfo.m_listPlayers[0].m_strName;
            }
            info.m_listPlayers.Add(p1);


            PlayerInfo p2 = new PlayerInfo();

            p2.m_iPlayerUID = 2;
            p2.m_iTeam = 2;
            p2.m_strName = "P2";
            p2.m_iInstructionID = 400001;
            p2.m_iHP = GameGOW.Get().DataMgr.GetCharacterInstructionByID(p2.m_iInstructionID).m_iHP;
            p2.m_iMoveEnergy = GameGOW.Get().DataMgr.GetCharacterInstructionByID(p2.m_iInstructionID).m_iMoveEnergy;
            p2.m_iAddEnergy = GameGOW.Get().DataMgr.GetCharacterInstructionByID(p2.m_iInstructionID).m_iAddEnergy;

            if (Network.gamemode == BattleType.AI_1v1)
            {
                p2.m_strName = "心是六月的情";
                p2.m_iInstructionID = Network.Ai_tank * 100000 + 1;
                p2.m_iHP = GameGOW.Get().DataMgr.GetCharacterInstructionByID(p2.m_iInstructionID).m_iHP;
                p2.m_iMoveEnergy = GameGOW.Get().DataMgr.GetCharacterInstructionByID(p2.m_iInstructionID).m_iMoveEnergy;
                p2.m_iAddEnergy = GameGOW.Get().DataMgr.GetCharacterInstructionByID(p2.m_iInstructionID).m_iAddEnergy;

                p2.m_iPlayerUID = Network.playerid + 1;
                Network.playerid2 = p2.m_iPlayerUID;
                p2.m_iHP *= 2;
                p2.m_iMoveEnergy *= 10;
            }
            else if (Network.NetworkMode == true)
            {
                p2.m_iPlayerUID = Network.battleinfo.m_listPlayers[1].m_iPlayerUID;
                p2.m_iInstructionID = Network.battleinfo.m_listPlayers[1].m_iInstructionID;
                p2.m_iTeam = Network.battleinfo.m_listPlayers[1].m_iTeam;
                p2.m_strName = Network.battleinfo.m_listPlayers[1].m_strName;
            }

            info.m_listPlayers.Add(p2);

            if (Network.gamemode == BattleType.PVP_2V2 || Network.gamemode == BattleType.PVP_3V3)
            {
                info.m_iMapIndex = 200 + info.m_iMapIndex;
                //info.m_iMapIndex = 201;
            }

            if (info.m_iMapIndex >= 201 || (info.m_iMapIndex > 300 && info.m_iMapIndex <= 303))
            {
                PlayerInfo p3 = new PlayerInfo();

                p3.m_iPlayerUID = 3;
                p3.m_iTeam = 1;
                p3.m_strName = "p3";
                p3.m_iInstructionID = 100001;
                p3.m_iHP = GameGOW.Get().DataMgr.GetCharacterInstructionByID(p3.m_iInstructionID).m_iHP;
                p3.m_iMoveEnergy = GameGOW.Get().DataMgr.GetCharacterInstructionByID(p3.m_iInstructionID).m_iMoveEnergy;
                p3.m_iAddEnergy = GameGOW.Get().DataMgr.GetCharacterInstructionByID(p3.m_iInstructionID).m_iAddEnergy;

                if (Network.gamemode == BattleType.PVP_2V2 || Network.gamemode == BattleType.PVP_3V3)
                {
                    p3.m_iPlayerUID = Network.battleinfo.m_listPlayers[2].m_iPlayerUID;
                    p3.m_iInstructionID = Network.battleinfo.m_listPlayers[2].m_iInstructionID;
                    p3.m_iTeam = Network.battleinfo.m_listPlayers[2].m_iTeam;
                    p3.m_strName = Network.battleinfo.m_listPlayers[2].m_strName;
                }

                info.m_listPlayers.Add(p3);

                PlayerInfo p4 = new PlayerInfo();

                p4.m_iPlayerUID = 4;
                p4.m_iTeam = 2;
                p4.m_strName = "p4";
                p4.m_iInstructionID = 200001;
                p4.m_iHP = GameGOW.Get().DataMgr.GetCharacterInstructionByID(p4.m_iInstructionID).m_iHP;
                p4.m_iMoveEnergy = GameGOW.Get().DataMgr.GetCharacterInstructionByID(p4.m_iInstructionID).m_iMoveEnergy;
                p4.m_iAddEnergy = GameGOW.Get().DataMgr.GetCharacterInstructionByID(p4.m_iInstructionID).m_iAddEnergy;

                if (Network.gamemode == BattleType.PVP_2V2 || Network.gamemode == BattleType.PVP_3V3)
                {
                    p4.m_iPlayerUID = Network.battleinfo.m_listPlayers[3].m_iPlayerUID;
                    p4.m_iInstructionID = Network.battleinfo.m_listPlayers[3].m_iInstructionID;
                    p4.m_iTeam = Network.battleinfo.m_listPlayers[3].m_iTeam;
                    p4.m_strName = Network.battleinfo.m_listPlayers[3].m_strName;
                }

                info.m_listPlayers.Add(p4);

            }

            if (Network.gamemode == BattleType.PVP_3V3)
            {
                info.m_iMapIndex = 100 + info.m_iMapIndex;
                //info.m_iMapIndex = 301;
            }

            if (info.m_iMapIndex > 300 && info.m_iMapIndex <= 303)
            {
                PlayerInfo p5 = new PlayerInfo();

                p5.m_iPlayerUID = 5;
                p5.m_iTeam = 1;
                p5.m_strName = "p5";
                p5.m_iInstructionID = 100001;
                p5.m_iHP = GameGOW.Get().DataMgr.GetCharacterInstructionByID(p5.m_iInstructionID).m_iHP;
                p5.m_iMoveEnergy = GameGOW.Get().DataMgr.GetCharacterInstructionByID(p5.m_iInstructionID).m_iMoveEnergy;
                p5.m_iAddEnergy = GameGOW.Get().DataMgr.GetCharacterInstructionByID(p5.m_iInstructionID).m_iAddEnergy;

                if (Network.gamemode == BattleType.PVP_3V3)
                {
                    p5.m_iPlayerUID = Network.battleinfo.m_listPlayers[4].m_iPlayerUID;
                    p5.m_iInstructionID = Network.battleinfo.m_listPlayers[4].m_iInstructionID;
                    p5.m_iTeam = Network.battleinfo.m_listPlayers[4].m_iTeam;
                    p5.m_strName = Network.battleinfo.m_listPlayers[4].m_strName;
                }

                info.m_listPlayers.Add(p5);

                PlayerInfo p6 = new PlayerInfo();

                p6.m_iPlayerUID = 6;
                p6.m_iTeam = 2;
                p6.m_strName = "p6";
                p6.m_iInstructionID = 200001;
                p6.m_iHP = GameGOW.Get().DataMgr.GetCharacterInstructionByID(p6.m_iInstructionID).m_iHP;
                p6.m_iMoveEnergy = GameGOW.Get().DataMgr.GetCharacterInstructionByID(p6.m_iInstructionID).m_iMoveEnergy;
                p6.m_iAddEnergy = GameGOW.Get().DataMgr.GetCharacterInstructionByID(p6.m_iInstructionID).m_iAddEnergy;

                if (Network.gamemode == BattleType.PVP_3V3)
                {
                    p6.m_iPlayerUID = Network.battleinfo.m_listPlayers[5].m_iPlayerUID;
                    p6.m_iInstructionID = Network.battleinfo.m_listPlayers[5].m_iInstructionID;
                    p6.m_iTeam = Network.battleinfo.m_listPlayers[5].m_iTeam;
                    p6.m_strName = Network.battleinfo.m_listPlayers[5].m_strName;
                }

                info.m_listPlayers.Add(p6);
            }


            if (Network.gamemode == BattleType.AI_1v1 || Network.NetworkMode == false)
            {
                //Network.playerid = 1;
                Network.playerindex = 0;
                Network.battleinfo = info;
            }


            battleinfo = info;

            Network.GameLoading_Cnt = 0;
            Network.GameLoading = 0;
            for (int i = 1; i <= 10; i++) Network.GameLoading_num[i] = 0;
            if (Network.gamemode == BattleType.AI_1v1) {
                Network.GameLoading_num[1] = UnityEngine.Random.Range(10, (Network.GameLoading + 1) * 20);
                Network.GameLoading = 1;
            }
            else if (Network.NetworkMode == true)
            {
                Network.Send_GameLoading(UnityEngine.Random.Range(10, (Network.GameLoading + 1) * 20));
            }
            else
            {
                Network.GameLoading_num[1] = UnityEngine.Random.Range(10, (Network.GameLoading + 1) * 20);
                Network.GameLoading = 1;
            }


            //SwitchMap(info);
        }
        #endregion
    }
}
