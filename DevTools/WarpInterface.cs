using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using Common.UI;
using System.Linq;

namespace DevTools;

public class WarpInterface : IClickableMenu
{
    private DialogueBoxWidget Menu;

    public WarpInterface()
    {
        initialize(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height);

        Menu = new DialogueBoxWidget()
        {
            Title = "Warps",
            Child = new ScrollViewWidget()
            {
                Children = Game1.locations.Select(l => new TextWidget() { Data = l.Name }).ToArray(),
                Gap = 10,
            }
        };
        Menu.Init();
        Menu.SetLocation(Point.Zero);
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        base.gameWindowSizeChanged(oldBounds, newBounds);
    }

    public override void receiveScrollWheelAction(int direction)
    {
        var mousePos = Game1.getMousePosition();
        Menu.TryReceiveScrollWheelAction(mousePos.X, mousePos.Y, direction, Point.Zero);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        Menu.TryReceiveLeftClick(x, y, playSound, Point.Zero);
    }

    public override void draw(SpriteBatch b)
    {
        b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

        Menu.Draw(b, Point.Zero);

        drawMouse(b);
    }
}
