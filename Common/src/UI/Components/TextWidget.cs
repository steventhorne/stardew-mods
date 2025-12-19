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

        public override void CalculateSizes(Point constraints)
        {
            TextSize = Font.MeasureString(Data).ToPoint();

            var actualSize = new Point(
                Math.Min(constraints.X, TextSize.X),
                Math.Min(constraints.Y, TextSize.Y)
            );
            SetSize(actualSize);
        }

        public override void Draw(SpriteBatch b, Point offset)
        {
            if (WithShadow)
                Utility.drawTextWithShadow(b, Data, Font, (offset + Rect.Location).ToVector2(), Color);
            else
                b.DrawString(Font, Data, (offset + Rect.Location).ToVector2(), Color);
        }

        public override bool TryReceiveGamePadButton(Buttons button)
        {
            throw new NotImplementedException();
        }

        public override bool TryReceiveLeftClick(int x, int y, bool playSound, Point offset)
        {
            return false;
        }

        public override bool TryReceiveRightClick(int x, int y, bool playSound, Point offset)
        {
            throw new NotImplementedException();
        }

        public override bool TryReceiveScrollWheelAction(int x, int y, int direction, Point offset)
        {
            return false;
        }
    }
}
