using UnityEngine;
using System.Collections.Generic;
namespace TKGame
{
    public class MapManager:ManagerBase
    {
        public delegate void AddTerrainCell(Texture2D texture, MapCellData pData);
        public event AddTerrainCell OnTerrainCellNew;

        public Rect m_stBound;
        public List<Tile> m_listTerrains;
        public List<Tile> m_listStones;
        public float Wind = 0;
        private float wind_range = 0;
        private float wind_step = 0;
        public float Gravity = -0.98f;
        public float AirResistance = 0;

        private bool _mapChanged = false;
        public bool mapChanged
        {
            get { return _mapChanged; }
        }

        private List<TKPhysicalObj> m_lstObjectInMap;
        public MapManager()
            :base(ManagerType.TerrainManager)
        {
            m_lstObjectInMap = new List<TKPhysicalObj>();
            m_stBound.Set(0, 0, 2000, 2000);
        }

        public override void Initialize()
        {
            base.Initialize();
            m_listTerrains = new List<Tile>();
            m_listStones = new List<Tile>();
        }

        public override void UnInitialize()
        {
            m_listTerrains.Clear();
            m_listTerrains = null;

            m_listStones.Clear();
            m_listStones = null;

            base.UnInitialize();
        }

        public float WindPercent
        {
            get {
                if (wind_range > 0)
                {
                     return Wind / wind_range;
                }
                else
                {
                    return 0;
                }
            }
            
        }


        public void RandomWind()
        {
            if (Network.gamemode == BattleType.AI_1v1) {
                float flag = ((GOWRandom.GetRandom() & 1) << 1) - 1;
                if (Wind > -300 && Wind < 300)
                {
                    Wind += wind_step * GOWRandom.GetRandom() / 1000.0f * flag * 0.5f;
                }
                else
                {
                    Wind += wind_step * GOWRandom.GetRandom() / 1000.0f * flag * 1.2f;
                }
            }
            else if (Network.NetworkMode == true)
            {
                if(Wind>-300 && Wind<300)
                {
                    Wind += wind_step*Network.Wind / 1000.0f *0.5f;
                }
                else
                {
                    Wind += wind_step * Network.Wind / 1000.0f * 1.2f;
                }
               
                
            }
            else
            {
                float flag= ((GOWRandom.GetRandom() & 1) << 1) - 1;
                if (Wind > -300 && Wind < 300)
                {
                    Wind += wind_step * GOWRandom.GetRandom() / 1000.0f * flag * 0.5f;
                }
                else
                {
                    Wind += wind_step * GOWRandom.GetRandom() / 1000.0f * flag * 1.2f;
                }

            }

            if (Wind > wind_range)
            {
                Wind %= wind_range;
                Wind = -Wind;
            }
            else if (Wind < -wind_range)
            {
                Wind = -Wind;
                Wind %= -wind_range;
            }
            //Wind = -wind_range;
        }

        public void SetBound(int x, int y, int width, int height)
        {
            m_stBound.Set(x, y, width, height);
        }

        public bool InInGround(Vector3 position)
        {
            return !IsEmpty((int)position.x - 1, (int)position.y) || !IsEmpty((int)position.x + 1, (int)position.y);
        }

