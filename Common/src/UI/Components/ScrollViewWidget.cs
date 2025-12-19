using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace Common.UI;

public class ScrollViewWidget : Widget
{
    public Widget[] Children { get; init; }
    public int Gap { get; init; }

    public Rectangle ViewportRect { get; protected set; }
    public int ScrollPosition { get; protected set; }

    internal override void Init(Widget parent)
    {
        base.Init(parent);

        foreach (var child in Children)
            child.Init(this);
    }

    public override void CalculateSizes(Point constraints)
    {
        var width = constraints.X;
        var height = 0;
        var i = 0;
        foreach (var child in Children)
        {
            child.CalculateSizes(constraints);
            child.SetLocation(0, height);
            var childSize = child.Rect.Size;
            if (childSize.X > width) width = childSize.X;
            height += childSize.Y;
            if (i < Children.Length) height += Gap;
        }
        ViewportRect = new Rectangle(0, 0, width, height);
        SetSize(constraints);
    }

    public override void Draw(SpriteBatch b, Point offset)
    {
        b.End();

        var gd = Game1.graphics.GraphicsDevice;
        var oldScissor = gd.ScissorRectangle;
        var oldRaster = gd.RasterizerState;
        var raster = new RasterizerState()
        {
            CullMode = CullMode.None,
            ScissorTestEnable = true
        };

        gd.ScissorRectangle = Rect;

        b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, raster);

        offset += new Point(Rect.Location.X, Rect.Location.Y - ScrollPosition);

        foreach (var child in Children)
            child.Draw(b, offset);

        b.End();

        gd.ScissorRectangle = oldScissor;
        gd.RasterizerState = oldRaster;

        b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
    }

    public override bool TryReceiveGamePadButton(Buttons button)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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

        int scrollMax = Math.Max(ViewportRect.Height - Rect.Height, 0);
        ScrollPosition = Math.Clamp(ScrollPosition - direction, 0, scrollMax);
        return true;
    }
}
