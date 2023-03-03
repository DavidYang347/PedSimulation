using System;
using System.Collections.Generic;

namespace PedSimulation.Geometry
{
    public class Particle
    {
        public Particle(Vec2d position, double mass, double _damping, double _friction)
        {
            this.p = position;
            this.v = new Vec2d();
            this.a = new Vec2d();
            this.m = mass;
            this.a0 = new Vec2d();
            this.v0 = new Vec2d();
            this.p0 = this.p;
            this.f = new Vec2d();
            this.DampingRatio = _damping;
            this.FrictionRatio = _friction;
            this.posRecord = new LinkedList<Vec2d>();
        }

        public Vec2d getPosition()
        {
            return this.p;
        }

        public Vec2d getVelocity()
        {
            return this.v;
        }

        public Vec2d getAcceleration()
        {
            return this.a;
        }

        public double DampingRatio { get; set; }

        public double FrictionRatio { get; set; }

        public double getSpeed()
        {
            return this.getVelocity().magnitude();
        }

        protected void updateVAndP(double dt, Vec2d wallDir)
        {
            this.v0 = new Vec2d(this.v.X, this.v.Y);
            this.p0 = new Vec2d(this.p.X, this.p.Y);
            Tuple<Vec2d, Vec2d> tuple = Particle.integrateLinearFunctionTwice_ZeroSpeedAgainstWall(this.a0, this.a, this.v0, this.p0, dt, wallDir);
            this.v = tuple.Item1;
            this.p = tuple.Item2;
            bool flag = this.posRecord.Count >= Particle.posRecordLength;
            if (flag)
            {
                this.posRecord.RemoveFirst();
            }
            this.posRecord.AddLast(this.p);
        }

        private static Tuple<Vec2d, Vec2d> integrateLinearFunctionTwice(Vec2d a0, Vec2d a1, Vec2d v0, Vec2d p0, double dt)
        {
            bool flag = dt == 0.0;
            Tuple<Vec2d, Vec2d> result;
            if (flag)
            {
                result = new Tuple<Vec2d, Vec2d>(Vec2d.zeroVector(), Vec2d.zeroVector());
            }
            else
            {
                double num = (a1.X - a0.X) / dt;
                double num2 = (a1.Y - a0.Y) / dt;
                double num3 = 0.5 * num * dt * dt + a0.X * dt + v0.X;
                double num4 = 0.5 * num2 * dt * dt + a0.Y * dt + v0.Y;
                Vec2d item = new Vec2d(num3, num4);
                double x = 0.16666666666666666 * num * dt * dt * dt + 0.5 * a0.X * dt * dt + num3 * dt + p0.X;
                double y = 0.16666666666666666 * num2 * dt * dt * dt + 0.5 * a0.X * dt * dt + num4 * dt + p0.Y;
                Vec2d item2 = new Vec2d(x, y);
                result = new Tuple<Vec2d, Vec2d>(item, item2);
            }
            return result;
        }

        private static Tuple<Vec2d, Vec2d> integrateLinearFunctionTwice_ZeroSpeedAgainstWall(Vec2d a0, Vec2d a1, Vec2d v0, Vec2d p0, double dt, Vec2d zeroSpeedDir)
        {
            bool flag = dt == 0.0;
            Tuple<Vec2d, Vec2d> result;
            if (flag)
            {
                result = new Tuple<Vec2d, Vec2d>(Vec2d.zeroVector(), Vec2d.zeroVector());
            }
            else
            {
                double num = (a1.X - a0.X) / dt;
                double num2 = (a1.Y - a0.Y) / dt;
                double num3 = 0.5 * num * dt * dt + a0.X * dt + v0.X;
                double num4 = 0.5 * num2 * dt * dt + a0.Y * dt + v0.Y;
                Vec2d vec2d = new Vec2d(num3, num4);
                double x = 0.16666666666666666 * num * dt * dt * dt + 0.5 * a0.X * dt * dt + num3 * dt;
                double y = 0.16666666666666666 * num2 * dt * dt * dt + 0.5 * a0.X * dt * dt + num4 * dt;
                Vec2d vec2d2 = new Vec2d(x, y);
                bool flag2 = zeroSpeedDir != null && !zeroSpeedDir.Equals(Vec2d.zeroVector());
                if (flag2)
                {
                    double num5 = vec2d.projectOnVector(zeroSpeedDir);
                    zeroSpeedDir = zeroSpeedDir.normalize();
                    bool flag3 = num5 > 0.0;
                    if (flag3)
                    {
                        Vec2d p = zeroSpeedDir.multiply(num5);
                        vec2d = vec2d.subtract(p);
                    }
                    double num6 = vec2d2.projectOnVector(zeroSpeedDir);
                    bool flag4 = num6 > 0.0;
                    if (flag4)
                    {
                        Vec2d p2 = zeroSpeedDir.multiply(num6);
                        vec2d2 = vec2d2.subtract(p2);
                    }
                }
                Vec2d item = p0.add(vec2d2);
                result = new Tuple<Vec2d, Vec2d>(vec2d, item);
            }
            return result;
        }

        protected void applyForce(Vec2d _f)
        {
            bool flag = _f == null;
            if (!flag)
            {
                this.f = this.f.add(_f);
            }
        }

        public void clearForce()
        {
            this.f = Vec2d.zeroVector();
        }

        protected void updateAcceleration()
        {
            Vec2d rawAcc = this.f.multiply(1.0 / this.m);
            this.a0 = this.a;
            this.a = this.getDampedAcceleration(rawAcc);
        }

        private Vec2d getDampedAcceleration(Vec2d rawAcc)
        {
            double num = rawAcc.magnitude();
            Vec2d vec2d = this.getVelocity().normalize();
            bool flag = this.getSpeed() > 0.0;
            Vec2d result;
            if (flag)
            {
                Vec2d vec2d2 = rawAcc.subtract(this.getVelocity().multiply(this.DampingRatio));
                vec2d2 = vec2d2.subtract(vec2d.multiply(this.FrictionRatio));
                result = vec2d2;
            }
            else
            {
                bool flag2 = num - this.FrictionRatio >= 0.0;
                if (flag2)
                {
                    double num2 = num - this.FrictionRatio;
                    bool flag3 = num2 < 0.0;
                    if (flag3)
                    {
                        num2 = 0.0;
                    }
                    result = rawAcc.normalize().multiply(num2);
                }
                else
                {
                    result = Vec2d.zeroVector();
                }
            }
            return result;
        }

        private static int posRecordLength = 20;

        private double m;

        private Vec2d p;

        private Vec2d v;

        private Vec2d a;

        private Vec2d p0;

        private Vec2d v0;

        private Vec2d a0;

        private Vec2d f;

        private LinkedList<Vec2d> posRecord;
    }
}
