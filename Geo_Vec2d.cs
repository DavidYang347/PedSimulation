using System;

namespace PedSimulation.Geometry
{

    public class Vec2d
    {

        public Vec2d()
        {
            this.X = 0.0;
            this.Y = 0.0;
        }

        public Vec2d(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vec2d(Vec2d original)
        {
            this.X = original.X;
            this.Y = original.Y;
        }

        public static Vec2d zeroVector()
        {
            return new Vec2d();
        }

        public Vec2d add(Vec2d p1)
        {
            double x = this.X + p1.X;
            double y = this.Y + p1.Y;
            return new Vec2d(x, y);
        }

        public static Vec2d operator +(Vec2d a, Vec2d b)
        {
            return a.add(b);
        }

        public Vec2d subtract(Vec2d p1)
        {
            double x = this.X - p1.X;
            double y = this.Y - p1.Y;
            return new Vec2d(x, y);
        }

        public static Vec2d operator -(Vec2d a, Vec2d b)
        {
            return a.subtract(b);
        }

        public Vec2d multiply(double a)
        {
            double x = this.X * a;
            double y = this.Y * a;
            return new Vec2d(x, y);
        }

        public static Vec2d operator *(Vec2d v, double a)
        {
            return v.multiply(a);
        }

        public static Vec2d operator *(double a, Vec2d v)
        {
            return v.multiply(a);
        }

        public static double operator *(Vec2d a, Vec2d b)
        {
            return a.dotProduct(b);
        }

        public double dotProduct(Vec2d p1)
        {
            return this.X * p1.X + this.Y * p1.Y;
        }

        public double projectOnVector(Vec2d v)
        {
            bool flag = v.magnitude() == 0.0;
            double result;
            if (flag)
            {
                result = 0.0;
            }
            else
            {
                Vec2d p = v.multiply(1.0 / v.magnitude());
                result = this.dotProduct(p);
            }
            return result;
        }

        public Vec2d dotProductLeft(Vec2d[] m)
        {
            double x = m[0].X * this.X + m[1].X * this.Y;
            double y = m[0].Y * this.X + m[1].Y * this.Y;
            return new Vec2d(x, y);
        }

        public double magnitude()
        {
            return Math.Sqrt(this.X * this.X + this.Y * this.Y);
        }

        public double distance(Vec2d pt)
        {
            Vec2d vec2d = this.subtract(pt);
            return vec2d.magnitude();
        }

        public double getXAngle()
        {
            bool flag = this.X == 0.0 && this.Y == 0.0;
            double result;
            if (flag)
            {
                result = 0.0;
            }
            else
            {
                bool flag2 = this.X == 0.0 && this.Y > 0.0;
                if (flag2)
                {
                    result = 1.5707963267948966;
                }
                else
                {
                    bool flag3 = this.X == 0.0 && this.Y < 0.0;
                    if (flag3)
                    {
                        result = 4.71238898038469;
                    }
                    else
                    {
                        double d = this.Y / this.X;
                        double num = Math.Atan(d);
                        bool flag4 = this.X < 0.0;
                        if (flag4)
                        {
                            result = num + 3.141592653589793;
                        }
                        else
                        {
                            bool flag5 = this.Y < 0.0;
                            if (flag5)
                            {
                                result = num + 6.283185307179586;
                            }
                            else
                            {
                                result = num;
                            }
                        }
                    }
                }
            }
            return result;
        }

        public double getAngleWithVec2d(Vec2d v)
        {
            bool flag = this.magnitude() == 0.0 || v.magnitude() == 0.0;
            double result;
            if (flag)
            {
                result = 0.0;
            }
            else
            {
                double d = this.dotProduct(v) / (this.magnitude() * v.magnitude());
                result = Math.Acos(d);
            }
            return result;
        }

        public Vec2d normalize()
        {
            double num = this.magnitude();
            bool flag = num == 0.0;
            Vec2d result;
            if (flag)
            {
                result = new Vec2d(0.0, 0.0);
            }
            else
            {
                result = new Vec2d(this.X / num, this.Y / num);
            }
            return result;
        }

        public Vec2d rotate(double a)
        {
            Vec2d vec2d = new Vec2d(Math.Cos(a), Math.Sin(a));
            Vec2d vec2d2 = new Vec2d(-Math.Sin(a), Math.Cos(a));
            Vec2d[] m = new Vec2d[]
            {
                vec2d,
                vec2d2
            };
            return this.dotProductLeft(m);
        }

        public Vec2d negative()
        {
            return Vec2d.zeroVector().subtract(this);
        }

        public static Vec2d operator -(Vec2d a)
        {
            return a.negative();
        }

        public override string ToString()
        {
            return this.X + "," + this.Y;
        }

        public override bool Equals(object obj)
        {
            Vec2d vec2d = obj as Vec2d;
            bool flag = vec2d == null;
            return !flag && vec2d.X == this.X && vec2d.Y == this.Y;
        }

        public override int GetHashCode()
        {
            int num = 17;
            num = num * 23 + this.X.GetHashCode();
            return num * 23 + this.Y.GetHashCode();
        }

        public double X;

        public double Y;
    }
}

