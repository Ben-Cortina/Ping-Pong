using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ping_Pong
{
    /// <summary>
    /// This class is the base for an object in the game
    /// </summary>
    class GameObject
    {
        /// <summary>
        /// the X coord of the left of the obj
        /// </summary>
        protected float m_X;
        public float X
        {
            get { return m_X; }
            set { m_X = value; }
        }

        /// <summary>
        /// the Y coord of the top of the obj
        /// </summary>
        protected float m_Y;
        public float Y
        {
            get { return m_Y; }
            set { m_Y = value; }
        }

        protected float m_Width;
        public float Width
        {
            get { return m_Width; }
            set { m_Width = value; }
        }

        protected float m_Height;
        public float Height
        {
            get { return m_Height; }
            set { m_Height = value; }
        }

        /// <summary>
        /// The bounding Rectangle
        /// </summary>
        public Rectangle Rect
        {
            get { return new Rectangle((int)X, (int)Y, (int)Width, (int)Height); }
        }

        protected object m_Visual = null;
        public object Visual
        {
            get { return m_Visual; }
            set { m_Visual = value; }
        }
    }
}
