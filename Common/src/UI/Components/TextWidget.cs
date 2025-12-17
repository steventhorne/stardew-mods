using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace Common.UI
{
    public class TextWidget : Widget
    {
        public string Data { get; init; } = "";
        public SpriteFont Font { get; init; } = Game1.dialogueFont;
        public Color Color { get; init; } = Color.Black;
        public bool WithShadow { get; init; } = true;

        private Point TextSize;

        public TextWidget() { }

        public override void CalculateSizes(Rectangle constraints)
        {
            TextSize = Font.MeasureString(Data).ToPoint();

            var actualSize = new Point(
                Math.Min(constraints.Size.X, TextSize.X),
                Math.Min(constraints.Size.Y, TextSize.Y)
            );
            SetSize(actualSize);
        }

        public override void Draw(SpriteBatch b)
        {
            if (WithShadow)
                Utility.drawTextWithShadow(b, Data, Font, Rect.Location.ToVector2(), Color);
            else
                b.DrawString(Font, Data, Rect.Location.ToVector2(), Color);
        }

        public override bool TryReceiveGamePadButton(Buttons button)
        {
            throw new NotImplementedException();
        }

        public override bool TryReceiveLeftClick(int x, int y, bool playSound)
        {
            throw new NotImplementedException();
        }

        public override bool TryReceiveRightClick(int x, int y, bool playSound)
        {
            throw new NotImplementedException();
        }

        public override bool TryReceiveScrollWheelAction(int direction)
        {
            throw new NotImplementedException();
        }
    }
}
