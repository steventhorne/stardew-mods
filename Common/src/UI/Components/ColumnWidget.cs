using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Common.UI;

public class ColumnWidget : Widget
{
    public IEnumerable<Widget> Children { get; init; }
    public int Gap { get; init; }

    public ColumnWidget() { }

    internal override void Init(Widget parent)
    {
        base.Init(parent);

        foreach (var child in Children)
            child.Init(this);
    }

    public override void CalculateSizes(Rectangle constraints)
    {
        int heightConstraint = constraints.Height / Children.Count();
        int width = 0;
        int height = 0;
        var i = 0;
        foreach (var child in Children)
        {
            child.CalculateSizes(new Rectangle(constraints.X, constraints.Y + height, constraints.Width, heightConstraint));
            child.SetLocation(constraints.X, constraints.Y + height);
            var childSize = child.Rect.Size;
            if (childSize.X > width) width = childSize.X;
            height += childSize.Y;
            if (i < Children.Count() && heightConstraint > childSize.Y)
                height += System.Math.Min(heightConstraint - childSize.Y, Gap);
            i++;
        }
        SetSize(width, height);
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

    public override bool TryReceiveGamePadButton(Buttons button)
    {
        throw new System.NotImplementedException();
    }

    public override void Draw(SpriteBatch b)
    {
        foreach (var child in Children)
            child.Draw(b);
    }
}
