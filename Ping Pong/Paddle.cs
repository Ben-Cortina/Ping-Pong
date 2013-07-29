using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ping_Pong
{
    class Paddle : GameObject
    {
        protected object m_VisualHit = null;
        public object VisualHit
        {
            get { return m_VisualHit; }
            set { m_VisualHit = value; }
        }

    }
}
