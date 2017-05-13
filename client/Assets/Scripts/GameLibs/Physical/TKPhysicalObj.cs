using UnityEngine;
using System.Collections.Generic;
namespace TKGame
{
    public class TKPhysicalObj : TKPhysics
    {

        protected int _id;
        protected Rect _testRect;
        protected bool _canCollided;
        protected bool _isLiving;
        protected int _layerType;

        public int ID
        {
            get { return _id; }
        }

        public int LayerType
        {
            get { return _layerType; }
        }

        public bool CanCollided
        {
            set { _canCollided = value; }
            get { return _canCollided; }
        }

        public bool IsLiving
        {
            get { return _isLiving; }
        }

        public TKPhysicalObj(int id, int layerType = 1, float mass = 1, float gravityFactor = 1, float windFactor = 1, float airResitFactor = 1)
            : base(mass, gravityFactor, windFactor, airResitFactor)
        {
            _id = id;
            _layerType = layerType;
            _canCollided = false;
            _testRect = new Rect(-5, -5, 10, 10);
            _isLiving = true;
        }

        override public void Initialize()
        {
            base.Initialize();
            _id = 0;
            _isLiving = true;
        }

        public void SetCollideRect(int left, int top, int right, int bottom)
        {
            _testRect.xMin = left;
            _testRect.yMin = bottom;
            _testRect.xMax = right;
            _testRect.yMax = top;
        }

        public Rect GetCollideRect()
        {
            return _testRect;
        }

        override public void MoveTo(Vector2 pos)
        {
            if(pos.y<0)
            {
                _pos = pos;
                FlyOutMap();
            }
            if (Vector2.Distance(_pos, pos) >= 3)
            {
                int dx = Mathf.Abs((int)pos.x - (int)_pos.x);
                int dy = Mathf.Abs((int)pos.y - (int)_pos.y);
                int count = dx > dy ? dx : dy;
                float dt = 1.0f / count;
                Vector2 cur = _pos;
                Vector2 dest = Vector2.zero;
                for (int t = count; t > 0; t -= 3)
                {
                    dest = MathUtil.Interpolate(cur, pos, dt * t);
                    Rect rect = GetCollideRect();
                    rect.position += dest;
                    List<TKPhysicalObj> lst = _map.GetObjectsInMap(rect, this);
                    if (lst.Count > 0)
                    {
                        _pos = dest;
                        CollideObject(lst);
                    }
                    else if (!_map.IsEmpty((int)dest.x, (int)dest.y))
                    {
                        _pos = dest;
                        CollideGround();
                    }
                    
                    else if (_map.IsOutMap(dest))
                    {
                        if (SkillManager.CurrentSkillId == 7)
                        {
                            _pos = dest;
                            //FlyOutMap();
                        }
                    }

                    if (!_isMoving)
                    {
                        return;
                    }
                }
                _pos = pos;
            }
        }
        /***
         * 计算物体的角度,取前后间隔为2像素的8个点， 计算其平均角度。
         */
        public float CalcObjectAngle(float bounds = 16)
        {
            if (_map != null)
            {
                List<Vector2> pre_array = new List<Vector2>();
                List<Vector2> next_array = new List<Vector2>();
                Vector2 pre = Vector2.zero;
                Vector2 next = Vector2.zero;
                float bound = bounds;
                for (float m = 1; m <= bound; m += 2)
                {
                    //由下往上查找空的点
                    for (int i = -10; i <= 10; i++)
                    {
                        if (_map.IsEmpty((int)(_pos.x + m), (int)_pos.y + i))
                        {
                            if (i == -10) break;
                            pre_array.Add(new Vector2(_pos.x + m, _pos.y + i));
                            break;
                        }
                    }
                    for (int j = -10; j <= 10; j++)
                    {
                        if (_map.IsEmpty((int)(_pos.x - m), (int)_pos.y + j))
                        {
                            if (j == -10) break;
                            next_array.Add(new Vector2(_pos.x - m, _pos.y + j));
                            break;
                        }
                    }
                }

                pre = new Vector2(_pos.x, _pos.y);
                next = new Vector2(_pos.x, _pos.y);
                for (int n = 0; n < pre_array.Count; n++)
                {
                    pre = pre + pre_array[n];
                }
                for (int nn = 0; nn < next_array.Count; nn++)
                {
                    next = next + next_array[nn];
                }
                pre.x = pre.x / (pre_array.Count + 1);
                pre.y = pre.y / (pre_array.Count + 1);

                next.x = next.x / (next_array.Count + 1);
                next.y = next.y / (next_array.Count + 1);
                return MathUtil.GetAngleTwoPoint(pre, next);
            }
            else
            {
                return 0f;
            }
        }

        virtual protected void CollideObject(List<TKPhysicalObj> lst)
        {
        }

        virtual protected void CollideGround()
        {
            if (_isMoving)
            {
                StopMoving();
            }
        }

        virtual public bool CollideByObject(TKPhysicalObj obj)
        {
            return false;
        }

        virtual protected void FlyOutMap()
        {
            if (_isLiving)
            {
                Die();
            }
        }

        public void Die()
        {
          
            _isLiving = false;
            if (_isMoving)
            {
                StopMoving();
            }
        }

        public override void AddToMap()
        {
            base.AddToMap();
            _map.AddToMap(this);
        }
        public override void RemoveFromMap()
        {
            _map.RemoveFromMap(this);
            base.RemoveFromMap();
        }
    }
}