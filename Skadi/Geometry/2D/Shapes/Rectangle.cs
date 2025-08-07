namespace Skadi.Geometry._2D.Shapes;

public readonly record struct Rectangle
{
    public Vector2D Center { get; }
    public Vector2D Size { get; }

    public double Width => Size.X;
    public double Height => Size.Y;

    public double Left => Center.X - Width / 2;
    public double Right => Center.X + Width / 2;
    public double Bottom => Center.Y - Height / 2;
    public double Top => Center.Y + Height / 2;

    public Vector2D LeftBottom => new(Left, Bottom);
    public Vector2D LeftTop => new(Left, Top);
    public Vector2D RightBottom => new(Right, Bottom);
    public Vector2D RightTop => new(Right, Top);

    public IEnumerable<Vector2D> Vertices => new[]
    {
        LeftBottom,
        RightBottom,
        RightTop,
        LeftTop
    };

    public Rectangle(Vector2D leftBottom, Vector2D size)
    {
        Center = new Vector2D(leftBottom.X + size.X / 2, leftBottom.Y + size.Y / 2);
        Size = size;
    }

    public static Rectangle FromCenter(Vector2D center, Vector2D size)
    {
        var leftBottom = new Vector2D(center.X - size.X / 2, center.Y - size.Y / 2);
        return new Rectangle(leftBottom, size);
    }

    public Rectangle(double left, double right, double bottom, double top)
    {
        var width = right - left;
        var height = top - bottom;
        Center = new Vector2D(left + width / 2, bottom + height / 2);
        Size = new Vector2D(width, height);
    }
}
