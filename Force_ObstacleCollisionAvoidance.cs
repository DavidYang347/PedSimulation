using System;
using PedSimulation.Geometry;

namespace PedSimulation.Simulation.ForceBehaviors
{
    // Token: 0x02000016 RID: 22
    public class ObstacleCollisionAvoidance : ForceBehavior
    {
        // Token: 0x06000107 RID: 263 RVA: 0x00005F0B File Offset: 0x0000410B
        public ObstacleCollisionAvoidance(Person _parent, Map _parentMap)
        {
            this.parent = _parent;
            this.parentMap = _parentMap;
        }

        // Token: 0x06000108 RID: 264 RVA: 0x00005F24 File Offset: 0x00004124
        public override Vec2d CalculateForce()
        {
            this.wallDir = Vec2d.zeroVector();
            bool flag = this.parent.NeighboringObstacles.Count == 0;
            Vec2d result;
            if (flag)
            {
                result = Vec2d.zeroVector();
            }
            else
            {
                SystemSettings settings = this.parentMap.Settings;
                double obstacleRepulsionA = settings.ObstacleRepulsionA;
                double obstacleRepulsionB = settings.ObstacleRepulsionB;
                double obstacleRepulsionMax = settings.ObstacleRepulsionMax;
                double obstacleRepulsionThreshold = settings.ObstacleRepulsionThreshold;
                Vec2d position = this.parent.getPosition();
                Vec2d p = null;
                Line2d line2d = null;
                bool flag2 = false;
                double num = double.MaxValue;
                foreach (Polygon2d polygon2d in this.parent.NeighboringObstacles)
                {
                    for (int i = 0; i < polygon2d.Points.Count; i++)
                    {
                        Vec2d vec2d = polygon2d.Points[i];
                        double num2 = position.distance(vec2d);
                        bool flag3 = num2 <= num;
                        if (flag3)
                        {
                            p = vec2d;
                            num = num2;
                            flag2 = false;
                        }
                        bool flag4 = i == polygon2d.Points.Count - 1;
                        Vec2d b;
                        if (flag4)
                        {
                            b = polygon2d.Points[0];
                        }
                        else
                        {
                            b = polygon2d.Points[i + 1];
                        }
                        Line2d line2d2 = new Line2d(vec2d, b);
                        Vec2d vec2d2;
                        double num3 = ObstacleCollisionAvoidance.lineSegmentDistanceToPoint(vec2d, b, position, out vec2d2);
                        bool flag5 = num3 < num;
                        if (flag5)
                        {
                            line2d = line2d2;
                            num = num3;
                            flag2 = true;
                        }
                    }
                }
                bool flag6 = flag2;
                Vec2d vec2d3;
                if (flag6)
                {
                    Vec2d p2 = ObstacleCollisionAvoidance.pointProjectionOnLine(position, line2d.Start, line2d.End);
                    vec2d3 = position.subtract(p2);
                }
                else
                {
                    vec2d3 = position.subtract(p);
                }
                double repulsionForce_Exp = ObstacleCollisionAvoidance.GetRepulsionForce_Exp(num, obstacleRepulsionA, obstacleRepulsionB, obstacleRepulsionMax, obstacleRepulsionThreshold);
                bool flag7 = num <= this.parent.BodyRadius;
                if (flag7)
                {
                    this.wallDir = vec2d3.negative();
                    this.parent.CurrentWallDir = this.wallDir;
                }
                Vec2d vec2d4 = vec2d3.normalize().multiply(repulsionForce_Exp);
                result = vec2d4;
            }
            return result;
        }

        // Token: 0x06000109 RID: 265 RVA: 0x00006178 File Offset: 0x00004378
        private static double lineSegmentDistanceToPoint(Vec2d a, Vec2d b, Vec2d pt, out Vec2d proj)
        {
            Line2d line2d = new Line2d(a, b);
            proj = ObstacleCollisionAvoidance.pointProjectionOnLine(pt, a, b);
            double num = proj.distance(a);
            double num2 = proj.distance(b);
            double num3 = a.distance(b);
            bool flag = num + num2 - num3 * 1.001 > 0.0;
            double result;
            if (flag)
            {
                double num4 = pt.distance(a);
                double num5 = pt.distance(b);
                result = ((num4 < num5) ? num4 : num5);
            }
            else
            {
                double num6 = proj.distance(pt);
                result = num6;
            }
            return result;
        }

        // Token: 0x0600010A RID: 266 RVA: 0x00006208 File Offset: 0x00004408
        private static Vec2d pointProjectionOnLine(Vec2d p, Vec2d a, Vec2d b)
        {
            Vec2d vec2d = p.subtract(a);
            Vec2d vec2d2 = b.subtract(a);
            double num = vec2d.magnitude();
            double num2 = vec2d2.magnitude();
            bool flag = num == 0.0;
            Vec2d result;
            if (flag)
            {
                result = a;
            }
            else
            {
                bool flag2 = num2 == 0.0;
                if (flag2)
                {
                    result = a;
                }
                else
                {
                    double num3 = vec2d2.dotProduct(vec2d) / num2 / num;
                    Vec2d vec2d3 = vec2d2.normalize().multiply(num * num3);
                    result = vec2d3.add(a);
                }
            }
            return result;
        }

        // Token: 0x0600010B RID: 267 RVA: 0x00006290 File Offset: 0x00004490
        private static double GetRepulsionForce_Exp(double dist, double a, double b, double max, double threshold)
        {
            double num = 0.0;
            bool flag = dist == 0.0;
            if (flag)
            {
                num = 0.0;
            }
            else
            {
                bool flag2 = dist < threshold;
                if (flag2)
                {
                    num = a * Math.Exp(-b * dist);
                }
            }
            bool flag3 = num > max;
            if (flag3)
            {
                num = max;
            }
            return num;
        }

        // Token: 0x0600010C RID: 268 RVA: 0x000062EC File Offset: 0x000044EC
        public override string ToString()
        {
            return "Obstacle collision avoidance force";
        }

        // Token: 0x04000084 RID: 132
        private Person parent;

        // Token: 0x04000085 RID: 133
        private Map parentMap;

        // Token: 0x04000086 RID: 134
        private Vec2d wallDir;
    }
}