        public bool IsEmpty(int x, int y)
        {
            for(int i = 0; i < m_listTerrains.Count; ++i)
            {
                float alpha = m_listTerrains[i].GetAlpha(x, y);
                if (alpha > 0.95)
                {
                    return false;
                }
            }
            for(int i = 0; i < m_listStones.Count; ++i)
            {
                float alpha = m_listStones[i].GetAlpha(x, y);
                if (alpha > 0.95)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsOutMap(Vector2 pos)
        {
            return !m_stBound.Contains(pos);
        }

        public bool CanMove(int x, int y)
        {
            return IsEmpty(x, y) && !IsOutMap(new Vector2(x, y));
        }
        
        public Vector2 FindYLineNotEmptyPointUp(int x, int y, int h)
        {
            if (x <= m_stBound.xMin)
            {
                x = (int)m_stBound.xMin;
            }
            else if (x >= m_stBound.xMax)
            {
                x = (int)m_stBound.xMax - 1;
            }
            /*
            if (y <= m_stBound.yMin)
            {
                y = (int)m_stBound.yMin;
            }
            if (y + h >= m_stBound.yMax)
            {
                h = (int)m_stBound.yMax - y - 1;
            }*/
            if (y - h <= m_stBound.yMin)
            {
                h = y - (int)m_stBound.yMin;
            }
            if (y >= m_stBound.yMax)
            {
                y = (int)m_stBound.yMax;
            }

            for (int i = 0; i < h; ++i)
            {
                /*
                bool flag = false;
                int xnow = x;
                while (xnow != endx)
                {
                    if (IsEmpty(xnow, y + 1))
                    flag = true;
                }
                if (!IsEmpty(x, y + 1))
                {
                    return new Vector2(x, y);
                }*/
                if (!IsEmpty(x - 1, y) || !IsEmpty(x + 1, y))
                {
                    if (IsEmpty(x, y + 2))
                    {
                        return new Vector2(x, y);//超过90度
                    }
                    else
                    {
                        return Vector2.zero;
                    }
                }
                --y;
            }
            return Vector2.zero;
        }
        public Vector2 FindYLineNotEmptyPointUp_(int x, int y, int h)
        {
        
            if (x <= m_stBound.xMin)
            {
                x = (int)m_stBound.xMin;
            }
            else if (x >= m_stBound.xMax)
            {
                x = (int)m_stBound.xMax - 1;
            }
            if (y - h <= m_stBound.yMin)
            {
                h = y - (int)m_stBound.yMin;
            }
            if (y >= m_stBound.yMax)
            {
                y = (int)m_stBound.yMax;
            }
            ++y;
            for (int i = 0; i < h; ++i)
            {
                if (!IsEmpty(x, y-1) )
                {
                   
                    Vector2 p = new Vector2(x, y);
                    if(!IsOutMap(p) &&  IsEmpty(x,y))
                    {
                        return new Vector2(x, y);
                    }   
                }
                --y;
            }
            return Vector2.zero;
        }
        public Vector2 FindYLineNotEmptyPointDown(int x, int y, int h)
        {
            if (x <= 0)
            {
                x = 0;
            }
            else if (x >= m_stBound.xMax)
            {
                x = (int)m_stBound.xMax - 1;
            }

            if (y <= 0)
            {
                y = 0;
            }
            if (y + h >= m_stBound.yMax)
            {
                h = (int)m_stBound.yMax - y;
            }
            for (int i = 0; i < h; ++i)
            {
                if (!IsEmpty(x - 1, y) || !IsEmpty(x + 1, y))
                {
                    return new Vector2(x, y);
                }
                ++y;
            }
            return Vector2.zero;
        }

        public Vector2 findNextWalkPoint(int fromPosX, int fromPosY, int direction, int stepX, int stepY)
        {
            Vector2 p = Vector2.zero;
            if (direction != GameDefine.FACING_LEFT && direction != GameDefine.FACING_RIGHT)
            {
                return p;
            }
            int tx = (int)(fromPosX + direction * stepX);
            p = FindYLineNotEmptyPointUp(tx, (int)(fromPosY + stepY + 1), (int)m_stBound.height);
            if (tx < 0 || tx > m_stBound.xMax)
            {
                return p;
            }
            return p;
        }

        public Vector3 SomethingMove(Vector3 from, Vector3 to)
        {
            if (to.y < 0)
            {
                to.y = 0;
            }
            return to;
        }

        public void Dig(Vector2 pos, Texture2D shape, Texture2D bounds)
        {
            _mapChanged = true;
            for(int i = 0; i < m_listTerrains.Count; ++i)
            {
                m_listTerrains[i].Dig((int)pos.x, (int)pos.y, shape, bounds);
            }
        }

        public void AddToMap(TKPhysicalObj obj)
        {
            m_lstObjectInMap.Add(obj);
        }

        public void RemoveFromMap(TKPhysicalObj obj)
        {
            m_lstObjectInMap.Remove(obj);
        }

        public List<TKPhysicalObj> GetObjectsInMap(Rect rect, TKPhysicalObj except)
        {
            List<TKPhysicalObj> ret = new List<TKPhysicalObj>();
            for (int i = 0; i < m_lstObjectInMap.Count; ++i)
            {
                TKPhysicalObj phy = m_lstObjectInMap[i];
                if (phy != except &&
                    phy.IsLiving &&
                    phy.CanCollided)
                {
                    Rect t = phy.GetCollideRect();
                    t.position += phy.Position;
                    if(t.Overlaps(rect))
                    {
                        ret.Add(phy);
                    }
                }
            }
            return ret;
        }

        public List<TKPhysicalObj> GetObjectsInMap(Vector2 center, float range, TKPhysicalObj except)
        {
            List<TKPhysicalObj> ret = new List<TKPhysicalObj>();
            for (int i = 0; i < m_lstObjectInMap.Count; ++i)
            {
                TKPhysicalObj phy = m_lstObjectInMap[i];
                if (phy != except &&
                    phy.IsLiving &&
                    phy.CanCollided)
                {
                    Rect t = phy.GetCollideRect();
                    t.position += phy.Position;
                    float vx = Mathf.Abs(t.center.x - center.x);
                    float vy = Mathf.Abs(t.center.y - center.y);

                    float ux = vx - t.width / 2;
                    float uy = vy - t.height / 2;

                    if (ux*ux+uy*uy < range*range)
                    {
                        ret.Add(phy);
                    }
                }
            }
            return ret;
        }

        public void Clear()
        {
            m_listTerrains.Clear();
            m_listStones.Clear();
        }

        public void SetData(MapData pData)
        {
            m_stBound.Set(pData.bound_left, pData.bound_bottom, pData.bound_right - pData.bound_left, pData.bound_top - pData.bound_bottom);
            Gravity = pData.gravity;
            AirResistance = pData.air_resistance;
            wind_range = pData.wind_range;
            wind_step = pData.wind_step;
            for (int i = 0; i < pData.m_listTerrain.Count; ++i)
            {
                MapCellData cellData = pData.m_listTerrain[i];
                var srcTexture = GameGOW.Get().ResourceMgr.GetRes<Texture2D>(cellData.resource);
				Texture2D terrain = new Texture2D(srcTexture.width, srcTexture.height);
                terrain.SetPixels(srcTexture.GetPixels());
                terrain.Apply();
				//因原图采用了压缩格式，长宽有变化，并非最初设计效果，所以这里对创建的图重新设置长宽（设置为原图的长宽值，值由地形的XML读入）
				TextureScale.Bilinear(terrain,cellData.Width,cellData.Height);
                Tile tile = new Tile(cellData.pos_x * 100, cellData.pos_y * 100, terrain);
                tile.m_bIsDigable = cellData.digable;
                if (cellData.digable)
                {
                    m_listTerrains.Add(tile);
                }
                else
                {
                    m_listStones.Add(tile);
                }

                if (OnTerrainCellNew != null) OnTerrainCellNew(terrain, cellData);
            }
        }
    }
}