using UnityEngine;
using TKBase;

namespace TKGame
{
    public class TKPhysics
    {
        protected float _mass;
        protected float _gravityFactor; //重力
        protected float _windFactor; //风速
        protected float _airResitFactor; //空气阻力
        protected EulerVector _vx;//x方向
        protected EulerVector _vy;//y方向
        protected Vector2 _ef;//外表的持久力
        protected bool _isMoving;
        protected MapManager _map;

        protected Vector2 _posTemp;
        protected Vector2 _pos
        {
            get { return _posTemp;  }
            set {
                _posTemp = value;
            }
        }

        public Vector2 Position
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public float Vx
        {
            get { return _vx.m_fV; }
        }

        public float Vy
        {
            get { return _vy.m_fV; }
        }

        public float MotionAngle
        {
            get { return Mathf.Atan2(_vy.m_fV, _vx.m_fV); }
        }

        public bool IsMoving
        {
            get { return _isMoving; }
        }

        public TKPhysics(float mass = 1, float gravityFactor = 1, float windFactor = 1, float airResitFactor = 1)
        {
            _mass = mass;
            _gravityFactor = gravityFactor;
            _windFactor = windFactor;
            _airResitFactor = airResitFactor;

            _vx = new EulerVector(0, 0, 0);
            _vy = new EulerVector(0, 0, 0);
            _ef = new Vector2(0, 0);
        }

        virtual public void Initialize()
        {
            _vx = new EulerVector(0, 0, 0);
            _vy = new EulerVector(0, 0, 0);
            _ef = new Vector2(0, 0);
        }

        public void AddExternForce(float forceX, float forceY)
        {
            _ef.x += forceX;
            _ef.y += forceY;
            if (!_isMoving && _map != null)
            {
                StartMoving();
            }
        }

        public void AddSpeedXY(float vectorX, float vectorY)
        {
            _vx.m_fV += vectorX;
            _vy.m_fV += vectorY;

            if (!_isMoving && _map != null)
            {
                StartMoving();
            }
        }

        public void SetSpeedXY(float vectorX, float vectorY)
        {
            _vx.m_fV = vectorX;
            _vy.m_fV = vectorY;

            if (!_isMoving && _map != null)
            {
                StartMoving();
            }
        }

        public void StartMoving()
        {
            _isMoving = true;
        }

        public void StopMoving()
        {
            _vx.clearMotion();
            _vy.clearMotion();
            _isMoving = false;
        }

        protected float _arf = 0;
        protected float _gf = 0;
        protected float _wf = 0;
        virtual public void AddToMap()
        {
            _map = GameGOW.Get().MapMgr;
            _arf = _map.AirResistance * _airResitFactor;
            _gf = _map.Gravity * _gravityFactor * _mass;
            _wf = _map.Wind * _windFactor;
        }

        virtual public void RemoveFromMap()
        {
            _map = null;
        }

        protected Vector2 ComputeFallNextXY(float dt)
        {
            //临时方案 by xue
            if(SkillManager.CurrentSkillId == (int)SkillManager.SkillType.Rocket) {
                _vx.ComputeOneEulerStep(_mass, _arf, 0 + _ef.x, dt);
                _vy.ComputeOneEulerStep(_mass, _arf, 0 + _ef.y, dt);
            }
            else if(SkillManager.CurrentSkillId == (int)SkillManager.SkillType.ThunderBomb)
            {
                _vx.ComputeOneEulerStep(_mass, _arf, 0 + _ef.x, dt);
                _vy.ComputeOneEulerStep(_mass, _arf, _gf + _ef.y, dt);
            }
            else {
                _vx.ComputeOneEulerStep(_mass, _arf, _wf + _ef.x, dt);
                _vy.ComputeOneEulerStep(_mass, _arf, _gf + _ef.y, dt);
            }
            return new Vector2(_pos.x + _vx.m_fX, _pos.y + _vy.m_fX);
        }

        virtual public void Update(float dt)
        {
            if (_isMoving && _map != null)
            {
                UpdatePosition(dt);
            }
        }

        protected void UpdatePosition(float dt)
        {
            MoveTo(ComputeFallNextXY(dt));
        }

        virtual public void MoveTo(Vector2 pos)
        {
            if (pos.x != _pos.x || pos.y != _pos.y)
            {
                _pos = pos;
            }
        }
    }
}