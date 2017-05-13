namespace TKBase
{
    public class EulerVector
    {
        public float m_fX;
        public float m_fV;
        public float m_fA;
        public EulerVector(float fX0, float fX1, float fX2)
        {
            m_fX = fX0;
            m_fV = fX1;
            m_fA = fX2;
        }

        public void Clear()
        {
            m_fX = 0;
            m_fV = 0;
            m_fA = 0;
        }

        public void clearMotion()
        {
            m_fV = 0;
            m_fA = 0;
        }


        /**
         *  Solve a.x'' + b.x' + c.x = d equation using Euler method.
         */
        public void ComputeOneEulerStep(float m, float af, float f, float dt)
        {
            m_fA = (f - af * m_fV) / m;
            m_fV = m_fV + m_fA * dt;
            m_fX = m_fV * dt;
        }

        public override string ToString()
        {
            return "x:" + m_fX + ",v:" + m_fV + ",a" + m_fA;
        }
    }

}