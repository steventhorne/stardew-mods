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

    public override void CalculateSizes(Rectangle constraints)
    {
        SetSize(constraints.Size);

        int titleReserve = 0;
        if (TitleWidget != null)
        {
            TitleWidget.CalculateSizes(constraints);
            TitleWidget.SetLocation((Rect.Width / 2) - (TitleWidget.Size.X / 2), constraints.Y + IClickableMenu.borderWidth);
            titleReserve += TitleWidget.Size.Y + TITLE_MARGIN_BOTTOM;
        }

        if (Child != null)
        {
            var childConstraints = new Rectangle(
                constraints.X + IClickableMenu.borderWidth,
                constraints.Y + IClickableMenu.borderWidth + titleReserve,
                constraints.Width - (IClickableMenu.borderWidth * 2),
                constraints.Height - (IClickableMenu.borderWidth * 2) - titleReserve
            );
            Child.CalculateSizes(childConstraints);
            Child.SetLocation(childConstraints.Location);
        }
    }

    public override void Draw(SpriteBatch b)
    {
        Game1.drawDialogueBox(Rect.X, Rect.Y - 64, Rect.Width, Rect.Height + 64, false, true);
        TitleWidget.Draw(b);
        Child.Draw(b);
    }

    public override bool TryReceiveGamePadButton(Buttons button)
    {
        throw new System.NotImplementedException();
    }

    public override bool TryReceiveLeftClick(int x, int y, bool playSound)
    {
        throw new System.NotImplementedException();
    }

    public override bool TryReceiveRightClick(int x, int y, bool playSound)
    {
        throw new System.NotImplementedException();
    }

    public override bool TryReceiveScrollWheelAction(int direction)
    {
        throw new System.NotImplementedException();
    }
}
