using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace Common.UI
{
    public class ButtonWidget : Widget
    {
        public Widget Child { get; init; }
        public Action OnLeftClick { get; init; }
        public Action OnRightClick { get; init; }
        public Rectangle Padding { get; init; } = new Rectangle(10, 5, 10, 5);

        internal override void Init(Widget parent)
        {
            base.Init(parent);

            Child?.Init(this);
        }

        public override void CalculateSizes(Point constraints)
        {
            var size = Point.Zero;
            if (Child != null)
            {
                Child.CalculateSizes(constraints);
                size = Child.Size;
                size += new Point(Padding.X + Padding.Width, Padding.Y + Padding.Height);
                Child.SetLocation(Padding.Location);
            }

            SetSize(size);
        }

        public override void Draw(SpriteBatch b, Point offset)
        {
            offset += Rect.Location;
            Rectangle texRect = new Rectangle(0, 5 * 64, 64, 64);
            IClickableMenu.drawTextureBox(b, Game1.menuTexture, texRect, offset.X, offset.Y, Size.X, Size.Y, Color.White, 1, false);
            Child?.Draw(b, offset);
        }

        public override bool TryReceiveGamePadButton(Buttons button)
        {
            throw new NotImplementedException();
        }

        public override bool TryReceiveLeftClick(int x, int y, bool playSound, Point offset)
        {
            if (!ContainsPoint(x, y, offset)) return false;
            offset += Rect.Location;

            if (Child != null)
            {
                if (Child.TryReceiveLeftClick(x, y, playSound, offset))
                    return true;
            }

            OnLeftClick?.Invoke();
            // TODO: play sound
            return true;
        }

        public override bool TryReceiveRightClick(int x, int y, bool playSound, Point offset)
        {
            if (!ContainsPoint(x, y, offset)) return false;
            offset += Rect.Location;

            if (Child != null)
            {
                if (Child.TryReceiveRightClick(x, y, playSound, offset))
                    return true;
            }

            OnRightClick?.Invoke();
            // TODO: play sound
            return true;
        }

        public override bool TryReceiveScrollWheelAction(int x, int y, int direction, Point offset)
        {
            if (Child == null) return false;
            return Child.TryReceiveScrollWheelAction(x, y, direction, offset);
        }
    }
}
