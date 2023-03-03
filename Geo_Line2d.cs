using System;

namespace PedSimulation.Geometry
{
    public class Line2d
    {

        public Vec2d Start { get; set; }

        public Vec2d End { get; set; }

        public Line2d(Vec2d a, Vec2d b)
        {
            this.Start = a;
            this.End = b;
        }

        public Vec2d Dir
        {
            get
            {
                return this.End.subtract(this.Start);
            }
        }
    }
}





