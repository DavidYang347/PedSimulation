using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace PedSimulation.Simulation
{
    public class PersonTrace
    {
        public PersonTrace()
        {
            this.TimeInFrames = new List<int>();
            this.Position = new List<Point3d>();
            this.Speed = new List<double>();
            this.State = new List<PersonState>();
        }


        public List<int> TimeInFrames { get; set; }


        public List<Point3d> Position { get; set; }


        public List<double> Speed { get; set; }


        public List<PersonState> State { get; set; }
    }
}

