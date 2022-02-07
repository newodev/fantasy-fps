using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS
{
    struct Point
    {
        public float X;
        public float Y;

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Point operator +(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);
        public static Point operator -(Point a, Point b) => new Point(a.X - b.X, a.Y - b.Y);
        public static Point operator *(Point a, int b) => new Point(a.X * b, a.Y * b);
        public static Point operator /(Point a, int b) => new Point(a.X / b, a.Y / b);
    }

    struct IntPoint
    {
        public int X;
        public int Y;

        public IntPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static IntPoint operator +(IntPoint a, IntPoint b) => new IntPoint(a.X + b.X, a.Y + b.Y);
        public static IntPoint operator -(IntPoint a, IntPoint b) => new IntPoint(a.X - b.X, a.Y - b.Y);
        public static IntPoint operator *(IntPoint a, int b) => new IntPoint(a.X * b, a.Y * b);
        public static IntPoint operator /(IntPoint a, int b) => new IntPoint(a.X / b, a.Y / b);

        public static bool operator ==(IntPoint a, IntPoint b) => (a.X == b.X && a.Y == b.Y);
        public static bool operator !=(IntPoint a, IntPoint b) => !(a == b);
    }
}
