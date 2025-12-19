using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace Common.UI;

public class DialogueBoxWidget : Widget
{
    public string Title { get; init; }
    public SpriteFont TitleFont { get; init; } = Game1.dialogueFont;
    public Widget Child { get; init; }

    private TextWidget TitleWidget;

    private const int TITLE_MARGIN_BOTTOM = 10;

    public DialogueBoxWidget() { }

    internal override void Init(Widget parent)
    {
        base.Init(parent);

        if (!string.IsNullOrWhiteSpace(Title))
        {
            TitleWidget = new TextWidget() { Data = Title };
            TitleWidget.Init(this);
        }

        Child?.Init(this);
    }

    public override void CalculateSizes(Point constraints)
    {
        SetSize(constraints);

        int titleReserve = 0;
        if (TitleWidget != null)
        {
            TitleWidget.CalculateSizes(constraints);
            TitleWidget.SetLocation((Rect.Width / 2) - (TitleWidget.Size.X / 2), IClickableMenu.borderWidth);
            titleReserve += TitleWidget.Size.Y + TITLE_MARGIN_BOTTOM;
        }

        if (Child != null)
        {
            var childConstraints = new Rectangle(
                IClickableMenu.borderWidth,
                IClickableMenu.borderWidth + titleReserve,
                constraints.X - (IClickableMenu.borderWidth * 2),
                constraints.Y - (IClickableMenu.borderWidth * 2) - titleReserve
            );
            Child.CalculateSizes(childConstraints.Size);
            Child.SetLocation(childConstraints.Location);
        }
    }

    public override void Draw(SpriteBatch b, Point offset)
    {
        offset += Rect.Location;

        Game1.drawDialogueBox(offset.X + Rect.X, offset.Y + Rect.Y - 64, Rect.Width, Rect.Height + 64, false, true);
        TitleWidget.Draw(b, offset);
        Child.Draw(b, offset);
    }

    public override bool TryReceiveGamePadButton(Buttons button)
    {
        throw new System.NotImplementedException();
    }

    public override bool TryReceiveLeftClick(int x, int y, bool playSound, Point offset)
    {
        if (!Rect.Contains(x, y)) return false;
        offset += Rect.Location;

        return Child.TryReceiveLeftClick(x, y, playSound, offset);
    }

    public override bool TryReceiveRightClick(int x, int y, bool playSound, Point offset)
    {
        throw new System.NotImplementedException();
    }

    public override bool TryReceiveScrollWheelAction(int x, int y, int direction, Point offset)
    {
        if (!Rect.Contains(x, y)) return false;
        offset += Rect.Location;

        return Child.TryReceiveScrollWheelAction(x, y, direction, offset);
    }
}
