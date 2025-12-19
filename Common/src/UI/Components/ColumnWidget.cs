using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Common.UI;

public class ColumnWidget : Widget
{
    public Widget[] Children { get; init; }
    public int Gap { get; init; }

    public ColumnWidget() { }

    internal override void Init(Widget parent)
    {
        base.Init(parent);

        foreach (var child in Children)
            child.Init(this);
    }

    public override void CalculateSizes(Point constraints)
    {
        int heightConstraint = constraints.Y / Children.Length;
        int width = 0;
        int height = 0;
        var i = 0;
        foreach (var child in Children)
        {
            child.CalculateSizes(new Point(constraints.X, heightConstraint));
            child.SetLocation(0, height);
            var childSize = child.Rect.Size;
            if (childSize.X > width) width = childSize.X;
            height += childSize.Y;
            if (i < Children.Length && heightConstraint > childSize.Y)
                height += System.Math.Min(heightConstraint - childSize.Y, Gap);
            i++;
        }
        SetSize(width, height);
    }

    public override bool TryReceiveLeftClick(int x, int y, bool playSound, Point offset)
    {
        if (!Rect.Contains(x, y)) return false;
        offset += Rect.Location;

        foreach (var child in Children)
        {
            if (child.TryReceiveLeftClick(x, y, playSound, offset))
                return true;
        }

        return false;
    }

    public override bool TryReceiveRightClick(int x, int y, bool playSound, Point offset)
    {
        throw new System.NotImplementedException();
    }

    public override bool TryReceiveScrollWheelAction(int x, int y, int direction, Point offset)
    {
        if (!Rect.Contains(x, y)) return false;
        offset += Rect.Location;

        foreach (var child in Children)
        {
            if (child.TryReceiveScrollWheelAction(x, y, direction, offset))
                return true;
        }

        return false;
    }

    public override bool TryReceiveGamePadButton(Buttons button)
    {
        throw new System.NotImplementedException();
    }

    public override void Draw(SpriteBatch b, Point offset)
    {
        offset += Rect.Location;

        foreach (var child in Children)
            child.Draw(b, offset);
    }
}
