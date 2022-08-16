
using System;
using System.Collections;
using UnityEngine;
public struct GridPosition : IEquatable<GridPosition>
{
    public int x;
    public int y;

    public GridPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(object obj)
    {
        return obj is GridPosition position &&
               x == position.x &&
               y == position.y;
    }

    public bool Equals(GridPosition other)
    {
        return this == other;
    }

    public override int GetHashCode()
    {
        int hashCode = 1502939027;
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        return hashCode;
    }

    public override string ToString()
    {
        return $"x:{x} y:{y}";
    }

    public static bool operator ==(GridPosition a,GridPosition b)
    {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(GridPosition a, GridPosition b)
    {
        return !(a == b);
    }

    public static GridPosition operator +(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x + b.x, a.y + b.y);
    }

    public static GridPosition operator -(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x - b.x, a.y - b.y);
    }

    public static float Distance(GridPosition a, GridPosition b)
    {
        float distance;
        float xdistance = (b.x - a.x)^2;
        float ydistance = (b.y - a.y)^2;
        distance = Mathf.Sqrt(xdistance + ydistance);
        return Mathf.Abs( distance);
    }

}
