using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace PedSimulation.Simulation
{
    public class Goal
    {

        public Goal()
        {
            this.AccessPoints = new List<AccessPoint>();
        }

        public string Name { get; set; }

        public virtual List<AccessPoint> AccessPoints { get; set; }

        public virtual Point3d TagPosition { get; set; }

        public double AccessRadius { get; set; }

        public double VisitDuration { get; set; }

        public int GetVisitCount
        {
            get
            {
                return this.visitCount;
            }
        }

        public void AddVisitCount()
        {
            this.visitCount++;
        }

        public void ClearVisitCount()
        {
            this.visitCount = 0;
        }

        protected Point3d getAvgPoint()
        {
            Point3d point3d= new Point3d(0.0, 0.0, 0.0);
            //point3d..ctor(0.0, 0.0, 0.0);
            foreach (AccessPoint accessPoint in this.AccessPoints)
            {
                Point3d point = accessPoint.Point;
                point3d += point;
            }
            point3d /= (double)this.AccessPoints.Count;
            return point3d;
        }

        public virtual void Refresh()
        {
            this.ClearVisitCount();
        }

        private int visitCount = 0;
    }
}

