using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace PedSimulation.Simulation
{

    public abstract class PersonVision
    {

        public abstract List<Curve> GetFOVCurves(Person p, Map map);


        public abstract List<Polyline> GetFOVPolylines(Person p, Map map);
    }
}

