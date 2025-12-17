using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using Common.UI;
using System.Linq;

namespace DevTools;

public class WarpInterface : IClickableMenu
{
    private Rectangle Rect;

    private DialogueBoxWidget Menu;

    public WarpInterface()
    {
        Rect = new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height);
        initialize(Rect.X, Rect.Y, Rect.Width, Rect.Height);

        Menu = new DialogueBoxWidget()
        {
            Title = "Warps",
            Child = new ColumnWidget()
            {
                Children = Game1.locations.Select(l => new TextWidget() { Data = l.Name }).ToArray(),
                Gap = 10,
            }
        };
        Menu.Init();
        Menu.SetLocation(Rect.X, Rect.Y);
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        base.gameWindowSizeChanged(oldBounds, newBounds);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        Menu.TryReceiveLeftClick(x, y, playSound);
    }

    public override void draw(SpriteBatch b)
    {
        b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

        Menu.Draw(b);

        drawMouse(b);
    }
}
