using System.Numerics;
using MemoryPack;

namespace gameplay.math;

/// <summary>
/// 圆形
/// </summary>
public struct Circle
{
    public Vector2E center;
    public float radius;

    public Circle(Vector2E center, float radius)
    {
        this.center = center;
        this.radius = radius;
    }
}

public struct Sphere
{
    public Vector3E center;
    public float radius;

    public Sphere(Vector3E center, float radius)
    {
        this.center = center;
        this.radius = radius;
    }
}

public struct Line2
{
    public Vector2E start;
    public Vector2E end;

    public Line2(Vector2E start, Vector2E end)
    {
        this.start = start;
        this.end = end;
    }
}

public struct Line3
{
    public Vector3E start;
    public Vector3E end;

    public Line3(Vector3E start, Vector3E end)
    {
        this.start = start;
        this.end = end;
    }
}

public struct Capsule
{
    public Vector3E start;
    public Vector3E end;
    public float radius;

    public Capsule(Vector3E start, Vector3E end, float radius)
    {
        this.start = start;
        this.end = end;
        this.radius = radius;
    }
}

public struct Sector2
{
}

public struct Ellipse
{
    public Vector3E position;
    public Vector3E forward;
    public float radius1;
    public float radius2;

    public Ellipse(Vector3E position, Vector3E forward, float radius1, float radius2)
    {
        this.position = position;
        this.forward = forward;
        this.radius1 = radius1;
        this.radius2 = radius2;
    }
}

public struct Cone
{
}

public struct Polyline3
{
}

/// <summary>
/// 扇形
/// </summary>
public struct Sector
{
    public Vector2E center;
    public Vector2E direction;
    public float radius;
    public float degree;
}

/// <summary>
/// 铲形 可以简单理解为 圆心&中心线&角度相同的 大扇形减去小扇形剩下的区域
/// </summary>
public struct Spatula
{
    public Vector2E center;
    public float nearRadius;
    public float farRadius;
    public Vector2E direction;
    public float degree;
}

/// <summary>
/// 干扰扇形 left --> 顺时针 --> right
/// </summary>
public struct InfluenceSector
{
    public int left;
    public int right;
}

/// <summary>
/// 矩形区域
/// </summary>
public struct Area
{
    public float left;
    public float right;
    public float back;
    public float front;
}

/// <summary>
/// 竖直平面，垂直于地面
/// </summary>
public struct PlaneV
{
    public PlaneV(Vector2E normal, float d)
    {
        Normal = normal;
        D = d;
    }
    public PlaneV(float normalX, float normalY, float d)
    {
        Normal = new Vector2E(normalX, normalY);
        D = d;
    }

    public Vector2E Normal;
    public float D;

    public static implicit operator Plane(PlaneV plane)
    {
        return new Plane(plane.Normal, plane.D);
    }
    public static explicit operator PlaneV(Plane plane)
    {
        return new PlaneV((Vector2E)plane.Normal, plane.D);
    }
}

[MemoryPackable]
public partial struct Label2
{
    public string? text;
    public Vector2E position;

    public Label2(string text, Vector2E position)
    {
        this.text = text;
        this.position = position;
    }
}

[MemoryPackable]
public partial struct Label3
{
    public string? text;
    public Vector3E position;

    public Label3(string text, Vector3E position)
    {
        this.text = text;
        this.position = position;
    }
}

[MemoryPackable]
public partial struct Point3
{
    public Vector3E position;

    public Point3(Vector3E position)
    {
        this.position = position;
    }
}
