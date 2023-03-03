using System;
using PedSimulation.Geometry;

namespace PedSimulation.Simulation.ForceBehaviors
{
    // Token: 0x02000012 RID: 18
    public class AnticipatoryCollisionAvoidance : ForceBehavior
    {
        // Token: 0x060000F3 RID: 243 RVA: 0x00005714 File Offset: 0x00003914
        public AnticipatoryCollisionAvoidance(Person _parent, Map _parentMap)
        {
            this.parent = _parent;
            this.parentMap = _parentMap;
            this.x0 = this.parent.getPosition();
            this.v0 = this.parent.getVelocity();
            this.r0 = this.parent.BodyRadius;
        }

        // Token: 0x060000F4 RID: 244 RVA: 0x00005788 File Offset: 0x00003988
        public override Vec2d CalculateForce()
        {
            Person nearestNeighbor = this.GetNearestNeighbor(this.parent);
            bool flag = nearestNeighbor == null;
            Vec2d result;
            if (flag)
            {
                result = Vec2d.zeroVector();
            }
            else
            {
                bool flag2 = this.hasCollided(this.parent, nearestNeighbor);
                if (flag2)
                {
                    result = Vec2d.zeroVector();
                }
                else
                {
                    Tuple<Person, double> firstCollision = this.GetFirstCollision();
                    bool flag3 = firstCollision == null || firstCollision.Item2 > AnticipatoryCollisionAvoidance.timeHorizon;
                    if (flag3)
                    {
                        result = Vec2d.zeroVector();
                    }
                    else
                    {
                        Person item = firstCollision.Item1;
                        Vec2d position = item.getPosition();
                        Vec2d velocity = item.getVelocity();
                        double bodyRadius = item.BodyRadius;
                        double item2 = firstCollision.Item2;
                        result = this.getAvoidanceForce(this.x0, position, this.v0, velocity, item2);
                    }
                }
            }
            return result;
        }

        // Token: 0x060000F5 RID: 245 RVA: 0x00005848 File Offset: 0x00003A48
        private bool hasCollided(Person a, Person b)
        {
            Vec2d position = a.getPosition();
            Vec2d position2 = b.getPosition();
            double num = position.distance(position2);
            return num < a.BodyRadius + b.BodyRadius;
        }

        // Token: 0x060000F6 RID: 246 RVA: 0x00005880 File Offset: 0x00003A80
        private Person GetNearestNeighbor(Person a)
        {
            Person result = null;
            double num = double.MaxValue;
            foreach (Person person in this.parent.Neighbors)
            {
                Vec2d position = a.getPosition();
                Vec2d position2 = person.getPosition();
                double num2 = position.distance(position2);
                bool flag = num2 < num;
                if (flag)
                {
                    num = num2;
                    result = person;
                }
            }
            return result;
        }

        // Token: 0x060000F7 RID: 247 RVA: 0x00005918 File Offset: 0x00003B18
        private Tuple<Person, double> GetFirstCollision()
        {
            Person person = null;
            double num = double.MaxValue;
            foreach (Person person2 in this.parent.Neighbors)
            {
                Vec2d position = person2.getPosition();
                Vec2d velocity = person2.getVelocity();
                double bodyRadius = person2.BodyRadius;
                double collisionTime = this.getCollisionTime(this.x0, position, this.v0, velocity, this.r0, bodyRadius);
                bool flag = collisionTime > 0.0 && collisionTime < num;
                if (flag)
                {
                    person = person2;
                    num = collisionTime;
                }
            }
            bool flag2 = person != null;
            Tuple<Person, double> result;
            if (flag2)
            {
                result = new Tuple<Person, double>(person, num);
            }
            else
            {
                result = null;
            }
            return result;
        }

        // Token: 0x060000F8 RID: 248 RVA: 0x000059F4 File Offset: 0x00003BF4
        private double getCollisionTime(Vec2d x0, Vec2d x1, Vec2d v0, Vec2d v1, double r0, double r1)
        {
            Vec2d vec2d = x1 - x0;
            Vec2d vec2d2 = v1 - v0;
            double num = vec2d2 * vec2d2;
            bool flag = num == 0.0;
            double result;
            if (flag)
            {
                result = -1.0;
            }
            else
            {
                double num2 = 2.0 * vec2d2 * vec2d;
                double num3 = vec2d * vec2d - (r0 + r1) * (r0 + r1);
                double num4 = num2 * num2 - 4.0 * num * num3;
                bool flag2 = num4 <= 0.0;
                if (flag2)
                {
                    result = -1.0;
                }
                else
                {
                    double num5 = (-num2 + Math.Sqrt(num4)) / 2.0 / num;
                    double num6 = (-num2 - Math.Sqrt(num4)) / 2.0 / num;
                    bool flag3 = num5 < 0.0 && num6 < 0.0;
                    if (flag3)
                    {
                        result = -1.0;
                    }
                    else
                    {
                        bool flag4 = num5 >= 0.0 && num6 >= 0.0;
                        if (flag4)
                        {
                            result = Math.Min(num5, num6);
                        }
                        else
                        {
                            result = 0.0;
                        }
                    }
                }
            }
            return result;
        }

        // Token: 0x060000F9 RID: 249 RVA: 0x00005B48 File Offset: 0x00003D48
        private Vec2d getAvoidanceForce(Vec2d x0, Vec2d x1, Vec2d v0, Vec2d v1, double t)
        {
            bool flag = t <= 0.0;
            if (flag)
            {
                throw new ArgumentOutOfRangeException("Collision time <=0");
            }
            bool flag2 = t < this.tMinForMaxAvoidanceForce;
            if (flag2)
            {
                t = this.tMinForMaxAvoidanceForce;
            }
            Vec2d vec2d = x0 + v0 * t - (x1 + v1 * t);
            vec2d = vec2d.normalize();
            double a = this.collisionForceConstant * (AnticipatoryCollisionAvoidance.timeHorizon - t) / t;
            return vec2d * a;
        }

        // Token: 0x060000FA RID: 250 RVA: 0x00005BD8 File Offset: 0x00003DD8
        public override string ToString()
        {
            return "Anticipatory collision avoidance force";
        }

        // Token: 0x04000074 RID: 116
        private Person parent;

        // Token: 0x04000075 RID: 117
        private Map parentMap;

        // Token: 0x04000076 RID: 118
        private static double timeHorizon = 1.5;

        // Token: 0x04000077 RID: 119
        private Vec2d x0;

        // Token: 0x04000078 RID: 120
        private Vec2d v0;

        // Token: 0x04000079 RID: 121
        private double r0;

        // Token: 0x0400007A RID: 122
        private double collisionForceConstant = 8.0;

        // Token: 0x0400007B RID: 123
        private double tMinForMaxAvoidanceForce = 0.2;
    }
}

