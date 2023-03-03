using System;
using System.Collections.Generic;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;

namespace PedSimulation.Geometry
{

    public static class RhinoGeoMethods
    {

        public static bool IsPositionVisible(Point3d lineFrom, Point3d lineTo, List<Polygon2d> obsList)
        {
            Line line = new Line(lineFrom, lineTo);
            //line..ctor(lineFrom, lineTo);
            foreach (Polygon2d polygon2d in obsList)
            {
                List<Line> list = new List<Line>();
                int count = polygon2d.Points.Count;
                Vec2d vec2d = polygon2d.Points[count - 1];
                Vec2d vec2d2 = polygon2d.Points[0];
                Point3d point3d = new Point3d(vec2d.X, vec2d.Y, 0.0);
                //point3d..ctor(vec2d.X, vec2d.Y, 0.0);
                Point3d point3d2 = new Point3d(vec2d2.X, vec2d2.Y, 0.0);
                //point3d2..ctor(vec2d2.X, vec2d2.Y, 0.0);
                Line item = new Line(point3d, point3d2);
                //item..ctor(point3d, point3d2);
                list.Add(item);
                for (int i = 0; i < count - 1; i++)
                {
                    vec2d = polygon2d.Points[i];
                    vec2d2 = polygon2d.Points[i + 1];
                    point3d = new Point3d(vec2d.X, vec2d.Y, 0.0);
                    point3d2 = new Point3d(vec2d2.X, vec2d2.Y, 0.0);

                    //point3d..ctor(vec2d.X, vec2d.Y, 0.0);
                    //point3d2..ctor(vec2d2.X, vec2d2.Y, 0.0);
                    //item..ctor(point3d, point3d2);
                    list.Add(item);
                }
                foreach (Line line2 in list)
                {
                    double num;
                    double num2;
                    bool flag = Intersection.LineLine(line, line2, out num, out num2, RhinoGeoMethods.curveIntersectTolerance, true);
                    bool flag2 = flag;
                    if (flag2)
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        public static bool IsPositionVisible(Vec2d currentPos, Vec2d targetPos, List<Polygon2d> obsList)
        {
            Point3d lineFrom = new Point3d(currentPos.X, currentPos.Y, 0.0);
            //lineFrom..ctor(currentPos.X, currentPos.Y, 0.0);
            Point3d lineTo = new Point3d(targetPos.X, targetPos.Y, 0.0);
            //lineTo..ctor(targetPos.X, targetPos.Y, 0.0);
            return RhinoGeoMethods.IsPositionVisible(lineFrom, lineTo, obsList);
        }


        private static double curveIntersectTolerance = 0.001;
    }
}

