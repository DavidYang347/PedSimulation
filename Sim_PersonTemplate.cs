using System;
using System.Collections.Generic;
using PedSimulation.Geometry;
using PedSimulation.RouteGraph;

namespace PedSimulation.Simulation
{

    public class PersonTemplate
    {

        public Gate StartGate { get; set; }

        public Gate DestinationGate { get; set; }

        public double Probability { get; set; }

        public int TimeLimit { get; set; }

        public List<Program> Interests { get; set; }

        public List<int> NeedValues { get; set; }

        public PersonVision Vision { get; set; }

        public int Number { get; set; }

        public double BodyRadius { get; set; }

        public double Mass { get; set; }

        public double TargetForce { get; set; }

        public Path InitialPath { get; set; }

        public PersonTemplate(Gate _startGate, Gate _destinationGate, int _timeLimit, double _probability, int _number, double _bodyRadius, double _mass, double _targetForce)
        {
            this.StartGate = _startGate;
            this.DestinationGate = _destinationGate;
            this.TimeLimit = _timeLimit;
            this.Probability = _probability;
            this.Number = _number;
            this.BodyRadius = _bodyRadius;
            this.Mass = _mass;
            this.TargetForce = _targetForce;
            this.Interests = new List<Program>();
            this.Vision = new PanoVision();
            this.InitialPath = null;
        }

        public void PlanInitialPath(Map map)
        {
            bool flag = map.GateSpecGraphs.Count == 0;
            if (flag)
            {
                throw new Exception("PersonTemplate cannot plan initial path: map graph is not ready.");
            }
            GoalSpecificGraph goalSpecificGraph = map.GateSpecGraphs[this.DestinationGate];
            Vertex goalVertex = goalSpecificGraph.GoalVertex;
            Vec2d position = this.StartGate.AccessPoints[0].Position;
            Path initialPath = this.PlanPathForVertex(position, goalVertex, map, goalSpecificGraph);
            this.InitialPath = initialPath;
        }

        private Path PlanPathForVertex(Vec2d a, Vertex gv, Map map, GoalSpecificGraph g)
        {
            Vertex vertex = new Vertex(a, null, 0);
            List<Vertex> list = new List<Vertex>
            {
                vertex
            };
            Vec2d position = gv.Position;
            bool flag = RhinoGeoMethods.IsPositionVisible(a, position, map.ObstaclePolylines);
            Path result;
            if (flag)
            {
                list.Add(gv);
                Path path = new Path(list);
                result = path;
            }
            else
            {
                bool flag2 = g == null || g.Vertices.Count == 0;
                if (flag2)
                {
                    throw new Exception("PlanPathForVertex: The Graph is null and there are obstacles in the way.");
                }
                List<Vertex> list2 = new List<Vertex>();
                foreach (Vertex vertex2 in g.Vertices)
                {
                    bool flag3 = RhinoGeoMethods.IsPositionVisible(a, vertex2.Position, map.ObstaclePolylines);
                    if (flag3)
                    {
                        list2.Add(vertex2);
                    }
                }
                bool flag4 = list2.Count == 0;
                if (flag4)
                {
                    throw new Exception("PlanPathForVertex: There is no visible start vertex.");
                }
                Vertex parent = list2[0];
                double num = double.MaxValue;
                foreach (Vertex vertex3 in list2)
                {
                    double num2 = vertex3.Position.distance(a);
                    bool flag5 = num2 + vertex3.GValue < num;
                    if (flag5)
                    {
                        num = num2 + vertex3.GValue;
                        parent = vertex3;
                    }
                }
                vertex.Parent = parent;
                Path path = vertex.TracePath();
                result = path;
            }
            return result;
        }
    }
}

