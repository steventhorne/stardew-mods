using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace Common.UI
{
    public abstract class Widget
    {
        public Widget Parent { get; protected set; }

        public Rectangle Constraint { get; protected set; }
        public Rectangle Rect
        {
            get;
            protected set;
        }
        public Point Location { get { return Rect.Location; } }
        public Point Size { get { return Rect.Size; } }

        public Widget() { }

        public void Init()
        {
            Init(new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height));
        }

        public void Init(Rectangle constraints)
        {
            Init(null);

            CalculateSizes(constraints);
        }

        internal virtual void Init(Widget parent)
        {
            Parent = parent;
        }

        public void SetLocation(int x, int y)
        {
            Rect = new Rectangle(x, y, Rect.Width, Rect.Height);
        }

        public void SetLocation(Point pos)
        {
            Rect = new Rectangle(pos.X, pos.Y, Rect.Width, Rect.Height);
        }

        protected void SetSize(int width, int height)
        {
            Rect = new Rectangle(Rect.X, Rect.Y, width, height);
        }

        protected void SetSize(Point size)
        {
            Rect = new Rectangle(Rect.X, Rect.Y, size.X, size.Y);
        }

        public abstract void CalculateSizes(Rectangle constraints);

        public abstract bool TryReceiveLeftClick(int x, int y, bool playSound);
        public abstract bool TryReceiveRightClick(int x, int y, bool playSound);
        public abstract bool TryReceiveScrollWheelAction(int direction);
        public abstract bool TryReceiveGamePadButton(Buttons button);

        public abstract void Draw(SpriteBatch b);
    }
}
