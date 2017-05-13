using TKBase;

namespace TKGame
{
    public class BaseAction : IPoolableObect
    {
        protected bool _isPrepare;
        protected bool _isFinished;
        public bool isFinished
        {
            get { return _isFinished; }
        }
        public BaseAction()
        {
            _isFinished = false;
            _isPrepare = false;
        }

        virtual public bool Connect(BaseAction action)
        {
            return false;
        }

        virtual public bool CanReplace(BaseAction action)
        {
            return false;
        }

        
        virtual public void Prepare()
        {
            if (_isPrepare) return;
            _isPrepare = true;
        }

        virtual public void Execute()
        {
            ExecuteAtOnce();
            _isFinished = true;
        }

        virtual public void ExecuteAtOnce()
        {
            Prepare();
            _isFinished = true;
        }

        virtual public void Cancel()
        { 
        }

        #region(IPoolableObect接口)
        virtual public bool Initialize(object[] args)
        {
            _isFinished = false;
            _isPrepare = false;
            return true;
        }
        virtual public void UnInitialize()
        {
        }
        #endregion
    }
}
