using Microsoft.Xna.Framework;

namespace Common;

public static class UIUtils
{
    public static Point CenterInRect(Rectangle rect, Point size)
    {
        return new Point(rect.X + (rect.Width - size.X)/2, rect.Y + (rect.Height - size.Y)/2);
    }
}
