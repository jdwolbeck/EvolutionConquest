// *****************************************************************************
// http://www.progware.org/blog/ - Collision Detection Algorithms
// *****************************************************************************

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

public enum UseForCollisionDetection { Triangles, Rectangles, Circles }

public static class CollisionDetection
{
    public static UseForCollisionDetection CDPerformedWith { get; set; }

    public static bool BoundingRectangle(int x1, int y1, int width1, int height1, int x2, int y2, int width2, int height2)
    {
        Rectangle rectangleA = new Rectangle((int)x1, (int)y1, width1, height1);
        Rectangle rectangleB = new Rectangle((int)x2, (int)y2, width2, height2);

        int top = Math.Max(rectangleA.Top, rectangleB.Top);
        int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
        int left = Math.Max(rectangleA.Left, rectangleB.Left);
        int right = Math.Min(rectangleA.Right, rectangleB.Right);

        if (top >= bottom || left >= right)
            return false;

        return true;
    }
    public static bool BoundingCircle(int x1, int y1, int radius1, int x2, int y2, int radius2)
    {
        Vector2 V1 = new Vector2(x1, y1);
        Vector2 V2 = new Vector2(x2, y2);

        Vector2 Distance = V1 - V2;

        if (Distance.Length() < radius1 + radius2)
            return true;

        return false;
    }
    public static bool BoundingTriangles(List<Vector2> p1, List<Vector2> p2)
    {
        for (int i = 0; i < 3; i++)
            if (_isPointInsideTriangle(p1, p2[i])) return true;

        for (int i = 0; i < 3; i++)
            if (_isPointInsideTriangle(p2, p1[i])) return true;
        return false;
    }
    private static bool _isPointInsideTriangle(List<Vector2> TrianglePoints, Vector2 p)
    {
        // Translated to C# from: http://www.ddj.com/184404201
        Vector2 e0 = p - TrianglePoints[0];
        Vector2 e1 = TrianglePoints[1] - TrianglePoints[0];
        Vector2 e2 = TrianglePoints[2] - TrianglePoints[0];

        float u, v = 0;
        if (e1.X == 0)
        {
            if (e2.X == 0) return false;
            u = e0.X / e2.X;
            if (u < 0 || u > 1) return false;
            if (e1.Y == 0) return false;
            v = (e0.Y - e2.Y * u) / e1.Y;
            if (v < 0) return false;
        }
        else
        {
            float d = e2.Y * e1.X - e2.X * e1.Y;
            if (d == 0) return false;
            u = (e0.Y * e1.X - e0.X * e1.Y) / d;
            if (u < 0 || u > 1) return false;
            v = (e0.X - e2.X * u) / e1.X;
            if (v < 0) return false;
            if ((u + v) > 1) return false;
        }

        return true;
    }
}