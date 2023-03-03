using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace PedSimulation.Geometry
{
    public class Polygon2d
    {
        public List<Vec2d> Points { get; set; }

        public Polygon2d(List<Vec2d> _points)
        {
            this.Points = _points;
        }

        public Polygon2d(Polyline poly)
        {
            this.Points = new List<Vec2d>();
            bool flag = poly.Count <= 2;
            if (!flag)
            {
                bool flag2 = poly.Last != poly.First;
                if (!flag2)
                {
                    for (int i = 0; i < poly.Count - 1; i++)
                    {
                        Point3d point3d = poly[i];
                        Vec2d item = new Vec2d(point3d.X, point3d.Y);
                        this.Points.Add(item);
                    }
                }
            }
        }

        public bool IsValid()
        {
            return this.Points.Count > 2;
        }

        public Polyline ToRhinoPolyline()
        {
            bool flag = !this.IsValid();
            Polyline result;
            if (flag)
            {
                result = null;
            }
            else
            {
                List<Point3d> list = new List<Point3d>();
                foreach (Vec2d vec2d in this.Points)
                {
                    Point3d item = new Point3d(vec2d.X, vec2d.Y, 0.0);
                    //item..ctor(vec2d.X, vec2d.Y, 0.0);
                    list.Add(item);
                }
                list.Add(list[0]);
                Polyline polyline = new Polyline(list);
                result = polyline;
            }
            return result;
        }

        public Polygon2d Offset(double dist)
        {
            bool flag = !this.IsValid();
            Polygon2d result;
            if (flag)
            {
                result = null;
            }
            else
            {
                bool isClockwise = Polygon2d.IsPolygonClockwise(this);
                List<Vec2d> list = new List<Vec2d>();
                int count = this.Points.Count;
                Vec2d p = this.Points[0];
                Vec2d prev = this.Points[count - 1];
                Vec2d next = this.Points[1];
                Vec2d item = Polygon2d.OffsetCornerPoint(prev, p, next, isClockwise, dist);
                list.Add(item);
                for (int i = 1; i < count - 1; i++)
                {
                    p = this.Points[i];
                    prev = this.Points[i - 1];
                    next = this.Points[i + 1];
                    item = Polygon2d.OffsetCornerPoint(prev, p, next, isClockwise, dist);
                    list.Add(item);
                }
                p = this.Points[count - 1];
                prev = this.Points[count - 2];
                next = this.Points[0];
                item = Polygon2d.OffsetCornerPoint(prev, p, next, isClockwise, dist);
                list.Add(item);
                result = new Polygon2d(list);
            }
            return result;
        }

        private static Vec2d OffsetCornerPoint(Vec2d prev, Vec2d p, Vec2d next, bool isClockwise, double dist)
        {
            Vec2d vec2d = p.subtract(prev);
            Vec2d vec2d2 = next.subtract(p);
            double xangle = vec2d.getXAngle();
            double xangle2 = vec2d2.getXAngle();
            double num = xangle2 - xangle;
            bool flag = num < -3.141592653589793;
            if (flag)
            {
                num += 6.283185307179586;
            }
            bool flag2 = num > 3.141592653589793;
            if (flag2)
            {
                num = -6.283185307179586 + num;
            }
            double a;
            if (isClockwise)
            {
                a = (num + 3.141592653589793) * 0.5;
            }
            else
            {
                a = (num - 3.141592653589793) * 0.5;
            }
            Vec2d vec2d3 = vec2d.rotate(a).normalize();
            return p.add(vec2d3.multiply(dist));
        }

        private static bool IsPolygonClockwise(Polygon2d polygon)
        {
            double num = 0.0;
            int count = polygon.Points.Count;
            bool flag = count < 3;
            if (flag)
            {
                throw new ArgumentException("IsPolygonClockwise: the input is not a polygon (with fewer than three points).");
            }
            Vec2d vec2d = polygon.Points[count - 1];
            Vec2d vec2d2 = polygon.Points[0];
            double x = vec2d.X;
            double y = vec2d.Y;
            double x2 = vec2d2.X;
            double y2 = vec2d2.Y;
            double num2 = (x2 - x) * (y2 + y);
            num += num2;
            for (int i = 0; i < count - 1; i++)
            {
                vec2d = polygon.Points[i];
                vec2d2 = polygon.Points[i + 1];
                x = vec2d.X;
                y = vec2d.Y;
                x2 = vec2d2.X;
                y2 = vec2d2.Y;
                num2 = (x2 - x) * (y2 + y);
                num += num2;
            }
            bool flag2 = num > 0.0;
            bool result;
            if (flag2)
            {
                result = true;
            }
            else
            {
                bool flag3 = num < 0.0;
                if (!flag3)
                {
                    throw new ArgumentException("IsPolygonClockwise: the input curve's direction cannot be determined.");
                }
                result = false;
            }
            return result;
        }
    }
}
