using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace PedSimulation.Simulation
{

    public class PanoVision : PersonVision
    {

        public override List<Curve> GetFOVCurves(Person p, Map map)
        {
            return new List<Curve>();
        }


        public override List<Polyline> GetFOVPolylines(Person p, Map map)
        {
            return new List<Polyline>();
        }
    }
}
