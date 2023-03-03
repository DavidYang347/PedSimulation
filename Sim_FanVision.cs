using System;
using System.Collections.Generic;
using PedSimulation.Geometry;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;

namespace PedSimulation.Simulation
{

    public class FanVision : PersonVision
    {

        public double Angle { get; set; }

        public double Distance { get; set; }

        public double AngularResolution { get; set; }

        public FanVision(double _angle, double _distance, double _angularRes)
        {
            this.Angle = _angle;
            this.Distance = _distance;
            bool flag = _angularRes <= 0.0;
            if (flag)
            {
                throw new ArgumentOutOfRangeException("Angular Resolution must be positive");
            }
            this.AngularResolution = _angularRes;
        }

        public override List<Polyline> GetFOVPolylines(Person p, Map map)
        {
            double num = this.AngularResolution / 180.0 * 3.141592653589793;
            List<double> list = new List<double>();
            double num2 = this.Angle / 2.0;
            double a = -num2;
            Vec2d position = p.getPosition();
            Point3d point3d = new Point3d(position.X, position.Y, 0.0);
            //point3d..ctor(position.X, position.Y, 0.0);
            Vec2d velocity = p.getVelocity();
            Vec2d vec2d = new Vec2d(velocity);
            Vector3d vector3d = new Vector3d(vec2d.X, vec2d.Y, 0.0);
            //vector3d..ctor(vec2d.X, vec2d.Y, 0.0);
            List<Line> list2 = new List<Line>();
            List<Line> obstacleLinesInRange = this.GetObstacleLinesInRange(point3d, this.Distance, map.ObstaclePolylines);
            double num3 = num2 % num;
            int num4 = (int)((this.Angle - num3 * 2.0) / num);
            vec2d = vec2d.rotate(a);
            vector3d = new Vector3d(vec2d.X, vec2d.Y, 0.0);
            //vector3d..ctor(vec2d.X, vec2d.Y, 0.0);
            list2.Add(this.GetObstructedLine(new Line(point3d, vector3d, this.Distance), obstacleLinesInRange));
            vec2d = vec2d.rotate(num3);
            vector3d = new Vector3d(vec2d.X, vec2d.Y, 0.0);
            //vector3d..ctor(vec2d.X, vec2d.Y, 0.0);
            list2.Add(this.GetObstructedLine(new Line(point3d, vector3d, this.Distance), obstacleLinesInRange));
            for (int i = 0; i < num4; i++)
            {
                vec2d = vec2d.rotate(num);
                vector3d = new Vector3d(vec2d.X, vec2d.Y, 0.0);
                //vector3d..ctor(vec2d.X, vec2d.Y, 0.0);
                list2.Add(this.GetObstructedLine(new Line(point3d, vector3d, this.Distance), obstacleLinesInRange));
            }
            vec2d = vec2d.rotate(num3);
            vector3d = new Vector3d(vec2d.X, vec2d.Y, 0.0);
            //vector3d..ctor(vec2d.X, vec2d.Y, 0.0);
            list2.Add(this.GetObstructedLine(new Line(point3d, vector3d, this.Distance), obstacleLinesInRange));
            List<Point3d> list3 = new List<Point3d>();
            list3.Add(point3d);
            foreach (Line line in list2)
            {
                list3.Add(line.To);
            }
            list3.Add(point3d);
            return new List<Polyline>
            {
                new Polyline(list3)
            };
        }

        public override List<Curve> GetFOVCurves(Person p, Map map)
        {
            List<Polyline> fovpolylines = this.GetFOVPolylines(p, map);
            bool flag = fovpolylines.Count == 0;
            List<Curve> result;
            if (flag)
            {
                result = new List<Curve>();
            }
            else
            {
                Polyline polyline = fovpolylines[0];
                PolylineCurve item = new PolylineCurve(polyline);
                result = new List<Curve>
                {
                    item
                };
            }
            return result;
        }

        private Line GetObstructedLine(Line line0, List<Line> relevantObstacles)
        {
            double num = 0.001;
            Point3d from = line0.From;
            Point3d point3d = line0.To;
            double num2 = double.MaxValue;
            foreach (Line line in relevantObstacles)
            {
                double num3;
                double num4;
                bool flag = Intersection.LineLine(line0, line, out num3, out num4, num, true);
                if (flag)
                {
                    bool flag2 = num3 < num2;
                    if (flag2)
                    {
                        num2 = num3;
                        point3d = line0.PointAt(num3);
                    }
                }
            }
            return new Line(from, point3d);
        }

        private List<Line> GetObstacleLinesInRange(Point3d center, double dist, List<Polygon2d> obsList)
        {
            List<Line> list = new List<Line>();
            foreach (Polygon2d polygon2d in obsList)
            {
                List<Line> list2 = new List<Line>();
                int count = polygon2d.Points.Count;
                Vec2d vec2d = polygon2d.Points[count - 1];
                Vec2d vec2d2 = polygon2d.Points[0];
                Point3d point3d = new Point3d(vec2d.X, vec2d.Y, 0.0);
                //point3d..ctor(vec2d.X, vec2d.Y, 0.0);
                Point3d point3d2 = new Point3d(vec2d2.X, vec2d2.Y, 0.0);
                //point3d2..ctor(vec2d2.X, vec2d2.Y, 0.0);
                Line item = new Line(point3d, point3d2);
                //item..ctor(point3d, point3d2);
                list2.Add(item);
                for (int i = 0; i < count - 1; i++)
                {
                    vec2d = polygon2d.Points[i];
                    vec2d2 = polygon2d.Points[i + 1];
                    point3d = new Point3d(vec2d.X, vec2d.Y, 0.0);
                    //point3d..ctor(vec2d.X, vec2d.Y, 0.0);
                    point3d2 = new Point3d(vec2d2.X, vec2d2.Y, 0.0);
                    //point3d2..ctor(vec2d2.X, vec2d2.Y, 0.0);
                    item = new Line(point3d, point3d2);
                    //item..ctor(point3d, point3d2);
                    list2.Add(item);
                }
                foreach (Line item2 in list2)
                {
                    bool flag = center.DistanceTo(item2.From) < dist || center.DistanceTo(item2.To) < dist;
                    if (flag)
                    {
                        list.Add(item2);
                    }
                }
            }
            return list;
        }
    }
}

